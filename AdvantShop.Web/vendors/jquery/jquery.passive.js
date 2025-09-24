const checkAvalableAddEventListener = (self) => self != null && self.addEventListener != null;

jQuery.event.special.touchstart = {
    setup: function (_, ns, handle) {
        if (checkAvalableAddEventListener(this)) {
            this.addEventListener("touchstart", handle, {
                passive: !ns.some(
                    function (it) {
                        return it === "noPreventDefault" || it === "jstree" || it === "vakata";
                    }
                )
            });
        }
    }
};
jQuery.event.special.touchmove = {
    setup: function (_, ns, handle) {
        if (checkAvalableAddEventListener(this)) {
            this.addEventListener("touchmove", handle, {
                passive: !ns.some(
                    function (it) {
                        return it === "noPreventDefault" || it === "jstree" || it === "vakata";
                    }
                )
            });
        }
    }
};
jQuery.event.special.wheel = {
    setup: function (_, ns, handle) {
        this.addEventListener("wheel", handle, {passive: true});
    }
};
jQuery.event.special.mousewheel = {
    setup: function (_, ns, handle) {
        this.addEventListener("mousewheel", handle, {passive: true});
    }
};
