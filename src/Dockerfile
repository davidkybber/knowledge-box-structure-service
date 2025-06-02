# Use the official .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy all source code and build the application
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official .NET 8.0 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' --uid 1000 appuser && chown -R appuser /app
USER appuser

# Copy the published application from the build stage
COPY --from=build /app/out .

# Expose the port that the application listens on
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Define the entry point for the container
ENTRYPOINT ["dotnet", "KnowledgeBox.Structure.dll"] 