/*
 * 
 * Rest Api Types
 * 
 */

var APIVersion = 1;

// -------------------------------------------------------------------------
// Api error types and error codes
// -------------------------------------------------------------------------

var api_error =
{
    UNDEFINED: 0,
    API: 1,
    PAIRING: 2,
};

var api_ecode =
{
    UNDEFINED: 0,
    USERNOTCONFIRMED: 1,
    PORTEVENT: 2,
};

// -------------------------------------------------------------------------
// Thing
// -------------------------------------------------------------------------

function Thing() {
    this.ThingKey = null;   // string 
    this.Name = null;       // string 
    this.Config = [];       // array of ConfigParameter() elements
    this.Ports = [];        // array of Port() elements
    this.Type = null;       // string 
    this.BlockType = null;  // string 
    this.Removable = false  // boolean ( default: false)
    this.UIHints = null;    // instaceof ThingUIHints()
};

function ConfigParameter() {
    this.Name = null;       // string
    this.Value = null;      // string
    this.Description = null;// string 
};

function ThingUIHints() {
    this.IconURI = null;    // string
    this.Description = null;// string
};

function Port() {
    this.PortKey = null;    // string
    this.Name = null;       // string
    this.Description = null;// string
    this.ioDirection = null;// instanceof api_ioDirection
    this.Type = null;       // instanceof api_porttype
    this.Semantics = null;  // string 
    this.State = null;      // string
    this.RevNum = 0;        // integer
    this.ConfFlags = null; // instanceof api_portconf

};

var api_ioDirection =
{
    UNDEFINED: 0,
    INPUTOUTPUT: 1,
    OUTPUT: 2,
    INPUT: 3
};

var api_porttype =
{
    UNDEFINED: 0,
    INTEGER: 1,
    DECIMAL: 2,
    DECIMALHIGH: 3,
    BOOLEAN: 4,
    COLOR: 5,
    STRING: 6,
    VIDEODESCRIPTOR: 7,
    AUDIODESCRIPTOR: 8,
    BINARYRESOURSEDESCRIPTOR: 9,
    I2CDDESCRIPTOR: 10,
    JSONSTRING: 11
};

var api_portconf = 
{
    NONE: 0,
    PROPAGATEALLEVENTS: 1,
    ISTRIGGER: 2,
};

// -------------------------------------------------------------------------
// ThingType
// -------------------------------------------------------------------------
function ThingType() {
    this.Type = null;       // string 
    this.Searchable = false;// boolean 
    this.Description = null;// string 
    this.Model = [];        // array of ThingModelType() elements
};

function ThingModelType() {
    this.Id = null;         // string 
    this.Name = null;       // string 
    this.Description = null;// string 
    this.Config = [];       // array of ConfigDescription() elements
    this.Port = [];         // array of PortDescription() elements
};

function ConfigDescription() {
    this.DefaultValue = null;// string 
    this.Description = null;// string 
    this.Label = null;      // string 
    this.Name = null;       // string 
    this.Required = false;  // boolean 
    this.Type = null;       // string 
    this.Minimum = 0;       // double 
    this.Maximum = 0;       // double
    this.Stepsize = 0;      // double
    this.ReadOnly = false;  // boolean 
};

function PortDescription() {
    this.Description = null;// string 
    this.Id = null;         // string 
    this.Label = null;      // string 
    this.Category = null;   // string 
    this.State = null;      // instanceof  StateDescription()
};

function StateDescription() {
    this.Minimum = 0;       // double
    this.Maximum = 0;       // double
    this.Step = 0;          // double
    this.Pattern = null;    // string 
    this.ReadOnly = false;  // boolean
    this.Type = false;      //  instanceof api_porttype
};

// -------------------------------------------------------------------------
// PortEvent and PortEventMsg
// -------------------------------------------------------------------------

function PortEvent() {
    this.PortKey = null;    // string
    this.State = null;      // string
    this.RevNum = 0;        // integer
};

function PortEventMsg() {
    this.PortEvents = [];   // array of PortEvent() elements
};

// -------------------------------------------------------------------------
// PortState
// -------------------------------------------------------------------------

function PortState() {
    this.PortKey = null;    // string 
    this.State = null;      // string
    this.RevNum = 0;        // integer
    this.IsDeployed = false;// boolean (default: false)
};

function PortStateSet() {
    this.Operation = null;  // instanceof api_pstateoperation
    this.PortStates = [];   // array of PortState() elements
};

function PortStateGet() {
    this.Operation = null;  // instanceof api_pstateoperation
    this.PortKeys = [];     // array of PortState() elements
};

var api_pstateoperation =
{
    INVALID: 0,
    SPECIFICKEYS: 1,
    ACTIVEPORT: 2,
    ALLPORT: 3,
};

// -------------------------------------------------------------------------
// ThingSet 
// -------------------------------------------------------------------------

function ThingsSet() {
    this.Operation = 0;     // instanceof api_thingoperation
    this.Status = null;     // boolean
    this.Data = [];         // array of Thing()
    this.RevNum = 0;        // integer
};

var api_thingoperation =
{
    INVALID: 0,
    UPDATE: 1,
    OVERWRITE: 2,
    DELETE: 3,
    GET: 4,
    SCAN: 5,
    SYNC: 6,
};

// -------------------------------------------------------------------------
// NodeInfoRsp
// -------------------------------------------------------------------------

function NodeInfoRsp(){
    this.Name = null;           // string 
    this.Type = null;           // instanceof api_nodeType
    this.Capabilities = null;   // instanceof api_capabilities   
    this.ThingTypes = [];       // array of ThingType() elements
    this.ThingsRevNum = 0;      // integer
    this.SupportedApiRev = 0;   // integer
    this.BlockLibraries = [];   // array of strings
}

var api_nodeType =
{
    UNKNOWN: 0,
    GATEWAY: 1,
    ENDPOINTSINGLE: 2,
    TESTGATEWAY: 3,
    TESTENDPOINT: 4,
    WSENDPOINT: 5,
    ANDROID: 6,
    WSSAMPLE: 200,
    RESTSAMPLE: 201,
};

var api_capabilities =
{
    NONE: 0,
    SUPPORTSGRAPHSOLVING: 1,
};

// -------------------------------------------------------------------------
// Rest API configuration object
// -------------------------------------------------------------------------

function config(surl, name, uuid) {
    /**
    * surl Server Url.
    * name Node's Name.
    * uuid Application Unique User ID.
    *   example:
    * var cfg = new config("https://cyan.yodiwo.com", "Rest Sample Node", guid());
    */
    this.surl = ((arguments.length >= 1) ? arguments[0] : null);
    this.name = ((arguments.length >= 2) ? arguments[1] : null);
    this.uuid = ((arguments.length >= 3) ? arguments[2] : null);
};

/*
 * 
 * Rest Api
 * 
 */

function restApi(cfg) {
    /*
     * Public Properties
     */
    this.surl = cfg.surl;
    this.name = cfg.name;
    this.uuid = cfg.uuid;
    this.nodeKey = getCookie("nodekeystr");
    this.secretKey = getCookie("secretkey");;

    /*
     * Callbacks
     */
    this.onError = null;
    this.onNodeInfo = null;
    this.onThingsGet = null;

    /*
     * Private Properties
     */
    var self = this;

    /*
     * Public Methods
     */
     
     // set nodekey and secret key 
    this.setSecrets = function (nodekey, secretkey) {
        self.nodeKey = nodekey;
        self.secretKey = secretkey;
    }

    // return true if this node is paired, else return false
    this.IsPaired = function()
    {
        // get nodekey and secretKey cookies
        var nodeKey = getCookie("nodekeystr");
        var secretKey = getCookie("secretkey");
        
        if(this.nodeKey != null && this.secretKey != null )   
            return true;
        else
            return false;
    }

    // return  true if pairing phase 2 is not completed yet
    this.pairingPending = function()
    {
        // get token2
        var token2 = getCookie("token2");
        if(token2 != null)
            return true;
        else
            return false;
    }

    this.start = function () {
         /*
          *
          * At first, check if the node is paired. If we browser has the cookies "secretkey" and "nodekeystr", the node is paired. Else we need to pair this node.
          *
          * To pair a node we need to follow the steps below:
          *     - Post a 'gettokensreq' message. The server will respond with a pair of tokens. We store them for the next step
          *     - Confirm user by posting to 
          *         https://cyan.yodiwo.com/pairing/1/userconfirm?token2=<token2>&uuid=<uuid>&noderedirect={window.location.href};
                    This will redirect the user in the login page.
          *     - Finally, post a 'getkeysreq' message. The server will respond with a nodeKey and secretKey strings. We store these strings in cookies, so we don't need to pair the node again.
          *
          * After pairing we need to notify server about node's information. So we post a 'nodeinforsp' message. 
          * We need also to notify server about node's things. So we post a 'nodeinforsp' message. 
          * 
          */
        if(!this.IsPaired())
        {
            this.pair();
            // check if pairing phase 2 is complete. check if token2 is not stored in the cookies
            if(!this.pairingPending())
            {
                this.nodeKey = getCookie("nodekeystr");
                this.secretKey = getCookie("secretkey");

                // send '/nodeinforsp' msg and notify backend for node's thing types
                this.onNodeInfoReq();
                // send '/thingsset' msg and notify backend for node's things
                this.onThingsGetReq();
            }
        }
    };

    // used to complete pairing phase 1, in which the user needs to login in https://cyan.yodiwo.com
    this.userConfirm = function()
    {
        // get token2
        var token2 = getCookie("token2");
        if(token2 != null)
        {
            // complete phase1 pairing by redirecting to cyan.yodiwo.com for user confirmation
            var rurl = self.surl + '/pairing/1/userconfirm?' + 'token2=' + token2 + '&uuid=' + self.uuid + '&noderedirect=' + window.location.href;
            window.location.replace(rurl);
        }
        else{
            console.log("User already confirmed");
        }
        
    }

    // perform pairing
    this.pair = function () {

        /*
         * Check if "token1" and "token2" are stored in the cookies. 
         * If false, send a "getkeysreq" msg.
         * Else, send a "gettokensreq" msg.
         */
        var token1 = getCookie("token1");
        var token2 = getCookie("token2");

        if (token1 != null && token2 != null) {
            // "getkeysreq"
             var getKeysReqMsg = {
                        token1: token1,
                        token2: token2,
                    };
            
            // generate post url
            var url = self.surl + self.getPairingPath() + '/getkeysreq';
            $.ajax({
                url: url,
                type: 'POST',
                crossDomain: true,
                async:false,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify(getKeysReqMsg),
                success: function(data){
                },
                statusCode: {
                    200 : function(rsp) {
                        console.log('Get Keys Request -> Response:%O',rsp);

                        // store "nodekeystr" and "secretkey"
                        setCookie("nodekeystr", rsp.nodeKey);
                        setCookie("secretkey", rsp.secretKey);

                        // delete tokens
                        deleteCookie("token1");
                        deleteCookie("token2");
                    },
                    401 : function() {
                        console.log("onNodeInfoReq() unauthorized. Need user confirmation");
                        sendError(api_error.PAIRING, api_ecode.USERNOTCONFIRMED, "Confirm user before requesting a 'getkeysreq' msg");
                        
                    }},
                error: function(jqXHR, textStatus, errorThrown) {
                    console.log("Get Keys Request error textStatus:%O errorThrown:%O",textStatus,errorThrown);
                }
            });
        }
        else
        {
            // Get Tokens Request
            var getTokensReqMsg = {
                name: self.name,
                uuid: self.uuid,
                RedirectUri: window.location.href
            };

            // generate post url
            var url = self.surl + self.getPairingPath() + '/gettokensreq';
            // store userConfirm() function. use it in ajax's success function
            var tokensConfirm = this.userConfirm;
            $.ajax({
                    url: url,
                    type: 'POST',
                    crossDomain: true,
                    async:false,
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: JSON.stringify(getTokensReqMsg),
                    success: function (rsp) {
                        console.log('Get Tokens Request -> Response:%O',rsp);
               
                        // save "token1" and "token2" cookies
                        setCookie("token1", rsp.token1, 0.1);
                        setCookie("token2", rsp.token2, 0.1);

                        // confirm user
                        tokensConfirm();
                    },
                    error: function(jqXHR, textStatus, errorThrown) {
                        console.log("[Error] Get Tokens Request error jqXHR:%O textStatus:%O errorThrown:%O",jqXHR,textStatus,errorThrown);
                        sendError(api_error.PAIRING, api_ecode.UNDEFINED, jqXHR);
                    }
            });
        }

    };

    // handle Port Event
    this.onPortEvent = function (events) {
        // initialize PortEventMsg
        var porteventmsg = new PortEventMsg();

        // check if events is an array or it is just append portevent in porteventmsg.PortEvents array
        if (Object.prototype.toString.call(events) === '[object Array]') {
            events.forEach(function (e) {
                var event = new PortEvent();
                event.PortKey = self.nodeKey + '-' + e.ThingKey + '-' + e.PortKey;
                ActivePortKeys.forEach(function (pkey) {
                    if (pkey === event.PortKey) {
                        event.State = e.State;
                        event.RevNum = 0;
                        porteventmsg.PortEvents.push(event);
                    }
                });
            });
        }
        else {
            events.PortKey = self.nodeKey + '-' + events.ThingKey + '-' + events.PortKey;
            porteventmsg.PortEvents.push(events);
        }

        // send if we have at least a PortEvent
        if (porteventmsg.PortEvents.length > 0)
        {
            var url = self.surl + self.getPostPath() + '/porteventmsg';
            $.ajax({
                    url: url,
                    type: 'POST',
                    crossDomain: true,
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(porteventmsg),
                    success: function (rsp) {
                        console.log('[Success] onPortEvent() rsp:%O',rsp);
                    },
                    statusCode: {
                        200 : function() {
                            console.log("onPortEvent() Success");
                        },
                        401 : function() {
                              console.log("onPortEvent() Bad Request");
                        }
                    },
                    error: function(jqXHR, textStatus, errorThrown) {
                      console.log("[Error] Post PortEventMsg() jqXHR:%O textStatus:%O errorThrown:%O",jqXHR,textStatus,errorThrown);
                      sendError(api_error.API, api_ecode.PORTEVENT, jqXHR);
                    }
            });
        }
        // else don't send something
    };

    /*
     * After successful pairing of a node the cloud sends a 'nodeinforeq' message
     * and the node must respond with a 'nodeinforsp' message
     *
     * In this case, the cloud cannot send the request, so we just respond with 'nodeinforsp' message.
     * 
     */
    this.onNodeInfoReq = function (data, syncId) {
        if (self.onNodeInfo && typeof (self.onNodeInfo) === "function") {
            var types = self.onNodeInfo();

            var nodeInfoRsp = new NodeInfoRsp();
            nodeInfoRsp.Name = self.name;          
            nodeInfoRsp.Type = api_nodeType.RESTSAMPLE;     
            nodeInfoRsp.Capabilities = api_capabilities.NONE;
            nodeInfoRsp.ThingTypes = types;      
            nodeInfoRsp.ThingsRevNum = 0;   
            nodeInfoRsp.SupportedApiRev = APIVersion;

            var url = self.surl + self.getPostPath() + '/nodeinforsp';
            var tokensConfirm = this.userConfirm;
            $.ajax({
                url: url,
                type: 'POST',
                crossDomain: true,
                async:false,
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(nodeInfoRsp),
                statusCode: {
                    200 : function() {
                        console.log("[Success] onNodeInfoReq()");
                    },
                    401 : function() {
                          console.log("[Error] onNodeInfoReq() unauthorized. Needs user confirmation.");
                          // confirm user
                          tokensConfirm();
                    }
                },
                error: function(jqXHR, textStatus, errorThrown) {
                    console.log("[Error] onNodeInfoReq jqXHR:%O textStatus:%O errorThrown:%O",jqXHR,textStatus,errorThrown);
                    sendError(api_error.API, api_ecode.CONFIGURATION, jqXHR);
                }
            });
        }
        else
        {
            sendError(api_error.API, api_ecode.UNDEFINED, "onNodeInfo must be defined");
        }
    };

    /*
     * After 'nodeinforsp' message response, the cloud sends a 'thingsget' message
     * and the node must respond with a 'thingsset' message
     *
     * In this case, the cloud cannot send the request, so we just respond with 'thingsset' message.
     *
     */
    this.onThingsGetReq = function () {

        if (self.onThingsGet && typeof (self.onThingsGet) === "function") {
            // call mandatory onThingGet callback
            var originalThings = self.onThingsGet();
            try {
                var things = clone(originalThings);
            } catch (err) {
                console.log(err);
            }

            // create an ThingsSet object
            var thingSet = new ThingsSet();
            thingSet.RevNum = 0;
            thingSet.Operation = api_thingoperation.OVERWRITE;
            thingSet.Status = true;

            if (things != null) {
                things.forEach(function (thing) {
                    thing.ThingKey = self.nodeKey + '-' + thing.ThingKey;
                    thing.Ports.forEach(function (port) {
                        port.PortKey = thing.ThingKey + '-' + port.PortKey;
                    });
                    thingSet.Data.push(thing);
                });
            }

            // respond with a 'thingsset' message
            var url = self.surl + self.getPostPath() + '/thingsset';
            $.ajax({
                url: url,
                type: 'POST',
                crossDomain: true,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify(thingSet),
                success: function (rsp) {
                    console.log("[Success] handleThingsGet() response:%O",rsp);
                },
                error: function(jqXHR, textStatus, errorThrown) {
                    console.log("[Error] handleThingsGet jqXHR:%O textStatus:%O errorThrown:%O",jqXHR,textStatus,errorThrown);
                    sendError(api_error.API, api_ecode.CONFIGURATION, jqXHR);
                }
            });
        }
        else{
            sendError(api_error.API, api_ecode.UNDEFINED, "onThingGet must be defined");
        }
    };

    // return pairing url 
    this.getPairingPath = function()
    {
        return `/pairing/${APIVersion}`;
    }

    // return post url for every message
    this.getPostPath = function()
    {
        var nodeKey = getCookie("nodekeystr");
        var secretKey = getCookie("secretkey");
        return `/api/${APIVersion}/${nodeKey}/${secretKey}`
    }

    // remove cookies in case the user unpaired the node from Cyan
    this.clear = function()
    {
        // delete cookies
        deleteCookie("nodekeystr");
        deleteCookie("secretkey");
        deleteCookie("token1");
        deleteCookie("token2");

        this.nodeKey = null;
        this.secretKey = null;
    }

    /*
     * PRIVATE MEMBERS
     */

     // trigger 'onError' callback in case of an api error
     var sendError = function (error, ecode, msg) {
        if (self.onError && typeof (self.onError) === "function") {
            self.onError(error, ecode, msg);
        }
        else
        {
            console.log("[Error] onError must be defined (error:%O, ecode:%O, msg:%O)",error, ecode, msg);
        }
    };

};


/*
 *
 * Helper Functions
 *
 */

// -------------------------------------------------------------------------
// Cookies ( set, get and delete cookies )
// -------------------------------------------------------------------------

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

// -------------------------------------------------------------------------
// Guid Generation
// -------------------------------------------------------------------------

// generate guid
function guid() {
    return [guidHelper(2), guidHelper(1), guidHelper(1), guidHelper(1), guidHelper(3)].join("-");
}

// create random string of length 'length'
function guidHelper(length) {
    var output = "";
    for (var i = 0; i < length; i++) {
        output += (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
    return output;
}

// -------------------------------------------------------------------------
// Clone Object
// -------------------------------------------------------------------------

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