# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Payroll.API/Payroll.API.csproj", "Payroll.API/"]
COPY ["Payroll.Application/Payroll.Application.csproj", "Payroll.Application/"]
COPY ["Payroll.Domain/Payroll.Domain.csproj", "Payroll.Domain/"]
COPY ["Payroll.Infrastructure/Payroll.Infrastructure.csproj", "Payroll.Infrastructure/"]
RUN dotnet restore "Payroll.API/Payroll.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Payroll.API"
RUN dotnet build "Payroll.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Payroll.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published app
COPY --from=publish /app/publish .

# Create directory for SQLite database
RUN mkdir -p /app/data

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Payroll.API.dll"]