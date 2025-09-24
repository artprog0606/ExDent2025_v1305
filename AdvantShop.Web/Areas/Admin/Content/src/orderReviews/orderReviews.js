(function (ng) {
    'use strict';

    var OrderReviewsCtrl = function (uiGridConstants, uiGridCustomConfig) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Text',
                    displayName: 'Комментарий',
                    enableCellEdit: false,
                    filter: {
                        placeholder: 'Комментарий',
                        type: uiGridConstants.filter.INPUT,
                        name: 'Text',
                    },
                },
                {
                    name: 'Ratio',
                    displayName: 'Оценка',
                    enableCellEdit: false,
                    width: 160,
                    cellTemplate: `
                    <div class="ui-grid-cell-contents">
                        <ul class="rating" rating current="{{row.entity.Ratio}}"
                            readonly="true">
                            <li class="rating-item"
                                ng-class="{'rating-item-selected': 5 <= rating.current}">
                                <svg width="25" height="25" class="rating__star" xmlns="http://www.w3.org/2000/svg" viewBox="3 0 22 22">
                                    <path fill="currentColor" d="M19.392 9.202c0-.248-.188-.403-.565-.463l-5.061-.736-2.268-4.588c-.128-.275-.293-.413-.494-.413-.202 0-.367.138-.495.413L8.241 8.003l-5.061.736c-.377.06-.565.215-.565.463 0 .142.084.303.252.484l3.67 3.57-.867 5.04c-.014.095-.02.162-.02.202 0 .141.035.26.105.358.07.098.177.146.318.146.12 0 .255-.04.403-.12l4.527-2.38 4.527 2.38c.141.08.276.12.403.12a.355.355 0 0 0 .308-.146.595.595 0 0 0 .106-.358 1.51 1.51 0 0 0-.01-.201l-.868-5.042 3.66-3.569c.175-.174.262-.336.262-.484" />
                                </svg>
                            </li>
                            <li class="rating-item"
                                ng-class="{'rating-item-selected': 4 <= rating.current}">
                                <svg width="25" height="25" class="rating__star" xmlns="http://www.w3.org/2000/svg" viewBox="3 0 22 22">
                                    <path fill="currentColor" d="M19.392 9.202c0-.248-.188-.403-.565-.463l-5.061-.736-2.268-4.588c-.128-.275-.293-.413-.494-.413-.202 0-.367.138-.495.413L8.241 8.003l-5.061.736c-.377.06-.565.215-.565.463 0 .142.084.303.252.484l3.67 3.57-.867 5.04c-.014.095-.02.162-.02.202 0 .141.035.26.105.358.07.098.177.146.318.146.12 0 .255-.04.403-.12l4.527-2.38 4.527 2.38c.141.08.276.12.403.12a.355.355 0 0 0 .308-.146.595.595 0 0 0 .106-.358 1.51 1.51 0 0 0-.01-.201l-.868-5.042 3.66-3.569c.175-.174.262-.336.262-.484" />
                                </svg>
                            </li>
                            <li class="rating-item"
                                ng-class="{'rating-item-selected': 3 <= rating.current}">
                                <svg width="25" height="25" class="rating__star" xmlns="http://www.w3.org/2000/svg" viewBox="3 0 22 22">
                                    <path fill="currentColor" d="M19.392 9.202c0-.248-.188-.403-.565-.463l-5.061-.736-2.268-4.588c-.128-.275-.293-.413-.494-.413-.202 0-.367.138-.495.413L8.241 8.003l-5.061.736c-.377.06-.565.215-.565.463 0 .142.084.303.252.484l3.67 3.57-.867 5.04c-.014.095-.02.162-.02.202 0 .141.035.26.105.358.07.098.177.146.318.146.12 0 .255-.04.403-.12l4.527-2.38 4.527 2.38c.141.08.276.12.403.12a.355.355 0 0 0 .308-.146.595.595 0 0 0 .106-.358 1.51 1.51 0 0 0-.01-.201l-.868-5.042 3.66-3.569c.175-.174.262-.336.262-.484" />
                                </svg>
                            </li>
                            <li class="rating-item"
                                ng-class="{'rating-item-selected': 2 <= rating.current}">
                                <svg width="25" height="25" class="rating__star" xmlns="http://www.w3.org/2000/svg" viewBox="3 0 22 22">
                                    <path fill="currentColor" d="M19.392 9.202c0-.248-.188-.403-.565-.463l-5.061-.736-2.268-4.588c-.128-.275-.293-.413-.494-.413-.202 0-.367.138-.495.413L8.241 8.003l-5.061.736c-.377.06-.565.215-.565.463 0 .142.084.303.252.484l3.67 3.57-.867 5.04c-.014.095-.02.162-.02.202 0 .141.035.26.105.358.07.098.177.146.318.146.12 0 .255-.04.403-.12l4.527-2.38 4.527 2.38c.141.08.276.12.403.12a.355.355 0 0 0 .308-.146.595.595 0 0 0 .106-.358 1.51 1.51 0 0 0-.01-.201l-.868-5.042 3.66-3.569c.175-.174.262-.336.262-.484" />
                                </svg>
                            </li>
                            <li class="rating-item"
                                ng-class="{'rating-item-selected': 1 <= rating.current}">
                                <svg width="25" height="25" class="rating__star" xmlns="http://www.w3.org/2000/svg" viewBox="3 0 22 22">
                                    <path fill="currentColor" d="M19.392 9.202c0-.248-.188-.403-.565-.463l-5.061-.736-2.268-4.588c-.128-.275-.293-.413-.494-.413-.202 0-.367.138-.495.413L8.241 8.003l-5.061.736c-.377.06-.565.215-.565.463 0 .142.084.303.252.484l3.67 3.57-.867 5.04c-.014.095-.02.162-.02.202 0 .141.035.26.105.358.07.098.177.146.318.146.12 0 .255-.04.403-.12l4.527-2.38 4.527 2.38c.141.08.276.12.403.12a.355.355 0 0 0 .308-.146.595.595 0 0 0 .106-.358 1.51 1.51 0 0 0-.01-.201l-.868-5.042 3.66-3.569c.175-.174.262-.336.262-.484" />
                                </svg>
                            </li>
                        </ul>
                    </div>`,
                    filter: {
                        placeholder: 'Оценка',
                        type: 'range',
                        rangeOptions: {
                            from: {
                                name: 'RatioFrom',
                            },
                            to: {
                                name: 'RatioTo',
                            },
                        },
                    },
                },
                {
                    name: 'OrderNumber',
                    displayName: 'Номер заказа',
                    enableCellEdit: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a ng-href="orders/edit/{{row.entity.OrderId}}" target="_blank">{{COL_FIELD}}</a></div>',
                    width: 90,
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };
    };

    OrderReviewsCtrl.$inject = ['uiGridConstants', 'uiGridCustomConfig'];

    ng.module('orderReviews', ['uiGridCustom', 'rating']).controller('OrderReviewsCtrl', OrderReviewsCtrl);
})(window.angular);
