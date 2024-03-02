FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PodcastGPT.Api/PodcastGPT.Api.csproj", "PodcastGPT.Api/"]
COPY ["PodcastGPT.Core/PodcastGPT.Core.csproj", "PodcastGPT.Core/"]
COPY ["PodcastGPT.Data/PodcastGPT.Data.csproj", "PodcastGPT.Data/"]
RUN dotnet restore "PodcastGPT.Api/PodcastGPT.Api.csproj"
COPY . .
WORKDIR "/src/PodcastGPT.Api"
RUN dotnet build "PodcastGPT.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PodcastGPT.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

VOLUME ["/database"]
ENTRYPOINT ["dotnet", "PodcastGPT.Api.dll"]
