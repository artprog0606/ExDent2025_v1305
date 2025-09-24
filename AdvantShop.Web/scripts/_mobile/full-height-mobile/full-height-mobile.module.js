import './full-height-mobile.scss';

const moduleName = 'fullHeightMobile';

angular.module(moduleName, []).directive('fullHeightMobile', [
    '$window',
    '$document',
    function ($window, $document) {
        return {
            controller: function FullHeightMobileCtrl() {
                var ctrl = this;
                var root = $document[0].querySelector(':root');

                ctrl.onOrientationChangeHandler = function () {
                    const value = $window.innerHeight;
                    root.style.setProperty('--min-full-height-native', `100vh`);
                    root.style.setProperty('--min-full-height-native', `100dvh`);
                    root.style.setProperty('--min-full-height', `var(--min-full-height-native, ${value}px)`);
                    root.style.setProperty('--min-full-height-raw', value);
                };
            },
            link: function (scope, element, attrs, fullHeightMobileCtrl) {
                fullHeightMobileCtrl.onOrientationChangeHandler();

                $window.addEventListener('resize', fullHeightMobileCtrl.onOrientationChangeHandler);
            },
        };
    },
]);

export default moduleName;
