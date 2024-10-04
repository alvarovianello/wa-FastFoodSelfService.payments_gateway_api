FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

ENV ASPNETCORE_URLS=http://+:5003
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 5003

# Instalar as dependências nativas necessárias para o SkiaSharp
RUN apt-get update && apt-get install -y \
    libc6 \
    libfontconfig1 \
    libfreetype6 \
    libharfbuzz0b \
    libpng16-16 \
    libx11-6 \
    libxext6 \
    libxrender1 \
    libxcb1 \
    libx11-xcb1 \
    libxdamage1 \
    libxfixes3 \
    libxxf86vm1 \
    libgbm1 \
    libasound2

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ./Api/Api.csproj Api/
COPY ./Application/Application.csproj Application/
COPY ./Infrastructure/Infrastructure.csproj Infrastructure/
COPY ./Domain/Domain.csproj Domain/
COPY ./CrossCutting/CrossCutting.csproj CrossCutting/

RUN dotnet restore Api/Api.csproj

COPY . .

RUN dotnet build Api/Api.csproj -c Release -o /app

FROM build AS publish

RUN dotnet publish Api/Api.csproj -c Release -o /app

FROM base AS final

WORKDIR /app

COPY --from=publish /app .

ENTRYPOINT ["dotnet", "Api.dll"]