// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const registerServiceWorker = async () => {

    if ("serviceWorker" in navigator) { // Test that "serviceWorker" is supported

        try {

            // tldr; Always put 'service-worker' scripts at the root level!
            // Reference: stack overflow article: "navigator.serviceWorker is never ready"
            // The scope is tied to the location the service-worker.
            // So if you put the service worker Javascript file in a subdirectory (example '/js/service-worker.js'),
            // the scope needs to be change to '/js/'
            // However, this poses another issue when using Notifications API.
            // If we call Notification.requestPermission(), it is commonly registered at the site's root '/'
            // If the service worker is registered at the sub-directory level, it will not catch the notifications.
            const registration = await navigator.serviceWorker.register("/service-worker.js", {
                scope: "/",
            });

            let pollCount = 0;
            var refreshId = setInterval(function () {

                if (registration.installing) {
                    console.log(pollCount.toString() + "] Service worker installing");
                } else if (registration.waiting) {
                    console.log(pollCount.toString() + "] Service worker installed");
                } else if (registration.active) {
                    console.log(pollCount.toString() + "] Service worker active");
                    clearInterval(refreshId);
                }

                pollCount++;

            }, 0);

        } catch (error) {
            console.error(`Registration failed with ${error}`);
        }

        navigator.serviceWorker.ready.then(serviceWorkerRegistration => {
            //navigator.serviceWorker.getNotifications().then(notification => {
            //    console.log("Do something with notification");
            //});
            //serviceWorkerRegistration.showNotification('Service work ready ;-)');

            //let noticeServed = 0;
            //let intervalId = setInterval(() => {
            //    serviceWorkerRegistration.showNotification('Service work ready' + noticeServed.toString());
            //    if (noticeServed >= 3) {
            //        clearInterval(intervalId);
            //    }
            //}, 3000);

            //serviceWorkerRegistration.active.postMessage('Service worker is ready for action');
        });
    }
};

registerServiceWorker();

//const webSocketWorker = new SharedWorker('/web-socket-shared-worker.js');

//const sendMessageToSocket = (message) => {
//    webSocketWorker.port.postMessage({
//        action: 'send',
//        value: message
//    });
//};

//window.addEventListener('beforeunload', () => {
//    console.log("beforeunload triggered");
//    webSocketWorker.port.postMessage({
//        action: 'unload',
//        value: null
//    });

//    webSocketWorker.port.close();
//    //webSocketWorker.close();
//});


//webSocketWorker.port.addEventListener('message', ({ data }) => {
//    console.log(data);
//});

//webSocketWorker.port.start();

//console.log("OK - started");



const connectedPorts = new Set();

// Create socket instance.
const socket = new WebSocket("wss://localhost:5041/ws?username=mobuser001");

// Send initial package on open.
socket.addEventListener('open', () => {
    const data = JSON.stringify({
        "time": 123456,
        "channel": "futures.tickers",
        "event": "subscribe",
        "payload": ["BTC_USD", "ETH_USD"]
    });

    socket.send(data);
});

// Send data from socket to all open tabs.
socket.addEventListener('message', ({ data }) => {
    console.log("received message", data);
});

socket.addEventListener('connect', ({ ports }) => {

    console.log('Web socket connected');

    ///**
    // * Receive data from main thread and determine which
    // * actions it should take based on the received data.
    // */
    //port.addEventListener('message', ({ data }) => {
    //    const { action, value } = data;

    //    console.log(`WebSocket server received message ${action} -- ${value} ; ${data}`);

    //    // Send message to socket.
    //    if (action === 'send') {
    //        socket.send(JSON.stringify(value));

    //        // Remove port from connected ports list.
    //    } else if (action === 'unload') {
    //        port.postMessage(action);
    //        connectedPorts.delete(port);
    //    }
    //});


    //// Start the port broadcasting.
    //port.start();

});