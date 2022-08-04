FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["services/Prism.Picshare.Services.Workers/Prism.Picshare.Services.Workers.csproj", "Prism.Picshare.Services.Workers/"]
RUN dotnet restore "services/Prism.Picshare.Services.Workers/Prism.Picshare.Services.Workers.csproj"
COPY . .
WORKDIR "/src/Prism.Picshare.Services.Workers"
RUN dotnet build "Prism.Picshare.Services.Workers.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Prism.Picshare.Services.Workers.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Prism.Picshare.Services.Workers.dll"]
