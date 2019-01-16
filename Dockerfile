FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet restore

# Copy everything else and build
WORKDIR /app/API
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/API/out .
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "API.dll"]
