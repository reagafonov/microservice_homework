FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
ARG BUILD_CONFIGURATION=Release
EXPOSE 8080

WORKDIR /src
COPY [".", "."]
ARG BUILD_CONFIGURATION=Release
RUN dotnet restore "VParkingSettings.csproj"
COPY . .
WORKDIR "/src/VParkingSettings"
RUN dotnet build "VParkingSettings.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM base AS publish
RUN dotnet publish "VParkingSettings.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "VParkingSettings.dll"]