#!/usr/bin/env bash

# Copyright 2022 Ayoub Kaanich <kayoub5@live.com>
# SPDX-License-Identifier: MIT

set -e

curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin -c 6.0 "$@"

dotnet --list-sdks
