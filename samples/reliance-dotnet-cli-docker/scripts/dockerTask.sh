imageName="docker.dotnet.debug"
containerName="${imageName}_1"
workdir="src"

# Kills all containers based on the image
killContainers () {
  echo "Killing all containers based on the ${imageName} image"
  docker rm --force $(docker ps -q -a --filter "ancestor=${imageName}")
}

# Removes the Docker image
removeImage () {
  imageId=$(docker images -q ${imageName})
  if [[ -z ${imageId} ]]; then
    echo "${imageName} is not found."
  else
    echo "Removing image ${imageName}"
    docker rmi ${imageId}
  fi
}

# Builds the Docker image.
buildImage () {

  dockerFileName="Dockerfile"

  if [[ ! -f "$workdir/$dockerFileName" ]]; then
    echo "File '$dockerFileName' does not exist."
  else
    echo "Building the image $imageName."
    docker build -t $imageName -f "$workdir/$dockerFileName" $workdir
  fi
}

# Runs a new container
runContainer () {
    echo "Running a new container $containerName"
    if [[ -z $(docker images -q $imageName) ]]; then
        echo "Couldn not find an image named $imageName"
    else
        docker run --privileged -v ~/.nuget/packages:/root/.nuget/packages:ro -v /dev/bus/usb:/dev/bus/usb  -d --name $containerName $imageName
    fi
}

# Runs a new container
runContainerInteractive () {
    echo "Running a new container $containerName"
    if [[ -z $(docker images -q $imageName) ]]; then
        echo "Couldn not find an image named $imageName"
    else
        docker run --rm -it --privileged -v ~/.nuget/packages:/root/.nuget/packages:ro  -v /dev/bus/usb:/dev/bus/usb -d --name $containerName --entrypoint /bin/bash $imageName
    fi
}

# Shows the usage for the script.
showUsage () {
  echo "Usage: dockerTask.sh [COMMAND]"
  echo "    Runs command"
  echo ""
  echo "Commands:"
  echo "    cleanup: Removes the image '$imageName' and kills all containers based on this image."
  echo "    buildForDebug: Builds the debug image and runs docker container."
  echo "    interactive: Builds the image in interactive mode, overwriting the default entry point."
  echo ""
  echo "Example:"
  echo "    ./dockerTask.sh buildForDebug"
  echo ""
  echo "    This will:"
  echo "        Build a Docker image named $imageName using debug environment and start a new Docker container."
}

if [ $# -eq 0 ]; then
  showUsage
else
  case "$1" in
    "cleanup")
            killContainers
            removeImage
            ;;
    "buildForDebug")            
            killContainers
            buildImage
            runContainer            
            ;;
    "interactive")            
            killContainers
            buildImage               
            runContainerInteractive
            ;;
    *)
            showUsage
            ;;
  esac
fi