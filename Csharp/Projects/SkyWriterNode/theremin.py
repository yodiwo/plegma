import skywriter
import os
import time
import json
import sys
import signal

'''deserialization class CSharp2Python'''
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

some_value = 5000
'''events are not transferred to csharp, if activation  message is not sent'''
sendgestureevents = False
sendposition = False
gesturewatcherid =0
gesturesyncid =0
posititionwatcherid=0
positionsyncid =0

@skywriter.move()
def move(x, y, z):
    print(x,y,z)
    payload = {'x':x,'y':y,'z':z}
    a= json.dumps(payload)
    msg = Python2Sharp('True',posititionwatcherid,positionsyncid,a,0)
    send2sharpposition(json.dumps(msg.__dict__)+"\n")

@skywriter.flick()
def flick(start,finish):
    print("flick")
    payload = {'flick':start+finish,'tap':"",'touch':"",'doubletap':"",'airwheel':""}
    a= json.dumps(payload)
    msgtx = Python2Sharp('True',gesturewatcherid,gesturesyncid,a,0)
    send2sharpgestures(json.dumps(msgtx.__dict__)+"\n")

@skywriter.airwheel()
def spinny(delta):
    global some_value
    some_value += delta
    if some_value < 0:
  	    some_value = 0
    if some_value > 10000:
        some_value = 10000
    payload = {'flick':"",'tap':"",'touch':"",'doubletap':"",'airwheel':str(some_value/100)}
    a= json.dumps(payload)
    msgtx = Python2Sharp('True',gesturewatcherid,gesturesyncid,a,0)
    send2sharpgestures(json.dumps(msgtx.__dict__)+"\n")

@skywriter.double_tap()
def doubletap(position):
    print"doubletap"
    payload = {'flick':"",'tap':"",'touch':"",'doubletap':position,'airwheel':""}
    a= json.dumps(payload)
    msgtx = Python2Sharp('True',gesturewatcherid,gesturesyncid,a,0)
    send2sharpgestures(json.dumps(msgtx.__dict__)+"\n")

@skywriter.tap()
def tap(position):
    print("tap")
    payload = {'flick':"",'tap':position,'touch':"",'doubletap':"",'airwheel':""}
    a= json.dumps(payload)
    msgtx = Python2Sharp('True',gesturewatcherid,gesturesyncid,a,0)
    send2sharpgestures(json.dumps(msgtx.__dict__)+"\n")

@skywriter.touch()
def touch(position):
    print "touch"
    payload = {'flick':"",'tap':"",'touch':position,'doubletap':"",'airwheel':""}
    a= json.dumps(payload)
    msgtx = Python2Sharp('True',gesturewatcherid,gesturesyncid,a,0)
    send2sharpgestures(json.dumps(msgtx.__dict__)+"\n")

sys.stdout.flush()

def send2sharpposition(msg):
    if sendposition:
        print("Send position")
        sys.stdout.flush()
        sys.stdout.write(msg)

def send2sharpgestures(msg):
    if sendgestureevents:
        print("Send Event\n")
        sys.stdout.flush()
        sys.stdout.write(msg)


print "Python Started"
while True:
    input = sys.stdin.readline()
    sys.stdout.flush()
    json_acceptable_string = input.replace("'", "\"")
    try:
        msg = Sharp2python(json_acceptable_string)
        print("======message=====",msg.operation);
        if len(input) > 1 :
            try:
                if msg.operation == 0:
                    sendgestureevents = True
                    gesturewatcherid = msg.watcherid
                    gesturesyncid = msg.syncid
                elif msg.operation == 1:
                    sendposition = True
                    posititionwatcherid = msg.watcherid
                    positionsyncid = msg.syncid
            except Exception, e:
                sys.stdout.write("Exc:" + str(e) + e.message + '\n')
    except:
        pass
