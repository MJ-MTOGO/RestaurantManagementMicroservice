FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build-env
WORKDIR /App

# Copy the project files
COPY . .

# Restore dependencies
RUN dotnet restore

# Build the application
RUN dotnet build -c Release -o out

# Publish the application
RUN dotnet publish -c Release -o out

# Use the runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .

# Set environment variable for EF migrations
ENV ASPNETCORE_ENVIRONMENT=Development

# Expose the application port
EXPOSE 8080:5000 ?

ENTRYPOINT ["dotnet", "RestaurantManagementService.dll"]
