FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["src/ContratacaoService/ContratacaoService.API/ContratacaoService.API.csproj", "ContratacaoService.API/"]
COPY ["src/ContratacaoService/ContratacaoService.Application/ContratacaoService.Application.csproj", "ContratacaoService.Application/"]
COPY ["src/ContratacaoService/ContratacaoService.Domain/ContratacaoService.Domain.csproj", "ContratacaoService.Domain/"]
COPY ["src/ContratacaoService/ContratacaoService.Infrastructure/ContratacaoService.Infrastructure.csproj", "ContratacaoService.Infrastructure/"]

# Restaurar dependências
RUN dotnet restore "ContratacaoService.API/ContratacaoService.API.csproj"

# Copiar código fonte
COPY src/ContratacaoService/ .

# Build
WORKDIR "/src/ContratacaoService.API"
RUN dotnet build "ContratacaoService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContratacaoService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContratacaoService.API.dll"]

