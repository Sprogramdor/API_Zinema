#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Zinema.API/Zinema.API.csproj", "Zinema.API/"]
COPY ["Zinema.Modelo/Zinema.Modelo.csproj", "Zinema.Modelo/"]
RUN dotnet restore "Zinema.API/Zinema.API.csproj"
COPY . .
WORKDIR "/src/Zinema.API"
RUN dotnet build "Zinema.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Zinema.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Zinema.API.dll"]