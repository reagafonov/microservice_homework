FROM alpine:3.2.03 AS base
ARG BUILD_CONFIGURATION=Release
RUN apk update
RUN apk add dotnet8-sdk
RUN apk add dotnet8-runtime
EXPOSE 8000
WORKDIR /app

WORKDIR /src
COPY ["LiveProbeTemplate/DockerHealthApp.sln", "DockerHealthApp/"]
ARG BUILD_CONFIGURATION=Release
RUN dotnet restore "LiveProbeTemplate/DockerHealthApp.sln"
COPY . .
WORKDIR "/src/LiveProbeTemplate"
RUN dotnet build "LiveProbeTemplate.sln" -c $BUILD_CONFIGURATION -o /app/build

FROM base AS publish
RUN dotnet publish "LiveProbeTemplate.sln" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "LiveProbeTemplate.dll"]