/*@ngInject*/
function logoGeneratorService($controller, $http, $q, $window, modalService, $cookies, urlHelper) {
    var service = this,
        theme = $cookies.get('userTheme'),
        localStorageKey = theme === 'dark-theme' ? 'logoGeneratorDarkTheme' : 'logoGenerator',
        storage = {},
        listWait = {},
        emptySymbol = '🞎',
        regexLatin = new RegExp('[a-zA-Z]+', 'g'),
        regexCyrilic = new RegExp('[а-яА-Я]+', 'g');

    service.addLogoGenerator = function (logoGeneratorId, logoGenerator) {
        if (logoGeneratorId == null || logoGeneratorId.length === 0) {
            throw Error('logoGeneratorId is required parameter');
        }

        storage[logoGeneratorId].logoGenerator = logoGenerator;

        if (listWait[logoGeneratorId] != null) {
            listWait[logoGeneratorId].forEach(function (callback) {
                callback(storage[logoGeneratorId].logoGenerator);
            });
        }

        return storage[logoGeneratorId].logoGenerator;
    };

    service.getLogoGenerator = function (logoGeneratorId, callback) {
        if (storage[logoGeneratorId] != null && storage[logoGeneratorId].logoGenerator != null) {
            callback(storage[logoGeneratorId].logoGenerator);
        } else {
            listWait[logoGeneratorId] = listWait[logoGeneratorId] || [];

            listWait[logoGeneratorId].push(callback);
        }
    };

    service.addLogoGeneratorPreview = function (logoGeneratorId, logoGeneratorPreview) {
        storage[logoGeneratorId] = storage[logoGeneratorId] || {};

        if (storage[logoGeneratorId].preview == null) {
            storage[logoGeneratorId].preview = logoGeneratorPreview;
        }

        return storage[logoGeneratorId].preview;
    };

    service.getLogoGeneratorPreview = function (logoGeneratorId) {
        return $q.when(storage[logoGeneratorId].preview);
    };

    service.showModal = function (logoGeneratorId, urlSave, params, successFn, logoGeneratorFontsOptions, logoGeneratorOptions) {
        var modalId = logoGeneratorId + 'Modal';

        storage[logoGeneratorId] = storage[logoGeneratorId] || {};

        modalService.renderModal(
            modalId,
            'Генерация логотипа',
            '<logo-generator logo-generator-id="' +
                logoGeneratorId +
                '" logo-generator-fonts-options="logoGeneratorFontsOptions" logo-generator-options="logoGeneratorOptions"></logo-generator>',
            '<div><button type="button" data-ladda="logoGeneratorModal.savingLogo" class="logo-generator-modal-btn-save btn btn-small btn-submit btn--xs" data-ng-click=\'logoGeneratorModal.save("' +
                logoGeneratorId +
                '", "' +
                urlSave +
                '", ' +
                JSON.stringify(params || {}) +
                ', successFn)\'>Сохранить</button> <button type="button" data-ng-click=\'logoGeneratorModal.close("' +
                logoGeneratorId +
                '")\' class="logo-generator-modal-btn-close btn btn-small btn-action btn--xs">Закрыть</button></div>',
            {
                backgroundEnable: false,
                isFloating: true,
                modalOverlayClass: 'logo-generator-modal',
                isOpen: true,
                callbackClose: "logoGeneratorModal.callbackClose('" + logoGeneratorId + "')",
                destroyOnClose: true,
                zIndex: 1000,
            },
            {
                logoGeneratorModal: $controller('LogoGeneratorModalCtrl'),
                successFn: successFn,
                logoGeneratorFontsOptions: logoGeneratorFontsOptions,
                logoGeneratorOptions: logoGeneratorOptions,
            },
        );

        service.setActivity(logoGeneratorId, true);
    };

    service.showSubModal = function (type, parentCtrl) {
        let modalId;
        if (type === 'logo') {
            modalId = 'modalLogoGeneratorLogoFonts';
            modalService.renderModal(
                modalId,
                'Выбор шрифта для логотипа',
                '<logo-generator-fonts data-ng-if="modal.isOpen" data-on-select="$ctrl.onSelectLogoFont(font)" data-fonts-list="$ctrl.fonts.items" data-logo="$ctrl.logo" data-slogan="$ctrl.slogan" data-is-use-slogan="$ctrl.isUseSlogan" data-language="$ctrl.logoLanguage" data-obj-type="logo" data-options="$ctrl.logoGeneratorFontsOptions"></logo-generator-fonts>',
                null,
                { isFloating: true, destroyOnClose: true, modalOverlayClass: 'modal-logo-generator-fonts' },
                { $ctrl: parentCtrl },
            );
        } else if (type === 'slogan') {
            modalId = 'modalLogoGeneratorSloganFonts';
            modalService.renderModal(
                modalId,
                'Выбор шрифта для cлогана',
                '<logo-generator-fonts data-ng-if="modal.isOpen" data-on-select="$ctrl.onSelectSlogonFont(font)" data-fonts-list="$ctrl.fonts.items" data-logo="$ctrl.logo" data-slogan="$ctrl.slogan" data-is-use-slogan="$ctrl.isUseSlogan" data-language="$ctrl.sloganLanguage" data-obj-type="slogan" data-options="$ctrl.logoGeneratorFontsOptions"></logo-generator-fonts>',
                null,
                { isFloating: true, destroyOnClose: true, modalOverlayClass: 'modal-logo-generator-fonts' },
                { $ctrl: parentCtrl },
            );
        }

        modalService.getModal(modalId).then(function (modal) {
            modal.modalScope.open();
        });
    };

    service.closeModal = function (logoGeneratorId) {
        var modalId = logoGeneratorId + 'Modal';
        service.setActivity(logoGeneratorId, false);
        modalService.close(modalId);
    };

    service.getFontsList = function () {
        return import(
            /* webpackChunkName: "logoGeneratorFonts" */
            /* webpackMode: "lazy" */
            '../fonts/data.json'
        ).then((module) => {
            return module.default;
        });
    };

    service.saveLogo = function (urlSave, dataUrl, fileExtension, options, params) {
        //'logogenerator/savelogo'
        return $http
            .post(urlSave, angular.extend({ dataUrl: dataUrl, fileExtension: fileExtension, fontFamilyLogo: options.logo.font.fontFamily }, params))
            .then(function (response) {
                $cookies.put('cookieThemeImgKey', response.data.ImgSource);
                return response.data;
            })
            .then(function (data) {
                service.saveData(options);
                return data;
            })
            .finally(() => {
                const baseHref = urlHelper.getBaseHref();
                if ($window.location.href === baseHref) {
                    $window.location.reload(true);
                } else {
                    $window.location.href = baseHref;
                }
            });
    };

    service.saveData = function (data) {
        $window.localStorage.setItem(localStorageKey, JSON.stringify(data));
    };

    service.getDataFromStorage = function () {
        var valueString = $window.localStorage.getItem(localStorageKey);

        return valueString != null && valueString.length > 0 ? JSON.parse(valueString) : null;
    };

    service.getData = function () {
        return $q.when(
            service.getDataFromStorage() ||
                $http.get('logogenerator/getdata').then(function (response) {
                    return response.data;
                }),
        );
    };

    service.setActivity = function (logoGeneratorId, isActive) {
        return service.getLogoGenerator(logoGeneratorId, function (logoGenerator) {
            return (logoGenerator.isActive = isActive);
        });
    };

    service.updateLogoSrc = function (logoGeneratorId, src) {
        storage[logoGeneratorId].preview.img.setAttribute('src', src);
    };

    service.parseLanguage = function (text) {
        var result = [];

        if (regexCyrilic.test(text)) {
            result.push('cyrillic');
        }

        if (regexLatin.test(text)) {
            result.push('latin');
        }

        return result;
    };

    service.isCyrillic = function (text) {
        return service.parseLanguage(text).indexOf('cyrillic') !== -1;
    };

    service.isLatin = function (text) {
        return service.parseLanguage(text).indexOf('latin') !== -1;
    };

    service.replaceUnsupportOnSymbol = function (text, language) {
        return text.replace(language === 'cyrillic' ? regexCyrilic : regexLatin, emptySymbol);
    };

    service.fontFamilyEscape = function (fontFamily) {
        //шрифты, у которых в наименовании есть числа не вставляются в стили, надо обязательно обернуть в кавычки
        return /\d/.test(fontFamily) ? '"' + fontFamily + '"' : fontFamily;
    };
}

export default logoGeneratorService;
