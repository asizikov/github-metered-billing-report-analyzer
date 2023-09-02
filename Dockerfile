FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/ActionsUsageAnalyser.Domain/*.csproj ./ActionsUsageAnalyser.Domain/
COPY src/ActionsUsageAnalyzer.Cli/*.csproj ./ActionsUsageAnalyzer.Cli/
RUN dotnet restore ./ActionsUsageAnalyzer.Cli/ActionsUsageAnalyzer.Cli.csproj

# Copy everything else and build
COPY src/ActionsUsageAnalyser.Domain ./ActionsUsageAnalyser.Domain/
COPY src/ActionsUsageAnalyzer.Cli ./ActionsUsageAnalyzer.Cli/
RUN dotnet publish ./ActionsUsageAnalyzer.Cli/ActionsUsageAnalyzer.Cli.csproj -c Release -o out
RUN ls /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.
ENV INPUT_DIRECTORY=/input
ENV OUTPUT_DIRECTORY=/output

WORKDIR /app
COPY --from=build-env /app/out ./
ENTRYPOINT ["dotnet", "ActionsUsageAnalyzer.Cli.dll"]