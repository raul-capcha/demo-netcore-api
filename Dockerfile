# =========================
# STAGE 1: Build & Publish
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos los csproj primero (para cache de restore)
COPY Demo.Api/Demo.Api.csproj Demo.Api/
COPY Demo.Security.Application/Demo.Security.Application.csproj Demo.Security.Application/
COPY Demo.Security.Domain/Demo.Security.Domain.csproj Demo.Security.Domain/
COPY Demo.Security.Infrastructure/Demo.Security.Infrastructure.csproj Demo.Security.Infrastructure/

# Restaura dependencias de la API principal (arrastrará las otras)
RUN dotnet restore Demo.Api/Demo.Api.csproj

# Copiamos el resto del código
COPY . .

# Publicamos
RUN dotnet publish Demo.Api/Demo.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# =========================
# STAGE 2: Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# La imagen aspnet suele exponer 8080. Forzamos sólo HTTP en 0.0.0.0:8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

# (Opcional) Ajusta el entorno explícitamente a Production
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "Demo.Api.dll"]
