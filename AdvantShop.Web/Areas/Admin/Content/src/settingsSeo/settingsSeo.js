import addEdit301RedTemplate from '../settings/modal/addEdit301Red/addEdit301Red.html';
(function (ng) {
    'use strict';

    var SettingsSeoCtrl = function (
        Upload,
        $http,
        toaster,
        $q,
        $location,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        SweetAlert,
        $translate,
        isMobileService,
    ) {
        var ctrl = this;

        ctrl.resetMeta = function (metaType) {
            return SweetAlert.confirm($translate.instant('Admin.Js.SettingsSeo.AreYouSureResetMeta'), {
                title: $translate.instant('Admin.Js.SettingsSeo.ResettingMetaInfortmation'),
            }).then(function (result) {
                return result === true || result.value
                    ? $http
                          .post('/settingsseo/ResetMetaInfoByType', {
                              metaType: metaType,
                          })
                          .then(function (response) {
                              if (response.data.result === true) {
                                  toaster.pop('success', $translate.instant('Admin.Js.SettingsSeo.MetaInfrotmationWasReset'), '');
                              } else {
                                  toaster.pop({
                                      type: 'error',
                                      title: $translate.instant('Admin.Js.SettingsSeo.ErrorWhileResettingMeta'),
                                      timeout: 0,
                                  });
                              }
                          })
                    : //$q.resolve('sweetAlertConfirm')

                      $q.reject('sweetAlertCancel');
            });
        };

        ctrl.googleAnalitycsOauth = function (link) {
            var width = 600;
            var hight = 600;
            var left = screen.width / 2 - width / 2;
            var top = screen.height / 2 - hight / 2;

            var params = 'width = ' + width + ', height = ' + hight + ', left = ' + left + ', top = ' + top;

            var basePath = document.getElementsByTagName('base')[0].getAttribute('href');
            link = basePath.replace('adminv3/', '').replace('adminv2/', '').replace('admin/', '') + link;
            var oauthWindow = $window.open(link, 'GoogleAnalitics login', params);
            oauthWindow.focus();
        };

        //#region start 301Redirect

        ctrl.upload301 = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                Upload.upload({
                    url: '/Redirect301/Import',
                    data: {
                        file: $file,
                        rnd: Math.random(),
                    },
                }).then(function (response) {
                    var data = response.data;
                    if (data.Result === true) {
                        toaster.pop('succes', $translate.instant('Admin.Js.SettingsSeo.FileSuccessfullyUploaded'));
                        ctrl.grid.fetchData();
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.SettingsSeo.ErrorUploading'), data.Error);
                    }
                });
            } else if ($invalidFiles.length > 0) {
                toaster.pop(
                    'error',
                    $translate.instant('Admin.Js.SettingsSeo.ErrorUploading'),
                    $translate.instant('Admin.Js.SettingsSeo.FileDoesNotMeetRequirements'),
                );
            }
        };

        ctrl.startExport301Red = function () {
            $window.location.assign('redirect301/Export');
        };

        ctrl.setActive301 = function (state) {
            $http
                .get('redirect301/getactive', {
                    params: { active: state, rnd: Math.random() },
                })
                .then(function (response) {
                    var active = response.data;
                    if (active == null || active == false || active == undefined) {
                        ctrl.active301 = false;
                        ctrl.active301in404 = false;
                    } else {
                        ctrl.active301 = true;
                        ctrl.active301in404 = true;
                    }
                });
        };

        var columnDefs301Red = [
            {
                name: 'RedirectFrom',
                displayName: $translate.instant('Admin.Js.SettingsSeo.Whence'),
                enableCellEdit: true,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsSeo.Whence'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'RedirectFrom',
                },
            },
            {
                name: 'RedirectTo',
                displayName: $translate.instant('Admin.Js.SettingsSeo.Where'),
                enableCellEdit: true,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsSeo.Where'),
                    name: 'RedirectTo',
                    type: uiGridConstants.filter.INPUT,
                },
                uiGridCustomEdit: { replaceNullable: false },
            },
            {
                name: 'ProductArtNo',
                displayName: $translate.instant('Admin.Js.SettingsSeo.ProductVendorCodeOptional'),
                type: uiGridConstants.filter.INPUT,
                enableCellEdit: true,
                width: 150,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsSeo.ProductVendorCodeOptional'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'ProductArtNo',
                },
                uiGridCustomEdit: { replaceNullable: false },
            },
            {
                name: 'CreatedFormatted',
                displayName: $translate.instant('Admin.Js.SettingsSeo.Created'),
                width: 140,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsSeo.Created'),
                    type: 'datetime',
                    term: {
                        from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                        to: new Date(),
                    },
                    datetimeOptions: {
                        from: { name: 'CreatedFrom' },
                        to: { name: 'CreatedTo' },
                    },
                },
            },
            {
                name: 'EditedFormatted',
                displayName: $translate.instant('Admin.Js.SettingsSeo.Edited'),
                width: 140,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsSeo.Edited'),
                    type: 'datetime',
                    term: {
                        from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                        to: new Date(),
                    },
                    datetimeOptions: {
                        from: { name: 'EditedFrom' },
                        to: { name: 'EditedTo' },
                    },
                },
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 75,
                enableSorting: false,
                useInSwipeBlock: true,
                cellTemplate:
                    '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div class="">' +
                    '<ui-modal-trigger data-controller="\'ModalAddEdit301RedCtrl\'" controller-as="ctrl" ' +
                    'template-url="' +
                    addEdit301RedTemplate +
                    '" ' +
                    'data-resolve="{\'id\': row.entity.ID}" ' +
                    'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                    '<button type="button" class="btn-icon ui-grid-custom-service-icon fas fa-pencil-alt">{{COL_FIELD}}</button>' +
                    '</ui-modal-trigger>' +
                    '<ui-grid-custom-delete url="redirect301/deleteRedirect301" params="{\'Ids\': row.entity.ID}"></ui-grid-custom-delete></div></div>' +
                    '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="redirect301/deleteRedirect301" params="{\'Ids\': row.entity.ID}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
            },
        ];

        ctrl.gridOptions301Red = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs301Red,
            uiGridCustom: {
                rowUrl: '',
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.SettingsSystem.DeleteSelected'),
                        url: 'redirect301/deleteRedirect301',
                        field: 'ID',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.SettingsSystem.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.SettingsSystem.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
            paginationPageSize: 100,
        });

        ctrl.get301Red = function (params) {
            return $http
                .post('redirect301/getRedirect301', {
                    params: params,
                    rnd: Math.random(),
                })
                .then(function (response) {
                    return response.data;
                });
        };

        ctrl.delete301Red = function (IDs) {
            return $http
                .post('redirect301/deleteRedirect301', {
                    Ids: IDs,
                    rnd: Math.random(),
                })
                .then(function (response) {
                    return response.data;
                });
        };

        ctrl.grid301RedOnInit = function (grid301Red) {
            ctrl.grid301Red = grid301Red;
        };

        //#endregion 301Redirect

        //#region start ErrorLog404

        var columnDefsErrorLog404 = [
            {
                name: 'Url',
                displayName: $translate.instant('Admin.Js.SettingsSeo.404OfPage'),
                enableCellEdit: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsSeo.404OfPage'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Url',
                },
            },
            {
                name: 'UrlReferer',
                displayName: $translate.instant('Admin.Js.SettingsSeo.Referrer'),
                enableCellEdit: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsSeo.Referrer'),
                    name: 'UrlReferer',
                    type: uiGridConstants.filter.INPUT,
                },
            },
            {
                name: 'RedirectTo',
                displayName: $translate.instant('Admin.Js.SettingsSeo.301redirect'),
                type: uiGridConstants.filter.INPUT,
                enableCellEdit: false,
                width: 130,
                cellTemplate:
                    '<div class="ui-grid-cell-contents"><div ng-if="row.entity.RedirectTo == null || row.entity.RedirectTo == \'\'">' +
                    '<ui-modal-trigger data-controller="\'ModalAddEdit301RedCtrl\'" controller-as="ctrl" ' +
                    'template-url="' +
                    addEdit301RedTemplate +
                    '" ' +
                    'data-resolve="{\'from\': { value: row.entity.Url}}" ' +
                    'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                    '<a ng-href="">' +
                    $translate.instant('Admin.Js.SettingsSeo.CreateARedirect') +
                    '</a>' +
                    '</ui-modal-trigger>' +
                    '</div><div ng-if="row.entity.RedirectTo != null && row.entity.RedirectTo != \'\'">{{COL_FIELD}}</div></div>',
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsSeo.301redirect'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'RedirectTo',
                },
            },
            {
                name: 'DateAddedFormatted',
                displayName: $translate.instant('Admin.Js.SettingsSeo.Date'),
                enableCellEdit: false,
                width: 120,
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 45,
                enableSorting: false,
                useInSwipeBlock: true,
                cellTemplate:
                    '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div class="">' +
                    '<ui-grid-custom-delete url="ErrorLog404/DeleteItems" params="{\'Ids\': row.entity.Id}"></ui-grid-custom-delete></div></div>' +
                    '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="ErrorLog404/DeleteItems" params="{\'Ids\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
            },
        ];

        ctrl.gridOptionsErrorLog404 = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsErrorLog404,
            uiGridCustom: {
                rowUrl: '',
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.SettingsSystem.DeleteSelected'),
                        url: 'ErrorLog404/DeleteItems',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.SettingsSystem.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.SettingsSystem.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });

        ctrl.gridErrorLog404OnInit = function (gridErrorLog404) {
            ctrl.gridErrorLog404 = gridErrorLog404;
        };

        //#endregion ErrorLog404

        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };
    };

    SettingsSeoCtrl.$inject = [
        'Upload',
        '$http',
        'toaster',
        '$q',
        '$location',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        'SweetAlert',
        '$translate',
        'isMobileService',
    ];

    ng.module('settingsSeo', [
        'ngFileUpload',
        'toaster',
        'as.sortable',
        'paymentMethodsList',
        'import',
        'fileUploader',
        'uiAceTextarea',
        'isMobile',
    ]).controller('SettingsSeoCtrl', SettingsSeoCtrl);
})(window.angular);
