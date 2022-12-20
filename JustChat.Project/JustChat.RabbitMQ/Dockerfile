#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["JustChat.RabbitMQ/JustChat.RabbitMQ.csproj", "JustChat.RabbitMQ/"]
COPY ["BLL/JustChat.BLL.csproj", "BLL/"]
COPY ["DAL/JustChat.DAL.csproj", "DAL/"]
RUN dotnet restore "JustChat.RabbitMQ/JustChat.RabbitMQ.csproj"
COPY . .
WORKDIR "/src/JustChat.RabbitMQ"
RUN dotnet build "JustChat.RabbitMQ.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JustChat.RabbitMQ.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JustChat.RabbitMQ.dll"]