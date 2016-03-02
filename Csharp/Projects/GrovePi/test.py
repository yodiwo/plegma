import sys
import os
import time
import json
import grovepi
from grove_rgb_lcd import *

'''Csharp2Python'''
class Python2Sharp(object):
    def __init__(self, isRequest, watcherid, syncid, payload, operation):
        self.isRequest = isRequest
        self.watcherid = watcherid
        self.syncid = syncid
        self.payload = payload
        self.operation = operation

'''Python2CSharp'''
class Sharp2python(object):
    def __init__(self, j):
        self.__dict__ = json.loads(j)

'''handle csharp requests'''
def handlerequests(req):
    if req.operation==0:
        pin = req.pin.split("Pin")[1]
        grovepi.pinMode(int(pin), "INPUT")
        try:
            sensorval = grovepi.digitalRead(int(pin))
        except:
            sensorval =0
        msgtx = Python2Sharp(msg.isRequest, msg.watcherid, msg.syncid, str(sensorval), msg.operation)
        sys.stdout.write(json.dumps(msgtx.__dict__)+"\n")
    elif req.operation == 1:
        pin = req.pin.split("Pin")[1]
        grovepi.pinMode(int(pin), "OUTPUT")
        try:
            grovepi.digitalWrite(int(pin), int(req.payload))
        except:
            grovepi.digitalWrite(int(pin), int(req.payload))
    elif req.operation == 2:
        pin = req.pin.split("Pin")[1]
        grovepi.pinMode(int(pin), "INPUT")
        try:
            sensorval = grovepi.analogRead(int(pin))
        except:
            sensorval = 0
        msgtx = Python2Sharp(msg.isRequest, msg.watcherid, msg.syncid, str(sensorval), msg.operation)
        sys.stdout.write(json.dumps(msgtx.__dict__)+"\n")
    elif req.operation ==4:
        pin = req.pin.split("Pin")[1]
        try:
            [temp,humidity] = grovepi.dht(int(pin),1)
        except:
            temp=0
            humidity =0
        a= json.dumps({'temp':temp,'humidity':humidity})
        msgtx = Python2Sharp(msg.isRequest,msg.watcherid,msg.syncid,a,msg.operation)
        sys.stdout.write(json.dumps(msgtx.__dict__)+"\n")
    elif req.operation == 5:
        pin = req.pin.split("Pin")[1]
        try:
            sensorval=grovepi.ultrasonicRead(int(pin))
        except:
            sensorval=0
        msgtx = Python2Sharp(msg.isRequest, msg.watcherid, msg.syncid, str(sensorval), msg.operation)
        sys.stdout.write(json.dumps(msgtx.__dict__)+"\n")
    elif req.operation == 6:
        try:
            setRGB(0,128,64)
            setText(req.payload)
        except:
            pass

sys.stdout.flush()

print("Python Starts")

while True:
    input = sys.stdin.readline()
    sys.stdout.flush()
    try:
        json_acceptable_string = input.replace("'", "\"")
        msg = Sharp2python(json_acceptable_string)
        if len(input) > 1:
            try:
                handlerequests(msg)
            except Exception, e:
                sys.stdout.write("Exc:" + str(e) + e.message + '\n')
    except Exception, e:
        pass
