# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy and restore
COPY ["Ekklesia.Api.csproj", "./"]
RUN dotnet restore "Ekklesia.Api.csproj"

# Copy everything and build
COPY . .
RUN dotnet publish "Ekklesia.Api.csproj" -c Release -o /app/publish

# Runtime stage  
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Ekklesia.Api.dll"]