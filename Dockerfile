FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["UserManagementService/UserManagementService.csproj", "UserManagementService/"]
COPY ["Data.Layer/Data.Layer.csproj", "Data.Layer/"]
COPY ["Service.Layer/Service.Layer.csproj", "Service.Layer/"]

RUN dotnet restore "UserManagementService/UserManagementService.csproj"

COPY . .

WORKDIR "/src/UserManagementService"
RUN dotnet build "UserManagementService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserManagementService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "UserManagementService.dll"]
