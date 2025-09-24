import zoomerWindowTemplate from '../templates/zoomerWindow.html';
var isTouchDevice = 'ontouchstart' in document.documentElement;

/* @ngInject */
function zoomerDirective($rootScope, $window, $compile, zoomerConfig) {
    return {
        restrict: 'A',
        scope: {
            previewPath: '=',
            originalPath: '=',
            type: '@',
            zoomWidth: '=?',
            zoomHeight: '=?',
            zoomerTitle: '=?',
        },
        replace: true,
        transclude: true,
        template: '<a data-ng-class="zoomer.getZoomerClass()" class="zoomer" data-ng-href="{{zoomer.originalPath}}" data-ng-transclude></a>',
        controller: 'ZoomerCtrl',
        controllerAs: 'zoomer',
        bindToController: true,
        link: function (scope, element, attrs, ctrl) {
            var init = function () {
                if (angular.isUndefined(ctrl.zoomWidth)) {
                    ctrl.zoomWidth = zoomerConfig.zoomWidth;
                }

                if (angular.isUndefined(ctrl.zoomHeight)) {
                    ctrl.zoomHeight = zoomerConfig.zoomHeight;
                }

                if (angular.isUndefined(ctrl.type)) {
                    ctrl.type = zoomerConfig.type;
                }

                var scopeWindow = scope.$new(),
                    zoomerWindow = angular.element('<div zoomer-window></div>');

                scopeWindow.parentScope = ctrl;

                element.after(zoomerWindow);
                $compile(zoomerWindow)(scopeWindow);

                ['touchstart', 'mouseenter'].forEach(function (eventName) {
                    element[0].addEventListener(eventName, function (event) {
                        ctrl.active(event);
                        scope.$apply();
                    });
                });

                ['touchmove', 'mousemove'].forEach(function (eventName) {
                    element[0].addEventListener(eventName, function (event) {
                        ctrl.update(event);
                        scope.$apply();
                    });
                });

                ['touchend', 'mouseleave'].forEach(function (eventName) {
                    element[0].addEventListener(eventName, function (event) {
                        ctrl.deactive(event);
                        scope.$apply();
                    });
                });

                if (ctrl.type !== 'inner') {
                    var scopeLens = $rootScope.$new(),
                        zoomerLens = angular.element('<div zoomer-lens></div>');

                    scopeLens.parentScope = ctrl;

                    element.append(zoomerLens);
                    $compile(zoomerLens)(scopeLens);
                }
            };

            if (document.readyState != `complete`) {
                $window.addEventListener('load', function load() {
                    $window.removeEventListener('load', load);
                    init();
                });
            } else {
                init();
            }
        },
    };
}

/* @ngInject */
function zoomerLensDirective() {
    return {
        //require: '^zoomer',
        restrict: 'A',
        replace: true,
        scope: true,
        template:
            "<div data-ng-show=\"parentScope.isShowZoom\" class=\"zoomer-lens\" data-ng-style=\"{'top': parentScope.lensSizes.top + 'px', 'left': parentScope.lensSizes.left + 'px', 'width':  +  parentScope.lensSizes.width + 'px','height':  +  parentScope.lensSizes.height + 'px' }\"></div>",
        link: function (scope, element, attrs, ctrl) {
            //var zoomerCtrl = ctrl;
            //element[0].addEventListener('touchmove', function (event) {
            //    zoomerCtrl.update(event);
            //    scope.$apply();
            //});
        },
    };
}

function zoomerWindowDirective() {
    return {
        restrict: 'A',
        replace: true,
        scope: true,
        templateUrl: zoomerWindowTemplate,
        link: function (scope, element, attrm, ctrls) {
            //var zoomerCtrl = ctrls[0];

            //element[0].addEventListener('mouseenter', zoomerCtrl.active);
            //element[0].addEventListener('mouseleave', zoomerCtrl.deactive);
            //element[0].addEventListener('mousemove', zoomerCtrl.update);

            if (scope.$parent.parentScope.type === 'inner') {
                ['touchstart', 'mouseenter'].forEach(function (eventName) {
                    element[0].addEventListener(eventName, function (event) {
                        scope.$parent.parentScope.active(event);
                        scope.$apply();
                    });
                });
            }

            ['touchmove', 'mousemove'].forEach(function (eventName) {
                element[0].addEventListener(eventName, function (event) {
                    scope.$parent.parentScope.update(event);
                    scope.$apply();
                });
            });

            ['touchend', 'mouseleave'].forEach(function (eventName) {
                element[0].addEventListener(eventName, function (event) {
                    scope.$parent.parentScope.deactive(event);
                    scope.$apply();
                });
            });
        },
    };
}

export { zoomerDirective, zoomerLensDirective, zoomerWindowDirective };
