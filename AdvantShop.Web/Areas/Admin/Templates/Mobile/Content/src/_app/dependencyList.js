(function (ng) {
    'use strict';

    var dependencyService = ng.injector(['dependency']).get('dependencyService');

    dependencyService.add([`fullHeightMobile`, `isMobile`, `details`, `swipeLine`, `sidebarsContainer`, `mainMenu`, `setCssCustomProps`]);
})(window.angular);
