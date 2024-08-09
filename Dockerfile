# Base image for the runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY polling-app-backend/polling-app-backend.csproj polling-app-backend/
WORKDIR /src/polling-app-backend
RUN dotnet restore

# Copy the rest of the source code
COPY polling-app-backend/. .

# Build the application
RUN dotnet build -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "polling-app-backend.dll"]


    
