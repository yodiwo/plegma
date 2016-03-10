USE_SSL = true

IDIR = include include/paho libsrc/yodiwo libsrc/mongoose # -Ilibs/org.eclipse.paho.mqtt.c-1.0.3/src 
LDIR = lib

CC=gcc
CFLAGS= -pthread -g -O0

ODIR=obj
SRCDIR=src
LIBSRCDIR=libsrc

SRC = $(wildcard $(SRCDIR)/*.c)
SRC +=$(wildcard libsrc/yodiwo/*.c)
SRC +=$(LIBSRCDIR)/mongoose/mongoose.c

IDIR_ = $(patsubst %, -I%, $(IDIR))
LDIR_ = $(patsubst %, -L%, $(LDIR))

ifeq ($(USE_SSL),true)
	LIBPAHO = paho-mqtt3cs
else
	LIBPAHO = paho-mqtt3c
endif

EXTLIBS=-lm -l$(LIBPAHO) -lcurl


vpath %.c $(SRCDIR) $(LIBSRCDIR)


_TMP = $(patsubst $(SRCDIR)%.c,$(ODIR)%.o,$(SRC))
OBJ = $(patsubst $(LIBSRCDIR)%.c,$(ODIR)%.o,$(_TMP))

$(ODIR)/%.o:  %.c | odir
	$(CC) -c -o $(patsubst $(SRCDIR)%,$(OBJDIR)%,$@) $< $(CFLAGS) $(IDIR_)

seanode: $(OBJ) # paho
	gcc -o $(patsubst $(SRCDIR)%,$(OBJDIR)%,$@) $(OBJ) $(CFLAGS) $(EXTLIBS) $(LDIR_)

odir:
	mkdir -p $(ODIR)/yodiwo
	mkdir -p $(ODIR)/mongoose
	
#paho:
#	make -C libs/org.eclipse.paho.mqtt.c-1.0.3/
#	cp libsrc/org.eclipse.paho.mqtt.c-1.0.3/build/output/libpaho-mqtt3c.so* lib/
##	make -f paho.mk mylib

.PHONY: clean all odir #paho

clean:
	rm -rf ./$(ODIR)/mongoose/*.o ./$(ODIR)/yodiwo/*.o ./$(ODIR)/*.o seanode 

all: seanode
