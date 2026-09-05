# Stage 1 : build avec le SDK complet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copie le .csproj et restaure les dépendances (couche cachée)
COPY "comptabilité/comptabilité.csproj" "comptabilité/"
RUN dotnet restore "comptabilité/comptabilité.csproj"

# Copie le reste et publie en mode Release
COPY "comptabilité/" "comptabilité/"
RUN dotnet publish "comptabilité/comptabilité.csproj" -c Release -o /app/publish --no-restore

# Stage 2 : runtime léger (sans SDK)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS app
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "NexFlux.dll"]
