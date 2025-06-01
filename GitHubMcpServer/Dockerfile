FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["GitHubMcpServer/GitHubMcpServer.csproj", "GitHubMcpServer/"]
RUN dotnet restore "GitHubMcpServer/GitHubMcpServer.csproj"
COPY . .
WORKDIR "/src/GitHubMcpServer"
RUN dotnet build "GitHubMcpServer.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "GitHubMcpServer.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GitHubMcpServer.dll"]
