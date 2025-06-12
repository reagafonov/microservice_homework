FROM cr.yandex/crpo2q44fm7kalr36b4h/dotnet-ef-9:v2 AS sdk
EXPOSE 80

WORKDIR /Common/src
COPY Common/src . 

WORKDIR /vparking/src
COPY vparking-notification/src .

RUN dotnet restore

ENV Kafka.BootstrapServers=""
ENV Kafka.Topic=""
ENV Kafka.GroupId=""
EXPOSE 22

ARG BUILD_CONFIGURATION=Debug

RUN dotnet build -c $BUILD_CONFIGURATION

RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM cr.yandex/crpo2q44fm7kalr36b4h/asp9-debug as final
WORKDIR /app
COPY --from=sdk /app/publish .

ENTRYPOINT  ["bash","-c", "/etc/init.d/ssh start && dotnet VParkingNotification.dll"]     