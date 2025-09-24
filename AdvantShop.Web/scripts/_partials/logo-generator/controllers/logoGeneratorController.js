/*@ngInject*/
function LogoGeneratorCtrl($q, $timeout, $window, logoGeneratorService, modalService) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.colorPickerOptions = {
            swatchBootstrap: false,
            format: 'hex',
            alpha: false,
            case: 'lower',
            allowEmpty: false,
            required: true,
            inputClass: 'input input-small',
            pos: 'top left',
        };

        ctrl.loadData();

        //ctrl.colorPickerLogoBgValue = 'ffffff';

        //logoGeneratorService.getFontsList().then(function (fontsData) {
        //    ctrl.logo.style.fontFamily = ctrl.fontsList[0].fontFamily;
        //    ctrl.slogan.style.fontFamily = ctrl.fontsList[0].fontFamily;
        //});

        ctrl.colorPickerLogoEventApi = {};
        //ctrl.colorPickerLogoBgEventApi = {};
        ctrl.colorPickerSloganEventApi = {};

        ctrl.colorPickerLogoEventApi.onBlur = ctrl.colorPickerLogoEventApi.onChange = function (api, value) {
            if (value != null && value.length === 6) {
                ctrl.logo.style.color = '#' + value;
            }
        };

        ctrl.colorPickerSloganEventApi.onBlur = ctrl.colorPickerSloganEventApi.onChange = function (api, value) {
            if (value != null && value.length === 6) {
                ctrl.slogan.style.color = '#' + value;
            }
        };

        logoGeneratorService.addLogoGenerator(ctrl.logoGeneratorId, ctrl);
    };

    ctrl.loadData = function () {
        return logoGeneratorService.getData().then(function (data) {
            ctrl.logo = data.logo;
            ctrl.slogan = data.slogan;
            ctrl.isUseSlogan = data.isUseSlogan;

            ctrl.logo.style.color = (data.logo.style.color.charAt(0) !== '#' ? '#' : '') + data.logo.style.color;
            ctrl.slogan.style.color = (data.slogan.style.color.charAt(0) !== '#' ? '#' : '') + data.slogan.style.color;
            ctrl.colorPickerLogoValue = data.logo.style.color.replace('#', '');
            ctrl.colorPickerSloganValue = data.slogan.style.color.replace('#', '');

            ctrl.isLoaded = true;
        });
    };

    ctrl.onSelectLogoFont = function (font) {
        ctrl.logo.font = font;
        ctrl.logo.style.fontFamily = logoGeneratorService.fontFamilyEscape(font.fontFamily);

        modalService.close('modalLogoGeneratorLogoFonts');
    };

    ctrl.onSelectSlogonFont = function (font) {
        ctrl.slogan.font = font;
        ctrl.slogan.style.fontFamily = logoGeneratorService.fontFamilyEscape(font.fontFamily);

        modalService.close('modalLogoGeneratorSloganFonts');
    };

    ctrl.setSloganMargin = function (value) {
        ctrl.slogan.style[ctrl.slogan.style.verticalAlign === 'top' ? 'marginBottom' : 'marginTop'] = value;
    };

    ctrl.showFontsModal = function (objType) {
        $q.when(ctrl.fonts != null && ctrl.fonts.items.length > 0 ? ctrl.fonts : logoGeneratorService.getFontsList()).then(function (fontsData) {
            ctrl.fonts = fontsData;

            if (objType === 'logo') {
                var logoLang = logoGeneratorService.parseLanguage(ctrl.logo.text);
                ctrl.logoLanguage = logoLang.length < 1 ? '' : logoLang[0];
            } else if (objType === 'slogan') {
                var sloganLang = logoGeneratorService.parseLanguage(ctrl.slogan.text);
                ctrl.sloganLanguage = sloganLang.length < 1 ? '' : sloganLang[0];
            }

            logoGeneratorService.showSubModal(objType, ctrl);
        });
    };

    ctrl.resizeModal = function () {
        $timeout(function () {
            let modalWrap = document.querySelector('.logo-generator-modal');
            let getPositionTopOfmodalWrap = parseInt(window.getComputedStyle(modalWrap).top);
            let windowHeight = window.innerHeight;
            let result = document.documentElement.clientHeight - (modalWrap.clientHeight + getPositionTopOfmodalWrap);
            if (result < 200) {
                modalWrap.querySelector('.adv-modal-inner').classList.add('adv-modal-inner--for-small-size');
            }
        }, 100);
    };
}

export default LogoGeneratorCtrl;
