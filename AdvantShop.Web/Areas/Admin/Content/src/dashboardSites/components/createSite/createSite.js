(function (ng) {
    'use strict';

    /* @ngInject */
    var CreateSiteCtrl = function ($http, SweetAlert, toaster, $translate, designService, $timeout, $location, $window) {
        var ctrl = this;

        ctrl.animationImgs = [];
        ctrl.hoverImgStyle = [];

        var initStateTemplatesParams = {
            page: 1, //шаг бесплатных шаблонов
            infiniteScrollTemplatesTerminated: false, //флаги выключения infinite-scroll
            isLoadingTemplates: false, //флаги для спиннеров
        };

        ctrl.$onInit = function () {
            ctrl.activePageLanding = 0;
            ctrl.initSkipItems = 0;
            ctrl.SIZE = 12; // количество шаблонов за раз
            ctrl.templatesFree = [];
            ctrl.templatesPaid = [];
            ctrl.stateTemplateParams = Object.assign({}, initStateTemplatesParams);
            ctrl.isLoadingFunnelTemplates = false;
        };

        ctrl.selectCategory = function (category) {
            if (ctrl.current == category) {
                return;
            }

            if (ctrl.current == null) {
                var urlParams = $location.search();
                ctrl.current = urlParams.tabs || category;
            } else {
                ctrl.current = category;
                $location.search({ tabs: ctrl.current });
            }

            ctrl.getAllTemplates();
        };

        ctrl.getAllTemplates = function () {
            ctrl.isLoadingFunnelTemplates = true;
            $http
                .get('dashboard/getSiteTemplates', { params: { category: ctrl.current } })
                .then(function (reponse) {
                    ctrl.lpTemplates = null;
                    $timeout(function () {
                        ctrl.lpTemplates = reponse.data.LpTemplates;

                        ctrl.siteTemplates = reponse.data.Templates;
                        ctrl.getTemplates();

                        ctrl.isLoadingFunnelTemplates = false;
                    }, 0);
                })
                .catch(function () {
                    ctrl.isLoadingFunnelTemplates = false;
                });
        };

        ctrl.getTemplates = function () {
            if (ctrl.siteTemplates == null) {
                return;
            }

            ctrl.stateTemplateParams.isLoadingTemplates = true;

            var templates;

            if (ctrl.currentTypeStore == 'paid') {
                templates = ctrl.siteTemplates.filter(function (x) {
                    return x.Price > 0;
                });
            } else if (ctrl.currentTypeStore == 'free') {
                templates = ctrl.siteTemplates.filter(function (x) {
                    return x.Price == 0;
                });
            } else {
                templates = ctrl.siteTemplates;
            }

            var take = ctrl.SIZE * ctrl.stateTemplateParams.page;

            templates = templates.slice(0, take);

            ctrl.stateTemplateParams.page++;

            ctrl.templates = templates;
            ctrl.stateTemplateParams.isLoadingTemplates = false;
        };

        ctrl.setTypeStore = function (type) {
            ctrl.currentTypeStore = type;

            ctrl.resetTemplatesParams();
            ctrl.getTemplates();
        };

        ctrl.resetTemplatesParams = function () {
            ctrl.stateTemplateParams = Object.assign({}, initStateTemplatesParams);
        };

        ctrl.installTemplate = function (stringId, id, version, redirectUrl) {
            SweetAlert.info(null, {
                title: '<i class="fa fa-spinner fa-spin"></i>&nbsp;' + $translate.instant('Admin.Js.Design.TemplateInstalling'),
                showConfirmButton: false,
                allowOutsideClick: false,
                allowEscapeKey: false,
            });

            return designService
                .installTemplate(stringId, id, version)
                .then(function (response) {
                    if (response.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Design.TemplateInstalled'));
                        if (redirectUrl != null && redirectUrl.length > 0) {
                            $window.location.assign(redirectUrl);
                        } else {
                            $window.location.reload(true);
                        }
                    } else {
                        swal.close();
                        toaster.pop('error', '', $translate.instant('Admin.Js.Design.ErrorInstalledTemplate'));
                    }
                })
                .catch(function () {
                    swal.close();
                });
        };

        ctrl.getAnimationForImg = function () {
            if (event.target.height >= event.target.parentElement.offsetHeight) {
                var calcAnim =
                    (event.target.parentElement.offsetHeight / event.target.height) * 100 - 100; /* Высоту контейнера делим на высоту картинки*/

                return { transform: `translateY(${calcAnim}%)` };
            }
        };

        ctrl.initCarousel = function (carousel) {
            ctrl.carousel = carousel;
        };

        ctrl.selectPageLanding = function (index) {
            ctrl.activePageLanding = index;
            ctrl.carousel.goto(ctrl.activePageLanding, true);
        };

        ctrl.buttonCarousel = function (index, maxIndex) {
            if (index > maxIndex - 1) {
                ctrl.activePageLanding = 0;
            } else if (index < 0) {
                ctrl.activePageLanding = maxIndex - 1;
            } else {
                ctrl.activePageLanding = index;
            }
        };

        ctrl.setTabCategories = function (tabsCategory) {
            ctrl.tabsCategory = tabsCategory;
        };
    };

    ng.module('createSite', ['carousel', 'infinite-scroll']).controller('CreateSiteCtrl', CreateSiteCtrl);
})(window.angular);
