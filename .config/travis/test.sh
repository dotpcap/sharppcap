#!/bin/bash
set -e

# Test on osx
if [ "$TRAVIS_OS_NAME" = "osx" ]
then
    sudo dotnet test -p:CollectCoverage=true
fi

# Test on linux
if [ "$TRAVIS_OS_NAME" = "linux" ]
then
    sudo dotnet test -p:CollectCoverage=true --filter TestCategory!=SendPacket
fi

bash <(curl -s https://codecov.io/bash)
