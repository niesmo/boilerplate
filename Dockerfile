FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BoilerplateApp.Web/BoilerplateApp.Web.csproj", "BoilerplateApp.Web/"]
COPY ["BoilerplateApp.ServiceDefaults/BoilerplateApp.ServiceDefaults.csproj", "BoilerplateApp.ServiceDefaults/"]
RUN dotnet restore "BoilerplateApp.Web/BoilerplateApp.Web.csproj"

COPY BoilerplateApp.Web/ BoilerplateApp.Web/
COPY BoilerplateApp.ServiceDefaults/ BoilerplateApp.ServiceDefaults/

WORKDIR /src/BoilerplateApp.Web
RUN dotnet publish "BoilerplateApp.Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "BoilerplateApp.Web.dll"]
