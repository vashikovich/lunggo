﻿window.monitor = new tv.Monitor("desktop"); $(function () { window.monitor.initialize(); window.onbeforeunload = function (a) { var b = window.monitor; b.postMonitorData(); for (var c = null, d = 0; d < b.unloadHooks.length; ++d) { var e = b.unloadHooks[d](); e && (c = e) } a = a || window.event; c && a && (a.returnValue = c); if (c) return c } });

//@ sourceMappingURL=../map/monitordesktop.js.map