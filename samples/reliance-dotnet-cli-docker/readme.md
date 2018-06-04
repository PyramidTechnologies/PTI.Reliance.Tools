# CLI Runnin Inside Docker
This demonstrates how to run an debug your code inside a container. This is useful for rapidly debugging HID access inside your Docker container.

# Nuget
Your nuget cache might need to be cleared if you are iterating quickly.
```
    dotnet nuget locals all --clear
```

# Debugging
The image is built using the script dockerTask.sh in the scripts directory. This is called automatically by the default vs code launch task. All you should have to do is click the green debug button in VS Code to start debugging your PTIRelianceLib application inside your container.

If you don't want to debug and prefer to run interactively:
- 1 Comment out the entry point line in the Dockerfile
- 2 Run ./scripts/dockerTask.sh interactive
- 3 Run sudo docker run --rm -it --privileged -v /dev/bus/usb:/dev/bus/usb docker.dotnet.debug:latest /bin/bash

Note that you are debugging a library, you must have your symbols pushed to a symbol server listed in launch.json else you won't be able to step into your library code. See symbolOptions for more options and details.

# monitor_usb.sh
This is a debugging script that monitor the arrival and departure of the Reliance Printer. If you don't want to use it, omit usbutils and inotify-tools from your Docker setup (apt-get install ...).

## Credits
Debugging learned from https://github.com/sleemer/docker.dotnet.debug
Some changes were made to make compatible with the lastest VS Code but otherwise, this is based on their work.