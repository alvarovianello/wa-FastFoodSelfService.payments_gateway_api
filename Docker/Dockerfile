FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

ENV ASPNETCORE_URLS=http://+:5003
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 5003

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