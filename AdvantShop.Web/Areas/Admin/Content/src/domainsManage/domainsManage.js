(function (ng) {
    'use strict';

    var DomainsManageCtrl = function ($window, $document, $timeout, urlHelper, $scope) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.switchOnDomainsManage();
            ctrl.addOnLoadIframe();

            if ($window.location.search == '?modal=open') {
                ctrl.connectYourDomain('#iframeDomainsManage');
            }
        };

        ctrl.$postLink = function () {
            ctrl.pageIsReady = true;

            var selectedValueDomainBinding = urlHelper.getUrlParamByName('selectedValueDomainBinding');
            if (selectedValueDomainBinding != null && selectedValueDomainBinding.length > 0) {
                ctrl.connectYourDomain('#iframeDomainsManage', selectedValueDomainBinding);
            }

            var openFunnelId = urlHelper.getUrlParamByName('openFunnel');
            if (openFunnelId != null && openFunnelId.length > 0) {
                ctrl.sendMessage('#iframeDomainsManage', 'openFunnel', openFunnelId);
            }
        };

        ctrl.switchOnDomainsManage = function (injectToNg) {
            ctrl.iframeType = 'domainsManage';
            if (injectToNg) {
                $scope.$digest();
            }
        };

        ctrl.switchOnPay = function () {
            ctrl.iframeType = 'pay';
            doPostMessageDeleteCallback('domainDataLoaded');
        };

        ctrl.connectYourDomain = function (iframeId, selectedValue) {
            if (ctrl.iframeType !== 'domainsManage') {
                ctrl.switchOnDomainsManage();
            }
            ctrl.togglePopover = false;

            $timeout(function () {
                doPostMessageWait('domainDataLoaded', function () {
                    doPostMessage($document[0].querySelector(iframeId), JSON.stringify({ name: 'connectYourDomain', selectedValue: selectedValue }));
                });
            }, 100);
        };

        ctrl.sendMessage = function (iframeId, name, selectedValue) {
            if (ctrl.iframeType !== 'domainsManage') {
                ctrl.switchOnDomainsManage();
            }

            $timeout(function () {
                doPostMessageWait('domainDataLoaded', function () {
                    doPostMessage($document[0].querySelector(iframeId), JSON.stringify({ name: name, selectedValue: selectedValue }));
                });
            }, 100);
        };

        ctrl.addOnLoadIframe = function () {
            window.onLoadIframeHandler = function () {
                ctrl.loadedIframe = true;
            };
        };
    };

    DomainsManageCtrl.$inject = ['$window', '$document', '$timeout', 'urlHelper', '$scope'];

    ng.module('domainsManage', []).controller('DomainsManageCtrl', DomainsManageCtrl);
})(window.angular);
