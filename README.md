# PodcastGPT
Generate podcasts from news articles using [OpenAI's completions API](https://openai.com/blog/openai-api).

# WORK IN PROGRESS
This project is in a work in progress state. Expect breaking changes on updates.

## Purpose
Simplifier is a [.NET Core ](https://dotnet.microsoft.com/en-us/download) application with a [Next.JS](https://nextjs.org/) front-end that uses OpenAI's completions API to generate converational podcasts between a presenter & guest. The main purpose is the create custom podcasts from news sources the user is interested in and turn them into conversations.

## Running the application
### Prerequisites:
- [Node.js LTS >=18](https://nodejs.org/en)
- [dotnet 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/products/docker-desktop/)

### docker compose
Create a copy of the .env.template file found in the project root and add a valid OpenAI API key to it.
`cp .env.template .env`

Run the following command:
`docker compose up --build -d`

The application will be running under `http://localhost:8080`. 