FROM microsoft/dotnet:2.0.7-runtime-stretch AS base

RUN apt-get update && \
    apt-get install -y --no-install-recommends unzip procps libusb-1.0-0-dev usbutils && \
    curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l ~/vsdbg && \
    rm -rf /var/lib/apt/lists/*

FROM microsoft/dotnet:2.0-sdk AS build-env
WORKDIR /app
COPY nuget.config ./
COPY *.csproj ./
RUN dotnet restore --configfile nuget.config

COPY . ./
COPY reliance_1.27.171.ptix /app/out/
RUN dotnet publish -c Debug /p:Platform=x64 -o out
FROM base
WORKDIR /app
COPY --from=build-env /app/out ./