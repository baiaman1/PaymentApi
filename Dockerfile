# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем весь проект
COPY . .

# Восстанавливаем зависимости и публикуем
RUN dotnet restore
RUN dotnet publish PaymentApi.Api/PaymentApi.Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Настройка порта
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "PaymentApi.Api.dll"]
