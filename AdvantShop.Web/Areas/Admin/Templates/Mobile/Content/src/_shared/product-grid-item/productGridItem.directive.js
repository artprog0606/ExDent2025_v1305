import '../../../../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';
import '../../../../../../Content/src/order/modal/editCustomOptions/ModalEditCustomOptionsCtrl.js';

import productGridItemTemplate from './product-grid-item.html';
import '../../../../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
(function (ng) {
    'use strict';

    ng.module('productGridItem', ['uiModal', 'ngClickCapture']).directive('productGridItem', [
        '$parse',
        '$filter',
        'uiGridCustomService',
        '$interpolate',
        function ($parse, $filter, uiGridCustomService, $interpolate) {
            return {
                controller: [
                    '$scope',
                    'domService',
                    function ($scope, domService) {
                        const ctrl = this;
                        ctrl.isShowCustomOptions = function (defaultFlag, addFlag) {
                            if (addFlag != null) {
                                return defaultFlag && addFlag;
                            }
                            return defaultFlag;
                        };
                        ctrl.checkIsEditProduct = function ($event, $element) {
                            const target = $event.target;
                            // кнопка раскрытия модификаций в offer-selectvizr
                            if (target.classList.contains('js-open-children-el')) {
                                return;
                            }
                            if (ctrl.gridScope.$ctrl.gridOptions.uiGridCustom.rowUrl != null) {
                                const uiGridRowUrl = ctrl.gridScope.$ctrl.gridOptions.uiGridCustom.rowUrl;
                                if (typeof uiGridRowUrl === 'string') {
                                    const rowUrl = $interpolate(ctrl.gridScope.$ctrl.gridOptions.uiGridCustom.rowUrl)($scope);
                                    window.location = rowUrl;
                                    $event.stopPropagation();
                                }
                                return;
                            }
                            if (
                                ['input', 'textarea', 'button'].indexOf($event.target.tagName.toLowerCase()) !== -1 ||
                                domService.closest(
                                    $event.target,
                                    [
                                        '.ui-select-choices-row-inner',
                                        '.js-grid-not-clicked',
                                        '.ui-select-container',
                                        '[data-swipe-line-left]',
                                        '[data-swipe-line-right]',
                                    ],
                                    $element[0],
                                ) != null ||
                                $event.target.querySelector('.js-grid-not-clicked') != null
                            ) {
                                $event.stopPropagation();
                                return;
                            }
                            if (target.classList.contains('onoffswitch-label') || target.closest('ui-grid-custom-switch')) {
                                // например кнопка исключения из выгрузки (exportfeed)
                                $event.stopPropagation();
                                return;
                            }
                            const countEditableColumns = $scope.editableColumns.length > 0;
                            const defaultCheck = countEditableColumns && ctrl.gridScope.$ctrl.gridOptions.uiGridCustom.rowClick == null;
                            if ($scope.extIsEditableParam != null) {
                                if (!(defaultCheck && $scope.extIsEditableParam)) {
                                    $event.stopPropagation();
                                }
                            }
                            if (ctrl.gridScope.$ctrl.gridOptions.enableRowSelection && ctrl.gridScope.$ctrl.gridOptions.enableFullRowSelection) {
                                ctrl.gridScope.$ctrl.gridApi.selection.toggleRowSelection($scope.row.entity, $event);
                                $event.stopPropagation();
                                $scope.$apply();
                            }
                            if (!defaultCheck) {
                                $event.stopPropagation();
                            }
                        };
                    },
                ],
                controllerAs: '$ctrl',
                scope: false,
                bindToController: true,
                templateUrl: productGridItemTemplate,
                transclude: {
                    info: '?productGridItemInfo',
                },
                link: function (scope, element, attrs, ctrl) {
                    const renderedColumns = $parse(attrs.rendererColumns)(scope) || [];
                    ctrl.gridScope = $parse(attrs.gridScope)(scope);
                    // Todo убрать сделал так как mainpageproduct нужна ссылка на товар
                    ctrl.customLink = $parse(attrs.customLink)(scope);
                    scope.editableColumns = [];
                    scope.row = $parse(attrs.row)(scope);
                    scope.extIsEditableFn = $parse(attrs.extIsEditableFn)(scope);
                    scope.extIsEditableParam = $parse(attrs.extIsEditableParam)(scope);
                    const entity = scope.row.entity;
                    const priceString = entity.Cost || entity.PriceString || entity.PriceFormatted;
                    scope.rowData = {
                        // enabled: ctrl.getColFromColDef('Enabled') != null ? entity.Enabled : true,
                        enabled: entity.Enabled != null ? entity.Enabled : true,
                        // productLink: entity.ProductLink || 'product/edit/' + entity.ProductId,
                        productLink: ctrl.customLink && entity.ProductLink == null ? 'product/edit/' + entity.ProductId : entity.ProductLink,
                        name: entity.Name,
                        imageSrc: entity.ImageSrc || entity.PhotoSrc,
                        productId: entity.ProductId,
                        productArtNo: entity.ProductArtNo,
                        artNo: entity.ArtNo,
                        orderItemId: entity.OrderItemId,
                        showDimensions:
                            entity.Length != null &&
                            entity.Length !== 0 &&
                            entity.Width != null &&
                            entity.Width !== 0 &&
                            entity.Height != null &&
                            entity.Height !== 0,
                        length: entity.Length,
                        width: entity.Width,
                        height: entity.Height,
                        showWeight: entity.Weight != null && entity.Weight !== 0,
                        weight: entity.Weight,
                        showCustomOptions: entity.CustomOptions != null && entity.CustomOptions.length > 0,
                        customOptions: entity.CustomOptions,
                        showEditCustomOptions: ctrl.isShowCustomOptions(entity.ShowEditCustomOptions, scope.extIsEditableParam),
                        showColorSize: entity.Color || entity.Size || entity.ColorName || entity.SizeName,
                        colorTextLabel: entity.ColorName
                            ? uiGridCustomService.getColByFieldName(ctrl.gridScope.$ctrl.gridOptions.columnDefs, 'ColorName')?.displayName
                            : null,
                        sizeTextLabel: entity.SizeName
                            ? uiGridCustomService.getColByFieldName(ctrl.gridScope.$ctrl.gridOptions.columnDefs, 'SizeName')?.displayName
                            : null,
                        color: entity.Color || entity.ColorName,
                        size: entity.Size || entity.SizeName,
                        available: entity.Available,
                        availableText: entity.AvailableText,
                        amount: entity.Amount,
                        cost: entity.Cost,
                        priceString: priceString != null && priceString.length ? priceString : null,
                        excludeFromExport: entity.ExcludeFromExport,
                        applyCoupon: entity.ApllyCoupon,
                        linkShipping: entity.LinkShipping,
                    };
                    scope.checkboxControls = uiGridCustomService.getColsByType(ctrl.gridScope.$ctrl.gridOptions.columnDefs, 'checkbox');
                    const unbindWatcher = scope.$watch(attrs.extIsEditableParam, function (newValue, oldValue) {
                        scope.extIsEditableParam = newValue;
                        scope.rowData.showEditCustomOptions = ctrl.isShowCustomOptions(scope.rowData.showEditCustomOptions, newValue);
                    });
                    if (scope.row) {
                        const isEditable = $filter('isEditable');
                        if (isEditable) {
                            scope.editableColumns = renderedColumns.filter((col) => {
                                return isEditable(col, scope.row, scope);
                            });
                        }
                    }
                    element.on('$destroy', () => {
                        unbindWatcher();
                    });
                },
            };
        },
    ]);
})(window.angular);
