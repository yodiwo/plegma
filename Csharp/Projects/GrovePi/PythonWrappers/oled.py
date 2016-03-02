
import grove_128_64_oled as oled

class oled (object):

    def __init__(self):
        self.oled.init()  #initialze SEEED OLED display

    def clear(self):
        self.oled.clearDisplay()

    #Set display to normal mode (i.e non-inverse mode)
    def setNormalDisplay(self):
        self.oled.setNormalDisplay()      #Set display to normal mode (i.e non-inverse mode)

    def setpagemode(self):
        self.oled.setPageMode()           #Set addressing mode to Page Mode

    def setcursor(self,x,y):
	    self.oled.setTextXY(i,i)          #Set the cursor to Xth Page, Yth Column

    def setText(self,message):
	    self.oled.putString("Hello World!") #Print the String