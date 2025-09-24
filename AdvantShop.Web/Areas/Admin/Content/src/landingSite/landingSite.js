import editFunnelNameTemplate from './modal/editFunnelName.html';
(function (ng) {
    'use strict';

    /* @ngInject */
    var LandingSiteAdminCtrl = function (
        uiGridConstants,
        uiGridCustomConfig,
        $translate,
        landingsService,
        toaster,
        $http,
        SweetAlert,
        $window,
        $location,
        $document,
        isMobileService,
        $uibModal,
        advTrackingService,
        $timeout,
        $q,
    ) {
        var TAB_SEARCH_NAME = 'landingAdminTab';
        var SETTINGSTAB_SEARCH_NAME = 'landingSettingsTab';
        var ctrl = this;
        ctrl.$onInit = function () {
            var search = $location.search();
            ctrl.initScaleIframe = [];
            ctrl.tab = (search != null && search[TAB_SEARCH_NAME]) || 'pages';
            ctrl.settingsTab = (search != null && search[SETTINGSTAB_SEARCH_NAME]) || 'common';
        };
        ctrl.changeTab = function (tab) {
            if (tab != null) {
                ctrl.tab = tab;
            }
            $location.search(TAB_SEARCH_NAME, ctrl.tab);
        };
        ctrl.changeSettingsTab = function (tab) {
            ctrl.settingsTab = tab;
            $location.search(SETTINGSTAB_SEARCH_NAME, tab);
        };
        ctrl.getDomainsTab = function () {
            $window.open($window.location.href + '?funnelTab=settings&funnelSettingsTab=domains', '_self');
        };
        ctrl.deleteFunnel = function (siteId) {
            SweetAlert.confirm($translate.instant('Admin.Js.SureWantDeleteSite'), {
                title: $translate.instant('Admin.Js.Delete'),
                cancelButtonText: $translate.instant('Admin.Js.Cancel'),
            }).then(function (result) {
                if (result && !result.isDismissed) {
                    $http
                        .post('dashboard/deleteSite', {
                            id: siteId,
                            type: 1,
                        })
                        .then(function (response) {
                            toaster.pop('success', '', $translate.instant('Admin.Js.Success'));
                            window.location.reload();
                        });
                }
            });
        };
        ctrl.initSite = function (id, siteUrl, orderSourceId) {
            ctrl.id = id;
            ctrl.actualSiteUrl = siteUrl;
            ctrl.orderSourceId = orderSourceId;
            ctrl.settings = {
                id: id,
            };
            ctrl.getDomain();
            ctrl.getSiteMapInfo();
            ctrl.getStats();
            ctrl.getFunnelDomains();
        };
        ctrl.openEditNameModal = function () {
            return $uibModal
                .open({
                    bindToController: true,
                    controller: 'EditFunnelNameModalCtrl',
                    controllerAs: 'ctrl',
                    keyboard: true,
                    templateUrl: editFunnelNameTemplate,
                    resolve: {
                        title: function () {
                            return ctrl.title;
                        },
                        id: function () {
                            return ctrl.id;
                        },
                    },
                })
                .result.then(function (title) {
                    ctrl.title = title;
                    ctrl.settings.SiteName = title;
                });
        };
        ctrl.updateTitle = function (id, value) {
            landingsService.updateTitle(id, value).then(function (data) {
                if (data.result == true) {
                    toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    ctrl.title = value;
                    ctrl.settings.SiteName = value;
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };
        ctrl.setEnabled = function (enabled) {
            $http
                .post('funnels/setSiteEnabled', {
                    id: ctrl.id,
                    enabled: enabled,
                })
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                        ctrl.siteEnabled = enabled;
                    }
                });
        };
        ctrl.copyLandingPage = function (pageId) {
            SweetAlert.confirm($translate.instant('Admin.Js.GridCustomComponent.AreYouSureCopy'), {
                title: $translate.instant('Admin.Js.GridCustomComponent.Copying'),
                confirmButtonText: $translate.instant('Admin.Js.GridCustomComponent.OK'),
                cancelButtonText: $translate.instant('Admin.Js.GridCustomComponent.Cancel'),
            }).then(function (result) {
                if (result === true || result.value) {
                    landingsService
                        .copyLandingPage(pageId)
                        .then(function (data) {
                            if (data.obj != null && data.obj.Lp != null) {
                                toaster.success('', $translate.instant('Admin.Js.GridCustom.ChangesSaved'));
                                $window.location.reload();
                            } else if (data.errors != null && data.errors.length > 0) {
                                toaster.error('', data.errors.join('<br>'));
                            }
                        })
                        .catch(function (err) {
                            console.error(err.message);
                        });
                }
            });
        };
        ctrl.copyLandingSite = function (siteId) {
            SweetAlert.confirm($translate.instant('Admin.Js.GridCustomComponent.AreYouSureCopy'), {
                title: $translate.instant('Admin.Js.GridCustomComponent.Copying'),
                confirmButtonText: $translate.instant('Admin.Js.GridCustomComponent.OK'),
                cancelButtonText: $translate.instant('Admin.Js.GridCustomComponent.Cancel'),
            }).then(function (result) {
                if (result === true || result.value) {
                    landingsService
                        .copyLandingSite(siteId)
                        .then(function (data) {
                            if (data.obj != null && data.obj != 0) {
                                toaster.success('', $translate.instant('Admin.Js.GridCustom.ChangesSaved'));
                                $window.location.assign('funnels/site/' + data.obj);
                            } else {
                                toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                            }
                        })
                        .catch(function (err) {
                            console.error(err.message);
                        });
                }
            });
        };

        // #region grid pages

        var columnDefs = [
            {
                name: 'Name',
                displayName: $translate.instant('Admin.Js.Landing.Name'),
                cellTemplate:
                    '<div class="ui-grid-cell-contents"><div><a ng-href="{{row.entity.TechUrl}}?inplace=true" onclick="return advTrack(\'Shop_Funnels_ViewPageEditor\');">{{row.entity.Name}}</a></div></div>',
            },
            {
                name: 'CreatedDateFormatted',
                displayName: $translate.instant('Admin.Js.Landing.DateAndTime'),
                width: 150,
            },
            {
                name: 'Enabled',
                displayName: $translate.instant('Admin.Js.Landing.Activ'),
                enableCellEdit: false,
                cellTemplate: '<ui-grid-custom-switch row="row" field-name="Enabled"></ui-grid-custom-switch>',
                width: 80,
            },
            {
                name: 'IsMain',
                displayName: $translate.instant('Admin.Js.Landing.Main'),
                enableCellEdit: false,
                cellTemplate: '<ui-grid-custom-switch row="row" field-name="IsMain"></ui-grid-custom-switch>',
                width: 80,
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 75,
                enableSorting: false,
                useInSwipeBlock: true,
                cellTemplate:
                    '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                    '<a href="" class="link-invert" title="Создать копию" ng-click="grid.appScope.$ctrl.gridExtendCtrl.copyLandingPage(row.entity.Id)"><i class="fa fa-clone"></i></a>' +
                    '<ui-grid-custom-delete url="funnels/deleteLandingPage" ng-if="!row.entity.IsMain" params="{\'Id\': row.entity.Id }"></ui-grid-custom-delete></div></div>' +
                    '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile && !row.entity.IsMain" url="funnels/deleteLandingPage" params="{\'Id\': row.entity.Id }" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">{{\'Admin.Js.Content.LandingSite.Remove\'|translate}}</ui-grid-custom-delete>',
            },
        ];
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Landing.DeleteSelected'),
                        url: 'funnels/deleteLandingPages',
                        field: 'Id',
                    },
                    {
                        text: $translate.instant('Admin.Js.Landing.MakeActive'),
                        url: 'funnels/activateLandingPages',
                        field: 'Id',
                    },
                    {
                        text: $translate.instant('Admin.Js.Landing.MakeInactive'),
                        url: 'funnels/disableLandingPages',
                        field: 'Id',
                    },
                ],
            },
        });
        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };
        ctrl.updateGrid = function () {
            ctrl.grid.fetchData();
        };

        // #endregion

        // #region Settings

        ctrl.getSettings = function () {
            $http
                .get('funnels/getSiteSettings', {
                    params: {
                        id: ctrl.id,
                    },
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result == true) {
                        ctrl.settings = data.obj;
                        ctrl.title = ctrl.settings.SiteName;
                        ctrl.actualSiteUrl = ctrl.settings.SiteUrl;
                        ctrl.settingsForm.$setPristine();
                    }
                });
        };
        ctrl.saveSettings = function () {
            $http.post('funnels/saveSiteSettings', ctrl.settings).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    // перезагрузка настроек, значения при сохранении могут измениться (напр., url)
                    ctrl.getSettings();
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };

        // #region Domain

        ctrl.getDomain = function () {
            $http
                .get('funnels/getSiteDomain', {
                    params: {
                        id: ctrl.id,
                    },
                })
                .then(function (response) {
                    ctrl.domain = response.data.domain;
                    ctrl.techdomain = response.data.techdomain;
                    ctrl.getSiteDomains();
                });
        };
        ctrl.getSiteDomains = function () {
            $http
                .get('funnels/getSiteDomains', {
                    params: {
                        siteId: ctrl.id,
                    },
                })
                .then(function (response) {
                    ctrl.domains = response.data;
                });
        };
        ctrl.getFunnelDomains = function () {
            $http
                .get('funnels/getFunnelDomains', {
                    params: {
                        siteId: ctrl.id,
                    },
                })
                .then(function (response) {
                    ctrl.funnelDomains = response.data;
                    if (ctrl.funnelDomains != null && ctrl.funnelDomains.length > 0) {
                        ctrl.reuseDomain = ctrl.funnelDomains[0];
                    }
                });
        };
        ctrl.addDomain = function (isAdditional) {
            $http
                .post('funnels/saveSiteDomain', {
                    id: ctrl.id,
                    domain: ctrl.newdomain,
                    isAdditional: isAdditional,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result) {
                        toaster.success('', $translate.instant('Admin.Js.Landing.SaveSiteDomain'));
                        ctrl.newdomain = '';
                        ctrl.getDomain();
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };
        ctrl.removeDomain = function (domain) {
            SweetAlert.confirm($translate.instant('Admin.Js.Landing.SureWantDeleteDomain'), {
                title: '',
            }).then(function (result) {
                if (result === true || result.value) {
                    $http
                        .post('funnels/removeSiteDomain', {
                            id: ctrl.id,
                            domain: domain,
                        })
                        .then(function (response) {
                            var data = response.data;
                            if (data.result) {
                                toaster.success('', $translate.instant('Admin.Js.Landing.DomainDeletedSuccessfully'));
                                ctrl.getDomain();
                            } else {
                                toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                            }
                        });
                }
            });
        };
        ctrl.addReuseDomain = function (reuseDomain) {
            $http
                .post('funnels/saveReuseSiteDomain', {
                    id: ctrl.id,
                    reuseDomain: reuseDomain,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result) {
                        toaster.success('', $translate.instant('Admin.Js.Landing.DomainAddedSuccessfully'));
                        ctrl.newdomain = '';
                        ctrl.getDomain();
                        ctrl.addDomainMode = 'new';
                        ctrl.getFunnelDomains();
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };

        // #endregion

        // #region Sitemap

        ctrl.generateSitemapXml = function () {
            $http
                .post('funnels/generateSitemapXml', {
                    siteId: ctrl.id,
                    useHttps: ctrl.settings.UseHttpsForSitemap,
                })
                .then(function (response) {
                    if (response.data.result) {
                        toaster.success('', $translate.instant('Admin.Js.Landing.GenerateSitemapXml'));
                    }
                    ctrl.getSiteMapInfo();
                });
        };
        ctrl.generateSitemapHtml = function () {
            $http
                .post('funnels/generateSitemapHtml', {
                    siteId: ctrl.id,
                    useHttps: ctrl.settings.UseHttpsForSitemap,
                })
                .then(function (response) {
                    if (response.data.result) {
                        toaster.success('', $translate.instant('Admin.Js.Landing.GenerateSitemapHtml'));
                    }
                    ctrl.getSiteMapInfo();
                });
        };
        ctrl.getSiteMapInfo = function () {
            $http
                .get('funnels/getSiteMapInfo', {
                    params: {
                        siteId: ctrl.id,
                    },
                })
                .then(function (response) {
                    ctrl.sitemapData = response.data;
                });
        };

        // #endregion

        // #region additional sales

        ctrl.gridFunnelProductsOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'ArtNo',
                    displayName: $translate.instant('Admin.Js.Landing.ArtNo'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a class="link-invert" ng-href="product/edit/{{row.entity.ProductId}}">{{COL_FIELD}}</a></div>',
                    width: 100,
                },
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Landing.Name'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="product/edit/{{row.entity.ProductId}}">{{COL_FIELD}}</a></div>',
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.unsetProductFunnel(row.entity.ProductId)" class="ui-grid-custom-service-icon fa fa-times link-invert"></a> ' +
                        '</div></div>' +
                        '<button ng-if="grid.appScope.$ctrl.isMobile" ng-click="grid.appScope.$ctrl.gridExtendCtrl.unsetProductFunnel(row.entity.ProductId)" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">{{\'Admin.Js.Content.LandingSite.Remove\'|translate}}</button>',
                },
            ],
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Landing.UnsetProductsFunnel'),
                        url: 'funnels/unsetProductsFunnel',
                        field: 'ProductId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Landing.SureUnsetProductsFunnel'), {
                                title: '',
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });
        ctrl.onGridFunnelProductsInit = function (grid) {
            ctrl.gridFunnelProducts = grid;
        };
        ctrl.unsetProductFunnel = function (productId) {
            SweetAlert.confirm($translate.instant('Admin.Js.Landing.SureUnsetProductFunnel'), {
                title: '',
            }).then(function (result) {
                if (result === true || result.value) {
                    $http
                        .post('funnels/unsetProductsFunnel', {
                            siteId: ctrl.id,
                            ids: [productId],
                        })
                        .then(function (response) {
                            ctrl.gridFunnelProducts.fetchData();
                        });
                }
            });
        };
        ctrl.addSiteProducts = function (result) {
            if (result == null || result.ids == null || result.ids.length == 0) return;
            $http
                .post('funnels/addSiteProducts', {
                    siteId: ctrl.id,
                    ids: result.ids,
                })
                .then(function (response) {
                    ctrl.gridFunnelProducts.fetchData();
                });
        };

        // #endregion

        // #region Auth

        ctrl.getDealStatuses = function (salesFunnelId) {
            if (salesFunnelId == null || salesFunnelId == '') return;
            $http
                .get('salesFunnels/getDealStatuses', {
                    params: {
                        salesFunnelId: salesFunnelId,
                    },
                })
                .then(function (response) {
                    ctrl.DealStatuses = response.data;
                });
        };
        ctrl.getAuthOrderProducts = function () {
            $http
                .get('funnels/getAuthOrderProducts', {
                    params: {
                        landingSiteId: ctrl.id,
                    },
                })
                .then(function (response) {
                    ctrl.AuthOrderProducts = response.data.obj;
                });
        };
        ctrl.selectAuthOrderProducts = function (result) {
            if (result == null || result.ids == null || result.ids.length === 0) return;
            $http
                .post('funnels/addAuthOrderProducts', {
                    landingSiteId: ctrl.id,
                    ids: result.ids,
                })
                .then(function (response) {
                    ctrl.getAuthOrderProducts();
                    toaster.success($translate.instant('Admin.Js.Common.ChangesSaved'));
                });
        };
        ctrl.deleteAuthOrderProduct = function (productId) {
            SweetAlert.confirm($translate.instant('Admin.Js.Common.Deleting'), {
                title: $translate.instant('Admin.Js.Common.AreYouSureDelete'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http
                        .post('funnels/deleteAuthOrderProduct', {
                            landingSiteId: ctrl.id,
                            productId: productId,
                        })
                        .then(function (response) {
                            ctrl.getAuthOrderProducts();
                            toaster.success($translate.instant('Admin.Js.Common.ChangesSaved'));
                        });
                }
            });
        };

        // #endregion

        // #region MobileApp

        ctrl.onChangeMobileAppActiveStateOffOn = function (checked) {
            ctrl.settings.MobileAppActive = checked;
        };

        // #endregion

        // #endregion

        // #region Funnel Stats

        var timerStats,
            documentIsVisible = $document[0].visibilityState === 'visible';
        ctrl.getStats = function () {
            if (documentIsVisible !== true) {
                return;
            }
            $http
                .get('funnels/getFunnelStats', {
                    params: {
                        orderSourceId: ctrl.orderSourceId,
                    },
                })
                .then(function (response) {
                    ctrl.stats = response.data;
                    if (timerStats != null) {
                        clearTimeout(timerStats);
                    }
                    timerStats = setTimeout(function () {
                        ctrl.getStats();
                    }, 30 * 1000);
                });
        };
        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };
        ctrl.onSelectTabInside = function (indexTab) {
            ctrl.tabInsideActiveIndex = indexTab;
        };
        ctrl.back = function () {
            if (ctrl.tabInsideActiveIndex != null) {
                ctrl.onSelectTabInside(null);
            } else {
                ctrl.onSelectTab(null);
            }
        };
        ctrl.addTrigger = function () {
            document.querySelector('.landig-site-trigger--mobile').click();
        };
        ctrl.settingsFormBtnClick = function () {
            $timeout(function () {
                document.querySelector('.btn-mobile__from-outsite-click').click();
            }, 0);
        };
        ctrl.scaleIframeDashboardSites = function () {
            if (window.matchMedia('(max-width: 1170px)').matches == true) {
                return {
                    transform: `scale(${((window.innerWidth - 32) / 1170).toFixed(2)})`,
                };
            }
            return {};
        };
        $document.on('visibilitychange', function () {
            documentIsVisible = $document[0].visibilityState === 'visible';
            if (documentIsVisible === true) {
                ctrl.getStats();
            }
        });

        // #endregion
    };
    ng.module('landingSite', [
        'landings',
        'isMobile',
        'triggers',
        'lead',
        'leadInfo',
        'leads',
        'leadEvents',
        'callRecord',
        'uiAceTextarea',
        'productsSelectvizr',
        'pictureUploader',
        'advTracking',
        'order',
        'tasks',
    ]).controller('LandingSiteAdminCtrl', LandingSiteAdminCtrl);
})(window.angular);
