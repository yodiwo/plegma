class Key(object):
    def __init__(self, key, uid):
        self.key = key
        self.uid = uid

    def __str__(self):
        return "{0}-{1}".format(self.key, self.uid)

    def toString(self):
        return str(self.__str__())

    def getKey(self):
        return "{0}-{1}".format(self.key, self.uid)

    def setKey(self, keyNuid):
        self.key = "-".join(keyNuid.split("-", keyNuid.count("-"))[:keyNuid.count("-")])
        self.uid = keyNuid.split("-")[-1]


class UserKey(object):
    def __init__(self, UserID=""):
        self.UserID = UserID

    def __str__(self):
        return "{0}".format(self.UserID)

    def toString(self):
        return str(self.__str__())

    def getUserKey(self):
        return self.UserID

    def setUserKey(self, ukey):
        self.UserID = ukey

class NodeKey(Key):
    def __init__(self, Userkey=UserKey(), NodeUID=""):
        Key.__init__(self, Userkey.getUserKey(), NodeUID)

class ThingKey(Key):
    def __init__(self, NodeKey=NodeKey(), ThingUID="", ThingCategory=None):
        Key.__init__(self, NodeKey.getKey(), ThingUID)
        self.ThingCategory = ThingCategory


class PortKey(Key):
    def __init__(self, ThingKey=ThingKey(), PortUID=""):
        Key.__init__(self, ThingKey.getKey(), PortUID)