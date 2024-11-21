FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy only the main project file
COPY RestaurantManagementService/*.csproj ./RestaurantManagementService/
RUN dotnet restore RestaurantManagementService/RestaurantManagementService.csproj

# Copy the remaining files
COPY . ./
RUN dotnet publish RestaurantManagementService/RestaurantManagementService.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .

ENTRYPOINT ["dotnet", "RestaurantManagementService.dll"]

