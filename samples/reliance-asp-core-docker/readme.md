# Reliance .NET Core on Docker
In this sample project we demonstrate how to access the Reliance thermal printer from inside a docker container.
This uses a basic [ASP.NET](https://hub.docker.com/r/microsoft/dotnet/])project and a standard dotnet Docker image. This project was created using the dotnet empty web project template.

    dotnet new web -o reliance-asp-core-docker
    dotnet add package PTIRelianceLib 

Startup.cs was edited with a simple check that reports connected status and printer firmware revision if connected. Otherwise, this is an entirely default project. This was tested with Docker version 18.03.1-ce, build 9ee9f40 on a Debian 9 64-bit host.


**This sample requires use of the --privileged docker flag so a Linux host is required**

## Sudo note
You may add your user to the docker group so you don't have to call sudo for every docker operation.

## Building the image

    sudo docker build -t reldocker .

## Running the image

    sudo docker run --rm -it -p 8080:80 --privileged reldocker
  