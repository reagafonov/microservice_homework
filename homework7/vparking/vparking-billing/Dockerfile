FROM cr.yandex/crpo2q44fm7kalr36b4h/sdk9 AS sdk
ARG BUILD_CONFIGURATION=Debug
EXPOSE 80
EXPOSE 22

WORKDIR /src

COPY vparking-billing/src . 


RUN dotnet restore

RUN dotnet build -c $BUILD_CONFIGURATION

RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM cr.yandex/crpo2q44fm7kalr36b4h/asp9-debug as final
WORKDIR /app
COPY --from=sdk /app/publish .

ENTRYPOINT  ["bash","-c", "/etc/init.d/ssh start && dotnet VParkingBilling.dll"]     