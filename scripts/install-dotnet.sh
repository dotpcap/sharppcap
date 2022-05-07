#!/bin/bash
set -e

wget -q https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb


apt-get update

# Install DotNet
apt-get install -y dotnet-sdk-5.0
