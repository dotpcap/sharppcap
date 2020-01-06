#!/bin/bash
set -e

wget -q https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb


apt-get update

# Install DotNet
apt-get install dotnet-sdk-2.1

# Install Libpcap
apt-get install -y libpcap-dev
