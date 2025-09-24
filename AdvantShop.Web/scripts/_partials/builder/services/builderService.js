/*@ngInject*/
function builderService($http, $q, modalService, builderTypes, $translate) {
    var service = this,
        builderLinks = {
            background: {
                element: undefined,
                pattern: undefined,
            },
            theme: {
                element: undefined,
                pattern: undefined,
            },
            colorScheme: {
                element: undefined,
                pattern: undefined,
            },
        },
        designVariantsBackup = {},
        designVariants;

    service.memoryStylesheet = function (type, element) {
        switch (type) {
            case builderTypes.background:
                builderLinks.background.element = element;
                builderLinks.background.pattern = element[0].href.replace(/\/backgrounds\/([\d\w\s_-]*)\//, function (str, group, offset, source) {
                    return str.replace(group, '{name}');
                });
                break;
            case builderTypes.theme:
                builderLinks.theme.element = element;
                builderLinks.theme.pattern = element[0].href.replace(/\/themes\/([\d\w\s_-]*)\//, function (str, group, offset, source) {
                    return str.replace(group, '{name}');
                });
                break;
            case builderTypes.colorScheme:
                builderLinks.colorScheme.element = element;
                builderLinks.colorScheme.pattern = element[0].href.replace(/\/colors\/([\d\w\s_-]*)\//, function (str, group, offset, source) {
                    return str.replace(group, '{name}');
                });
                break;
        }
    };

    //#region NewBuilder

    service.newBuilderGetDesignVariants = function () {
        return angular.isDefined(designVariants) ? $q.when(designVariants) : service.newBuilderRequestDesignVariants();
    };

    service.newBuilderRequestDesignVariants = function () {
        return $http.get('common/getDesignNewBuilder').then(function (response) {
            designVariants = response.data;
            designVariantsBackup = angular.copy(designVariants);
            return designVariants;
        });
    };

    service.setDesignVariants = function (design) {
        designVariants = design;
        designVariantsBackup = angular.copy(designVariants);
    };

    service.newBuilderApply = function (type, param) {
        switch (type) {
            case builderTypes.background:
                builderLinks.background.element.attr('href', builderLinks.background.pattern.replace('{name}', param));
                break;
            case builderTypes.theme:
                builderLinks.theme.element.attr('href', builderLinks.theme.pattern.replace('{name}', param));
                break;
            case builderTypes.colorScheme:
                builderLinks.colorScheme.element.attr('href', builderLinks.colorScheme.pattern.replace('{name}', param));
                designVariants.CurrentColorScheme = param;
                break;
        }

        return designVariants;
    };

    service.newBuilderApplyMenuStyle = function (menuStyleName) {
        return (designVariants.MenuStyle = menuStyleName);
    };

    service.newBuilderSave = function () {
        var response = {
            old: angular.copy(designVariantsBackup),
            new: angular.copy(designVariants),
        };

        designVariantsBackup = angular.copy(designVariants);

        return $http.post('common/savedesignnewbuilder', { data: designVariants }).then(function () {
            return response;
        });
    };

    service.getSettings = function () {
        return $http.get('common/getDesignBuilder').then(function (response) {
            return response.data;
        });
    };

    service.getAdditionalPhones = function () {
        return $http.get('common/getAdditionalPhones').then(function (response) {
            return response.data;
        });
    };

    service.convertToStandardPhone = function (phone) {
        return $http.post('adminv3/settings/convertToStandardPhone', { phone: phone }).then(function (response) {
            return response.data.obj;
        });
    };

    service.newBuilderReturn = function () {
        angular.extend(designVariants, angular.copy(designVariantsBackup));

        service.newBuilderApply(builderTypes.background, designVariants.CurrentBackGround);
        service.newBuilderApply(builderTypes.theme, designVariants.CurrentTheme);
        service.newBuilderApply(builderTypes.colorScheme, designVariants.CurrentColorScheme);

        return designVariants;
    };

    service.newBuilderDialogOpen = function (ctrl) {
        modalService.renderModal(
            'modalNewBuilder',
            'Настройки',
            '<div ng-include="\'scripts/_partials/builder/templates/newBuilder/body.html\'"></div>',
            '<div ng-include="\'scripts/_partials/builder/templates/newBuilder/footer.html\'"></div>',
            {
                isOpen: true,
                modalOverlayClass: 'builder-dialog',
                backgroundEnable: false,
                isFloating: true,
                destroyOnClose: true,
            },
            { builder: ctrl },
        );
    };

    service.newBuilderDialogClose = function () {
        modalService.close('modalNewBuilder');
    };

    //#endregion

    service.setAllProductsManualRatio = function (manualRatio) {
        return $http
            .post('product/setAllProductsManualRatio', {
                manualRatio: manualRatio,
            })
            .then(function (response) {
                return response;
            });
    };

    service.resizePictures = function () {
        return $http.post('common/resizePictures').then(function (response) {
            return response.data;
        });
    };

    service.resizeCategoryPictures = function (type) {
        return $http.post('common/resizeCategoryPictures', { type: type }).then(function (response) {
            return response.data;
        });
    };
}

export default builderService;
