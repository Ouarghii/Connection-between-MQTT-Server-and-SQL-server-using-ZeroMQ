# Use the SDK image as the base for building
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set the working directory inside the container
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["SQLServerConnector.csproj", ""]
RUN dotnet restore "./SQLServerConnector.csproj"

# Copy the remaining source code
COPY . .

# Build the application
WORKDIR "/src/."
RUN dotnet build "SQLServerConnector.csproj" -c Release -o /app/build

# Stage for publishing the application
FROM build AS publish

# Publish the application
RUN dotnet publish "SQLServerConnector.csproj" -c Release -o /app/publish

# Use the runtime image as the final base
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS final

# Set the working directory inside the container
WORKDIR /app

# Copy the published application and configuration files from the publish stage
COPY --from=publish /app/publish .
COPY config.json .

# Set the entry point for the container
CMD ["dotnet", "SQLServerConnector.dll"]


