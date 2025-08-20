# =========================
# Build Stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file & restore dependencies
COPY BlogFront.csproj ./
RUN dotnet restore BlogFront.csproj

# Copy everything else
COPY . ./

# Publish the app
RUN dotnet publish BlogFront.csproj -c Release -o /app/publish

# =========================
# Runtime Stage
# =========================
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Remove default nginx static assets
RUN rm -rf ./*

# Copy Blazor WASM published output
COPY --from=build /app/publish/wwwroot .

# Copy nginx config (optional)
# This ensures proper routing for Blazor WASM
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose port
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
