
Push-Location $env:TEMP

$arch = If ([Environment]::Is64BitOperatingSystem) {'x64'} Else {'x86'}
$url = "https://www.ntkernel.com/downloads/Windows Packet Filter 3.2.29.1 $arch.msi"

echo "Downloading $url"
Invoke-WebRequest $url -OutFile "WinpkFilter-$arch.msi"
Start-Process "WinpkFilter-$arch.msi" -ArgumentList "/norestart /quiet /l WinpkFilter-$arch.log" -wait
type "WinpkFilter-$arch.log"

Pop-Location
