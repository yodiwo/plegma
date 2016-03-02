from adxl345 import ADXL345
import time

class Accelerometer(object):

    def __init__(self):
        self.adxl345 = ADXL345()

    def getData(self):
        self.adxl345.getAxes(True)