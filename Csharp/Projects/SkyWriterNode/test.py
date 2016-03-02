import skywriter
import os
import time
import json
import sys
import signal

class Python2Sharp(object):
    def __init__(self, isRequest, watcherid, syncid, payload, operation):
        self.isRequest = isRequest
        self.watcherid = watcherid
        self.syncid = syncid
        self.payload = payload
        self.operation = operation

class Sharp2python(object):
    def __init__(self, j):
        self.__dict__ = json.loads(j)

some_value = 5000

@skywriter.move()
def move(x, y, z):
  sys.stdout.write( x, y, z )

@skywriter.flick()
def flick(start,finish):
  sys.stdout.write("Got a flick!", start, finish)

@skywriter.airwheel()
def spinny(delta):
  global some_value
  some_value += delta
  if some_value < 0:
  	some_value = 0
  if some_value > 10000:
    some_value = 10000
  sys.sdout.write("Airwheel:"+ some_value/100)

@skywriter.double_tap()
def doubletap(position):
  sys.stdout("Double tap!"+ position)

@skywriter.tap()
def tap(position):
  sys.stdout.write("Tap!" + position)

@skywriter.touch()
def touch(position):
  sys.stdout.write("Touch!"+ position)

sys.stdout.flush()
while True:
    input = sys.stdin.readline()
    sys.stdout.flush()
    json_acceptable_string = input.replace("'", "\"")
    msg = Sharp2python(json_acceptable_string)
    if len(input) > 1 :
        try:
            a= json.dumps({'x':2,'y':3,'z':10})
            msgtx = Python2Sharp(msg.isRequest,msg.watcherid,msg.syncid,a,msg.operation)
            sys.stdout.write(json.dumps(msgtx.__dict__))
            #sys.stdout.write(str(msg.operation))
        except Exception, e:
            sys.stdout.write("Exc:" + str(e) + e.message + '\n')
