###############################################################################
# Tools
AR = ar
TAR = tar
MV = mv -f
CP = cp -f
RM = rm -rf
FIND = find
PRINT = echo
CXX = g++
CC = gcc
MKDIR = mkdir -p
MKFILE_PATH := $(abspath $(lastword $(MAKEFILE_LIST)))
MKFILE_DIR := $(dir $(MKFILE_PATH))
CURRENT_DIR := $(patsubst %/,%,$(dir $(MKFILE_PATH)))

# Output directory and files
ifeq ($(BUILD_DIR),)
  BUILD_DIR := build
endif
ifeq ($(BUILD_DIR),.)
  BUILD_DIR := build
endif
OBJDIR := $(BUILD_DIR)/obj
BIN_DIR ?= out

# Array of system tools that must be available
EXTERNAL_TOOLS = mv cp rm ar ld mkdir find g++ cmake shuf tar

###############################################################################
## Set linker flags
LDFLAGS += -L. -L$(BIN_DIR) -lpthread -shared
#LDFLAGS += -Wl,-rpath . 
###############################################################################
## Set compiler flags
CXXFLAGS += -Wall -std=gnu++11 -fPIC -fvisibility=default
CXXFLAGS += -I. -IUtilities

# Optimization flag
DISABLE_OPTIMIZATIONS ?= true
ifeq ($(DISABLE_OPTIMIZATIONS),true)
	CXXFLAGS += -O0
else
	CXXFLAGS += -O2
endif

# Build for x86,x64,arm
CPU?=x64
ifeq ($(CPU),x86)
LDFLAGS += -m32
CXXFLAGS += -m32
endif


# Generate dependency information
CXXFLAGS += -MD -MP -MF $(BUILD_DIR)/.dep/$(subst $(OBJDIR),,$@.d)
###############################################################################
# Utilities library
UTILITIES_LIBRARY = $(BIN_DIR)/libutilities.so
UTILITIES_LIBRARY_SOURCES = $(wildcard Utilities/*.cpp)
UTILITIES_LIBRARY_OBJECTS = $(addprefix $(OBJDIR)/, $(UTILITIES_LIBRARY_SOURCES:.cpp=.o))

# CPP Plugin Library
CPP_PLUGIN_LIBRARY = $(BIN_DIR)/libplugin.so
CPP_PLUGIN_LIBRARY_SOURCES = $(wildcard *.cpp)
CPP_PLUGIN_LIBRARY_OBJECTS = $(addprefix $(OBJDIR)/, $(CPP_PLUGIN_LIBRARY_SOURCES:.cpp=.o))
###############################################################################
# All binaries
BINARIES = 

# All libraries
#  $(UTILITIES_LIBRARY)
LIBRARIES = $(CPP_PLUGIN_LIBRARY) 

###############################################################################
.PHONY : clean linux_tools

all : settings $(BINARIES) $(LIBRARIES)
	@ $(foreach library,$(LIBRARIES), [ -e $(library) ] && echo "[LIBRARY] $(library)";)
	@ $(foreach binary,$(BINARIES), [ -e $(binary) ] && echo "[BINARY] $(binary)";)


list :
	@ $(foreach library,$(LIBRARIES), echo "[LIBRARY] $(library)";)
	@ $(foreach binary,$(BINARIES), echo "[BINARY] $(binary)";)

linux_tools :
	@ echo "Checking for external linux tools"
	@ for TOOL in $(EXTERNAL_TOOLS) ; do \
		command -v "$$TOOL" || { echo "Tool [$$TOOL] does not exist" && exit 1 ; } \
	done
	@ mkdir -p $(OBJDIR)
	@ mkdir -p $(BIN_DIR)

show :
	@ echo [BINARIES]  $(BINARIES)
	@ echo [LIBRARIES] $(LIBRARIES)

settings :
	@ echo [CXXFLAGS]  $(CXXFLAGS)
	@ echo [LDFLAGS]   $(LDFLAGS)
	@ echo [BINARIES]  $(BINARIES)
	@ echo [LIBRARIES] $(LIBRARIES)
	@ mkdir -p $(OBJDIR)
	@ mkdir -p $(BIN_DIR)

clean :
	@ $(RM) $(BIN_DIR)/*.so $(BIN_DIR)/*.a $(BIN_DIR)/*.so.2.*
	@ $(RM) $(BUILD_DIR) $(BINARIES) $(LIBRARIES)

###############################################################################
# Targets Libraries
###############################################################################

$(UTILITIES_LIBRARY) : $(UTILITIES_LIBRARY_OBJECTS)
	@ echo [CC so] $@ $^
	@ $(CXX) $(CXXFLAGS) $^ -o $@ $(LDFLAGS) 

$(CPP_PLUGIN_LIBRARY) : $(CPP_PLUGIN_LIBRARY_OBJECTS) $(UTILITIES_LIBRARY_OBJECTS)
	@ echo [CC so] $@ $^
	$(CXX) $(CXXFLAGS) $^ -o $@ $(LDFLAGS) 

###############################################################################

$(OBJDIR)/%.o : | $(BUILD_DIR)
$(OBJDIR)/%.o : | $(BIN_DIR)

$(BUILD_DIR) $(OBJDIR) $(BIN_DIR):
	mkdir -p $(OBJDIR)
	mkdir -p $(BIN_DIR)
	
	
# Force build of opencv before any file because of generated .h files
$(OBJDIR)/%.o : %.cpp
	@ echo "[CPP] $@"
	@ mkdir -p $(dir $(subst ../,,$@))
	@ mkdir -p $(BUILD_DIR)/.dep/$(dir $(subst $(OBJDIR)/,,$@))
	@ $(CXX) $(CXXFLAGS) -o $@ -c $<

	

###############################################################################
#
# Include the dependency files, should be the last of the makefile
#
# Make does not offer a recursive wildcard function, so here's one:
rwildcard=$(wildcard $1$2) $(foreach d,$(wildcard $1*),$(call rwildcard,$d/,$2))
-include $(shell mkdir -p $(BUILD_DIR)/.dep 2>/dev/null) $(call rwildcard,$(BUILD_DIR)/.dep,*.o.d)
