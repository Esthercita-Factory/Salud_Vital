# Etapa 1: compilar con el SDK completo
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS compilacion
WORKDIR /src

# El .csproj se copia antes que el código para aprovechar la caché de NuGet.
COPY SaludVital/SaludVital.csproj SaludVital/
RUN dotnet restore SaludVital/SaludVital.csproj

COPY SaludVital/ SaludVital/
RUN dotnet publish SaludVital/SaludVital.csproj -c Release -o /publicar

# Etapa 2: ejecutar las pruebas (sólo se construye si se pide target: pruebas)
FROM compilacion AS pruebas
COPY SaludVital.Tests/ SaludVital.Tests/
COPY SaludVital.slnx .
RUN dotnet test SaludVital.slnx --no-restore --logger "console;verbosity=minimal"

# Etapa 3: imagen final con el runtime de ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=compilacion /publicar .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
USER app

ENTRYPOINT ["dotnet", "SaludVital.dll"]
