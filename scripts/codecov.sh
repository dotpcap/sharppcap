#!/bin/bash

# Logic for OS detection from https://circleci.com/developer/orbs/orb/codecov/codecov

family=$(uname -s | tr '[:upper:]' '[:lower:]')
arch=$(uname -m)
os="windows"
[[ $family == "darwin" ]] && os="macos"

[[ $family == "linux" ]] && os="linux"
[[ $os == "linux" ]] && osID=$(grep -e "^ID=" /etc/os-release | cut -c4-)
[[ $osID == "alpine" ]] && os="alpine"

filename="codecov"
[[ $os == "windows" ]] && filename+=".exe"

curl -Os "https://uploader.codecov.io/latest/${os}/${filename}"
chmod +x $filename

if [[ $arch = arm* ]] || [ $arch = aarch* ]
then
  # See https://github.com/codecov/uploader/issues/523
  dotnet tool install --global Codecov.Tool
  dotnet codecov "$@"
else
  ./$filename "$@"
fi
