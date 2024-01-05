
# Copyright 2022 Ayoub Kaanich <kayoub5@live.com>
# SPDX-License-Identifier: MIT

Push-Location $env:TEMP

$arch = If ([Environment]::Is64BitOperatingSystem) {'x64'} Else {'x86'}
$version = "3.2.32.1"
$url = "https://github.com/wiresock/ndisapi/releases/download/v$version/Windows.Packet.Filter.$version.$arch.msi"

echo "Downloading $url"
Invoke-WebRequest $url -OutFile "WinpkFilter-$arch.msi"
$process = Start-Process "WinpkFilter-$arch.msi" -ArgumentList "/norestart /quiet /l WinpkFilter-$arch.log" -PassThru -Wait
Get-Content "WinpkFilter-$arch.log"

Pop-Location

exit $process.ExitCode
