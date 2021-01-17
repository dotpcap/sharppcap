# CI TODO list

## Linux

linux can't send packets
at least when you make StartCapture on the same adapter nothing is received
possibly related to https://github.com/the-tcpdump-group/libpcap/issues/400

tests for linux require different device instances for sending and capturing

## Npcap & rpcapd

rpcapd is broken in Npcap
See https://github.com/nmap/nmap/issues/1329

tests for Npcap have to run using `--filter TestCategory!=RemotePcap`

## Configure coverage
