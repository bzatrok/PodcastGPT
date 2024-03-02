using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PodcastGPT.Core.Clients;
using PodcastGPT.Core.Helpers;
using PodcastGPT.Core.Repositories;
using PodcastGPT.Core.Services;
using PodcastGPT.Data;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace PodcastGPT.Api;

public class Program
{
	public static string CorsPolicy = "_CorsPolicy";

	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "PodcastGPT Playground",
				Version = "v2"
			});

			// Set the comments path for the Swagger JSON and UI.
			//var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (Assembly assembly in assemblies)
			{
				var assemblyName = assembly.GetName().Name;

				if (assemblyName.Contains("PodcastGPT") &&
				    !assemblyName.Contains("Tests"))
				{
					var xmlFile = $"{assemblyName}.xml";
					var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
					c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
				}
			}
			// c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

			var securityScheme = new OpenApiSecurityScheme
			{
				Name = "Authorization",
				Description = "JWT Authorization header using the Bearer scheme.",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.Http,
				Scheme = "bearer",
				BearerFormat = "JWT",
				Reference = new OpenApiReference
				{
					Id = "Bearer",
					Type = ReferenceType.SecurityScheme
				}
			};

			c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
			c.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					securityScheme, Array.Empty<string>()
				}
			});
		});

		var corsEnabledVar = Environment.GetEnvironmentVariable("CORS_ENABLED");
		var corsEnabledBool = false;

		var corsEnabledForDomains = new List<string>();

		if (!string.IsNullOrWhiteSpace(corsEnabledVar))
		{
			corsEnabledBool = Convert.ToBoolean(corsEnabledVar);

			if (corsEnabledBool)
			{
				var corsEnabledForDomainsVar = Environment.GetEnvironmentVariable("CORS_ENABLED_DOMAINS_LIST");

				if (!string.IsNullOrWhiteSpace(corsEnabledForDomainsVar))
				{
					try
					{
						corsEnabledForDomains = corsEnabledVar
							.Split(',')
							.ToList();

						if (corsEnabledForDomains.Count == 0)
						{
							Console.WriteLine("CORS_ENABLED_DOMAINS_LIST is empty. CORS_ENABLED_DOMAINS_LIST must be a comma separated list of domains if CORS_ENABLED is set to true");
							corsEnabledBool = false;
						}
					}
					catch (Exception e)
					{
						Console.WriteLine("CORS_ENABLED_DOMAINS_LIST is invalid. CORS_ENABLED_DOMAINS_LIST must be a comma separated list of domains if CORS_ENABLED is set to true");
					}
				}
				else
				{
					corsEnabledBool = false;
				}
			}
		}
		
		builder.Services.AddCors(options => options.AddPolicy(CorsPolicy,
			policy =>
			{
				if (corsEnabledBool != null &&
				    corsEnabledForDomains.Any())
				{
					policy.WithOrigins(corsEnabledForDomains.ToArray())
						.AllowAnyMethod()
						.AllowAnyHeader();
				}
				else
				{
					policy.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader();
				}
			}
		));
		
		// REDIS SETTINGS
	var redisUrl = Environment.GetEnvironmentVariable("REDIS_URL");

	if (!string.IsNullOrEmpty(redisUrl))
	{
		var password = string.Empty;
		var hostAndPort = string.Empty;
		var useSsl = false;

		if (redisUrl.StartsWith("rediss://") || redisUrl.StartsWith("redis://"))
		{
			var uri = new Uri(redisUrl);
			password = uri.UserInfo.Split(':')[1];
			hostAndPort = $"{uri.Host}:{uri.Port}";
			useSsl = uri.Scheme == "rediss";
		}
		else
		{
			var parts = redisUrl.Split('@');
			password = parts.Length > 1 ? parts[0] : null;
			hostAndPort = parts.Length > 1 ? parts[1] : parts[0];
		}

		var redisTimeoutSec = Environment.GetEnvironmentVariable("REDIS_TIMEOUT_MS") is not null ? Convert.ToInt32(Environment.GetEnvironmentVariable("REDIS_TIMEOUT_MS")) : 3000;

		var config = new ConfigurationOptions
		{
			EndPoints = { hostAndPort },
			Password = password,
			Ssl = useSsl || false,
			AbortOnConnectFail = false,
			ConnectTimeout = redisTimeoutSec, // This is for initial connection
			SyncTimeout = redisTimeoutSec, // This is for each sync operation
			AsyncTimeout = redisTimeoutSec // This is for each async operation
		};

		config.CertificateValidation += ValidateServerCertificate;

		bool ValidateServerCertificate(
			object sender,
			X509Certificate? certificate,
			X509Chain? chain,
			SslPolicyErrors sslPolicyErrors)
		{
			//BEN: ADDED TO MAKE HEROKU REDIS WORK
			return true;

			if (certificate is null)
			{
				return false;
			}

			var ca = new X509Certificate2("redis_ca.pem");
			bool verdict = (certificate.Issuer == ca.Subject);
			if (verdict)
			{
				return true;
			}

			// _logger.LogInformation("Certificate error: {0}", sslPolicyErrors);
			return false;
		}

		builder.Services.AddSingleton(ConnectionMultiplexer.Connect(config));
		builder.Services.AddSingleton<RedisHelper>();
	}

	var logLevelFromEnv = Environment.GetEnvironmentVariable("LOGLEVEL");

		if (!Enum.TryParse(logLevelFromEnv, true, out LogLevel logLevel))
		{
			logLevel = LogLevel.Error; // default to Information if parsing fails
			if (!string.IsNullOrWhiteSpace(logLevelFromEnv))
				Console.WriteLine("Invalid 'LOGLEVEL' var passed. Valid Log Levels are: Trace, Debug, Information, Warning, Error, Critical, None");
		}
		
		builder.Logging
			.ClearProviders()
			.AddConsole()
			.AddFilter("Microsoft.Hosting", LogLevel.Information)
			.SetMinimumLevel(logLevel);
		
		builder.Services.AddControllers()
			.AddJsonOptions(options =>
			{
				// options.JsonSerializerOptions.IgnoreNullValues = true;
				options.JsonSerializerOptions.PropertyNamingPolicy = null;
				options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
			})
			.AddNewtonsoftJson(o =>
			{
				o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
			});

		builder.Services.AddAuthorization();
		
		// Helpers
		
		builder.Services.AddTransient<AudioFileHelper>();
		
		// Clients
		
		builder.Services.AddTransient<HttpClient>();
		builder.Services.AddTransient<OpenAiClient>();

		// Repositories

		builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
		
		// Services
		
		builder.Services.AddTransient<ArticleService>();
		builder.Services.AddTransient<RssService>();
		builder.Services.AddTransient<ElevenlabsService>();
		builder.Services.AddTransient<OpenAIService>();
		builder.Services.AddTransient<AudioService>();
		
		builder.Services.AddSingleton<PodcastGenerationService>();
		
		builder.Services.AddTransient<DatabaseContext>();
		
		// builder.Services.AddHostedService<PodcastGenerationTask>();

		var app = builder.Build();

		app.UseSwagger();
		app.UseSwaggerUI();

		app.UseHttpsRedirection();

		app.UseCors(CorsPolicy);
		//app.UseHttpsRedirection();
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();

		using (var scope = app.Services.CreateScope())
		{
			var services = scope.ServiceProvider;

			var context = services.GetRequiredService<DatabaseContext>();

			context.Database.EnsureCreated();
			//
			// try
			// {
			// 	context.Database.Migrate();
			// }
			// catch (Exception e)
			// {
			// 	Console.WriteLine(e);
			// }
		}
		
		app.Run();
	}
}