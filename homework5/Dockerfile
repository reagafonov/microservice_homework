FROM cr.yandex/crpo2q44fm7kalr36b4h/dotnet-ef-9:v2 AS sdk
ARG BUILD_CONFIGURATION=Release
EXPOSE 80
WORKDIR /src
RUN mkdir /.nuget

COPY . . 

RUN dotnet restore

RUN dotnet build -c $BUILD_CONFIGURATION 

RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=sdk /app/publish .

ENTRYPOINT ["dotnet", "VParkingSettings.dll"]