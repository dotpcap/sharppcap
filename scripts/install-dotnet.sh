#!/bin/bash
set -e

curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin -c 6.0 --install-dir /usr/local/bin
