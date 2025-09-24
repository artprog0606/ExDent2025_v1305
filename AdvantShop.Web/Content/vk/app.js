class App {
    constructor() {
        const self = this;

        chrome.storage.sync.get(null, function (dataFromStorage) {
            self.widget = new Widget(dataFromStorage.siteUrl, dataFromStorage.apikey);
        });
    }

    signIn(url, sender, sendResponse) {
        const self = this;

        this.widget.authorize(url).then(function (response) {
            if (response.status !== 'error') {
                self.render();
            }

            sendResponse({
                result: response,
            });

            return response;
        });
        return true;
    }

    signOut() {
        this.widget.signOut();

        chrome.tabs.query({ active: true, lastFocusedWindow: true }, function (tabs) {
            if (tabs != null && tabs[0] != null) {
                chrome.tabs.sendMessage(tabs[0].id, { cmd: 'signOut' });
            }
        });
    }

    isAuthorized(callback) {
        this.widget.isAuthorized(callback);
    }

    render() {
        const self = this;

        this.getVkId(function (userVkId) {
            if (userVkId != null) {
                self.widget.getCustomerInfo(userVkId).then(function (data) {
                    chrome.tabs.query({ active: true, lastFocusedWindow: true }, function (tabs) {
                        if (tabs != null && tabs[0] != null) {
                            chrome.tabs.sendMessage(tabs[0].id, { cmd: 'render', params: data });
                        }
                    });
                });
            }
        });
    }

    getVkId(callback) {
        const self = this;

        chrome.tabs.query({ active: true, lastFocusedWindow: true }, function (tabs) {
            const currentUrl = tabs[0].url;

            const userVkId = self.widget.getVkId(currentUrl);

            callback(userVkId);
        });
    }

    getAuthorizeState(params, sender, sendResponse) {
        app.isAuthorized(function (result) {
            sendResponse({ result: result });
        });

        return true;
    }

    checkAuthorize() {
        app.isAuthorized(function (result) {
            if (result === true) {
                app.render();
            }
        });

        return true;
    }

    createOrder(params, sender, sendResponse) {
        const self = this;

        self.getVkId(function (userVkId) {
            if (userVkId != null) {
                self.widget.createOrder(userVkId).then(function (result) {
                    sendResponse({ result: result });
                });
            }
        });

        return true;
    }

    createLead(params, sender, sendResponse) {
        const self = this;

        self.getVkId(function (userVkId) {
            if (userVkId != null) {
                self.widget.createLead(userVkId).then(function (result) {
                    sendResponse({ result: result });
                });
            }
        });

        return true;
    }

    createTask(params, sender, sendResponse) {
        const self = this;

        self.getVkId(function (userVkId) {
            if (userVkId != null) {
                self.widget.createTask().then(function (result) {
                    sendResponse({ result: result });
                });
            }
        });

        return true;
    }

    createCustomer(params, sender, sendResponse) {
        const self = this;

        self.getVkId(function (userVkId) {
            if (userVkId != null) {
                self.widget.createCustomer(userVkId).then(function (result) {
                    sendResponse({ result: result });
                });
            }
        });

        return true;
    }
}

const app = new App();

chrome.extension.onMessage.addListener(function (request, sender, sendResponse) {
    if (app[request.cmd] != null) {
        return app[request.cmd](request.params, sender, sendResponse);
    } else {
        throw Error('Not found method in app.js');
    }
});

chrome.tabs.onUpdated.addListener(function (tabId, changeInfo, tab) {
    if (changeInfo.status === 'complete' && tab.active === true) {
        app.isAuthorized(function (result) {
            if (result === true) {
                app.render();
            }
        });
    }
});
