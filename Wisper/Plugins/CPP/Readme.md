## __C++/C Bridge Plugin__

#### __Overview__

The code here allows you to develop Wisper plugins with native C++/C. It's split into 2 main folders:
- CSharp
- Native

#### __CSharp__

This is the c# glue code that allows the main Wisper code to communicate with the Native C++ part. It opens as a plugin under the main VS 2015 solution [found here](https://github.com/yodiwo/plegma/tree/master/Wisper) and can be used as the rest of plugins: its built folder placed under the Plugins folder of built Wisper with the proper name as specified in PUID of manifest.json.

However it can be built once (or never, just grab it from the CSharp/Built/ folder here) and used in conjuction with the .so output from the Native part.

-- -

#### __Native__

This is native C++11 code that implements a simple 'Echo' Thing (it sends back whatever it receives), meant to act as a sample for more complex development.

It is offered as a VS 2017 solution, but also can be built with any C++11 capable compiler.
It uses libboost header files; on debian-based systems the required package is `libboost-dev`.
It has been tested with GCC v4.9+.

The built `libplugin.so` object should then be placed directly into the main CPP plugin folder (e.g. `Plugins/Yodiwo.mNode.Plugins.CPP/`)
