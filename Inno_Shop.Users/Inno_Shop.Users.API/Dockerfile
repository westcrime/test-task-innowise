# Use the official .NET Core SDK as a parent image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy the project file and restore any dependencies (use .csproj for the project name)
COPY *.sln .
COPY Inno_Shop.Users.API/*.csproj ./Inno_Shop.Users.API/
COPY Inno_Shop.Users.Application/*.csproj ./Inno_Shop.Users.Application/
COPY Inno_Shop.Users.Domain/*.csproj ./Inno_Shop.Users.Domain/
COPY Inno_Shop.Users.Infrastructure/*.csproj ./Inno_Shop.Users.Infrastructure/
COPY Inno_Shop.Users.API.IntegrationTests/*.csproj ./Inno_Shop.Users.API.IntegrationTests/
COPY Inno_Shop.Users.Tests/*.csproj ./Inno_Shop.Users.Tests/

RUN dotnet restore

# Copy the rest of the application code
COPY Inno_Shop.Users.API/. ./Inno_Shop.Users.API/
COPY Inno_Shop.Users.Application/. ./Inno_Shop.Users.Application/
COPY Inno_Shop.Users.Domain/. ./Inno_Shop.Users.Domain/
COPY Inno_Shop.Users.Infrastructure/. ./Inno_Shop.Users.Infrastructure/
COPY Inno_Shop.Users.API.IntegrationTests/. ./Inno_Shop.Users.API.IntegrationTests/
COPY Inno_Shop.Users.Tests/. ./Inno_Shop.Users.Tests/

# Publish the application
WORKDIR /app/Inno_Shop.Users.API
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Expose the port your application will run on
EXPOSE 5098

# Start the application
COPY --from=build /app/Inno_Shop.Users.API/out ./
ENTRYPOINT ["dotnet", "Inno_Shop.Users.API.dll"]