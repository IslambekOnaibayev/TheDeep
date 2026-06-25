# syntax=docker/dockerfile:1

# ---- Stage 1: build the Angular SPA ----
FROM node:22-alpine AS client
WORKDIR /client
COPY TheDeep/src/TheDeep.Web/ClientApp/package*.json ./
RUN npm ci
COPY TheDeep/src/TheDeep.Web/ClientApp/ ./
# angular.json outputPath is "../wwwroot", so the build lands in /wwwroot
RUN npm run build -- --configuration production

# ---- Stage 2: build & publish the .NET app ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
# Drop in the compiled SPA so it ships inside wwwroot
COPY --from=client /wwwroot ./TheDeep/src/TheDeep.Web/wwwroot
RUN dotnet restore TheDeep/src/TheDeep.Web/TheDeep.Web.csproj
RUN dotnet publish TheDeep/src/TheDeep.Web/TheDeep.Web.csproj -c Release -o /app --no-restore

# ---- Stage 3: runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app ./
ENV ASPNETCORE_ENVIRONMENT=Production
# Run EF migrations against the managed Postgres on startup.
ENV Database__ApplyMigrationsOnStartup=true
EXPOSE 8080
ENTRYPOINT ["dotnet", "TheDeep.Web.dll"]
