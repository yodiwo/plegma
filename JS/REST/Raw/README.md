### __Yodiwo RESTful Node__

__Overview__

This code creates the simplest form of a Yodiwo Node using pure HTTPS requests. It:
- creates a webpage with 4 simple widgets or "Things" (with textual, decimal and boolean values as readings)
- pairs the Node with the Yodiwo Cloud Platform (only needs to happen once)
- registers the Node and Things against the platform (only needs to happen once)
- sends values to the platform on every click

As a result, on [Cyan](https://cyan.yodiwo.com) a new node ("Rest Sample Node") will appear (on the Dashboard, Designer and Things Manager) with 4 Things:
- a boolean check box (sends single True or False events on click)
- a button (sends pulses, i.e. True->False events)
- a decimal slider (values span `-1.00 -> 1.00` with 0.01 resolution)
- a text box (sends text strings on Enter)

These Things can then be used in stories on Cyan and interact with other Nodes, Services or Logic blocks.

This code provides the raw source code that can be embedded in another project. It normally cannot run on its own, since the pairing process cannot save the received cookies; browsers usually do not allow cookies for local pages. However this can be bypassed on some browsers with custom startup switches.

__How to use this sample Node to create your own__

The main resources are in the Content/ folder while the web page itself is contained in the `index.html` file. The node, as all of our source code provided in this repository, is split into two main layers:
- the API layer, which implements the Yodiwo Plegma API and that you do not need to change, although you are welcome to look into
- the Node layer, which is basically *your* layer:
    -  it specifies the node's Things, and if custom, Types as well
    -  it initiates and uses the API layer to send events
    -  it receives callbacks from the API layer (e.g. gets informed about errors or the outcome of the pairing process)

As a result, for this REST sample node, the file *`restNode.js`* is the only file that you need to change and adapt to your project.