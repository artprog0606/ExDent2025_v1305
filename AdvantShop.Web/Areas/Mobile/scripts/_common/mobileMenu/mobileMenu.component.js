function mobileMenuRootDirective() {
    return {
        scope: true,
        controller: 'MobileMenuRootCtrl',
        controllerAs: 'mobileMenuRoot',
        bindToController: true,
    };
}

function mobileMenuDirective() {
    return {
        require: {
            menuRoot: '^^?mobileMenuRoot',
            parentMobileMenuItem: '^?mobileMenuItem',
        },
        scope: true,
        controller: 'MobileMenuCtrl',
        controllerAs: 'mobileMenu',
        bindToController: true,
    };
}

function mobileMenuItemDirective() {
    return {
        require: {
            mobileMenuRoot: '^^mobileMenuRoot',
            parentMobileMenu: '^^mobileMenu',
            parentMobileMenuItem: '^^?mobileMenuItem',
        },
        scope: true,
        controller: 'MobileMenuItemCtrl',
        controllerAs: 'mobileMenuItem',
        bindToController: true,
    };
}

export { mobileMenuRootDirective, mobileMenuDirective, mobileMenuItemDirective };
