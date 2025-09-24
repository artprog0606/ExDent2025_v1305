import './addTag.html';
(function (ng) {
    /* @ngInject */
    const ModalAddTagCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        const ctrl = this;

        ctrl.$onInit = function () {
            const resolve = ctrl.$resolve;
            ctrl.params = resolve.data;
            ctrl.tagPageUrl = `${window.location.origin}/tags/`;
            ctrl.isEditMode = ctrl.params?.tagId != null;
            ctrl.dataLoaded = false;

            if (!ctrl.isEditMode) {
                ctrl.tagData = {
                    IsEditMode: false,
                    DefaultMeta: true,
                    Enabled: true,
                    VisibilityForUsers: true,
                    SortOrder: 0,
                };
                ctrl.dataLoaded = true;
            } else {
                ctrl.getTag(ctrl.params.tagId)
                    .then((data) => {
                        ctrl.tagData = data;
                    })
                    .finally(() => {
                        ctrl.dataLoaded = true;
                    });
            }
            ctrl.getMetaDescription('tag').then((data) => {
                ctrl.metaDescriptionHtml = data;
            });

            ctrl.getMetaVariablesComplete('tag').then((data) => {
                ctrl.metaMetaVariablesCompleteList = data;
            });
        };

        ctrl.getTag = function (id) {
            return $http
                .get('tags/GetTag', { params: { id: id } })
                .then(function (response) {
                    return response.data;
                })
                .catch((error) => console.error(error));
        };

        ctrl.getMetaDescription = function (type, showInstruction = true) {
            return $http
                .get('common/MetaVariablesDescription', { params: { type, showInstruction } })
                .then(function (response) {
                    return response.data;
                })
                .catch((error) => console.error(error));
        };

        ctrl.getMetaVariablesComplete = function (type) {
            return $http
                .get('common/MetaVariablesComplete', { params: { type } })
                .then(function (response) {
                    return response.data;
                })
                .catch((error) => console.error(error));
        };

        ctrl.send = function (data) {
            ctrl.isProcessTag = true;
            if (data.SortOrder != null && data.SortOrder.length === 0) {
                data.SortOrder = 0;
            }
            $http
                .post('tags/AddTag', data)
                .then(function (response) {
                    const data = response.data;
                    if (data.result === true) {
                        const successText = ctrl.isEditMode ? 'Admin.Js.ChangesSaved' : 'Admin.Js.AddTag.TagAddedSuccessfully';
                        toaster.pop('success', '', $translate.instant(successText));

                        $uibModalInstance.close();
                    } else {
                        const errorText = ctrl.isEditMode ? 'Admin.Js.EditTag.Error' : 'Admin.Js.AddTag.Error';
                        toaster.pop('error', '', $translate.instant(errorText));
                    }
                })
                .finally(function () {
                    ctrl.isProcessTag = false;
                });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ng.module('uiModal').controller('ModalAddTagCtrl', ModalAddTagCtrl);
})(window.angular);
