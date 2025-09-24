const isMobile = document.documentElement.classList.contains('mobile-version');

class IsMobileController {
    constructor() {}

    $onInit() {
        this.value = isMobile;
    }
}

class IsMobileDirective {
    constructor() {
        this.priority = 10;
        this.scope = false;
        this.controllerAs = `isMobile`;
        this.controller = IsMobileController;
        this.bindToController = true;
    }
}

class IsMobileService {
    getValue() {
        return isMobile;
    }
}

angular
    .module('isMobile', [])
    .service('isMobileService', IsMobileService)
    .directive('isMobile', () => new IsMobileDirective());
