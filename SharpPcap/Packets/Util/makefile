# $Id: makefile,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $
#
#   package net.sourceforge.jpcap.util
#
PKG = net.sourceforge.jpcap.util
PKG_DIR = $(subst .,/, $(PKG))
REL = ../../../..
include ${MAKE_HOME}/os.makefile
include ${MAKE_HOME}/rules.makefile


JAVA = \
	Timeval \
	HexHelper \
	AsciiHelper \
	ArrayHelper \
	PropertyHelper \
	AnsiEscapeSequences \
	FileUtility \
	TcpdumpWriter


JAVA_SOURCE = $(addsuffix .java, $(JAVA))
JAVA_CLASSES = $(addsuffix .class, $(JAVA))


all: $(JAVA_CLASSES)


include ${MAKE_HOME}/targets.makefile
include ${MAKE_HOME}/depend.makefile
