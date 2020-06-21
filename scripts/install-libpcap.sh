#!/bin/bash

# the default installation of libpcap does not have remote pcap enabled
# so we need to compile our own
# see https://github.com/the-tcpdump-group/libpcap/issues/795 

# Those are the dependencies needed to build libpcap
if [[ "$OSTYPE" == "darwin"* ]]; then
    # Mac OSX, use https://www.macports.org/
    port install bison
    port install flex
else
    # Ubuntu
    apt-get install -y build-essential bison flex
fi

# clone into tmp folder
rm -rf /tmp/install-libpcap

# 1.9.1 is the version we tested to work
git clone --depth 1 -b libpcap-1.9.1 https://github.com/the-tcpdump-group/libpcap.git /tmp/install-libpcap

pushd /tmp/install-libpcap

# remote is disabled by default, so we need to enable it
./configure --enable-remote

# install the library
make install

# create the necessary links and cache
# see https://www.mono-project.com/docs/advanced/pinvoke/dllnotfoundexception/
ldconfig

popd
