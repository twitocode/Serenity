FROM golang:latest

#Inside the container, make a folder called app and cd into it
WORKDIR /app

#copy all local files into the current folder (app) of the container
COPY . .

#Run these commands at build time
#RUN go install github.com/cosmtrek/air@latest
RUN go mod tidy
#CMD is used for runtme commands
