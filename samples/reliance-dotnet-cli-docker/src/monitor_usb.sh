#!/bin/bash
# Monitor USB device arrival and removal

# Configuration
vid="0425"
pid="8147"
idstr="${vid}:${pid}"

# Checks if target is connected or not, prints message to console
checkPresence() {
    isConnected=$(lsusb | grep ${idstr} | wc -l)

    if [[ ( "$isConnected" == 1) ]]
    then
        echo "Target device is connected"
        echo "\t$(lsusb | grep ${idstr})"
    else
        echo "Target device is disconnected"
    fi
}

checkPresence

echo "Starting monitor for device ${idstr}"

inotifywait -q -r -m /dev/bus/usb -e CREATE -e DELETE | while read e
do
    checkPresence  
done