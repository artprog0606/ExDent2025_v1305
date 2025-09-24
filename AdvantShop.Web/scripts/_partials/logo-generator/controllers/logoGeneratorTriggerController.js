/*@ngInject*/
function LogoGeneratorTriggerCtrl($attrs, $element, logoGeneratorService) {
    var ctrl = this;

    ctrl.$postLink = function () {
        $element[0].addEventListener('click', function () {
            ctrl.showModal(
                ctrl.logoGeneratorId,
                ctrl.urlSave,
                ctrl.logoGeneratorParams,
                ctrl.logoGeneratorSuccessFn,
                ctrl.logoGeneratorFontsOptions,
                ctrl.logoGeneratorOptions,
                ctrl.logoGeneratorClickFn,
            );
        });
    };

    ctrl.showModal = function (logoGeneratorId, urlSave, params, successFn, logoGeneratorFontsOptions, logoGeneratorOptions, clickFn) {
        logoGeneratorService.showModal(logoGeneratorId, urlSave, params, successFn, logoGeneratorFontsOptions, logoGeneratorOptions);
        if (clickFn != null) {
            clickFn();
        }
    };
}

export default LogoGeneratorTriggerCtrl;
