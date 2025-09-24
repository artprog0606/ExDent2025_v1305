import html2canvas from 'html2canvas';

/*@ngInject*/
function LogoGeneratorModalCtrl(logoGeneratorService, $timeout, $document, $window, urlHelper) {
    var ctrl = this;

    ctrl.save = function (logoGeneratorId, urlSave, params, successFn) {
        ctrl.savingLogo = true;

        $timeout(function () {
            logoGeneratorService
                .getLogoGeneratorPreview(logoGeneratorId)
                .then(function (logoGeneratorPreview) {
                    return html2canvas(logoGeneratorPreview.element, {
                        backgroundColor: null,
                        //надо брать ширину без вертикального скролла так как тогда картинка смещается вправо
                        windowWidth: $document[0].body.clientWidth,
                        height: logoGeneratorPreview.element.clientHeight + 5, //чтобы с низу не обрезалось
                        scrollY: $window.scrollY,
                        scale: 1,
                    }).then(function (canvas) {
                        return {
                            logoGeneratorPreview: logoGeneratorPreview,
                            canvas: canvas,
                        };
                    });
                })
                .then(function (data) {
                    return logoGeneratorService.saveLogo(
                        urlSave,
                        data.canvas.toDataURL('image/png'),
                        '.png',
                        {
                            logo: {
                                style: data.logoGeneratorPreview.logoGenerator.logo.style,
                                text: data.logoGeneratorPreview.logoGenerator.logo.text,
                                font: data.logoGeneratorPreview.logoGenerator.logo.font,
                            },
                            slogan: {
                                style: data.logoGeneratorPreview.logoGenerator.slogan.style,
                                text: data.logoGeneratorPreview.logoGenerator.slogan.text,
                                font: data.logoGeneratorPreview.logoGenerator.slogan.font,
                                marginValue: data.logoGeneratorPreview.logoGenerator.slogan.marginValue,
                            },
                            isUseSlogan: data.logoGeneratorPreview.logoGenerator.isUseSlogan,
                        },
                        params,
                    );
                })
                .then(function (data) {
                    return $timeout(function () {
                        logoGeneratorService.updateLogoSrc(logoGeneratorId, data.ImgSource);
                        if (successFn != null) {
                            successFn({ src: data.ImgSource });
                        }
                        ctrl.close(logoGeneratorId);
                    }, 0);
                })
                .catch(function (error) {
                    console.error('Error on generate logo: ' + error);
                })
                .finally(function () {
                    ctrl.savingLogo = false;
                });
        }, 400);
    };

    ctrl.close = function (logoGeneratorId) {
        logoGeneratorService.closeModal(logoGeneratorId);
        urlHelper.setLocationQueryParams('logoGeneratorEditOnPageLoad', undefined);
        //urlHelper.setLocationQueryParams('tab', undefined);
    };

    ctrl.callbackClose = function (logoGeneratorId) {
        logoGeneratorService.setActivity(logoGeneratorId, false);
        urlHelper.setLocationQueryParams('logoGeneratorEditOnPageLoad', undefined);
    };
}

export default LogoGeneratorModalCtrl;
