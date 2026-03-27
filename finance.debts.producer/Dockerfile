FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore finance-debts-producer/finance.debts.producer/finance.debts.producer.csproj

RUN dotnet publish finance-debts-producer/finance.debts.producer/finance.debts.producer.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "finance.debts.producer.dll"]