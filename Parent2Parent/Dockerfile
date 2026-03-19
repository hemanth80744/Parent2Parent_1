# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY Parent2Parent.csproj ./
RUN dotnet restore

# Copy the remaining files and build the app
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Expose the port (Render provides $PORT, ASP.NET Core 8+ uses ASPNETCORE_HTTP_PORTS)
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Set the entry point for the container
ENTRYPOINT ["dotnet", "Parent2Parent.dll"]
