let dotNetHelper;
export function AddVisibilityWatcher(dotNet) {
    dotNetHelper = dotNet;
    window.addEventListener("visibilitychange", function () {
        if (document.visibilityState === "visible") {
            dotNetHelper.invokeMethodAsync('VisibilityChange');
        }
    });
}