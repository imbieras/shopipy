FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Shopipy.ApiService/Shopipy.ApiService.csproj", "Shopipy.ApiService/"]
COPY ["Shopipy.ServiceDefaults/Shopipy.ServiceDefaults.csproj", "Shopipy.ServiceDefaults/"]
RUN dotnet restore "Shopipy.ApiService/Shopipy.ApiService.csproj"
COPY . .
WORKDIR "/src/Shopipy.ApiService"
RUN dotnet build "Shopipy.ApiService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Shopipy.ApiService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shopipy.ApiService.dll"]
