/* global window */
(function stopAngularOverrides(angular) {
    'use strict';

    if (!angular) {
        throw new Error('Missing angular');
    }

    const ignoreList = ['$locale', 'ngLocale', /^uigrid|grid/i];

    let _module = angular.bind(angular, angular.module);

    function createUniqueNamingCheckFn(moduleName, type) {
        let existingNames = Object.create(null);
        return function (name) {
            if(ignoreList.some(item => typeof(item) === 'string' ? item === name :  item.test(name))){
                return  false;
            } else if (existingNames[name]) {
                return true;
            }else{
                existingNames[name] = true;
                return false;
            }
        };
    }

    let existingModulesCheck = createUniqueNamingCheckFn('module');
    let existingFiltersCheck = createUniqueNamingCheckFn('filter');
    let existingControllersCheck = createUniqueNamingCheckFn('controller');
    let existingServicesCheck = createUniqueNamingCheckFn('service');
    let existingDirectivesCheck = createUniqueNamingCheckFn('directive');
    let existingComponentCheck = createUniqueNamingCheckFn('component');
    function createProxyFn(module, moduleFn, existingNameCheck) {
        return function (name, fn) {
            if(existingNameCheck(name)){
                return module;
            }
            return moduleFn.call(module, name, fn);
        };
    }

    angular.module = function (name, deps, configFn) {
        if (!deps || existingModulesCheck(name)) {
            return _module(name);
        }

        let m = _module(name, deps, configFn);

        // proxy .filter calls to the new module
        m.filter = createProxyFn(m, m.filter, existingFiltersCheck);

        // proxy .controller calls to the new module
        m.controller = createProxyFn(m, m.controller, existingControllersCheck);

        // proxy .directive calls to the new module
        m.directive = createProxyFn(m, m.directive, existingDirectivesCheck);

        // proxy .directive calls to the new module
        m.component = createProxyFn(m, m.component, existingComponentCheck);

        // proxy .service calls to the new module
        m.service = createProxyFn(m, m.service, existingServicesCheck);

        // proxy .factory calls to the new module
        m.factory = createProxyFn(m, m.factory, existingServicesCheck);

        // proxy .value calls to the new module
        m.value = createProxyFn(m, m.value, existingServicesCheck);

        // proxy .provider calls to the new module
        m.provider = createProxyFn(m, m.provider, existingServicesCheck);

        return m;
    };

}(window.angular));
