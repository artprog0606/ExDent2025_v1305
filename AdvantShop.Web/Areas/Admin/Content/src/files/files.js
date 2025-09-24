(function (ng) {
    'use strict';

    /* @ngInject */
    var FilesCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, $q, SweetAlert, $http, $translate, toaster, Upload) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Files.FileName'),
                    enableSorting: true,
                    enableCellEdit: false,
                },
                {
                    name: 'FileSizeString',
                    displayName: $translate.instant('Admin.Js.Files.FileSize'),
                    enableSorting: true,
                    enableCellEdit: false,
                    width: 150,
                },
                {
                    name: 'CreatedDateFormatted',
                    displayName: $translate.instant('Admin.Js.Files.CreatingDate'),
                    enableSorting: true,
                    enableCellEdit: false,
                    width: 150,
                },
                {
                    name: 'ModifiedDateFormatted',
                    displayName: $translate.instant('Admin.Js.Files.EditingDate'),
                    enableSorting: true,
                    enableCellEdit: false,
                    width: 150,
                },
                {
                    name: 'Path',
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a ng-href="{{row.entity.DownLoadLink}}" download>' +
                        $translate.instant('Admin.Js.Files.Download') +
                        '</a></div>"',
                    displayName: $translate.instant('Admin.Js.Files.Link'),
                    enableSorting: false,
                    enableCellEdit: false,
                    width: 100,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<ui-grid-custom-delete url="files/deleteFile" params="{\'Id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="files/deleteFile" params="{\'Id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Files.DeleteSelected'),
                        url: 'files/deleteFiles',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Files.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Files.Deleting'),
                            })
                                .then(function (result) {
                                    if (result === true || result.value) {
                                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                                        return $q.resolve('sweetAlertConfirm');
                                    }

                                    return $q.reject('sweetAlertCancel');
                                })
                                .catch(function (dismiss) {
                                    if (dismiss !== 'sweetAlertCancel') {
                                        throw dismiss;
                                    }
                                });
                        },
                    },
                ],
            },
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.upload = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                Upload.upload({
                    url: '/files/uploadFile',
                    data: {
                        file: $file,
                        rnd: Math.random(),
                    },
                }).then(function (response) {
                    var data = response.data;
                    if (data.Result != true) {
                        toaster.pop('error', $translate.instant('Admin.Js.Files.ErrorWhileLoadingFile'), data.Error);
                    }
                });
            } else if ($invalidFiles.length > 0) {
                toaster.pop(
                    'error',
                    $translate.instant('Admin.Js.Files.ErrorWhileLoadingFile'),
                    $translate.instant('Admin.Js.Files.FileDoesNotMeet'),
                );
            }
        };

        ctrl.fetchData = function () {
            ctrl.grid.fetchData();
            return {
                result: true,
            };
        };

        ctrl.addFileMobile = function () {
            let btn_uploader = document.querySelector('.picture-uploader-buttons .btn-success');
            if (btn_uploader) {
                btn_uploader.click();
            }
        };
    };

    ng.module('files', ['uiGridCustom', 'urlHelper', 'fileUploader', 'pictureUploader']).controller('FilesCtrl', FilesCtrl);
})(window.angular);
