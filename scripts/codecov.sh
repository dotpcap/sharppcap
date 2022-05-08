#!/bin/bash

# Logic for OS detection from https://circleci.com/developer/orbs/orb/codecov/codecov

family=$(uname -s | tr '[:upper:]' '[:lower:]')

os="windows"
[[ $family == "darwin" ]] && os="macos"

[[ $family == "linux" ]] && os="linux"
[[ $os == "linux" ]] && osID=$(grep -e "^ID=" /etc/os-release | cut -c4-)
[[ $osID == "alpine" ]] && os="alpine"

filename="codecov"
[[ $os == "windows" ]] && filename+=".exe"

curl -Os "https://uploader.codecov.io/latest/${os}/${filename}"
chmod +x $filename

# Workaround until Codecov fix ARM support
# See https://github.com/codecov/uploader/issues/523
arch=$(uname -m)
if [[ $arch == arm64 ]] || [ $arch == aarch64 ]
then
  dotnet tool install --tool-path . Codecov.Tool
fi
# end arm workaround

./$codecov "$@"
