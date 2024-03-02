using PodcastGPT.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace PodcastGPT.Data;

public class DatabaseContext : DbContext
{
	public DbSet<Podcast> Podcasts { get; set; }
	public DbSet<PodcastSegment> PodcastSegments { get; set; }
	public DbSet<PodcastPersona> PodcastPersonas { get; set; }
	
	// public DbSet<NewsSite> NewsSites { get; set; }
	public DbSet<NewsSiteArticle> NewsSiteArticles { get; set; }
	// public DbSet<NewsSiteAuthor> NewsSiteAuthors { get; set; }
	
	// public DbSet<OpenAiConversation> OpenAiConversations { get; set; }
	// public DbSet<OpenAiMessage> OpenAiMessages { get; set; }
	
	public string DbPath { get; }

	public DatabaseContext()
	{
		var folder = Directory.GetCurrentDirectory();
		var dbFolder = Path.Join(folder, "database");
		
		try
		{
			Directory.CreateDirectory(dbFolder);
		} 
		catch (Exception e)
		{
			// Console.WriteLine(e);
		}

		DbPath = Path.Join(dbFolder, "podcastgpt_db.db");
	}
	
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var dbType = Environment.GetEnvironmentVariable("DB_TYPE") ?? "sqlite";
		
		// https://learn.microsoft.com/en-us/ef/core/querying/related-data/lazy
		optionsBuilder.UseLazyLoadingProxies();

		switch (dbType.ToLower())
		{
			case "postgres":
				var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
				optionsBuilder.UseNpgsql(connectionString);
				break;
			
			case "sqlite":
				optionsBuilder.UseSqlite($"Data Source={DbPath}");
				break;
			
			default:
				throw new Exception("DB_TYPE not set");
		}
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		
		// modelBuilder.Entity<NewsSite>()
		// 	.HasData(
		// 	// new NewsSite
		// 	// {
		// 	// 	NewsSiteId = Guid.NewGuid(), 
		// 	// 	Name = "Telex",
		// 	// 	Url = "https://telex.hu",
		// 	// 	RssFeedUrl = "https://telex.hu/rss/archivum?filters=%7B%22flags%22%3A%5B%22legfontosabb%22%5D%2C%22parentId%22%3A%5B%22null%22%5D%7D&perPage=10"
		// 	// },
		// 	new NewsSite
		// 	{
		// 		NewsSiteId = Guid.NewGuid(), 
		// 		Name = "Ars Technica",
		// 		Url = "https://arstechnica.com",
		// 		RssFeedUrl = "https://feeds.arstechnica.com/arstechnica/index"
		// 	}
		// );

		modelBuilder.Entity<PodcastPersona>()
			.HasMany(p => p.GuestOnPodcasts)
			.WithOne(p => p.PodcastGuestPersona)
			.HasForeignKey(p => p.PodcastGuestPersonaId);

		modelBuilder.Entity<PodcastPersona>()
			.HasMany(p => p.HostOnPodcasts)
			.WithOne(p => p.PodcastHostPersona)
			.HasForeignKey(p => p.PodcastHostPersonaId);
		
		modelBuilder.Entity<PodcastPersona>()
			.HasData(
				new PodcastPersona
				{
					PodcastPersonaId = Guid.NewGuid(),
					Name = "Robert",
					VoiceId = "onyx",
					Type = "interviewer"
				},
				new PodcastPersona
				{
					PodcastPersonaId = Guid.NewGuid(),
					Name = "Jessie",
					VoiceId = "alloy",
					Type = "guest"
				}
			);
		
		// Unable to determine the relationship represented by navigation 'Podcast.PodcastGuestPersona' of type 'PodcastPersona'.
		// Either manually configure the relationship, or ignore this property using the '[NotMapped]' attribute or by using 'EntityTypeBuilder.Ignore' in 'OnModelCreating'.
	}
}