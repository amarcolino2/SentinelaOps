# Multi-stage build for SentinelaOps API
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /src

# Copy solution and project files
COPY ["SentinelaOps.sln", "./"]
COPY ["src/SentinelaOps.Domain/SentinelaOps.Domain.csproj", "src/SentinelaOps.Domain/"]
COPY ["src/SentinelaOps.Application/SentinelaOps.Application.csproj", "src/SentinelaOps.Application/"]
COPY ["src/SentinelaOps.Infrastructure/SentinelaOps.Infrastructure.csproj", "src/SentinelaOps.Infrastructure/"]
COPY ["src/SentinelaOps.Api/SentinelaOps.Api.csproj", "src/SentinelaOps.Api/"]
COPY ["src/SentinelaOps.Skills.Abstractions/SentinelaOps.Skills.Abstractions.csproj", "src/SentinelaOps.Skills.Abstractions/"]
COPY ["src/SentinelaOps.Skills.Perimeter/SentinelaOps.Skills.Perimeter.csproj", "src/SentinelaOps.Skills.Perimeter/"]
COPY ["src/SentinelaOps.Skills.Intrusion/SentinelaOps.Skills.Intrusion.csproj", "src/SentinelaOps.Skills.Intrusion/"]
COPY ["src/SentinelaOps.Skills.FalsePositive/SentinelaOps.Skills.FalsePositive.csproj", "src/SentinelaOps.Skills.FalsePositive/"]
COPY ["src/SentinelaOps.Skills.Severity/SentinelaOps.Skills.Severity.csproj", "src/SentinelaOps.Skills.Severity/"]
COPY ["src/SentinelaOps.Skills.Summary/SentinelaOps.Skills.Summary.csproj", "src/SentinelaOps.Skills.Summary/"]

# Restore packages
RUN dotnet restore "SentinelaOps.sln"

# Copy source code
COPY . .

# Build API project
RUN dotnet build "src/SentinelaOps.Api/SentinelaOps.Api.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "src/SentinelaOps.Api/SentinelaOps.Api.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files from builder
COPY --from=builder /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=10s --retries=5 --start-period=40s \
  CMD curl -f http://localhost:8080/health || exit 1

# Expose ports
EXPOSE 8080 8081

# Set environment
ENV ASPNETCORE_URLS="http://+:8080;https://+:8081"
ENV ASPNETCORE_ENVIRONMENT="Production"

# Run API
ENTRYPOINT ["dotnet", "SentinelaOps.Api.dll"]
