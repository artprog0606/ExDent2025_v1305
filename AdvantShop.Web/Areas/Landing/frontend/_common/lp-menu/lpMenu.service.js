(function (ng) {
    'use strict';

    var lpMenuService = function ($rootScope, $window, domService, $document) {
        var service = this;
        var storage = {};
        var activeMenuId = null;
        var MEDIA_SHOW_MOBILE_MENU = '(max-width: 1024px)';
        var isIOS = /iPhone|iPad|iPod/i.test(navigator.userAgent);

        service.addInStorage = function (id) {
            return (storage[id] = {
                state: {
                    open: false,
                },
            });
        };

        service.open = function (id) {
            if (storage[id]) {
                storage[id].state.open = true;
                activeMenuId = id;
                if (isIOS) {
                    service.addOverflowHiddenForModal();
                }
            }
        };

        service.close = function (id) {
            if (storage[id]) {
                storage[id].state.open = false;
                activeMenuId = null;
                if (isIOS) {
                    service.removeOverflowHiddenForModal();
                }
            }
        };

        service.addOverflowHiddenForModal = function () {
            $document[0].body.classList.add('overflow-hidden-for-modal-ios');
        };

        service.removeOverflowHiddenForModal = function () {
            $document[0].body.classList.remove('overflow-hidden-for-modal-ios');
        };

        $window.addEventListener('click', function (event) {
            var isElInteractive;
            var inMenuContainer;

            if (activeMenuId != null) {
                isElInteractive = ['a', 'input', 'button'].some(function (item) {
                    return item === event.target.tagName.toLowerCase();
                });

                if (isElInteractive === true) {
                    inMenuContainer = Object.keys(storage).some(function (item) {
                        return domService.closest(event.target, '#' + item);
                    });

                    if (inMenuContainer === true) {
                        service.close(activeMenuId);
                        $rootScope.$apply();
                    }
                }
            }
        });

        var mql = $window.matchMedia(MEDIA_SHOW_MOBILE_MENU);

        mql.addListener(function (mql) {
            var storageKeys = Object.keys(storage);
            var isOpenMenu;
            for (var i = 0; i < storageKeys.length; i++) {
                var innerObj = storage[storageKeys[i]];
                if (innerObj != null && innerObj.state != null && innerObj.state.open) {
                    isOpenMenu = true;
                    break;
                }
            }
            if (mql.matches && isOpenMenu) {
                service.addOverflowHiddenForModal();
            } else {
                service.removeOverflowHiddenForModal();
            }
        });
    };

    ng.module('lpMenu').service('lpMenuService', lpMenuService);

    lpMenuService.$inject = ['$rootScope', '$window', 'domService', '$document'];
})(window.angular);
