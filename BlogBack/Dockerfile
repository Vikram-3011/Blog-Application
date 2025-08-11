# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the project file first to leverage Docker's build cache
COPY BlogBack/BlogBack.csproj BlogBack/
RUN dotnet restore BlogBack/BlogBack.csproj

# Copy the rest of the backend source code
COPY BlogBack/ BlogBack/

# Publish the app
RUN dotnet publish BlogBack/BlogBack.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "BlogBack.dll"]
