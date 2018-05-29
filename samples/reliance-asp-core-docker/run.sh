# If you've already built the image comment out this line, update line 6 accordingly
sudo docker build -t reldocker .

# We need --privileged access in order to access the Reliance printer
# Note that because of this requirement, Reliance API Docker supports Linux only
sudo docker run --rm -it -p 8080:80 --privileged reldocker