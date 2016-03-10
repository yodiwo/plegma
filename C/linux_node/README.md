This project contains an example implementation for a Yodiwo Plegma Node, written in pure C.

Copyright [2015] Yodiwo A.B.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

The gist of it is that you are free to use, modify, distribute and incorporate parts or all of the Yodiwo copyrighted code into your projects without any implied responsibility from Yodiwo.

INCLUDES:

[PahoMQTT]
This bundle also contains parts of the Paho MQTT client, which is Copyright (c) 2009, 2015 IBM Corp.
and distributed under the terms of the Eclipse Public License v1.0 and Eclipse Distribution License v1.0
[Mongoose web server]
It also contains the Mongoose web server, which is Copyright (c) 2015 Cesanta Software Limited
and distributed under the terms of the GNU General Public License v2.0


DEPENDS:

[libcurl]
This code depends on libcurl. On Debian / Ubuntu systems, the relevant package is libcurl4-openssl-dev, installable by running: 

sudo apt-get install libcurl4-openssl-dev


[lm-sensors]
the sensor thing periodically sends the output of the command defined in SENSOR_COMMAND
by default, this is "sensors | grep -oP 'Physical\\ id\\ 0:\\ *\\K([^\\s]*)'"
this requires the lm-sensors package to be installed and the relevant temperature sensing modules to be loaded
On Ubuntu, the lm-sensors package can be installed by running:

sudo apt-get install lm-sensors

the needed temperature sensing modules can be found and loaded by running:

sudo sensors-detect

and following the instructions
 

BUILD INSTRUCTIONS:

build with:
make


RUN INSTRUCTIONS:

run with:
./seanode.sh

(or run directly the executable, seanode, ensuring the Paho library is in your library search path.)
