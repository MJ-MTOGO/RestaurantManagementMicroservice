# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy only the project file to cache dependencies
COPY RestaurantManagementService.csproj ./RestaurantManagementService.csproj
RUN dotnet restore ./RestaurantManagementService.csproj

# Copy all application files
COPY . ./
RUN dotnet publish ./RestaurantManagementService.csproj -c Release -o /App/out

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .

# Specify the entry point to run the application
ENTRYPOINT ["dotnet", "RestaurantManagementService.dll"]


