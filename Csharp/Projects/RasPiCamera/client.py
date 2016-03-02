import socket

if __name__ == "__main__":
    sock = socket.socket()
    sock.connect(("localhost", 50839))

    with open("myimg.jpg", "rb") as fd:
        buf = fd.read(1024)
        '''
        file = open("image.png", "rb")
        imgData = file.read()
        s.send(imgData)
        s.close()
        '''
        while (buf):
            sock.send(buf)
            buf = fd.read(1024)
    sock.close()