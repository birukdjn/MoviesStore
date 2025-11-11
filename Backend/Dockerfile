# Development image with hot reload
FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app

# Copy csproj and restore dependencies
COPY Backend.csproj ./

RUN dotnet restore

# Copy all source code
COPY . .

# Expose API port
EXPOSE 8080

# Start API with hot reload
CMD ["dotnet", "watch", "run", "--urls", "http://+:8080"]
