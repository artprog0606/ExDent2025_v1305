import uiGridCustomTemplate from './templates/ui-grid-custom.html';
import uiGridCustomMobileTemplate from './templates/ui-grid-custom-mobile.html';
(function (ng) {
    'use strict';

    ng.module('uiGridCustom')
        .directive('uiGridCustom', [
            'uiGridCustomService',
            'urlHelper',
            function (uiGridCustomService, urlHelper) {
                return {
                    restrict: 'E',
                    templateUrl: uiGridCustomTemplate,
                    controller: 'UiGridCustomCtrl',
                    controllerAs: '$ctrl',
                    bindToController: true,
                    transclude: {
                        footer: '?uiGridCustomFooter',
                        overrideControl: '?uiGridCustomOverrideControl',
                        overrideHeaderControl: '?uiGridCustomOverrideHeaderControl',
                    },
                    scope: {
                        gridOptions: '<',
                        gridUrl: '<?',
                        gridInplaceUrl: '<?',
                        gridParams: '<?',
                        gridFilterEnabled: '<?',
                        gridFilterHiddenTotalItemsCount: '<?',
                        gridFilterSearchAutofocus: '<?',
                        gridSelectionEnabled: '<?',
                        gridPaginationEnabled: '<?',
                        gridTreeViewEnabled: '<?',
                        gridUniqueId: '@',
                        gridOnInplaceBeforeApply: '&',
                        gridOnInplaceApply: '&',
                        gridOnInplaceApplyAll: '&',
                        gridOnInplaceBeforeApplyAll: '&',
                        gridOnInit: '&',
                        gridSearchPlaceholder: '<?',
                        gridSearchVisible: '<?',
                        gridExtendCtrl: '<?',
                        gridEmptyText: '<?',
                        gridSelectionOnInit: '&',
                        gridSelectionOnChange: '&',
                        gridSelectionMassApply: '&',
                        gridOnFetch: '&',
                        gridOnDelete: '&',
                        gridOnBeforeDelete: '&',
                        gridOnPreinit: '&',
                        gridShowExport: '<?',
                        gridOnFilterInit: '&',
                        gridSelectionItemsSelectedFn: '&',
                        gridRowIdentificator: '<?',
                        gridPreventStateInHash: '<?',
                        gridFilterTemplateUrl: '<?',
                        gridKeyStoragePrefix: '<?',
                        gridSwipeLine: '<?',
                    },
                    compile: function (cElement, cAttrs, cTransclude) {
                        var uiGridElement = cElement[0].querySelector('[ui-grid]');
                        return function (scope, element, attrs, ctrl, transclude) {
                            //var uiGridElement = element[0].querySelector('[ui-grid]');

                            ctrl.gridSelectionEnabled == null || ctrl.gridSelectionEnabled === true
                                ? uiGridElement.setAttribute('ui-grid-selection', '')
                                : uiGridElement.removeAttribute('ui-grid-selection');
                            ctrl.gridTreeViewEnabled === true
                                ? uiGridElement.setAttribute('ui-grid-tree-view', '')
                                : uiGridElement.removeAttribute('ui-grid-tree-view');
                            scope.$on('modal.closing', function () {
                                ctrl.clearParams();
                                uiGridCustomService.removeFromStorage(ctrl.gridUniqueId);
                            });
                        };
                    },
                };
            },
        ])
        .component('uiGridCustomSwitch', {
            require: {
                uiGridCustom: '^uiGridCustom',
            },
            template:
                '<div class="ui-grid-cell-contents"><div class="js-grid-not-clicked"><switch-on-off checked="$ctrl.row.entity[$ctrl.fieldName || \'Enabled\']" on-change="$ctrl.uiGridCustom.setSwitchEnabled($ctrl.row.entity, checked, $ctrl.fieldName || \'Enabled\')" readonly="$ctrl.readonly" on-click="$ctrl.onClick()"></switch-on-off></div></div>',
            bindings: {
                row: '<',
                fieldName: '@',
                readonly: '<?',
                onClick: '&',
            },
        })
        .component('uiGridCustomDelete', {
            require: {
                uiGridCustom: '^^uiGridCustom',
            },
            transclude: true,
            template:
                '<button type="button" ng-click="$ctrl.delete($ctrl.url, $ctrl.params, $ctrl.confirmText)" ng-class="[$ctrl.classes, \'btn-icon\']" ng-transclude aria-label="Удалить"></button>',
            bindings: {
                url: '@',
                params: '<',
                confirmText: '@',
                onDelete: '&',
                classes: '@',
            },
            controller: [
                '$http',
                'SweetAlert',
                'toaster',
                'lastStatisticsService',
                '$translate',
                '$scope',
                function ($http, SweetAlert, toaster, lastStatisticsService, $translate, $scope) {
                    var ctrl = this;
                    ctrl.$onInit = function () {
                        if (ctrl.classes == null) {
                            ctrl.classes = 'ui-grid-custom-service-icon fa fa-times link-invert';
                        }
                    };
                    ctrl.delete = function (url, params, confirmText) {
                        SweetAlert.confirm(confirmText != null ? confirmText : $translate.instant('Admin.Js.GridCustomComponent.AreYouSureDelete'), {
                            title: $translate.instant('Admin.Js.GridCustomComponent.Deleting'),
                            confirmButtonText: $translate.instant('Admin.Js.GridCustomComponent.Confirm'),
                            cancelButtonText: $translate.instant('Admin.Js.GridCustomComponent.Cancel'),
                        }).then(function (result) {
                            if (result === true || result.value === true) {
                                if (ctrl.uiGridCustom.gridOnBeforeDelete != null) {
                                    ctrl.uiGridCustom.gridOnBeforeDelete();
                                }
                                $http.post(url, params).then(
                                    function (response) {
                                        var data = response.data;
                                        if (data === true || (data.result != null && data.result === true)) {
                                            toaster.pop('success', '', $translate.instant('Admin.Js.GridCustom.ChangesSaved'));
                                            lastStatisticsService.getLastStatistics();
                                            var rowEntity =
                                                $scope.$parent.$parent.row != null
                                                    ? $scope.$parent.$parent.row.entity
                                                    : $scope.$parent.$parent.$parent.row.entity;
                                            ctrl.uiGridCustom.deleteItem(rowEntity).then(function () {
                                                if (ctrl.onDelete != null) {
                                                    ctrl.onDelete();
                                                }
                                            });
                                        } else if (data.errors != null && data.errors.length > 0) {
                                            data.errors.forEach(function (error) {
                                                toaster.pop('error', '', error);
                                            });
                                        }
                                        return data;
                                    },
                                    function (response) {
                                        toaster.pop('success', '', $translate.instant('Admin.Js.GridCustomComponent.ErrorWhileDeletingWriting'));
                                    },
                                );
                            }
                        });
                    };
                },
            ],
        })
        .directive('uiGridCustomOverrideControl', [
            '$parse',
            function ($parse) {
                return {
                    require: {
                        uiGridCustom: '^uiGridCustom',
                    },
                    controller: 'UiGridCustomOverrideControlCtrl',
                    scope: true,
                    bindToController: true,
                    compile: function (element, attrs) {
                        var html = element[0].innerHTML;
                        element[0].innerHTML = '';
                        return function (scope, element, attrs, ctrl) {
                            ctrl.html = html;
                            ctrl.scope = scope;
                            ctrl.dynamicTemplate = $parse(attrs.dynamicTemplate)(scope);
                            ctrl.uiGridCustom.addOverrideControl(ctrl);
                        };
                    },
                };
            },
        ])
        .directive('uiGridCustomOverrideHeaderControl', function () {
            return {
                require: {
                    uiGridCustom: '^uiGridCustom',
                },
                controller: 'UiGridCustomOverrideControlCtrl',
                scope: true,
                bindToController: true,
                compile: function (element, attrs) {
                    var html = element[0].innerHTML;
                    element[0].innerHTML = '';
                    return function (scope, element, attrs, ctrl) {
                        ctrl.html = html;
                        ctrl.scope = scope;
                        ctrl.uiGridCustom.addOverrideHeaderControl(ctrl);
                    };
                },
            };
        })
        .directive('uiGridCustomMobile', [
            '$parse',
            'urlHelper',
            function ($parse, urlHelper) {
                return {
                    restrict: 'EA',
                    templateUrl: uiGridCustomMobileTemplate,
                    require: {
                        uiGridCustom: '^uiGridCustom',
                    },
                    scope: true,
                    link: function (scope, element, attrs) {
                        scope.itemTemplate = $parse(attrs.dynamicTemplate)(scope) || $parse(attrs.itemTemplate)(scope);
                    },
                };
            },
        ])
        .directive('uiGridCustomCell', [
            '$compile',
            '$parse',
            function ($compile, $parse) {
                return {
                    restrict: 'EA',
                    require: {
                        uiGridCustom: '^uiGridCustom',
                    },
                    scope: true,
                    link: function (scope, element, attrs) {
                        const colName = $parse(attrs.uiGridCustomCell)(scope);
                        const col = scope.colContainer.renderedColumns.find((x) => x.field === colName);
                        if (col != null) {
                            scope.col = col;
                            element.append('<div ui-grid-cell></div>');
                            $compile(element.contents())(scope);
                        }
                    },
                };
            },
        ]);
})(window.angular);
