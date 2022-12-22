#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NetFlex.WEB/NetFlex.WEB.csproj", "NetFlex.WEB/"]
COPY ["NetFlex.DAL/NetFlex.DAL.csproj", "NetFlex.DAL/"]
COPY ["NetFlex.BLL/NetFlex.BLL.csproj", "NetFlex.BLL/"]
RUN dotnet restore "NetFlex.WEB/NetFlex.WEB.csproj"
COPY . .
WORKDIR "/src/NetFlex.WEB"
RUN dotnet build "NetFlex.WEB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NetFlex.WEB.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetFlex.WEB.dll"]