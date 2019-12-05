FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Moksnes.BuyStuff.Silo/Moksnes.BuyStuff.Silo.csproj", "Moksnes.BuyStuff.Silo/"]
COPY ["Moksnes.BuyStuff.Grains/Moksnes.BuyStuff.Grains.csproj", "Moksnes.BuyStuff.Grains/"]
COPY ["Moksnes.BuyStuff.GrainInterfaces/Moksnes.BuyStuff.GrainInterfaces.csproj", "Moksnes.BuyStuff.GrainInterfaces/"]
RUN dotnet restore "Moksnes.BuyStuff.Silo/Moksnes.BuyStuff.Silo.csproj"
COPY . .
WORKDIR "/src/Moksnes.BuyStuff.Silo"
RUN dotnet build "Moksnes.BuyStuff.Silo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Moksnes.BuyStuff.Silo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Moksnes.BuyStuff.Silo.dll"]