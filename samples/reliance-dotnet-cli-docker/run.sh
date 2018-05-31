dotnet nuget locals all --clear
sudo docker build -t rcli .
sudo docker run --rm -it --privileged -v /dev/bus/usb:/dev/bus/usb rcli