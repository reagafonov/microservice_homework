FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
RUN apt-get update && apt-get install -y unzip procps curl bash openssh-server &&  rm -rf /var/lib/apt/lists/*
RUN echo 'root:qwerty' | chpasswd
RUN echo "PasswordAuthentication yes" >> /etc/ssh/sshd_config
RUN echo "PermitRootLogin yes" >> /etc/ssh/sshd_config
RUN echo "AllowTcpForwarding yes" >> /etc/ssh/sshd_config
RUN update-rc.d ssh enable
#USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 22

ENV Keycloak:AuthServerUrl=""
ENV Keycloak:Realm=""
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["vparking-users.csproj", "./"]
RUN dotnet restore "vparking-users.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "vparking-users.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "vparking-users.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["bash","-c", "/etc/init.d/ssh start && dotnet vparking-users.dll"]
