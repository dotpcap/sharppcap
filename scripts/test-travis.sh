#!/bin/bash
set -e

# Test on osx
if [ "$TRAVIS_OS_NAME" = "osx" ]
then
    sudo -E bash scripts/test.sh
fi

# Test on linux
if [ "$TRAVIS_OS_NAME" = "linux" ]
then
    sudo -E bash scripts/test.sh
fi
