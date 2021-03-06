FROM microsoft/dotnet:2.1-sdk
ENV NUGET_XMLDOC_MODE skip
WORKDIR /vsdbg
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       unzip libusb-1.0-0-dev usbutils inotify-tools \
    && rm -rf /var/lib/apt/lists/* \
    && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

RUN mkdir /app
WORKDIR /app

COPY nuget.config ./
COPY *.csproj /app
COPY monitor_usb.sh /app
RUN dotnet restore --configfile nuget.config

COPY *.cs /app/
RUN dotnet publish -c Debug /p:Platform=x64 -o out
COPY reliance_1.27.171.ptix /app/out/
COPY NLog.config /app/out/

RUN /app/monitor_usb.sh &

# Kick off a container just to wait debugger to attach and run the app
ENTRYPOINT ["/bin/bash", "-c", "sleep infinity"]
