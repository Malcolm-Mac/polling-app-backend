# Base image for the runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["polling-app-backend/polling-app-backend.csproj", "polling-app-backend/"]
RUN dotnet restore "polling-app-backend/polling-app-backend.csproj"
COPY . .
WORKDIR "/src/polling-app-backend"
RUN dotnet build "polling-app-backend.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "polling-app-backend.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["dotnet", "polling-app-backend.dll"]
