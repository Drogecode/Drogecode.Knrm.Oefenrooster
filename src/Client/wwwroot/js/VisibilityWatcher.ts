let dotNetHelper;
let queuedUpCalls = [];
let timerId;

function AddVisibilityWatcher(dotNet) {
    dotNetHelper = dotNet;
    document.addEventListener('visibilitychange', (event) => {
        this.EventQueue(document.visibilityState);
    });
}

function EventQueue(eventName) {
    if (queuedUpCalls.indexOf(eventName) != -1) {
        return;
    }
    queuedUpCalls.push(eventName);
    if (timerId) {
        return;
    }
    timerId = setInterval(function () {
        if (!queuedUpCalls.length) {
            clearInterval(timerId);
            timerId = null;
            return;
        }

        let nextCallArg = queuedUpCalls.shift();
        dotNetHelper.invokeMethodAsync('VisibilityChange', nextCallArg);
    }, 1000);
}
