
[System.IO.Directory]::CreateDirectory('bin')
Push-Location bin

$arch = If ([Environment]::Is64BitOperatingSystem) {'x64'} Else {'x86'}

Invoke-WebRequest "https://www.ntkernel.com/downloads/Windows Packet Filter 3.2.29.1%20$arch.msi" -OutFile "WinpkFilter-$arch.msi"
Start-Process "WinpkFilter-$arch.msi" -ArgumentList "/norestart /quiet /l WinpkFilter-$arch.log" -wait
type "WinpkFilter-$arch.log"

Pop-Location
