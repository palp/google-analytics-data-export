FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Google.Analytics.Data.Export.Service.csproj", "."]
RUN dotnet restore "Google.Analytics.Data.Export.Service.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Google.Analytics.Data.Export.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Google.Analytics.Data.Export.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Google.Analytics.Data.Export.Service.dll"]
