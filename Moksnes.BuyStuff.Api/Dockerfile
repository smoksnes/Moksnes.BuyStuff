FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Moksnes.BuyStuff.Api/Moksnes.BuyStuff.Api.csproj", "Moksnes.BuyStuff.Api/"]
RUN dotnet restore "Moksnes.BuyStuff.Api/Moksnes.BuyStuff.Api.csproj"
COPY . .
WORKDIR "/src/Moksnes.BuyStuff.Api"
RUN dotnet build "Moksnes.BuyStuff.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Moksnes.BuyStuff.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Moksnes.BuyStuff.Api.dll"]