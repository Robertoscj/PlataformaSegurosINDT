FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["src/PropostaService/PropostaService.API/PropostaService.API.csproj", "PropostaService.API/"]
COPY ["src/PropostaService/PropostaService.Application/PropostaService.Application.csproj", "PropostaService.Application/"]
COPY ["src/PropostaService/PropostaService.Domain/PropostaService.Domain.csproj", "PropostaService.Domain/"]
COPY ["src/PropostaService/PropostaService.Infrastructure/PropostaService.Infrastructure.csproj", "PropostaService.Infrastructure/"]

# Restaurar dependências
RUN dotnet restore "PropostaService.API/PropostaService.API.csproj"

# Copiar código fonte
COPY src/PropostaService/ .

# Build
WORKDIR "/src/PropostaService.API"
RUN dotnet build "PropostaService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PropostaService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PropostaService.API.dll"]

