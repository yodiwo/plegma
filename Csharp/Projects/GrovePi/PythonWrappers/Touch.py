import sys
import time

import MPR121

class TouchSensor(object):

    def __init__(self):
        self.cap = MPR121.MPR121()

    def getValue(self):
        self.cap.touched()