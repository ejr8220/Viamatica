FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Viamatica.sln", "./"]
COPY ["Viamatica.API/Viamatica.API.csproj", "Viamatica.API/"]
COPY ["Viamatica.Application/Viamatica.Application.csproj", "Viamatica.Application/"]
COPY ["Viamatica.Domain/Viamatica.Domain.csproj", "Viamatica.Domain/"]
COPY ["Viamatica.Infrastructure/Viamatica.Infrastructure.csproj", "Viamatica.Infrastructure/"]

RUN dotnet restore "Viamatica.sln"

COPY . .
RUN dotnet publish "Viamatica.API/Viamatica.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Viamatica.API.dll"]
