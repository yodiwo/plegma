import io
import socket
import struct
import time
import picamera
import json
import threading
import sys
import fractions
from PIL import Image


'''Python2CSharp'''
class Sharp2python(object):
    def __init__(self, j):
        self.__dict__ = json.loads(j)


class PiCamera(object):
    def __init__(self,hostname,port):
        # Connect a client socket to my_server:8000 (change my_server to the
        # hostname of your server)
        self.client_socket = socket.socket()
        self.client_socket.connect((hostname,port))
        self.isRunning = False;
        self.Filter = 'none'
        #self.thread1 = threading.Thread(target=self.startcapture)
        # Make a file-like object out of the connection
        self.connection = self.client_socket.makefile(('wb'))
        try:
            #print("Malformed Camera")
            #TODO:UNCOMMENT THE FOLLOWING TWO lINES
            self.picamera = picamera.PiCamera()
            self.picamera.framerate = 80
        except:
            self.connection.close()
            self.client_socket.close()


    def starttestcapture(self):
        print "StartCapture"
        sys.stdout.flush()
        #Malformed Camera
        #self.picamera.resolution = (320, 240)
        # Start a preview and let the camera warm up for 2 seconds
        #self.picamera.start_preview()
        time.sleep(2)
        # Note the start time and construct a stream to hold image data
        # temporarily (we could write it directly to connection but in this
        # case we want to find out the size of each capture first to keep
        # our protocol simple)
        start = time.time()
        stream = io.BytesIO()

        #for foo in self.picamera.capture(stream, 'jpeg'):
        img = open("/home/pi/YodiwoDev/test.jpg","rb")
        contents = img.read()
        img.close()
        img1 = open("/home/pi/YodiwoDev/test1.jpg","rb")
        contents1 = img1.read()
        img1.close()
        size= len(contents)
        size1=len(contents1)


        while True:
            sys.stdout.flush()
            self.connection.write(struct.pack('<L', size))
            self.connection.write(contents)
            self.connection.flush()
            time.sleep(1)
            self.connection.write(struct.pack('<L', size1))
            self.connection.write(contents1)
            self.connection.flush()
            time.sleep(1)
            print('sent...')
            sys.stdout.flush()
            if (self.isRunning==False):
                break;



    def startcapture(self):
        print "StartCapture"
        sys.stdout.flush()
        self.picamera.resolution = (320, 240)
        # Start a preview and let the camera warm up for 2 seconds
        self.picamera.start_preview()
        time.sleep(2)
        # Note the start time and construct a stream to hold image data
        # temporarily (we could write it directly to connection but in this
        # case we want to find out the size of each capture first to keep
        # our protocol simple)
        start = time.time()
        stream = io.BytesIO()
        while True:
            try:
                self.picamera.capture(stream,'jpeg',use_video_port = True)
                # Write the length of the capture to the stream and flush to
                # ensure it actually gets sent
                print ("=========>" + self.Filter);
                self.picamera.image_effect = self.Filter
                size = stream.tell()
                print("stream tell " + str(size))
                sys.stdout.flush()
                self.connection.write(struct.pack('<L', size))
                # Rewind the stream and send the image data over the wire
                stream.seek(0)
                #print(time.time()-start);
                self.connection.write(stream.read())
                start = time.time()
                # Reset the stream for the next capture
                stream.seek(0)
                stream.truncate()
                #print('sent...')
                sys.stdout.flush()
                if (self.isRunning==False):
                    break;
            except Exception,e:
                print e.message

    def stopcapture(self):
        print("==========Stop Capture")
        sys.stdout.flush()
        self.isRunning=False

    def handlerequests(self,req):
        if req.operation == 0:
            #TODO:CHANGE IT IN startcapture
            print ("Start Capture")
            t1 = threading.Thread(target=self.startcapture)
            t1.daemon = True
            self.isRunning = True
            t1.start()
        elif req.operation ==1:
            print("====Go to TearDown========")
            self.stopcapture()
            time.sleep(1)
            #t1.join()
        elif req.operation == 2:
            print ("==============>Filetr" + req.payload)
            self.Filter = req.payload

    def signal_handler(self, signal, frame):
        print('You pressed Ctrl+C!')
        sys.stdout.flush()
        sys.exit(0)



def signal_handler(self, signal, frame):
    print('You pressed Ctrl+C!')
    sys.exit(0)

sys.stdout.flush()
sys.stdout.write("Constuctor\n")

picamera = PiCamera("localhost",8000)

sys.stdout.write("Python Started\n")
sys.stdout.flush()


while True:
    sys.stdout.flush()
    input = sys.stdin.readline()
    sys.stdout.flush()
    #sys.stdout.write(input +"\n")
    sys.stdout.flush()
    json_acceptable_string = input.replace("'", "\"")
    print("==============>"+input)
    sys.stdout.flush()
    try:
        msg = Sharp2python(json_acceptable_string)
        if len(input) > 1:
            try:
                picamera.handlerequests(msg)
            except Exception, e:
                sys.stdout.write("Exc:" + str(e) + e.message + '\n')
                sys.stdout.flush()
    except Exception, e:
        pass







