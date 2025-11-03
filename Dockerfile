# Stage 1: Build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the project file and restore dependencies first to cache
COPY ["DotNetMongoCRUDApp.csproj", "./"]
RUN dotnet restore "DotNetMongoCRUDApp.csproj"

# Copy the rest of the source code and publish the app
COPY . .
RUN dotnet publish "DotNetMongoCRUDApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user and set permissions for better security
RUN useradd -m appuser && chown -R appuser:appuser /app
USER appuser

# Copy published output from build stage
COPY --from=build /app/publish .

# Expose the listening port (adjust as necessary)
EXPOSE 80

# Set environment variables for ASP.NET Core runtime
ENV ASPNETCORE_URLS=http://+:80 \
    ASPNETCORE_ENVIRONMENT=Production

# Set entrypoint to run the application dll
ENTRYPOINT ["dotnet", "DotNetMongoCRUDApp.dll"]

