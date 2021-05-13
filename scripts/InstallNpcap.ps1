# Install Npcap on the machine.
# Copied from https://github.com/secdev/scapy/blob/master/.config/appveyor/InstallNpcap.ps1

# Config:
$npcap_oem_file = "npcap-1.20-oem.exe"

# Note: because we need the /S option (silent), this script has two cases:
#  - The script is runned from a master build, then use the secure variable 'npcap_oem_key' which will be available
#    to decode the very recent npcap install oem file and use it
#  - The script is runned from a PR, then use the provided archived 0.96 version, which is the last public one to
#    provide support for the /S option

if (Test-Path Env:npcap_oem_key){  # Key is here: on master
    # Unpack the key
    # The format of the environment variable should be 'username,password'
    $user, $pass = (Get-ChildItem Env:npcap_oem_key).Value.replace("`"", "").split(",")
}
if($user -And $pass){
    echo "Using Npcap OEM version"
    $file = $PSScriptRoot+"\"+$npcap_oem_file
    # Download oem file using (super) secret credentials
    $pair = "${user}:${pass}"
    $encodedCreds = [System.Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes($pair))
    $basicAuthValue = "Basic $encodedCreds"
    $headers = @{ Authorization = $basicAuthValue }
    $secpasswd = ConvertTo-SecureString $pass -AsPlainText -Force
    $credential = New-Object System.Management.Automation.PSCredential($user, $secpasswd)
    Invoke-WebRequest -uri (-join("https://nmap.org/npcap/oem/dist/",$npcap_oem_file)) -OutFile $file -Headers $headers -Credential $credential
} else {  # No key: PRs
    echo "Using backup 0.96"
    $file = $PSScriptRoot+"\npcap-0.96.exe"
    # Download the 0.96 file from nmap servers
    Invoke-WebRequest -uri "https://nmap.org/npcap/dist/npcap-0.96.exe" -OutFile $file
    # Now let's check its checksum
    $_chksum = $(CertUtil -hashfile $file SHA256)[1] -replace " ",""
    if ($_chksum -ne "83667e1306fdcf7f9967c10277b36b87e50ee8812e1ee2bb9443bdd065dc04a1"){
        echo "Checksums does NOT match !"
        exit
    } else {
        echo "Checksums matches !"
    }
}
echo ('Installing: ' + $file)

# Run installer
Start-Process $file -ArgumentList "/loopback_support=yes /winpcap_mode /S" -wait
if($?) {
    echo "Npcap installation completed"
}
