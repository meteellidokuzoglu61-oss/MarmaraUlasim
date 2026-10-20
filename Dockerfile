# Build the Ionic/Angular frontend.
FROM node:22-alpine AS frontend-build
WORKDIR /src
COPY package.json package-lock.json ./
RUN npm ci
COPY angular.json tsconfig*.json ./
COPY src ./src
RUN npm run build

# Publish the ASP.NET Core API and bundle the frontend into wwwroot.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS api-build
WORKDIR /src
COPY MarmaraUlasim.API/MarmaraUlasim.API.csproj MarmaraUlasim.API/
RUN dotnet restore MarmaraUlasim.API/MarmaraUlasim.API.csproj
COPY MarmaraUlasim.API/ MarmaraUlasim.API/
COPY --from=frontend-build /src/www/ MarmaraUlasim.API/wwwroot/
RUN dotnet publish MarmaraUlasim.API/MarmaraUlasim.API.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

# Small runtime image; secrets are supplied through environment variables.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=api-build /app/publish/ ./
ENTRYPOINT ["dotnet", "MarmaraUlasim.API.dll"]
