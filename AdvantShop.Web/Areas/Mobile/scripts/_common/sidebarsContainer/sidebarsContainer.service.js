function sidebarsContainerService() {
    var service = this;
    var storageContainers = {};
    var storageObserver = {};

    const callbackList = {
        onClose: new Set(),
        onOpen: new Set(),
    };

    service.addContainer = function (id, ctrl) {
        return (storageContainers['default'] = ctrl);
    };

    service.open = function (options) {
        return storageContainers['default'].open(options);
    };

    service.close = function () {
        return storageContainers['default'].close();
    };

    service.toggle = function (options) {
        return storageContainers['default'].toggle(options);
    };

    service.getState = function () {
        return storageContainers['default'].getState();
    };

    service.addObserverState = function (containerId, contentId, fn) {
        contentId = contentId || 'all';
        storageObserver['default'] = storageObserver['default'] || {};
        storageObserver['default'][contentId] = storageObserver['default'][contentId] || [];
        storageObserver['default'][contentId].push(fn);
    };

    service.processObserver = function (containerId, contentId, data, isOpen) {
        if (storageObserver['default'] != null) {
            if (storageObserver['default'][contentId] != null && storageObserver['default'][contentId].length > 0) {
                storageObserver['default'][contentId].forEach(function (fn) {
                    fn(data, isOpen);
                });
            }
            if (storageObserver['default']['all'] != null && storageObserver['default']['all'].length > 0) {
                storageObserver['default']['all'].forEach(function (fn) {
                    fn(data, isOpen);
                });
            }
        }
    };

    service.addCallback = function (nameEvent, callback, needDeleteAfterCall = false) {
        const eventList = callbackList[nameEvent];

        if (callback != null && eventList != null) {
            eventList.add({ callback, needDeleteAfterCall });
        }
    };

    service.callCallbacks = function (nameEvent) {
        const eventList = callbackList[nameEvent];

        if (eventList.size > 0) {
            for (let callbackObj of eventList) {
                if (typeof callbackObj.callback === `function`) {
                    callbackObj.callback();
                }
            }

            for (let callbackObj of eventList) {
                if (callbackObj.needDeleteAfterCall) {
                    eventList.delete(callbackObj);
                }
            }
        }
    };

    //service.clearOnCloseCallbacks = function () {
    //    onCloseCallbackList.clear();
    //};
}

export default sidebarsContainerService;
