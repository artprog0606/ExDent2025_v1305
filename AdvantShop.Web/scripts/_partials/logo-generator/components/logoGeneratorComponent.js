import logoGeneratorFontsTemplate from '../templates/logo-generator-fonts.html';
import logoGeneratorTemplate from '../templates/logo-generator.html';
/*@ngInject*/
function logoGeneratorStartDirective($compile) {
    return function (scope, element, attrs, ctrl) {
        var objs = document.querySelectorAll('logo-generator-trigger');

        if (objs != null && objs.length > 0) {
            Array.prototype.slice.call(objs).forEach(function (item) {
                $compile(item)(angular.element(item).scope() || scope);
            });
        }
    };
}

const logoGeneratorComponent = {
    controller: 'LogoGeneratorCtrl',
    templateUrl: logoGeneratorTemplate,
    bindings: {
        logoGeneratorId: '@',
        logoGeneratorFontsOptions: '<?',
        logoGeneratorOptions: '<?',
    },
};

const logoGeneratorFontsComponent = {
    controller: 'LogoGeneratorFontsCtrl',
    templateUrl: logoGeneratorFontsTemplate,
    bindings: {
        fontsList: '<',
        logo: '<',
        slogan: '<?',
        isUseSlogan: '<?',
        language: '<?',
        objType: '@',
        onSelect: '&',
        options: '<?',
    },
};

const logoGeneratorPreviewComponent = {
    controller: 'LogoGeneratorPreviewCtrl',
    //transclude: true,
    // templateUrl: 'scripts/_partials/logo-generator/templates/logo-generator-preview.html',
    bindings: {
        logoGeneratorId: '@',
        editOnPageLoad: '<?',
    },
};

function logoGeneratorPreviewLogoDirective() {
    return {
        scope: true,
        require: {
            logoGeneratorPreviewCtrl: '^logoGeneratorPreview',
        },
        controller: [
            '$element',
            function ($element) {
                var ctrl = this;

                ctrl.$onInit = function () {
                    ctrl.logoGeneratorPreviewCtrl.addLogo($element[0]);
                };
            },
        ],
        bindToController: true,
        controllerAs: 'logoGeneratorPreviewLogo',
    };
}

function logoGeneratorPreviewSloganDirective() {
    return {
        scope: true,
        require: {
            logoGeneratorPreviewCtrl: '^logoGeneratorPreview',
        },
        controller: [
            '$element',
            function ($element) {
                var ctrl = this;

                ctrl.$onInit = function () {
                    ctrl.logoGeneratorPreviewCtrl.addSlogan($element[0]);
                };
            },
        ],
        bindToController: true,
        controllerAs: 'logoGeneratorPreviewSlogan',
    };
}

const logoGeneratorTriggerComponent = {
    controller: 'LogoGeneratorTriggerCtrl',
    bindings: {
        logoGeneratorId: '@',
        logoGeneratorParams: '<?',
        urlSave: '@',
        logoGeneratorSuccessFn: '&',
        logoGeneratorClickFn: '&',
        logoGeneratorFontsOptions: '<?',
        logoGeneratorOptions: '<?',
    },
};

export {
    logoGeneratorStartDirective,
    logoGeneratorComponent,
    logoGeneratorFontsComponent,
    logoGeneratorPreviewComponent,
    logoGeneratorPreviewLogoDirective,
    logoGeneratorPreviewSloganDirective,
    logoGeneratorTriggerComponent,
};
