#!/bin/bash

# Install on osx
if [ "$TRAVIS_OS_NAME" = "osx" ]
then
    brew update
    brew install libpcap
fi

# Install on linux
if [ "$TRAVIS_OS_NAME" = "linux" ]
then
  sudo apt-get update
  sudo apt-get -qy install libpcap-dev
fi
