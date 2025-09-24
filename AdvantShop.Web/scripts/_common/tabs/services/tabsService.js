/* @ngInject */
function tabsService($location) {
    var service = this,
        countInStorage = -1,
        storage = {},
        hashStart = `tab=`;

    service.addInStorage = function (tabs, id) {
        storage[id || (countInStorage += +1)] = tabs;
    };

    service.change = function (id) {
        var data = service.findTabByid(id);

        if (data != null) {
            data.tabs.change(data.pane);
        }
    };

    service.findTabByid = function (id) {
        var tabs, pane;

        for (var key in storage) {
            if (pane != null) {
                break;
            }

            tabs = storage[key];

            if (Object.prototype.hasOwnProperty.call(storage, key)) {
                pane = tabs.panes[id];
                break;
            }
        }

        return pane != null ? { tabs: tabs, pane: pane } : null;
    };

    service.getTabIdFromUrl = function () {
        return $location.hash().replace(hashStart, '');
    };

    service.changeUrl = function (tabId) {
        $location.hash(hashStart + tabId);
    };
}

export default tabsService;
