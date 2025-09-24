const CUSTOMER_INFO_TEMPLATE = chrome.extension.getURL('content/user-info/user-info.tpl.html');

class UserInfo {
    constructor(blockOnMainPageSelector, blockOnMessagePageSelector) {
        if (blockOnMainPageSelector == null && blockOnMessagePageSelector == null) {
            throw Error('Parameters of constructor "blockOnMainPage" or "blockOnMessagePage" is required.');
        }

        this.blockOnMainPageSelector = blockOnMainPageSelector;
        this.blockOnMessagePageSelector = blockOnMessagePageSelector;
    }

    getCustomerInfoTemplate() {
        return fetch(CUSTOMER_INFO_TEMPLATE)
            .then(function (response) {
                return response.text();
            })
            .catch(function () {
                throw 'Ошибка при загрузке шаблона расширения Advantshop CRM';
            });
    }

    bind() {
        const self = this,
            btnCreateOrder = document.getElementById('a_CRM_CreateOrder'),
            btnCreateLead = document.getElementById('a_CRM_CreateLead'),
            btnCreateCustomer = document.getElementById('a_CRM_CreateCustomer');

        if (btnCreateOrder != null) {
            btnCreateOrder.addEventListener('click', function () {
                self.createOrder();
            });
        }

        if (btnCreateLead) {
            btnCreateLead.addEventListener('click', function () {
                self.createLead();
            });
        }

        if (btnCreateCustomer) {
            btnCreateCustomer.addEventListener('click', function () {
                self.createCustomer();
            });
        }
    }

    createOrder() {
        chrome.runtime.sendMessage({ cmd: 'createOrder' }, function (response) {
            window.open(response.result.Data);
        });
    }

    createLead() {
        chrome.runtime.sendMessage({ cmd: 'createLead' }, function (response) {
            window.open(response.result.Data);
        });
    }

    createTask() {
        chrome.runtime.sendMessage({ cmd: 'createTask' }, function (response) {
            window.open(response.result.Data);
        });
    }

    createCustomer() {
        chrome.runtime.sendMessage({ cmd: 'createCustomer' }, function (response) {
            window.open(response.result.Data);
        });
    }

    destroy() {
        const blockOld = document.querySelector('.a-crm__wrapper');

        if (blockOld != null) {
            blockOld.parentNode.removeChild(blockOld);
        }
    }

    render(data) {
        const self = this;

        return this.getCustomerInfoTemplate().then(function (tpl) {
            const html = tmpl(tpl, data),
                blockOnMainPage = document.querySelector(self.blockOnMainPageSelector),
                blockOnMessagePage = document.querySelector(self.blockOnMessagePageSelector);

            if (blockOnMainPage) {
                blockOnMainPage.insertAdjacentHTML('afterEnd', html);
            } else if (blockOnMessagePage) {
                blockOnMessagePage.insertAdjacentHTML('beforeEnd', html);
            }

            return html;
        });
    }
}
