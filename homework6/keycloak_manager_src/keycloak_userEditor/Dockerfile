FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV Keycloak:AuthServerUrl=""
ENV Keycloak:Realm=""
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["keycloak_userEditor.csproj", "./"]
RUN dotnet restore "keycloak_userEditor.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "keycloak_userEditor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "keycloak_userEditor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "keycloak_userEditor.dll"]
