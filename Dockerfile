FROM node:22-alpine AS client
WORKDIR /client
COPY TheDeep/src/TheDeep.Web/ClientApp/package*.json ./
RUN npm ci
COPY TheDeep/src/TheDeep.Web/ClientApp/ ./
RUN npm run build -- --configuration production

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
COPY --from=client /wwwroot ./TheDeep/src/TheDeep.Web/wwwroot
RUN dotnet restore TheDeep/src/TheDeep.Web/TheDeep.Web.csproj
RUN dotnet publish TheDeep/src/TheDeep.Web/TheDeep.Web.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app ./
ENV ASPNETCORE_ENVIRONMENT=Production
ENV Database__ApplyMigrationsOnStartup=true
EXPOSE 8080
ENTRYPOINT ["dotnet", "TheDeep.Web.dll"]
