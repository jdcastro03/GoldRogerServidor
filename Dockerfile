FROM bitnami/dotnet-sdk:latest AS build
WORKDIR webapp

EXPOSE 80
EXPOSE 5024

# Copy csproj and restore as distinct layers
COPY ./*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out

# Build runtime image
FROM bitnami/dotnet-aspnet:latest
WORKDIR /webapp
COPY --from=build /webapp/out .
ENTRYPOINT ["dotnet", "GoldRogerServer.dll"]