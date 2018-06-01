FROM microsoft/dotnet:2.0.7-runtime-stretch AS base
ENV NUGET_XMLDOC_MODE skip
WORKDIR /vsdbg

RUN apt-get update && \
    apt-get install -y --no-install-recommends unzip procps libusb-1.0-0-dev usbutils && \
    curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg && \
    rm -rf /var/lib/apt/lists/*

# Copying source files into container and publish it
RUN mkdir /app
WORKDIR /app
COPY nuget.config ./
COPY *.csproj /app
RUN dotnet restore --configfile nuget.config
COPY . /app
COPY reliance_1.27.171.ptix /app/out/
COPY ../../PTIRelianceLib/ /app/out/src/
RUN dotnet publish -c Debug /p:Platform=x64 -o out

# Kick off a container just to wait debugger to attach and run the app
ENTRYPOINT ["/bin/bash", "-c", "sleep infinity"]