








/**
 * 
 * TYPES
 * 
 */
var APIVersion = 1;

var ws_error =
{
    UNDEFINED: 0,
    API: 1,
    PAIRING: 2,
    CONNECTION: 3,
    MSG: 4
};
var ws_ecode =
{
    UNDEFINED: 0,
    CONFIGURATION: 1,
    NOTPAIRED: 2,
    NOTCONNECTED: 3,
    MSGRX: 4,
    MSGTX: 5
};
var ws_connectionflags =
{
    NONE: 0,
    CREATE_ENDPOINT: 1,
    MASTER_ENDPOINT: 2
};
var ws_msgflags =
{
    MESSAGE: 0,
    REQUEST: 1,
    RESPONSE: 2
};
var ws_msgtype =
{
    UNDEFINED: 0,
    PAIRING: 1,
    API: 2,
};
var ws_porttype =
{
    UNDEFINED: 0,
    INTEGER: 1,
    DECIMAL: 2,
    DECIMALHIGH: 3,
    BOOLEAN: 4,
    COLOR: 5,
    STRING: 6
};
var ws_portdirection =
{
    UNDEFINED: 0,
    INPUTOUTPUT: 1,
    OUTPUT: 2,
    INPUT: 3
};

var ws_portconf =
{
    NONE: 0,
    RECEIVEALLEVENTS: 1,
    ISTRIGGER: 2
};

var ws_thingoperation =
{
    INVALID: 0,
    UPDATE: 1,
    OVERWRITE: 2,
    DELETE: 3,
    GET: 4,
    SCAN: 5,
};
var ws_capabilities =
{
    NONE: 0,
    GRAPHSPLIT: 1,
};

var ws_pstateoperation =
{
    INVALID: 0,
    SPECIFICKEYS: 1,
    ACTIVEPORT: 2,
    ALLPORT: 3,
};

function StateDescription() {
    this.Minimum = 0;
    this.Maximum = 0;
    this.Step = 0;
    this.Pattern = null;
    this.ReadOnly = false;
};
function ConfigDescription() {
    this.DefaultValue = null;
    this.Description = null;
    this.Label = null;
    this.Name = null;
    this.Required = false;
    this.Type = null;
    this.Minimum = 0;
    this.Maximum = 0;
    this.Stepsize = 0;
    this.ReadOnly = false;
};
function PortDescription() {
    this.Description = null;
    this.Id = null;
    this.Label = null;
    this.Category = null;
    this.State = null;
};
function NodeModelType() {
    this.Id = null;
    this.Name = null;
    this.Description = null;
    this.Config = [];
    this.Port = [];
};
function NodeThingType() {
    this.Type = null;
    this.Searchable = false;
    this.Description = null;
    this.Model = [];
};

function port() {
    this.PortKey = null;
    this.Name = null;
    this.Description = null;
    this.ioDirection = null;
    this.Type = null;
    this.State = null;
    this.RevNum = 0;
    this.ConfFlags = null;
};
function ThingUIHints() {
    this.IconURI = null;
    this.Description = null;
};
function ConfigParameter() {
    this.Name = null;
    this.Value = null;
};
function thing() {
    this.ThingKey = null;
    this.Name = null;
    this.Config = [];
    this.Ports = [];
    this.Type = null;
    this.BlockType = null;
    this.UIHints = [];
};

function portevent() {
    this.PortKey = null;
    this.State = null;
    this.RevNum = 0;
};

function porteventmsg() {
    this.PortEvents = [];
};

function portstate() {
    this.PortKey = null;
    this.State = null;
    this.RevNum = 0;
    this.IsDeployed = false;
};

function thingsreq() {
    this.SeqNo = 0
    this.Status = false;
    this.Operation = 0;
    this.Data = [];
};

function thingsrsp() {
    this.SeqNo = 0
    this.Status = false;
    this.Operation = 0;
    this.Data = [];
};

function portstatersp() {
    this.Operation = 0;
    this.PortStates = [];
};

function portstatereq() {
    this.Operation = null;
    this.PortKeys = [];
};

function activeportkeysmsg() {
    this.ActivePortKeys = [];
};

/* HELPER FUNCTIONS FOR COOKIES */
var setCookie = function (cname, cvalue, exdays) {
    if (exdays != null) {
        var d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toUTCString();
        document.cookie = cname + "=" + cvalue + "; " + expires;
    } else {
        document.cookie = cname + "=" + cvalue + "; ";
    }

};
var deleteCookie = function (cname) {
    document.cookie = cname + "=; " + "expires=Thu, 01 Jan 1970 00:00:00 UTC";
};
var getCookie = function (cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return null;
};

/**
* Basic WS API configuration object. 
* @member {surl} Server Url.
* @member {wsurl} Web Socket Url.
* @member {name} Application Name.
* @member {uuid} Application Unique User ID.
* @example
*
* var cfg = new config("http://localhost:3334", "ws://localhost:3335", "wsdemo", "e9b28a49-8910-470c-9254-cd1788a5ba84");
*/
function config(surl, wsurl, name, uuid) {
    this.surl = ((arguments.length >= 1) ? arguments[0] : null);
    this.wsurl = ((arguments.length >= 2) ? arguments[1] : null);
    this.name = ((arguments.length >= 3) ? arguments[2] : null);
    this.uuid = ((arguments.length >= 4) ? arguments[3] : null);
};

function wsmsg(id, payload) {
    this.Id = ((arguments.length >= 1) ? arguments[0] : null);
    this.Payload = ((arguments.length >= 2) ? arguments[1] : null);
};

function wsapi(cfg) {
    /*
     * PUBLIC PROPERTIES
     */

    this.surl = cfg.surl;
    this.wsurl = cfg.wsurl;
    this.name = cfg.name;
    this.uuid = cfg.uuid;

    /*
    * PRIVATE PROPERTIES
    */
    var self = this;
    var socket = null;
    var nodeKey = null;
    var secretKey = null;
    var connFlags = ws_connectionflags.NONE;
    var refSyncId = 0;

    /*
     * PUBLIC METHODS
     */
    this.setSecrets = function (nodekey, secretkey) {
        self.nodeKey = nodekey;
        self.secretKey = secretkey;
    }

    this.setConnectionFlags = function (flags) {
        self.connFlags = flags;
    }

    this.start = function () {

        /*
         * TODO: CHECK CONFIG == NULL
         */

        socket = new WebSocket(self.wsurl);
        socket.onopen = onopen;
        socket.onclose = onclose;
        socket.onerror = onerror;
        socket.onmessage = onmessage;
        socket.bufferedAmount = 100000;

    };
    this.pair = function () {

        var payload = {
            name: self.name,
            uuid: self.uuid,
            RedirectUri: window.location.href
        };

        var msg = {
            Id: ws_msgtype.PAIRING,
            SubId: "gettokensreq",
            Payload: JSON.stringify(payload)
        };


        if (socket != null) {
            socket.send(JSON.stringify(msg));
        } else {
            sendError(ws_error.CONNECTION, ws_ecode.NOTCONNECTED, "lost connection");
        }
    };
    this.sendPortStateReq = function (op, portKeys) {

        console.log(portKeys);
        var payload = new portstatereq();
        payload.Operation = op;

        if (op === ws_pstateoperation.ACTIVEPORT || op === ws_pstateoperation.ALLPORT) {

        }
        else if (op === ws_pstateoperation.SPECIFICKEYS && portKeys !== null) {
            payload.PortKeys.push(portKeys);
        }
        else {

        }

        var msg = {
            Id: ws_msgtype.API,
            SubId: "portstatereq",
            SyncId: refSyncId++,
            Flags: ws_msgflags.REQUEST,
            Payload: JSON.stringify(payload)
        };


        if (socket != null && socket.readyState == 1) {
            console.log(JSON.stringify(msg));
            socket.send(JSON.stringify(msg));
        } else {
            sendError(ws_error.CONNECTION, ws_ecode.NOTCONNECTED, "lost connection and event");
        }
    };
    this.sendPortEvent = function (events) {

        if (socket == null || socket.readyState != 1) {
            if (socket.readyState == 3)
                self.start();
        }

        var payload = new porteventmsg();
        var event = null;

        if (Object.prototype.toString.call(events) === '[object Array]') {
            events.forEach(function (e) {
                event = new portevent();
                event.PortKey = self.nodeKey + '-' + e.ThingKey + '-' + e.PortKey;
                event.State = e.State;
                event.RevNum = 0;
                payload.PortEvents.push(event);
            });
        } else {
            event = new portevent();
            event.PortKey = self.nodeKey + '-' + events.ThingKey + '-' + events.PortKey;
            event.State = events.State;
            event.RevNum = 0;
            payload.PortEvents.push(event);
        }


        var msg = {
            Id: ws_msgtype.API,
            SubId: "porteventmsg",
            SyncId: 0,
            Flags: ws_msgflags.MESSAGE,
            Payload: JSON.stringify(payload),
        };

        if (socket != null && socket.readyState == 1) {
            socket.send(JSON.stringify(msg));
        } else {
            if (socket.readyState == 3)
                sendError(ws_error.CONNECTION, ws_ecode.NOTCONNECTED, "lost connection and event");
        }

    };
    this.SendThingsGet = function () {
        //TODO:HANDLE ERROR CASES ThingKey, treq.Operation
        if (self.onThingGet && typeof (self.onThingGet) === "function") {
            var payload = {
                SeqNo: 0,
                Operation: ws_thingoperation.GET,
            };

            var msg = {
                Id: ws_msgtype.API,
                SubId: "thingsget",
                SyncId: ++refSyncId,
                Flags: ws_msgflags.REQUEST,
                Payload: JSON.stringify(payload)
            };
            socket.send(JSON.stringify(msg));
        }
    };

    this.onError = null;
    this.onNodeInfo = null;
    this.onThingGet = null;
    this.onThingUpdate = null;
    this.onThingCreate = null;
    this.onThingDelete = null;
    this.onPortgEvent = null;
    this.onPortState = null;
    this.onConnected = null;
    this.onPaired = null;

    /*
     * PRIVATE MEMBERS
     */
    var sendError = function (error, ecode, msg) {
        if (self.onError && typeof (self.onError) === "function") {
            self.onError(error, ecode, msg);
        }
    };

    var handleLoginReq = function (data, syncId) {
        if (self.nodeKey == null || self.secretKey == null)
            return;

        var payload = {
            SeqNo: 0,
            NodeKey: self.nodeKey,
            SecretKey: self.secretKey,
            Flags: self.connFlags
        };
        var msg = {
            Id: ws_msgtype.API,
            SubId: "loginrsp",
            SyncId: syncId,
            Flags: ws_msgflags.RESPONSE,
            Payload: JSON.stringify(payload)
        };
        socket.send(JSON.stringify(msg));
    };

    var handleNodeInfoReq = function (data, syncId) {
        if (self.onNodeInfo && typeof (self.onNodeInfo) === "function") {
            var types = self.onNodeInfo();

            var payload = {
                SeqNo: 0,
                Type: 5,
                Name: self.name,
                Capabilities: ws_capabilities.NONE,
                ThingTypes: types
            };

            var msg = {
                Id: ws_msgtype.API,
                SubId: "nodeinforsp",
                SyncId: syncId,
                Flags: ws_msgflags.RESPONSE,
                Payload: JSON.stringify(payload)
            };
            socket.send(JSON.stringify(msg));

            self.onCloudForcedNegotiationFinished();
        }
    };

    var handleThingsGet = function (data, syncId) {
        //TODO:HANDLE ERROR CASES ThingKey, treq.Operation
        if (self.onThingGet && typeof (self.onThingGet) === "function") {

            var thingId = data.ThingKey != null ? data.ThingKey.replace(self.nodeKey + '-', "") : null;

            var things_orig = self.onThingGet(thingId);
            try {
                var things = clone(things_orig);
            } catch (err) {
                console.log(err);
            }

            var payload = {
                SeqNo: 0,
                Operation: ws_thingoperation.UPDATE,
                Status: true,
                Data: []
            };
            if (things != null) {
                if (data.ThingKey == null) {
                    things.forEach(function (thing) {
                        thing.ThingKey = self.nodeKey + '-' + thing.ThingKey;
                        thing.Ports.forEach(function (port) {
                            port.PortKey = thing.ThingKey + '-' + port.PortKey;
                        });
                        payload.Data.push(thing);
                    });
                } else {
                    things[0].ThingKey = self.nodeKey + '-' + things[0].ThingKey;
                    things[0].Ports.forEach(function (port) {
                        port.PortKey = thing.ThingKey + '-' + port.PortKey;
                    });
                }
            }
            var msg = {
                Id: ws_msgtype.API,
                SubId: "thingsset",
                SyncId: syncId,
                Flags: ws_msgflags.RESPONSE,
                Payload: JSON.stringify(payload)
            };
            socket.send(JSON.stringify(msg));
        }
    };

    var handleThingsSet = function (data, syncId) {
        var rsp_payload = null;

        switch (payload.Operation) {
            case ws_thingoperation.UPDATE:
                rsp_payload = handleThingUpdate(payload, request.SyncId);
                break;
            case ws_thingoperation.OVERWRITE:
                rsp_payload = handleThingCreate(payload, request.SyncId);
                break;
            case ws_thingoperation.DELETE:
                rsp_payload = handleThingDelete(payload, request.SyncId);
                break;
        }
        if (rsp_payload != null) {
            var msg = {
                Id: ws_msgtype.API,
                SubId: "genericrsp",
                SyncId: syncId,
                Flags: ws_msgflags.RESPONSE,
                Payload: JSON.stringify(rsp_payload)
            };
            socket.send(JSON.stringify(msg));
        }
        else
            alert("Things operation failed");
    }

    var handleThingUpdate = function (data, syncId) {
        if (self.onThingUpdate && typeof (self.onThingUpdate) === "function") {

            if (data != null) {
                data.Data.forEach(function (thing) {
                    var tkey = thing.ThingKey;

                    thing.ThingKey = thing.ThingKey.replace(self.nodeKey + '-', "");
                    thing.Ports.forEach(function (port) {
                        port.PortKey = port.PortKey.replace(tkey + '-', "");
                    });

                    var status = self.onThingUpdate(thing);

                    thing.ThingKey = self.nodeKey + '-' + thing.ThingKey;
                    thing.Ports.forEach(function (port) {
                        port.PortKey = thing.ThingKey + '-' + port.PortKey;
                    });
                });
                var payload = {
                    SeqNo: 0,
                    IsSuccess: true
                };
                return payload;
            }
        }
    };
    var handleThingCreate = function (data, syncId) {
        //TODO:HANDLE ERROR CASES
        if (self.onThingCreate && typeof (self.onThingCreate) === "function") {
            if (data != null) {
                data.Data.forEach(function (thing) {
                    var tkey = thing.ThingKey;

                    thing.ThingKey = thing.ThingKey.replace(self.nodeKey + '-', "");
                    thing.Ports.forEach(function (port) {
                        port.PortKey = port.PortKey.replace(tkey + '-', "");
                    });

                    var status = self.onThingCreate(thing);

                    thing.ThingKey = self.nodeKey + '-' + thing.ThingKey;
                    thing.Ports.forEach(function (port) {
                        port.PortKey = thing.ThingKey + '-' + port.PortKey;
                    });
                });
                var payload = {
                    SeqNo: 0,
                    IsSuccess: true
                };
                return payload;
            }
        }

    };
    var handleThingDelete = function (data, syncId) {
        //TODO:HANDLE ERROR CASES data.ThingKey
        if (self.onThingDelete && typeof (self.onThingDelete) === "function") {
            data.Data.forEach(function (thing) {

                data.ThingKey = data.ThingKey.replace(self.nodeKey + '-', "");

                var status = self.onThingDelete(data.ThingKey);
            });
            var payload = {
                SeqNo: 0,
                IsSuccess: true
            };
            return payload;
        }
    };

    var handlePortEvent = function (events) {
        //TODO:HANDLE ERROR CASES
        if (self.onPortEvent && typeof (self.onPortEvent) === "function") {

            events.PortEvents.forEach(function (e) {
                var partials = e.PortKey.split("-");
                if (partials[2] != null && partials[3] != null) {
                    e.PortKey = partials[2] + '-' + partials[3];
                    if (self.onPortEvent != null) {
                        self.onPortEvent(e);
                    }
                }
            });
        }
    };

    var handleActivePortKeys = function (states) {
		//TODO: Implement
    };
    
    var handlePortStateRsp = function (states) {
        //TODO:HANDLE ERROR CASES
        if (self.onPortStateRsp && typeof (self.onPortStateRsp) === "function") {
            var newstates = [];
            var state = null;
            if (Object.prototype.toString.call(states) === '[object Array]') {
                states.forEach(function (e) {
                    state = new portstate();
                    state.PortKey = e.PortKey.replace(self.nodeKey + '-', "");
                    state.State = e.State;
                    state.RevNum = e.RevNum;
                    state.IsDeployed = e.IsDeployed;
                    newstates.push(state);
                });
            } else {
                state = new portstate();
                state.PortKey = states.PortKey.replace(self.nodeKey + '-', "");
                state.State = states.State;
                state.RevNum = states.RevNum;
                state.IsDeployed = states.IsDeployed;
                newstates.push(state);

            }
            self.onPortStateRsp(newstates);
        }
    };

    var handleNodeUnpairedReq = function (reasonCode, message, syncId) {
        if (self.onUnpaired && typeof (self.onUnpaired) === "function") {
            self.onUnpaired(reasonCode, message);
        }
        self.NodeKey = null;
        self.SecretKey = null;
        var msg = {
            Id: ws_msgtype.API,
            SubId: "nodeunpairedrsp",
            SyncId: syncId,
            Flags: ws_msgflags.RESPONSE,
            Payload: JSON.stringify(payload)
        };
    };

    var handlePortStateReq = function (portkeys, operation, syncId) {
        console.log("handle port state request");
        if (self.onPortStateReq && typeof (self.onPortStateReq) === "function") {
            console.log("on portstate exists");
            var res = self.onPortStateReq(portkeys, self.nodeKey);
            var payload = {
                SeqNo: 0,
                Operation: operation,
                PortStates: res,
            };
            var msg = {
                Id: ws_msgtype.API,
                SubId: "portstatersp",
                SyncId: syncId,
                Flags: ws_msgflags.RESPONSE,
                Payload: JSON.stringify(payload)
            };
            socket.send(JSON.stringify(msg));

        }
    };

    var handleUnknownReq = function (syncId) {
        if (syncId == 0)
            return;
        var msg = {
            Id: ws_msgtype.API,
            SubId: "UnknownRsp",
            SyncId: syncId,
            Flags: ws_msgflags.RESPONSE,
            Payload: ""
        };
        socket.send(JSON.stringify(msg));
    };

    /*
     * SOCKET HANDLERS 
     */
    var onmessage = function (evt) {

        var request = null;

        try {

            request = JSON.parse(evt.data);

            //console.log('request:', request);

            if (request != null) {

                var payload = JSON.parse(request.Payload);

                switch (request.Id) {
                    case ws_msgtype.PAIRING:
                        switch (request.SubId) {
                            case "tokensrsp":
                                var rurl = self.surl + '/pairing/1/userconfirm?' + 'token2=' + payload.token2 + '&uuid=' + self.uuid + '&noderedirect=' + window.location.href;
                                setCookie("token1", payload.token1, 0.1);
                                setCookie("token2", payload.token2, 0.1);
                                window.location.replace(rurl);
                                break;
                            case "keysrsp":
                                if (payload != null) {
                                    self.nodeKey = payload.nodeKey;
                                    self.secretKey = payload.secretKey;
                                    deleteCookie("token1");
                                    deleteCookie("token2");

                                    if (self.onPaired && typeof (self.onPaired) === "function") {
                                        self.onPaired(self.nodeKey, self.secretKey);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case ws_msgtype.API:
                        switch (request.SubId) {
                            case "loginreq":
                                handleLoginReq(payload, request.SyncId);
                                break;
                            case "nodeinforeq":
                                handleNodeInfoReq(payload, request.SyncId);
                                break;
                            case "nodeunpairedreq":
                                handleNodeUnpairedReq(payload.ReasonCode, payload.Message, request.SyncId);
                                break;
                            case "thingsget":
                                if (payload.Operation == ws_thingoperation.GET)
                                    handleThingsGet(payload, request.SyncId);
                                    break;
                            case "thingsset":
                                {
                                    var rsp_payload = null;
                                    switch (payload.Operation) {
                                        case ws_thingoperation.UPDATE:
                                            rsp_payload = handleThingUpdate(payload, request.SyncId);
                                            break;
                                        case ws_thingoperation.OVERWRITE:
                                            rsp_payload = handleThingCreate(payload, request.SyncId);
                                            break;
                                        case ws_thingoperation.DELETE:
                                            rsp_payload = handleThingDelete(payload, request.SyncId);
                                            break;
                                    }
                                    if (rsp_payload != null) {
                                        var msg = {
                                            Id: ws_msgtype.API,
                                            SubId: "genericrsp",
                                            SyncId: syncId,
                                            Flags: ws_msgflags.RESPONSE,
                                            Payload: JSON.stringify(rsp_payload)
                                        };
                                        socket.send(JSON.stringify(msg));
                                    }
                                    else
                                        alert("Things operation failed");
                                    break;
                                }
                                break;
                            case "porteventmsg":
                                handlePortEvent(payload);
                                break;
                            case "activeportkeysmsg":
                                handleActivePortKeys(payload);
                                break;
                            case "portstatereq":
                                handlePortStateReq(payload.PortKeys, payload.Operation, request.SyncId);
                                break;
                            case "portstatersp":
                                handlePortStateRsp(payload.PortStates);
                                break;
                            default:
                                if ((request.Flags & ws_msgflags.REQUEST) != 0)
                                    handleUnknownReq(request.SyncId);
                                break;
                        }
                        break;
                    default:
                        break;
                }

            }

        } catch (e) {

            sendError(ws_error.MSG, ws_ecode.MSGRX, e.message);
        }
    };
    var onopen = function (evt) {

        if (self.nodeKey != null && self.secretKey != null) {
            if (self.onConnected && typeof (self.onConnected) === "function") {
                self.onConnected(self.nodeKey);
            }
            return;
        };

        var token1 = getCookie("token1");
        var token2 = getCookie("token2");
        if (token1 != null && token2 != null) {

            var payload = {
                uuid: self.uuid,
                token1: token1,
                token2: token2
            };

            var msg = {
                Id: ws_msgtype.PAIRING,
                SubId: "getkeysreq",
                Payload: JSON.stringify(payload)
            };

            socket.send(JSON.stringify(msg));
        } else {
            sendError(ws_error.PAIRING, ws_ecode.NOTPAIRED, null);
        }
    };
    var onclose = function (evt) {
        sendError(ws_error.CONNECTION, ws_ecode.NOTCONNECTED, evt);
    };
    var onerror = function (evt) {
        sendError(ws_error.ERROR, ws_ecode.ERROR, evt);
    };

    var clone = function (src) {

        function mixin(dest, source, copyFunc) {
            var name, s, i, empty = {};
            for (name in source) {
                // the (!(name in empty) || empty[name] !== s) condition avoids copying properties in "source"
                // inherited from Object.prototype.	 For example, if dest has a custom toString() method,
                // don't overwrite it with the toString() method that source inherited from Object.prototype
                s = source[name];
                if (!(name in dest) || (dest[name] !== s && (!(name in empty) || empty[name] !== s))) {
                    dest[name] = copyFunc ? copyFunc(s) : s;
                }
            }
            return dest;
        }

        if (!src || typeof src != "object" || Object.prototype.toString.call(src) === "[object Function]") {
            // null, undefined, any non-object, or function
            return src; // anything
        }
        if (src.nodeType && "cloneNode" in src) {
            // DOM Node
            return src.cloneNode(true); // Node
        }
        if (src instanceof Date) {
            // Date
            return new Date(src.getTime()); // Date
        }
        if (src instanceof RegExp) {
            // RegExp
            return new RegExp(src); // RegExp
        }
        var r, i, l;
        if (src instanceof Array) {
            // array
            r = [];
            for (i = 0, l = src.length; i < l; ++i) {
                if (i in src) {
                    r.push(clone(src[i]));
                }
            }
        } else {
            // generic objects
            r = src.constructor ? new src.constructor() : {};
        }
        return mixin(r, src, clone);
    };
};