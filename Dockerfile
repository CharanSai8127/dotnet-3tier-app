# Stage 1: Build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file first (enables better layer caching)
COPY ["DotNetSqlCRUDApp.csproj", "./"]
RUN dotnet restore "DotNetSqlCRUDApp.csproj"

# Copy the remaining source and publish
COPY . .
RUN dotnet publish "DotNetSqlCRUDApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user (good security practice)
RUN useradd -m appuser && chown -R appuser:appuser /app
USER appuser

# Copy published output
COPY --from=build /app/publish .

# Expose the actual port the application listens on
EXPOSE 5035

# Set ASP.NET Core environment variables
ENV ASPNETCORE_URLS=http://+:5035 \
    ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "DotNetSqlCRUDApp.dll"]

