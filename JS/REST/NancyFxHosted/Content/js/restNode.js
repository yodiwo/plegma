// -------------------------------------------------------------------------
// Global variables
// -------------------------------------------------------------------------
var things; // Array of Thing Objects
var types;  // Array of Thing Type Objects
var api;    // restApi Object that will handle the Web Application

// -------------------------------------------------------------------------
// document.ready Handler
// -------------------------------------------------------------------------
$(document).ready(function () {

    // initialize handlers for thing's events
    initThingHandlers();

    // Fill the array of Things
    things = create_things();

    // Fill the array of Thing Types
    types = create_things_types();

    // create a cfg object containing api's configuration
    var cfg = new config("https://cyan.yodiwo.com", "Rest Sample Node", guid());

    // create restApi passing in the configuration
    api = new restApi(cfg);

    /*
     * Mandatory callbacks definition
     *
     * We need to define some callback handlers before we can start the application
     * We need to define the callbacks below:
     *  - onNodeInfo, called when the api wants to send a 'NodeInfo' Msg
     *      this message contaings node's custom Thing Types (if any) defined on 
     *      'create_things_types' function
     *  - onThingSet, called when the api wants to send a 'ThingSet' Msg
     *      this message contaings node's thing defined on 'create_things' function
     *  - onError, called when an error is caused
     */
    api.onError = function (error, ecode, msg) {

        console.log("ERROR: ", error, ecode, msg);

        switch (error) {
            case api_error.API:
                console.log("Api Error: " + msg);
                break;
            case api_error.MSG:
                console.log("Error: " + msg);
                break;

            case api_error.ERROR:
                console.log("Error: " + msg);
                break;

            default:
        }
    };

    // called when the api responds with 'NodeInfoRsp' msg
    api.onNodeInfo = function () {
        // just return array of our thing types
        return types;
    };

    // called when the api responds with a 'ThingSet' msg
    api.onThingsGet = function () {
        // just return array of things
        return things;
    };

    /*
     * If you have saved secrets from a previous session, set them here. 
     * This will make your web app bypass pairing and login directly
     */

    //api.setSecrets(nodekey, secretkey);

    /* 
     * Start the rest node, using the start button
     */
    startNode();
});

/* 
 *   SET YOUR THINGS HERE
 */
function create_things() {
    /*
     * THING #1 - BOOLEAN CHECKBOX
     */

    // port
    var checkboxPort = new Port();
    checkboxPort.Name = "port";
    checkboxPort.Type = api_porttype.BOOLEAN;
    checkboxPort.ioDirection = api_ioDirection.OUTPUT;
    checkboxPort.Description = "";
    checkboxPort.PortKey = "p1";
    checkboxPort.State = "False"
    checkboxPort.ConfFlags = api_portconf.NONE;

    // uihints
    var checkboxUiHints = new ThingUIHints();
    checkboxUiHints.IconURI = "/Content/VirtualGateway/img/icon-thing-checkbox.png";
    checkboxUiHints.Description = "";

    // Thing Object
    var checkboxThing = new Thing();
    checkboxThing.ThingKey = "t1";
    checkboxThing.Name = "CheckBox";
    checkboxThing.Ports.push(checkboxPort);
    checkboxThing.Type = "com.yodiwo.output.seekbars";
    checkboxThing.UIHints = checkboxUiHints;

    /*
     * THING #2 - BUTTON
     */

    // port
    var buttonPort = new Port();
    buttonPort.Name = "port";
    buttonPort.Type = api_porttype.BOOLEAN;
    buttonPort.ioDirection = api_ioDirection.OUTPUT;
    buttonPort.PortKey = "p1";
    buttonPort.Description = "";
    buttonPort.State = "False";
    buttonPort.ConfFlags = api_portconf.NONE;

    //uihints
    var buttonUiHints = new ThingUIHints();
    buttonUiHints.IconURI = "/Content/VirtualGateway/img/icon-thing-genericbutton.png";
    buttonUiHints.Description = "";

    // Thing Object
    var buttonThing = new Thing();
    buttonThing.ThingKey = "t2";
    buttonThing.Name = "Button";
    buttonThing.Ports.push(buttonPort);
    buttonThing.Type = "com.yodiwo.output.buttons";
    buttonThing.UIHints = buttonUiHints;


    /*
     * THING #3 - TEXT INPUT
     */

    // port
    var textInputPort = new Port();
    textInputPort.Name = "port";
    textInputPort.Type = api_porttype.STRING;
    textInputPort.ioDirection = api_ioDirection.OUTPUT;
    textInputPort.PortKey = "p1";
    textInputPort.Description = "";
    textInputPort.State = "";
    textInputPort.ConfFlags = api_portconf.NONE;

    // uihints
    var textInputUiHints = new ThingUIHints();
    textInputUiHints.IconURI = "/Content/VirtualGateway/img/icon-thing-text.png";
    textInputUiHints.Description = "";

    // Thing
    var textInputThing = new Thing();
    textInputThing.ThingKey = "t3";
    textInputThing.Name = "Text Input";
    textInputThing.Ports.push(textInputPort);
    textInputThing.Type = "com.yodiwo.output.text";
    textInputThing.UIHints = textInputUiHints;


    /*
     * THING #4 - DECIMAL SLIDER
     */

    //port
    var sliderPort = new Port();
    sliderPort.Name = "port";
    sliderPort.Type = api_porttype.DECIMAL;
    sliderPort.ioDirection = api_ioDirection.OUTPUT;
    sliderPort.Description = "";
    sliderPort.PortKey = "p1";
    sliderPort.ConfFlags = api_portconf.NONE;

    //uihints
    var sliderUiHints = new ThingUIHints();
    sliderUiHints.IconURI = "/Content/VirtualGateway/img/icon-thing-slider.png";
    sliderUiHints.Description = "";

    // Thing Object
    var sliderThing = new Thing();
    sliderThing.ThingKey = "t4";
    sliderThing.Name = "Slider";
    sliderThing.Ports.push(sliderPort);
    sliderThing.Type = "com.yodiwo.output.seekbars";
    sliderThing.UIHints = sliderUiHints;

    // push all Things in 'things' array
    var things = [];

    things.push(checkboxThing);
    things.push(buttonThing);
    things.push(textInputThing);
    things.push(sliderThing);
    return things;
}

/* 
 *   SET ANY CUSTOM THING TYPES HERE
 */
function create_things_types() {
    return null;
}


// -------------------------------------------------------------------------
// DOM Event Handlers
// -------------------------------------------------------------------------

/* 
 *  SET THING HANDLERS
 */

function initThingHandlers() {
    // initialize handlers for thing's events
    $("#checkbox").on("change", function () {
        sendCheckBoxState(this.checked, 0);
    });

    $("#button").on("mousedown", function () {
        sendButtonState(true, 1);
    });

    $("#button").on("mouseup", function () {
        sendButtonState(false, 1);
    });

    $("#text").on("keypress", function (e) {
        // did the user pressed the 'ENTER' key?
        if (e.keyCode == 13) {
            console.log("'ENTER' key pressed");
            sendTextFieldState(this.value, 2);
        }
    });

    $("#range").on("change", function () {
        sendSliderState(this.value, 3);
    });
}

function sendPortEventWithApi(value, id) {
    // update port state in things array 
    things[id].Ports[0].State = value;
    things[id].Ports[0].RevNum++;

    // create Port Event
    var event = new PortEvent();
    event.ThingKey = things[id].ThingKey;
    event.PortKey = things[id].Ports[0].PortKey;
    event.RevNum = things[id].Ports[0].RevNum;
    event.State = value;

    // send Event
    api.onPortEvent(event);
}

function sendCheckBoxState(value, id) {
    if (id == 0) {
        sendPortEventWithApi(value, id);
    }
}

function sendButtonState(value, id) {
    if (id == 1) {
        sendPortEventWithApi(value, id);
    }
}

function sendTextFieldState(value, id) {
    if (id == 2) {
        sendPortEventWithApi(value, id);
    }
}

function sendSliderState(value, id) {
    if (id == 3) {
        sendPortEventWithApi(value, id);
    }
}


function startNode() {
    if (api.IsPaired() || api.pairingPending()) {
        // hide start application button
        $('#start').fadeOut();

        // start api
        api.start();

        // enable handler for click event of button '#forget'
        $('#forget').on('click', forgetButtonClickHandler);

        // show #forget' button
        $('#forget').fadeIn();
    }
    else {
        // enable handler for click event of button '#start'
        $('#start').on('click', startButtonClickHandler);
        $('#start').fadeIn();
    }
}

// Click event Handler for 'Start Application' button
function startButtonClickHandler() {
    // notify user
    $('#start').html('Starting...');

    api.start();

    // hide button
    $(this).fadeOut();

    $('#start').html('Start Application');

    // disable handler
    $('#start').off('click', startButtonClickHandler);
}

// Click event Handler for 'Forget Keys' button
function forgetButtonClickHandler() {

    $('#forget').html('Forgeting...');

    api.clear();
    // notify user
    toastr["info"]("Deleted cookies. You need to unpair node from Cyan too. Reload page if you want to pair again.")

    // hide button
    $(this).fadeOut();

    $('#forget').html('Forgeting');

    // disable handler
    $('#forget').off('click', forgetButtonClickHandler);

    // call start node, so the pair button shows again
    startNode();
}

