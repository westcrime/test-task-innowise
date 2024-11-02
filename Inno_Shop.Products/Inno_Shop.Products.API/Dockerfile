# Use the official .NET Core SDK as a parent image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy the project file and restore any dependencies (use .csproj for the project name)
COPY *.sln .
COPY Inno_Shop.Products.API/*.csproj ./Inno_Shop.Products.API/
COPY Inno_Shop.Products.Application/*.csproj ./Inno_Shop.Products.Application/
COPY Inno_Shop.Products.Domain/*.csproj ./Inno_Shop.Products.Domain/
COPY Inno_Shop.Products.Infrastructure/*.csproj ./Inno_Shop.Products.Infrastructure/
COPY Inno_Shop.Products.IntegrationTests/*.csproj ./Inno_Shop.Products.IntegrationTests/
COPY Inno_Shop.Products.UnitTests/*.csproj ./Inno_Shop.Products.UnitTests/

RUN dotnet restore

# Copy the rest of the application code
COPY Inno_Shop.Products.API/. ./Inno_Shop.Products.API/
COPY Inno_Shop.Products.Application/. ./Inno_Shop.Products.Application/
COPY Inno_Shop.Products.Domain/. ./Inno_Shop.Products.Domain/
COPY Inno_Shop.Products.Infrastructure/. ./Inno_Shop.Products.Infrastructure/
COPY Inno_Shop.Products.IntegrationTests/. ./Inno_Shop.Products.IntegrationTests/
COPY Inno_Shop.Products.UnitTests/. ./Inno_Shop.Products.UnitTests/

# Publish the application
WORKDIR /app/Inno_Shop.Products.API
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Expose the port your application will run on
EXPOSE 5245   


# Start the application
COPY --from=build /app/Inno_Shop.Products.API/out ./
ENTRYPOINT ["dotnet", "Inno_Shop.Products.API.dll"]