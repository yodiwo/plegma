

$(document).ready(function () {

    $("#range").on("change", function () {

        var event = new portevent();
        event.ThingKey = things[0].ThingKey;
        event.PortKey = things[0].Ports[0].PortKey;
        things[0].Ports[0].State = this.value;
        event.State = this.value;
        api.sendPortEvent(event);
    });

    var things = create_things();
    var types = create_things_types();

    var cfg = (window.location.hostname == "localhost") ?
        new config("http://localhost:3334", "ws://localhost:3335", "wsdemo", "e9b28a49-8910-470c-9254-cd1788a5ba84") :
        new config("https://cyan.yodiwo.com", "wss://ws.yodiwo.com", "wsdemo", "e9b28a49-8910-470c-9254-cd1788a5ba84");

    var api = new wsapi(cfg);

    /*
     * CALLBACK HANDLERS - START
     */

    /*
     * MANDATORY 
     */
    api.onError = function (error, ecode, msg) {

        console.log('ERROR: ', error, ecode, msg);

        switch (error) {

            case ws_error.MSG:
                alert("Error: " + msg);
                break;

            case ws_error.ERROR:
                alert("Error: " + msg);
                break;

            case ws_error.CONNECTION:
                if (ws_ecode.NOTCONNECTED)
                    api.start();
                break;

            case ws_error.PAIRING:
                if (ws_ecode.NOTPAIRED) {
                    /*
                     Not paired notification.

                     If you can securely save secrets, then this need only happen once per UUID.

                     However it is possible to never save secrets (because saving them securely is impractical or difficult)
                      and repeat the pairing procedure (a glorified, fatter, login) every time. The server will provide the
                      same nodekey per UUID and existing graphs will continue to work.
                      In this case remove the confirmation dialog below and silently ask the API to pair.
                     */
                    if (confirm("Not Paired Yet! Pair now?") == true) {
                        api.pair();
                    }
                }
                break;
            default:
        }



    };
    api.onNodeInfo = function () {

        return types;
    };

    /*
     * onThingGet 
     * ID == NULL -> GETALL THINGS
     * ID != NULL -> GET  THING WITH ID
     */
    api.onThingGet = function (id) {
        var ret;

        if (id != null) {
            things.forEach(function (thing) {
                if (thing.ThingKey == id) {
                    ret = thing;
                }
            });
        } else {
            ret = things;
        }

        return ret;
    };

    api.onPortEvent = function (event) {
        console.log(event);
        if (event.PortKey == "t2-p1") {
            //TODO: check ID and Value
            var image = document.getElementById('bulb');
            // get value of slider
            var range = parseFloat(event.State);
            // convert value to 0-255
            var value = Math.floor(range == 1.0 ? 255 : range * 256.0);
            // apply background color
            image.style.background = "rgb(255,255," + (255 - value) + ")";
            // update thing's state
            things[1].Ports[0].State = event.State;

        } else if (event.PortKey == "t3-p1") {
            var messages = document.getElementById('messages');
            messages.innerHTML += "# " + event.State + "<br/>";
            //update thing's state
            things[2].Ports[0].State = event.State;
        }

    };


    api.onPortStateReq = function (portkeys, nodekey) {
        console.log('PORT STATE UPDATE', portkeys);
        var portstates = [];
        portkeys.forEach(function (portkey) {
            things.forEach(function (thing) {
                thing.Ports.forEach(function (port) {
                    var portstate = {
                        PortKey: null,
                        State: null,
                        IsDeployed: false,
                    };
                    //thing.ThingKey = nodekey + '-' + thing.ThingKey;
                    //port.PortKey = thing.ThingKey + '-' + port.PortKey;
                    var pk = nodekey + '-' + thing.ThingKey + '-' + port.PortKey;
                    if (pk == portkey) {
                        portstate.PortKey = portkey;
                        portstate.State = port.State;
                        portstate.IsDeployed = "true"
                        portstates.push(portstate);
                    }
                });
            });
        });
        return portstates;
    };


    api.onPortStateRsp = function (states) {

        console.log('PORT STATE UPDATE', states);
    };
    /*
     * OPTIONAL 
     */
    api.onThingUpdate = function (thing) {
        var ret = false;

        console.log('THING UPDATE', thing);
        ret = true;

        return ret;
    };
    api.onThingCreate = function (thing) {
        var ret = false;

        console.log('THING CREATE', thing);
        ret = true;

        return ret;
    };
    api.onThingDelete = function (id) {
        var ret = false;

        console.log('THING DELETE', id);
        ret = true;

        return ret;
    };
    api.onConnected = function (nodekey) {
        console.log(nodekey + " (re)connected");
    }
    api.onPaired = function (nodekey, secretkey) {
        /*
         Handle received secrets. You may:
         - save them (encrypted!), or
         - discard them; the app will happily continue this session without issues, 
            and will re-pair at the next refresh / redeploy
         */
        alert("paired with nodekey: " + nodekey);
    }
    api.onUnpaired = function (reason, msg) {
        /*
         Get informed of node's unpairing. From this point on, and until the node is re-paired
         no messages can be successfully exchanged with the Yodiwo Servers
         */
    }
    
    /*
     * CALLBACK HANDLERS - END
     */

    /*
     If you have saved secrets from a previous sessions, set them here. 
     This will make your web app bypass pairing and login directly
     */
    //api.setSecrets(mySecurelySavedNodeKey, mySecurelySavedSecretKey);

    /* Start the web application */
    api.start();
});


/* 
 SET YOUR THINGS HERE
 */
function create_things() {
    /*
     * THING #1 DECIMAL-SLIDER
     */
    var config1 = new ConfigParameter();
    config1.Name = "param_t1";
    config1.Value = "value_t1";

    var port1 = new port();
    port1.Name = "port";
    port1.Type = ws_porttype.DECIMAL;
    port1.ioDirection = ws_portdirection.OUTPUT;
    port1.ConfFlags = ws_portconf.NONE;
    port1.Description = "";
    port1.PortKey = "p1";

    var uihints1 = new ThingUIHints();
    uihints1.IconURI = "/Content/VirtualGateway/img/icon-thing-slider.png";
    uihints1.Description = "";

    var thing1 = new thing();
    thing1.ThingKey = "t1";
    thing1.Name = "Slider";
    thing1.Config.push(config1);
    thing1.Ports.push(port1);
    thing1.Type = "slider";
    thing1.UIHints = uihints1;


    /*
     * THING #2 DECIMAL - LIGHT
     */
    var config2 = new ConfigParameter();
    config2.Name = "param_t2";
    config2.Value = "value_t2";

    var port2 = new port();
    port2.Name = "Value";
    port2.Type = ws_porttype.DECIMAL;
    port2.ioDirection = ws_portdirection.INPUT;
    port2.PortKey = "p1";
    port2.Description = "";
    port2.ConfFlags = ws_portconf.RECEIVEALLEVENTS;


    var uihints2 = new ThingUIHints();
    uihints2.IconURI = "/Content/VirtualGateway/img/icon-thing-genericlight.png";
    uihints2.Description = "";

    var thing2 = new thing();
    thing2.ThingKey = "t2";
    thing2.Name = "DimmableLight";
    thing2.Config.push(config2);
    thing2.Ports.push(port2);
    thing2.Type = "bulb";
    thing2.UIHints = uihints2;

    /*
     * THING #3 TEXT-CONSOLE 
     */
    var config3 = new ConfigParameter();
    config3.Name = "param_t3";
    config3.Value = "value_t3";

    var port3 = new port();
    port3.Name = "port";
    port3.Type = ws_porttype.STRING;
    port3.ioDirection = ws_portdirection.INPUT;
    port3.PortKey = "p1";
    port3.Description = "";
    port3.ConfFlags = ws_portconf.RECEIVEALLEVENTS;

    var uihints3 = new ThingUIHints();
    uihints3.IconURI = "/Content/Designer/img/BlockImages/icon-consoleout.png";
    uihints3.Description = "";

    var thing3 = new thing();
    thing3.ThingKey = "t3";
    thing3.Name = "websocket console output";
    thing3.Config.push(config3);
    thing3.Ports.push(port3);
    thing3.Type = "console";
    thing3.UIHints = uihints3;

    /*
     * THING #4 CAMERA - IMAGE PICKER
     */
    var config4 = new ConfigParameter();
    config4.Name = "param_t4";
    config4.Value = "value_t4";

    var port4_1 = new port();
    port4_1.Name = "Trigger";
    port4_1.Type = ws_porttype.BOOLEAN;
    port4_1.ioDirection = ws_portdirection.INPUT;
    port4_1.PortKey = "p1";
    port4_1.Description = "";
    port4_1.ConfFlags = ws_portconf.ISTRIGGER;


    var port4_2 = new port();
    port4_2.Name = "Trigger";
    port4_2.Type = ws_porttype.STRING;
    port4_2.ioDirection = ws_portdirection.OUTPUT;
    port4_2.PortKey = "p2";
    port4_2.Description = "";
    port4_2.ConfFlags = ws_portconf.RECEIVEALLEVENTS;

    var uihints4 = new ThingUIHints();
    uihints4.IconURI = "/Content/Designer/img/BlockImages/icon-camera.svg";
    uihints4.Description = "";

    var thing4 = new thing();
    thing4.ThingKey = "t4";
    thing4.Name = "Image Picker";
    thing4.Config.push(config4);
    thing4.Ports.push(port4_1);
    thing4.Ports.push(port4_2);
    thing4.Type = "bulb";
    thing4.UIHints = uihints4;


    var things = [];

    things.push(thing1);
    things.push(thing2);
    things.push(thing3);
    things.push(thing4);
    return things;
}

function create_things_types() {

    /*
     * SLIDER
     */
    var config1 = new ConfigDescription();
    config1.DefaultValue = "0";
    config1.Description = "config param 1";
    config1.Label = "param1";
    config1.Name = "param1";
    config1.Required = false;
    config1.Type = "decimal";
    config1.Minimum = 0;
    config1.Maximum = 0;
    config1.Stepsize = 0;
    config1.ReadOnly = false;

    var state1 = new StateDescription();

    var port1 = new PortDescription();
    port1.Description = "Dimmer port";
    port1.Id = "slider";
    port1.Label = null;
    port1.Category = null;
    port1.State = state1;

    var model1 = new NodeModelType();
    model1.Id = "slider";
    model1.Name = "slider";
    model1.Description = "slider input";
    model1.Config.push(config1);
    model1.Port.push(port1);

    /*
     * BULB
     */
    var config2 = new ConfigDescription();
    config2.DefaultValue = "0";
    config2.Description = "config param 1";
    config2.Label = "param1";
    config2.Name = "param1";
    config2.Required = false;
    config2.Type = "decimal";
    config2.Minimum = 0;
    config2.Maximum = 0;
    config2.Stepsize = 0;
    config2.ReadOnly = false;

    var state2 = new StateDescription();
    state2.Minimum = 0;
    state2.Maximum = 1;
    state2.Step = 0.01;

    var port2 = new PortDescription();
    port2.Description = "Dimmer port";
    port2.Id = "DimmableLight";
    port2.Label = "DimmableLight";
    port2.Category = "DimmableLight";
    port2.State = state2;

    var model2 = new NodeModelType();
    model2.Id = "bulb";
    model2.Name = "bulb";
    model2.Description = "bulb output";
    model2.Config.push(config2);
    model2.Port.push(port2);


    /*
     * CONSOLE
     */
    var config3 = new ConfigDescription();
    config3.DefaultValue = "0";
    config3.Description = "config param 1";
    config3.Label = "param1";
    config3.Name = "param1";
    config3.Required = false;
    config3.Type = "decimal";
    config3.Minimum = 0;
    config3.Maximum = 0;
    config3.Stepsize = 0;
    config3.ReadOnly = false;

    var state3 = new StateDescription();

    var port3 = new PortDescription();
    port3.Description = "Text port";
    port3.Id = "console";
    port3.Label = null;
    port3.Category = null;
    port3.State = state3;

    var model3 = new NodeModelType();
    model3.Id = "console";
    model3.Name = "console";
    model3.Description = "console output";
    model3.Config.push(config3);
    model3.Port.push(port3);

    var type = new NodeThingType();
    type.Type = "wsthing";
    type.Searchable = false;
    type.Description = "web socket example things";
    type.Model.push(model1);
    type.Model.push(model2);
    type.Model.push(model3);


    var types = [];

    types.push(type);

    return types;
}