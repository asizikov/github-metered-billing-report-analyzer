FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/ActionsUsageAnalyser.Domain/*.csproj ./ActionsUsageAnalyser.Domain/
COPY src/ActionsUsageAnalyzer.Infrastructure/*.csproj ./ActionsUsageAnalyzer.Infrastructure/
COPY src/ActionsUsageAnalyzer.Cli/*.csproj ./ActionsUsageAnalyzer.Cli/
RUN dotnet restore ./ActionsUsageAnalyzer.Cli/ActionsUsageAnalyzer.Cli.csproj

# Copy everything else and build
COPY src/ActionsUsageAnalyser.Domain ./ActionsUsageAnalyser.Domain/
COPY src/ActionsUsageAnalyzer.Infrastructure ./ActionsUsageAnalyzer.Infrastructure/
COPY src/ActionsUsageAnalyzer.Cli ./ActionsUsageAnalyzer.Cli/
RUN dotnet publish ./ActionsUsageAnalyzer.Cli/ActionsUsageAnalyzer.Cli.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.0
ENV INPUT_DIRECTORY=/input
ENV OUTPUT_DIRECTORY=/output

WORKDIR /app
COPY --from=build-env /app/out ./

RUN useradd -m analyzer && \
    chown -R analyzer:analyzer /app && \
    mkdir /input && \
    mkdir /output && \
    chown -R analyzer:analyzer /input && \
    chown -R analyzer:analyzer /output

USER analyzer

ENTRYPOINT ["dotnet", "ActionsUsageAnalyzer.Cli.dll"]