import leadsListSourcesTemplate from './components/leadsListSources/leadsListSources.html';
import leadsListChartTemplate from './components/leadsListChart/leadsListChart.html';
(function (ng) {
    'use strict';

    ng.module('leads').directive('leadsList', [
        '$compile',
        '$timeout',
        function ($compile, $timeout) {
            return {
                controller: 'LeadsListCtrl',
                controllerAs: '$ctrl',
                //template: '<div bind-html-compile="$ctrl.data"></div>',
                transclude: true,
                bindToController: true,
                replace: true,
                scope: true,
                //scope: {
                //    excludeLeadListId: '<?'
                //}
            };
        },
    ]);
    ng.module('leads').directive('leadsListSources', function () {
        return {
            controller: 'LeadsListSourcesCtrl',
            controllerAs: '$ctrl',
            templateUrl: leadsListSourcesTemplate,
            bindToController: true,
            scope: {
                leadsListId: '@',
            },
        };
    });
    ng.module('leads').directive('leadsListChart', function () {
        return {
            controller: 'LeadsListChartCtrl',
            controllerAs: '$ctrl',
            templateUrl: leadsListChartTemplate,
            bindToController: true,
            scope: {
                leadsListId: '@',
            },
        };
    });
})(window.angular);
