#!/usr/bin/env bash

# Logic for OS detection from https://circleci.com/developer/orbs/orb/codecov/codecov

family=$(uname -s | tr '[:upper:]' '[:lower:]')
os="windows"

[[ $family == "darwin" ]] && os="macos"

[[ $family == "linux" ]] && os="linux"
[[ $os == "linux" ]] && osID=$(grep -e "^ID=" /etc/os-release | cut -c4-)
[[ $osID == "alpine" ]] && os="alpine"

filename="codecov"
[[ $os == "windows" ]] && filename+=".exe"

arch=$(uname -m)
if [[ $arch == arm64 ]] || [ $arch == aarch64 ]
then
  # Workaround until Codecov fix ARM support
  # See https://github.com/codecov/uploader/issues/523
  curl -s https://codecov.io/bash > $filename
else
  curl -Os "https://uploader.codecov.io/latest/${os}/${filename}"
fi

chmod +x $filename
./$filename "$@"
