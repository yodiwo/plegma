import grove_i2c_adc
import time


class ADC(object):
    def __init__(self):
        self.adc= grove_i2c_adc.ADC()

    def getData(self):
       return self.adc.adc_read()
