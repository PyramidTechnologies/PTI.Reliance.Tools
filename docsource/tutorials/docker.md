---
uid: tut_docker
---
# Docker Support
The abstraction mechanisms of Docker make certain types of hardware integration quite challenging in Docker. As long as your device is not removed and reinserted during the lifetime of your container you will likely not have any issues. However, certain operations like our flash update process generate USB reset event that will confound Docker. This tutorial explains solutions for this particular problem.

> [!IMPORTANT]
> Only Linux hosts are supported for Docker containers

## Privileged
The most reliable implementation we've tested leverages the ```--privileged``` flag and the mounting of ```/dev/bus/usb``` volume from host to container. Your results may vary and you may find this to be unnecessary for your environment.
```sh
sudo docker run --rm -it \
     -v /dev/bus/usb:/dev/bus/usb \
     --privileged \
     <YOUR_CONTAINER>:<VERSION>
```

## Host Permissions
Your host system may require additional permissions or even udev rules in order to reliably attach to your Reliance Thermal Printer. This is typically achieved by adding your user to the dialout group or equivalent for your environment. Another option is to create a udev rule, ```/etc/udev/rules.d/99-udev.rules``` with the following contents.
[!code[Main](99-hid.rules.sample)]

# Special Configurations
A pre-made configuration for the Debian Stretch Docker image can be configured with the following in your application start up code.
```cs
PTIRelianceLib.Library.Options = PTIRelianceLib.LibraryOptions.DockerLinuxStretch;
```

[!include[<Customization>](<customization.md>)]
