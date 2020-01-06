#!/bin/bash
set -e

# Test on osx
if [ "$TRAVIS_OS_NAME" = "osx" ]
then
    sudo dotnet test -f netcoreapp2.1
fi

# Test on linux
if [ "$TRAVIS_OS_NAME" = "linux" ]
then
    sudo dotnet test -f netcoreapp2.1
fi
