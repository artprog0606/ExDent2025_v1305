(function (ng) {
    'use strict';

    var ModalAddEditPropertyCtrl = function ($uibModalInstance, $http, $filter, toaster, $translate, $timeout) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            if (params.propertyId != null) {
                if (params.propertyId.selectedPropertyId != null) {
                    ctrl.propertyId = params.propertyId.selectedPropertyId || 0;
                } else {
                    ctrl.propertyId = params.propertyId || 0;
                }
            }

            ctrl.currentGroupId = params.groupId != null ? params.groupId : 0;
            ctrl.mode = ctrl.propertyId != null ? 'edit' : 'add';

            ctrl.getData().then(function () {
                if (ctrl.mode == 'add') {
                    ctrl.type = ctrl.types[0];
                    ctrl.group = $filter('filter')(ctrl.groups, { Value: ctrl.currentGroupId }, true)[0];
                    ctrl.useInFilter = true;
                    ctrl.useInDetails = true;
                    ctrl.sortOrder = 0;
                } else {
                    ctrl.getProperty(ctrl.propertyId);
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getData = function () {
            return $http.get('properties/getPropertyData').then(function (response) {
                var data = response.data;
                ctrl.types = data.types;
                ctrl.groups = data.groups;
            });
        };

        ctrl.getProperty = function (propertyId) {
            $http.get('properties/getProperty', { params: { propertyId: propertyId } }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.name = data.Name;
                    ctrl.nameDisplayed = data.NameDisplayed;
                    ctrl.description = data.Description;
                    ctrl.unit = data.Unit;
                    ctrl.useInFilter = data.UseInFilter;
                    ctrl.useInDetails = data.UseInDetails;
                    ctrl.useInBrief = data.UseInBrief;
                    ctrl.expanded = data.Expanded;
                    ctrl.sortOrder = data.SortOrder;

                    ctrl.type = $filter('filter')(ctrl.types, { Value: data.Type }, true)[0];
                    ctrl.group = data.GroupId != null ? $filter('filter')(ctrl.groups, { Value: data.GroupId }, true)[0] : ctrl.groups[0];

                    $timeout(function () {
                        if (CKEDITOR.instances['descriptionProperty'] != null && CKEDITOR.instances['descriptionProperty'] != undefined) {
                            CKEDITOR.instances['descriptionProperty'].setData(ctrl.description);
                        }
                    }, 1000);
                }
            });
        };

        ctrl.saveProperty = function () {
            var params = {
                propertyId: ctrl.propertyId,
                name: ctrl.name,
                nameDisplayed: ctrl.nameDisplayed,
                description: ctrl.description,
                unit: ctrl.unit,
                type: ctrl.type.Value,
                groupId: ctrl.group != null ? ctrl.group.Value : 0,
                useInFilter: ctrl.useInFilter,
                useInDetails: ctrl.useInDetails,
                useInBrief: ctrl.useInBrief,
                expanded: ctrl.expanded,
                sortOrder: ctrl.sortOrder,
            };

            var url = ctrl.mode == 'add' ? 'properties/addProperty' : 'properties/updateProperty';

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Properties.ChangesSaved'));
                    $uibModalInstance.close(params.name);
                } else {
                    toaster.pop(
                        'error',
                        $translate.instant('Admin.Js.Properties.Error'),
                        $translate.instant('Admin.Js.Properties.ErrorWhileCreatingEditing'),
                    );
                }
            });
        };
    };

    ModalAddEditPropertyCtrl.$inject = ['$uibModalInstance', '$http', '$filter', 'toaster', '$translate', '$timeout'];

    ng.module('uiModal').controller('ModalAddEditPropertyCtrl', ModalAddEditPropertyCtrl);
})(window.angular);
