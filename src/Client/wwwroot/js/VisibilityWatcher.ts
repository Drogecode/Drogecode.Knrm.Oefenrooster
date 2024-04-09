let dotNetHelper;
export function AddVisibilityWatcher(dotNet) {
    dotNetHelper = dotNet;
    window.addEventListener("visibilitychange", function () {
        console.log("Visibility changed");
        if (document.visibilityState === "visible") {
            console.log("APP resumed");
            dotNetHelper.invokeMethodAsync('VisibilityChange');
        }
    });
}