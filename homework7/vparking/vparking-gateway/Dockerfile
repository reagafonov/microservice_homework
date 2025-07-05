FROM cr.yandex/crpo2q44fm7kalr36b4h/asp9-debug AS base
#USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 22



ENV ADMIN_CLIENT_ID = "admin-cli" 
ENV CLIENT_ID="vparking"

ENV CLIENT_SECRET = ""

ENV URL = "http://keycloak.keycloak.svc.cluster.local" 
ENV USER_NAME = "user"
ENV USER_PASSWORD = "bitnami" 
ENV REALM = "vparking"

ENV BILLING_SERVICE_ADDRESS = "vparing-billing"

FROM cr.yandex/crpo2q44fm7kalr36b4h/sdk9  AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /vparking/src
COPY ["vparking-gateway/src/vparking-users.csproj", "./"]    
RUN dotnet restore "vparking-users.csproj"
WORKDIR /Common/src
COPY Common/src . 

WORKDIR /vparking/src
COPY vparking-gateway/src .

RUN dotnet build "vparking-users.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "vparking-users.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["bash","-c", "/etc/init.d/ssh start && dotnet vparking-users.dll"]     