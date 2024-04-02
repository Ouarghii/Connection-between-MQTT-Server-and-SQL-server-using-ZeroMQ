# # Use the official .NET SDK image for building the application
# FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

# # Set the working directory in the container
# WORKDIR /app

# # Copy the project file and restore dependencies
# COPY *.csproj ./
# RUN dotnet restore

# # Copy the remaining source code
# COPY . ./

# # Build the application
# RUN dotnet publish -c Release -o out

# # Build the runtime image
# FROM mcr.microsoft.com/dotnet/runtime:7.0
# WORKDIR /app
# COPY --from=build-env /app/out .
# COPY config.json .

# # Command to run the application
# ENTRYPOINT ["dotnet", "MqttConnector.dll"]

# Use the .NET Core SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src

# Copy the project file and restore dependencies
COPY ["MqttConnector.csproj", ""]
RUN dotnet restore "./MqttConnector.csproj"

# Copy the application code
COPY . .

# Build the application
RUN dotnet build "MqttConnector.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "MqttConnector.csproj" -c Release -o /app/publish

# Use the runtime image as the final base
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS final

WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Specify the command to run the application
CMD ["dotnet", "MqttConnector.dll"]

