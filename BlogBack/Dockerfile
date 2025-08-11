# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy and restore
COPY . . 
RUN dotnet restore "./BlogBack/BlogBack.csproj"

# Build and publish
RUN dotnet publish "./BlogBack/BlogBack.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port (optional, for local testing)
EXPOSE 80

# Start the app
ENTRYPOINT ["dotnet", "BlogBack.dll"]
