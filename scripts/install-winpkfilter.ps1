
Push-Location $env:TEMP

$arch = If ([Environment]::Is64BitOperatingSystem) {'x64'} Else {'x86'}
$version = "3.2.32.1"
$url = "https://github.com/wiresock/ndisapi/releases/download/v$version/Windows.Packet.Filter.$version.$arch.msi"

echo "Downloading $url"
Invoke-WebRequest $url -OutFile "WinpkFilter-$arch.msi"
Start-Process "WinpkFilter-$arch.msi" -ArgumentList "/norestart /quiet /l WinpkFilter-$arch.log" -wait
type "WinpkFilter-$arch.log"

Pop-Location
