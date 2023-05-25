#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Netflex.Statistic.Server/Netflex.Statistic.Server.csproj", "Netflex.Statistic.Server/"]
COPY ["Kafka.Consumer/Kafka.Consumer.csproj", "Kafka.Consumer/"]
RUN dotnet restore "Netflex.Statistic.Server/Netflex.Statistic.Server.csproj"
COPY . .
WORKDIR "/src/Netflex.Statistic.Server"
RUN dotnet build "Netflex.Statistic.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Netflex.Statistic.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Netflex.Statistic.Server.dll"]