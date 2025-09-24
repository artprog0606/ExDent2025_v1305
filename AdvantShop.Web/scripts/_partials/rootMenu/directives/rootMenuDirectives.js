function rootMenuDirective() {
    return {
        restrict: 'A',
        scope: true,
        controller: 'RootMenuCtrl',
        controllerAs: 'rootMenu',
        bindToController: true,
    };
}

export { rootMenuDirective };
