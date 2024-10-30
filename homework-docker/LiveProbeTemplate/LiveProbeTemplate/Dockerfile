FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
ARG BUILD_CONFIGURATION=Release
EXPOSE 8080
WORKDIR /src
COPY ["LiveProbeTemplate/LiveProbeTemplate.csproj", "LiveProbeTemplate/"]
ARG BUILD_CONFIGURATION=Release
RUN dotnet restore "LiveProbeTemplate/LiveProbeTemplate.csproj"
COPY . .
WORKDIR "/src/LiveProbeTemplate"
RUN dotnet build "LiveProbeTemplate.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM base AS publish
RUN dotnet publish "LiveProbeTemplate.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "LiveProbeTemplate.dll"]