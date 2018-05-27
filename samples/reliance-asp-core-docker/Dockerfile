FROM microsoft/dotnet:latest-runtime-stretch AS base

# set up network
ENV ASPNETCORE_URLS http://+:80
ENV ASPNETCORE_PKG_VERSION 2.0.8

# set up the runtime store
RUN for version in '2.0.0' '2.0.3' '2.0.5' '2.0.6' '2.0.7' '2.0.8'; do \
        curl -o /tmp/runtimestore.tar.gz https://dist.asp.net/runtimestore/$version/linux-x64/aspnetcore.runtimestore.tar.gz \
        && export DOTNET_HOME=$(dirname $(readlink $(which dotnet))) \
        && tar -x -C $DOTNET_HOME -f /tmp/runtimestore.tar.gz \
        && rm /tmp/runtimestore.tar.gz; \
done

RUN apt-get update && \
    apt-get install -y --no-install-recommends unzip procps libusb-1.0-0-dev  && \
    curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l ~/vsdbg && \
    rm -rf /var/lib/apt/lists/*

FROM microsoft/dotnet:2.0-sdk AS build-env
WORKDIR /app
COPY nuget.config ./
COPY *.csproj ./
RUN dotnet restore --configfile nuget.config

COPY . ./
RUN dotnet publish -c Debug -o out
FROM base
WORKDIR /app
COPY --from=build-env /app/out ./
ENTRYPOINT ["dotnet", "reliance-asp-core-docker.dll"]