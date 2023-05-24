#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
ENV ASPNETCORE_URLS=https://+:443
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["JustChat.API/JustChat.API.csproj", "JustChat.API/"]
COPY ["DAL/JustChat.DAL.csproj", "DAL/"]
COPY ["BLL/JustChat.BLL.csproj", "BLL/"]
COPY ["JustChat.RabbitMQ/JustChat.RabbitMQ.csproj", "JustChat.RabbitMQ/"]
RUN dotnet restore "JustChat.API/JustChat.API.csproj"
COPY . .
WORKDIR "/src/JustChat.API"
RUN dotnet build "JustChat.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JustChat.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JustChat.API.dll"]