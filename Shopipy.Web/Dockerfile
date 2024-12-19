FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Shopipy.Web/Shopipy.Web.csproj", "Shopipy.Web/"]
COPY ["Shopipy.ServiceDefaults/Shopipy.ServiceDefaults.csproj", "Shopipy.ServiceDefaults/"]
RUN dotnet restore "Shopipy.Web/Shopipy.Web.csproj"
COPY . .
WORKDIR "/src/Shopipy.Web"
RUN dotnet build "Shopipy.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Shopipy.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shopipy.Web.dll"]
