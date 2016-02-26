var things;
var api;

$(document).ready(function () {

    $("#checkbox").on("change", function () {
        sendCheckBoxState(this.checked);
    });

    $("#checkbox2").on("change", function () {
        sendCheckBox2State(this.checked);
    });

    $("#text").on("keypress", function (e) {
        if (e.keyCode == 13) {
            console.log("key pressed");
            var event = new portevent();
            event.ThingKey = things[3].ThingKey;
            event.PortKey = things[3].Ports[0].PortKey;
            things[3].Ports[0].State = this.value;
            event.State = this.value;
            api.sendPortEvent(event);
        }
    });

    $("#button").on("mousedown", function () {

        var event = new portevent();
        event.ThingKey = things[4].ThingKey;
        event.PortKey = things[4].Ports[0].PortKey;
        things[4].Ports[0].State = true
        event.State = true;
        api.sendPortEvent(event);
    });

    $("#button").on("mouseup", function () {

        var event = new portevent();
        event.ThingKey = things[4].ThingKey;
        event.PortKey = things[4].Ports[0].PortKey;
        things[4].Ports[0].State = false;
        event.State = false;
        api.sendPortEvent(event);
    });

    $("#button2").on("mousedown", function () {

        var event = new portevent();
        event.ThingKey = things[6].ThingKey;
        event.PortKey = things[6].Ports[0].PortKey;
        things[6].Ports[0].State = true
        event.State = true;
        api.sendPortEvent(event);
    });

    $("#button2").on("mouseup", function () {

        var event = new portevent();
        event.ThingKey = things[6].ThingKey;
        event.PortKey = things[6].Ports[0].PortKey;
        things[6].Ports[0].State = false;
        event.State = false;
        api.sendPortEvent(event);
    });

    $("#range1").on("change", function () {

        var event = new portevent();
        event.ThingKey = things[8].ThingKey;
        event.PortKey = things[8].Ports[0].PortKey;
        things[8].Ports[0].State = this.value;
        event.State = this.value;
        api.sendPortEvent(event);
    });

    $("#range2").on("change", function () {

        var event = new portevent();
        event.ThingKey = things[9].ThingKey;
        event.PortKey = things[9].Ports[0].PortKey;
        things[9].Ports[0].State = this.value;
        event.State = this.value;
        api.sendPortEvent(event);
    });

    things = create_things();
    var types = create_things_types();

    var cfg = new config("https://cyan.yodiwo.com", "wss://ws.yodiwo.com", "wsdemo", "e9b28a49-8910-470c-9254-cd1788a5ba84");

    api = new wsapi(cfg);

    /*
     * CALLBACK HANDLERS - START
     */

    /*
     * MANDATORY 
     */
    api.onError = function (error, ecode, msg) {

        console.log("ERROR: ", error, ecode, msg);

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
                    api.pair();
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
            var image = document.getElementById("bulb");
            // get value of slider
            var range = parseFloat(event.State);
            // convert value to 0-255
            var value = Math.floor(range == 1.0 ? 255 : range * 256.0);
            // apply background color
            image.style.background = "rgb(255,255," + (255 - value) + ")";
            //
            things[1].Ports[0].State = event.State;
        }
        else if (event.PortKey == "t8-p1") {
            //TODO: check ID and Value
            var image = document.getElementById("bulb2");
            // get value of slider
            var range = parseFloat(event.State);
            // convert value to 0-255
            var value = Math.floor(range == 1.0 ? 255 : range * 256.0);
            // apply background color
            image.style.background = "rgb(255,255," + (255 - value) + ")";
            //
            things[7].Ports[0].State = event.State;
        }
        else if (event.PortKey == "t3-p1") {
            var messages = document.getElementById("messages");
            messages.innerHTML += "# " + event.State + "<br>";
            things[2].Ports[0].State = event.State;
        }

    };
    api.onPortStateReq = function (portkeys) {

        console.log("PORT STATE UPDATE", portkeys);
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

        states.forEach(function (pstate) {
            if (pstate.IsDeployed === true) {
                if (pstate.PortKey === "t1-p1") {
                    document.getElementById("checkbox").checked = (pstate.State === "True") ? true : false;
                }
                else if (pstate.PortKey === "t2-p1") {
                    //TODO: check ID and Value
                    var image = document.getElementById("bulb");
                    // get value of slider
                    var range = parseFloat(pstate.State);
                    // convert value to 0-255
                    var value = Math.floor(range == 1.0 ? 255 : range * 256.0);
                    // apply background color
                    image.style.background = "rgb(255,255," + (255 - value) + ")";
                    //
                    things[1].Ports[0].State = pstate.State;
                }
                else if (pstate.PortKey === "t8-p1") {
                    //TODO: check ID and Value
                    var image = document.getElementById("bulb2");
                    // get value of slider
                    var range = parseFloat(pstate.State);
                    // convert value to 0-255
                    var value = Math.floor(range == 1.0 ? 255 : range * 256.0);
                    // apply background color
                    image.style.background = "rgb(255,255," + (255 - value) + ")";
                    //
                    things[7].Ports[0].State = pstate.State;
                }
                else if (pstate.PortKey == "t3-p1") {
                    var messages = document.getElementById("messages");
                    messages.innerHTML += "# " + pstate.State + "<br>";
                    things[2].Ports[0].State = pstate.State;
                }
            }
        });
    };
    api.onCloudForcedNegotiationFinished = function () {
        var portKeys = [];

        api.sendPortStateReq(ws_pstateoperation.ACTIVEPORT, portKeys);
    };

    /*
     * OPTIONAL 
     */
    api.onThingUpdate = function (thing) {
        var ret = false;

        console.log("THING UPDATE", thing);
        ret = true;

        return ret;
    };
    api.onThingCreate = function (thing) {
        var ret = false;

        console.log("THING CREATE", thing);
        ret = true;

        return ret;
    };
    api.onThingDelete = function (id) {
        var ret = false;

        console.log("THING DELETE", id);
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
     If you have (securely) saved secrets from a previous sessions, set them here. 
     This will make your web app bypass pairing and login directly
     */
    //api.setSecrets(nodekey, secretkey);

    /* Start the web application */
    api.start();
});

function sendCheckBoxState(checked) {
    var event = new portevent();
    event.ThingKey = things[0].ThingKey;
    event.PortKey = things[0].Ports[0].PortKey;
    things[0].Ports[0].State = this.value;
    event.State = checked;
    api.sendPortEvent(event);
}

function sendCheckBox2State(checked) {
    var event = new portevent();
    event.ThingKey = things[5].ThingKey;
    event.PortKey = things[5].Ports[0].PortKey;
    things[5].Ports[0].State = this.value;
    event.State = checked;
    api.sendPortEvent(event);
}

function getLastConsoleLog() {
    var event = new portevent();
    event.ThingKey = things[2].ThingKey;
    event.PortKey = things[2].Ports[0].PortKey;
    var log = $("#messages").html();
    var msgs = log.split("<br>");
    if (msgs.length === 1)
        return msgs[0].replace(/# /, "");
    else
        return msgs[msgs.length - 2].replace(/# /, "");
}

/* 
SET YOUR THINGS HERE
*/
function create_things() {

    /*
     * THING #1 BOOLEAN CHECKBOX 1
     */
    var config1 = new ConfigParameter();
    config1.Name = "param_t1";
    config1.Value = "value_t1";

    var port1 = new port();
    port1.Name = "port";
    port1.Type = ws_porttype.BOOLEAN;
    port1.ioDirection = ws_portdirection.OUTPUT;
    port1.ConfFlags = ws_portconf.NONE;
    port1.Description = "";
    port1.PortKey = "p1";
    port1.State = "False"

    var uihints1 = new ThingUIHints();

    uihints1.IconURI = "/Content/VirtualGateway/img/icon-thing-checkbox.png";
    uihints1.Description = "";

    var thing1 = new thing();
    thing1.ThingKey = "t1";
    thing1.Name = "CheckBox1";
    thing1.Config.push(config1);
    thing1.Ports.push(port1);
    thing1.Type = "yodiwo.output.seekbars";
    thing1.UIHints = uihints1;

    /*
    * THING #2 DECIMAL-LIGHT 1
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
    port2.State = "0";

    var uihints2 = new ThingUIHints();
    uihints2.IconURI = "/Content/VirtualGateway/img/icon-thing-genericlight.png";
    uihints2.Description = "";

    var thing2 = new thing();
    thing2.ThingKey = "t2";
    thing2.Name = "DimmableLight1";
    thing2.Config.push(config2);
    thing2.Ports.push(port2);
    thing2.Type = "yodiwo.input.lights.dimmable";
    thing2.UIHints = uihints2;

    /*
    *THING #3 CONSOLE OUTPUT
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
    port3.State = "";

    var uihints3 = new ThingUIHints();
    uihints3.IconURI = "/Content/Designer/img/BlockImages/icon-consoleout.png";
    uihints3.Description = "";

    var thing3 = new thing();
    thing3.ThingKey = "t3";
    thing3.Name = "Console Output";
    thing3.Config.push(config3);
    thing3.Ports.push(port3);
    thing3.Type = "yodiwo.input.console";
    thing3.UIHints = uihints3;

    /*
    *THING #4 TEXT INPUT
    */
    var config4 = new ConfigParameter();
    config4.Name = "param_t4";
    config4.Value = "value_t4";

    var port4 = new port();
    port4.Name = "port";
    port4.Type = ws_porttype.STRING;
    port4.ioDirection = ws_portdirection.OUTPUT;
    port4.PortKey = "p1";
    port4.Description = "";
    port4.ConfFlags = ws_portconf.RECEIVEALLEVENTS;
    port4.State = "";

    var uihints4 = new ThingUIHints();
    uihints4.IconURI = "/Content/VirtualGateway/img/icon-thing-text.png";
    uihints4.Description = "";

    var thing4 = new thing();
    thing4.ThingKey = "t4";
    thing4.Name = "Text Input";
    thing4.Config.push(config4);
    thing4.Ports.push(port4);
    thing4.Type = "yodiwo.output.text";
    thing4.UIHints = uihints4;

    /*
    *THING #5 BUTTON 1
    */
    var config5 = new ConfigParameter();
    config5.Name = "param_t5";
    config5.Value = "value_t5";

    var port5 = new port();
    port5.Name = "port";
    port5.Type = ws_porttype.BOOLEAN;
    port5.ioDirection = ws_portdirection.OUTPUT;
    port5.PortKey = "p1";
    port5.Description = "";
    port5.ConfFlags = ws_portconf.RECEIVEALLEVENTS;
    port5.State = "False";

    var uihints5 = new ThingUIHints();
    uihints5.IconURI = "/Content/VirtualGateway/img/icon-thing-genericbutton.png";
    uihints5.Description = "";

    var thing5 = new thing();
    thing5.ThingKey = "t5";
    thing5.Name = "Button1";
    thing5.Config.push(config5);
    thing5.Ports.push(port5);
    thing5.Type = "yodiwo.output.buttons";
    thing5.UIHints = uihints5;

    /*
     * THING #6 BOOLEAN CHECKBOX 2
     */
    var config6 = new ConfigParameter();
    config6.Name = "param_t6";
    config6.Value = "value_t6";

    var port6 = new port();
    port6.Name = "port";
    port6.Type = ws_porttype.BOOLEAN;
    port6.ioDirection = ws_portdirection.OUTPUT;
    port6.ConfFlags = ws_portconf.NONE;
    port6.Description = "";
    port6.PortKey = "p1";
    port6.State = "False"

    var uihints6 = new ThingUIHints();
    uihints6.IconURI = "/Content/VirtualGateway/img/icon-thing-checkbox.png";
    uihints6.Description = "";

    var thing6 = new thing();
    thing6.ThingKey = "t6";
    thing6.Name = "CheckBox2";
    thing6.Config.push(config6);
    thing6.Ports.push(port6);
    thing6.Type = "yodiwo.output.seekbars";
    thing6.UIHints = uihints6;

    /*
    *THING #7 BUTTON 2
    */
    var config7 = new ConfigParameter();
    config7.Name = "param_t7";
    config7.Value = "value_t7";

    var port7 = new port();
    port7.Name = "port";
    port7.Type = ws_porttype.BOOLEAN;
    port7.ioDirection = ws_portdirection.OUTPUT;
    port7.PortKey = "p1";
    port7.Description = "";
    port7.ConfFlags = ws_portconf.RECEIVEALLEVENTS;
    port7.State = "False";

    var uihints7 = new ThingUIHints();
    uihints7.IconURI = "/Content/VirtualGateway/img/icon-thing-genericbutton.png";
    uihints7.Description = "";

    var thing7 = new thing();
    thing7.ThingKey = "t7";
    thing7.Name = "Button2";
    thing7.Config.push(config7);
    thing7.Ports.push(port7);
    thing7.Type = "yodiwo.output.buttons";
    thing7.UIHints = uihints7;

    /*
    * THING #8 DECIMAL-LIGHT 2
    */
    var config8 = new ConfigParameter();
    config8.Name = "param_t8";
    config8.Value = "value_t8";

    var port8 = new port();
    port8.Name = "Value";
    port8.Type = ws_porttype.DECIMAL;
    port8.ioDirection = ws_portdirection.INPUT;
    port8.PortKey = "p1";
    port8.Description = "";
    port8.ConfFlags = ws_portconf.RECEIVEALLEVENTS;
    port8.State = "0";

    var uihints8 = new ThingUIHints();
    uihints8.IconURI = "/Content/VirtualGateway/img/icon-thing-genericlight.png";
    uihints8.Description = "";

    var thing8 = new thing();
    thing8.ThingKey = "t8";
    thing8.Name = "DimmableLight2";
    thing8.Config.push(config8);
    thing8.Ports.push(port8);
    thing8.Type = "yodiwo.input.lights.dimmable";
    thing8.UIHints = uihints8;

    /*
     * THING #9 DECIMAL-SLIDER 1
     */
    var config9 = new ConfigParameter();
    config9.Name = "param_t9";
    config9.Value = "value_t9";

    var port9 = new port();
    port9.Name = "port";
    port9.Type = ws_porttype.DECIMAL;
    port9.ioDirection = ws_portdirection.OUTPUT;
    port9.ConfFlags = ws_portconf.NONE;
    port9.Description = "";
    port9.PortKey = "p1";

    var uihints9 = new ThingUIHints();
    uihints9.IconURI = "/Content/VirtualGateway/img/icon-thing-slider.png";
    uihints9.Description = "";

    var thing9 = new thing();
    thing9.ThingKey = "t9";
    thing9.Name = "Slider1";
    thing9.Config.push(config9);
    thing9.Ports.push(port9);
    thing9.Type = "yodiwo.output.seekbars";
    thing9.UIHints = uihints9;

    /*
     * THING #10 DECIMAL-SLIDER 2
     */
    var config10 = new ConfigParameter();
    config10.Name = "param_t10";
    config10.Value = "value_t10";

    var port10 = new port();
    port10.Name = "port";
    port10.Type = ws_porttype.DECIMAL;
    port10.ioDirection = ws_portdirection.OUTPUT;
    port10.ConfFlags = ws_portconf.NONE;
    port10.Description = "";
    port10.PortKey = "p1";

    var uihints10 = new ThingUIHints();
    uihints10.IconURI = "/Content/VirtualGateway/img/icon-thing-slider.png";
    uihints10.Description = "";

    var thing10 = new thing();
    thing10.ThingKey = "t10";
    thing10.Name = "Slider2";
    thing10.Config.push(config10);
    thing10.Ports.push(port10);
    thing10.Type = "yodiwo.output.seekbars";
    thing10.UIHints = uihints10;


    /* Things Pushing */
    var things = [];

    things.push(thing1);
    things.push(thing2);
    things.push(thing3);
    things.push(thing4);
    things.push(thing5);
    things.push(thing6);
    things.push(thing7);
    things.push(thing8);
    things.push(thing9);
    things.push(thing10);
    return things;
}

function create_things_types() {

    /*
    * CHECKBOX 1
    */
    var config1 = new ConfigDescription();
    config1.DefaultValue = "0";
    config1.Description = "config param 1";
    config1.Label = "param1";
    config1.Name = "param1";
    config1.Required = false;
    config1.Type = "boolean";
    config1.Minimum = 0;
    config1.Maximum = 0;
    config1.Stepsize = 0;
    config1.ReadOnly = false;

    var state1 = new StateDescription();

    var port1 = new PortDescription();
    port1.Description = "CheckBoxPort port";
    port1.Id = "checkbox";
    port1.Label = null;
    port1.Category = null;
    port1.State = state1;

    var model1 = new NodeModelType();
    model1.Id = "checkbox";
    model1.Name = "checkbox";
    model1.Description = "checkbox input";
    model1.Config.push(config1);
    model1.Port.push(port1);

    /*
    * BULB 1
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

    /*
    * TEXT INPUT
    */
    var config4 = new ConfigDescription();
    config4.DefaultValue = "0";
    config4.Description = "config param 1";
    config4.Label = "param1";
    config4.Name = "param1";
    config4.Required = false;
    config4.Type = "decimal";
    config4.Minimum = 0;
    config4.Maximum = 0;
    config4.Stepsize = 0;
    config4.ReadOnly = false;

    var state4 = new StateDescription();

    var port4 = new PortDescription();
    port4.Description = "Text port";
    port4.Id = "text";
    port4.Label = null;
    port4.Category = null;
    port4.State = state4;

    var model4 = new NodeModelType();
    model4.Id = "text";
    model4.Name = "text";
    model4.Description = "textinput";
    model4.Config.push(config4);
    model4.Port.push(port4);

    /*
    * BUTTON 1
    */
    var config5 = new ConfigDescription();
    config5.DefaultValue = "0";
    config5.Description = "config param 1";
    config5.Label = "param1";
    config5.Name = "param1";
    config5.Required = false;
    config5.Type = "boolean";
    config5.Minimum = 0;
    config5.Maximum = 0;
    config5.Stepsize = 0;
    config5.ReadOnly = false;

    var state5 = new StateDescription();

    var port5 = new PortDescription();
    port5.Description = "Button port";
    port5.Id = "button";
    port5.Label = null;
    port5.Category = null;
    port5.State = state5;

    var model5 = new NodeModelType();
    model5.Id = "button";
    model5.Name = "button";
    model5.Description = "button input";
    model5.Config.push(config5);
    model5.Port.push(port5);

    /*
    * CHECKBOX 2
    */
    var config6 = new ConfigDescription();
    config6.DefaultValue = "0";
    config6.Description = "config param 1";
    config6.Label = "param1";
    config6.Name = "param1";
    config6.Required = false;
    config6.Type = "boolean";
    config6.Minimum = 0;
    config6.Maximum = 0;
    config6.Stepsize = 0;
    config6.ReadOnly = false;

    var state6 = new StateDescription();

    var port6 = new PortDescription();
    port6.Description = "CheckBox2Port port";
    port6.Id = "checkbox2";
    port6.Label = null;
    port6.Category = null;
    port6.State = state6;

    var model6 = new NodeModelType();
    model6.Id = "checkbox2";
    model6.Name = "checkbox2";
    model6.Description = "checkbox2 input";
    model6.Config.push(config6);
    model6.Port.push(port6);

    /*
    * BUTTON 2
    */
    var config7 = new ConfigDescription();
    config7.DefaultValue = "0";
    config7.Description = "config param 1";
    config7.Label = "param1";
    config7.Name = "param1";
    config7.Required = false;
    config7.Type = "boolean";
    config7.Minimum = 0;
    config7.Maximum = 0;
    config7.Stepsize = 0;
    config7.ReadOnly = false;

    var state7 = new StateDescription();

    var port7 = new PortDescription();
    port7.Description = "Button2 port";
    port7.Id = "button2";
    port7.Label = null;
    port7.Category = null;
    port7.State = state7;

    var model7 = new NodeModelType();
    model7.Id = "button2";
    model7.Name = "button2";
    model7.Description = "button2 input";
    model7.Config.push(config7);
    model7.Port.push(port7);

    /*
    * BULB 2
    */
    var config8 = new ConfigDescription();
    config8.DefaultValue = "0";
    config8.Description = "config param 1";
    config8.Label = "param1";
    config8.Name = "param1";
    config8.Required = false;
    config8.Type = "decimal";
    config8.Minimum = 0;
    config8.Maximum = 0;
    config8.Stepsize = 0;
    config8.ReadOnly = false;

    var state8 = new StateDescription();
    state8.Minimum = 0;
    state8.Maximum = 1;
    state8.Step = 0.01;

    var port8 = new PortDescription();
    port8.Description = "Dimmer2 port";
    port8.Id = "DimmableLight2";
    port8.Label = "DimmableLight2";
    port8.Category = "DimmableLight2";
    port8.State = state8;

    var model8 = new NodeModelType();
    model8.Id = "bulb2";
    model8.Name = "bulb2";
    model8.Description = "bulb2 output";
    model8.Config.push(config8);
    model8.Port.push(port8);

    /*
    * SLIDER 1
    */
    var config9 = new ConfigDescription();
    config9.DefaultValue = "0";
    config9.Description = "config param 1";
    config9.Label = "param1";
    config9.Name = "param1";
    config9.Required = false;
    config9.Type = "decimal";
    config9.Minimum = 0;
    config9.Maximum = 0;
    config9.Stepsize = 0;
    config9.ReadOnly = false;

    var state9 = new StateDescription();

    var port9 = new PortDescription();
    port9.Description = "Slider1 port";
    port9.Id = "slider1";
    port9.Label = null;
    port9.Category = null;
    port9.State = state9;

    var model9 = new NodeModelType();
    model9.Id = "slider1";
    model9.Name = "slider1";
    model9.Description = "slider1 input";
    model9.Config.push(config9);
    model9.Port.push(port9);

    /*
    * SLIDER 2
    */
    var config10 = new ConfigDescription();
    config10.DefaultValue = "0";
    config10.Description = "config param 1";
    config10.Label = "param1";
    config10.Name = "param1";
    config10.Required = false;
    config10.Type = "decimal";
    config10.Minimum = 0;
    config10.Maximum = 0;
    config10.Stepsize = 0;
    config10.ReadOnly = false;

    var state10 = new StateDescription();

    var port10 = new PortDescription();
    port10.Description = "Slider2 port";
    port10.Id = "slider2";
    port10.Label = null;
    port10.Category = null;
    port10.State = state10;

    var model10 = new NodeModelType();
    model10.Id = "slider2";
    model10.Name = "slider2";
    model10.Description = "slider2 input";
    model10.Config.push(config10);
    model10.Port.push(port10);


    /* Models Pushing */
    var type = new NodeThingType();
    type.Type = "wsnode";
    type.Searchable = false;
    type.Description = "web socket sample node";
    type.Model.push(model1);
    type.Model.push(model2);
    type.Model.push(model3);
    type.Model.push(model4);
    type.Model.push(model5);
    type.Model.push(model6);
    type.Model.push(model7);
    type.Model.push(model8);
    type.Model.push(model9);
    type.Model.push(model10);
    var types = [];

    types.push(type);

    return types;
}