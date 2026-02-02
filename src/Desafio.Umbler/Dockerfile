
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Desafio.Umbler.csproj", "."]
RUN dotnet restore "Desafio.Umbler.csproj"
COPY . .
RUN dotnet publish "Desafio.Umbler.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Desafio.Umbler.dll"]