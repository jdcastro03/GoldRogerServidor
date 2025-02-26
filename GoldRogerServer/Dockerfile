FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia todo el contenido del proyecto
COPY . .

# Localiza y restaura el proyecto principal (asumiendo que est� en la ra�z)
RUN dotnet restore 

# Construye y publica la aplicaci�n
RUN dotnet publish -c Release -o out

# Imagen de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80
EXPOSE 5024
ENTRYPOINT ["dotnet", "GoldRogerServer.dll"]