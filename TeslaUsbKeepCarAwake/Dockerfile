#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TeslaUsbKeepCarAwake/TeslaUsbKeepCarAwake.csproj", "TeslaUsbKeepCarAwake/"]
RUN dotnet restore "TeslaUsbKeepCarAwake/TeslaUsbKeepCarAwake.csproj"
COPY . .
WORKDIR "/src/TeslaUsbKeepCarAwake"
RUN dotnet build "TeslaUsbKeepCarAwake.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeslaUsbKeepCarAwake.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# install cifs-utils package in this linux container 
RUN apt-get update && apt-get install -y cifs-utils
ENTRYPOINT ["dotnet", "TeslaUsbKeepCarAwake.dll"]

