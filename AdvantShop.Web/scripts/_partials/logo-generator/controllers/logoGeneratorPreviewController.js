import logoGeneratorPreviewTemplate from '../templates/logo-generator-preview.html';

/*@ngInject*/
function LogoGeneratorPreviewCtrl($attrs, $compile, $document, $element, $q, $scope, $templateRequest, logoGeneratorService) {
    var ctrl = this,
        deferLogo,
        deferSlogan;

    ctrl.$onInit = function () {
        ctrl.element = $element[0];

        ctrl.element.parentElement.style.height = ctrl.element.parentElement.height;

        $templateRequest(logoGeneratorPreviewTemplate)
            .then((template) => {
                const nodeTemp = $document[0].createElement('div');
                const childsEl = ctrl.element.children;
                nodeTemp.innerHTML = template;
                nodeTemp.querySelector('.js-transclude').append(...childsEl);
                ctrl.element.append(...nodeTemp.children);
                $compile(ctrl.element.children)($scope);
                ctrl.element.parentElement.style.height = 'auto';
            })
            .then(() => {
                logoGeneratorService.addLogoGeneratorPreview(ctrl.logoGeneratorId, ctrl);
                if (ctrl.editOnPageLoad === true) {
                    logoGeneratorService.showModal(ctrl.logoGeneratorId, $attrs.urlSave, { cameFrom: 'adminArea' });
                }

                logoGeneratorService.getLogoGenerator(ctrl.logoGeneratorId, function (logoGenerator) {
                    ctrl.logoGenerator = logoGenerator;
                });
            });
    };

    ctrl.$postLink = function () {
        ctrl.img = $element[0].querySelector('[data-logo-generator-preview-img]');
    };

    //ctrl.addImg = function (img) {
    //    return ctrl.img = img;
    //};

    ctrl.addLogo = function (logo) {
        ctrl.logo = logo;

        if (deferLogo != null) {
            deferLogo.resolve(logo);
        }

        return logo;
    };

    ctrl.getLogo = function () {
        if (ctrl.logo == null) {
            deferLogo = $q.defer();
        } else {
            deferLogo.resolve(ctrl.logo);
        }

        return deferLogo.promise;
    };

    ctrl.addSlogan = function (slogan) {
        ctrl.slogan = slogan;

        if (deferSlogan != null) {
            deferSlogan.resolve(slogan);
        }

        return slogan;
    };

    ctrl.getSlogan = function () {
        if (ctrl.slogan == null) {
            deferSlogan = $q.defer();
        } else {
            deferSlogan.resolve(ctrl.logo);
        }

        return deferSlogan.promise;
    };
}

export default LogoGeneratorPreviewCtrl;
