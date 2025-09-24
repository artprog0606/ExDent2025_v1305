(function (ng) {
    'use strict';

    const readMoreDefinition = {
        controller: 'ReadmoreCtrl',
        template: [
            '$element',
            '$attrs',
            function ($element, $attrs) {
                return $element.closest('[data-inplace-rich]').length == 0
                    ? '<div data-ng-class="{\'readmore-expanded\' : $ctrl.expanded, \'readmore-collapsed\' : !$ctrl.expanded}"><div class="readmore-content" data-ng-style="{maxHeight: $ctrl.maxHeight, transitionDuration: $ctrl.speed}"><div class="js-readmore-inner-content" ' +
                          ($attrs.content == null ? 'data-ng-transclude' : 'ng-bind-html="$ctrl.content"') +
                          '></div></div><div class="readmore-controls" data-ng-if="$ctrl.isActive"><a class="readmore-link" tabindex="0" role="button" data-ng-click="$ctrl.switch($ctrl.expanded)">{{$ctrl.text | translate}}</a></div></div>'
                    : '';
            },
        ],
        transclude: true,
        bindings: {
            expanded: '<?',
            maxHeight: '<?',
            content: '<?',
            speed: '@',
            moreText: '@',
            lessText: '@',
            lineClamp: '<?',
        },
    };

    angular.module('readmore').component('readmore', readMoreDefinition).component('readMore', readMoreDefinition);
})(window.angular);
