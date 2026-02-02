FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Desafio.Umbler.Front.csproj", "."]
RUN dotnet restore "Desafio.Umbler.Front.csproj"
COPY . .
RUN dotnet publish "Desafio.Umbler.Front.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Desafio.Umbler.Front.dll"]
