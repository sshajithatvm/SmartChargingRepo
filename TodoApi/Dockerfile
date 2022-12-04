FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ENV ASPNETCORE_ENVIRONMENT=Development
WORKDIR /source
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 5000
ENV ASPNETCORE_HTTPS_PORT=https://+5001
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "PhoneNumbersApi.dll"]