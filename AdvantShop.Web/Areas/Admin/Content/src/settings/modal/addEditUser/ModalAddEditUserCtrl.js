(function (ng) {
    'use strict';

    var ModalAddEditUserCtrl = function ($http, $scope, $q, $uibModalInstance, SweetAlert, toaster, $translate, isMobileService) {
        var ctrl = this;
        ctrl.formInited = false;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve != null ? ctrl.$resolve.params || {} : {};
            ctrl.customerId = params.customerId;
            if (ctrl.customerId == 'me') {
                ctrl.mode = 'me';
            } else {
                ctrl.mode = ctrl.customerId != null ? 'edit' : 'add';
            }

            ctrl.getFormData().then(function () {
                if (ctrl.mode == 'add') {
                    ctrl.enabled = true;
                    ctrl.formInited = true;
                    ctrl.customerRole = ctrl.moderatorsAvailable ? '50' : ctrl.isAdmin ? '100' : null;
                    ctrl.adminAppNotificationsEnabled = true;
                } else {
                    ctrl.getUser(ctrl.customerId);
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getUser = function (customerId) {
            var url = ctrl.mode == 'me' ? 'account/getCurrentUser' : 'users/getUser';
            $http.get(url, { params: { customerId: customerId, rnd: Math.random() } }).then(function (response) {
                if (response.data.result === false) {
                    data.errors.forEach(function (error) {
                        toaster.error($translate.instant('Admin.Js.Settings.AddEditUserCtrl.Error'), error);
                    });
                    return;
                }
                var data = response.data.obj;
                if (data != null) {
                    ctrl.customerId = data.CustomerId;
                    ctrl.email = data.Email;
                    if (data.CustomerRole == 100)
                        ctrl.roleActionKeys.forEach(function (item) {
                            item.Enabled = false;
                        });
                    ctrl.customerRole = data.CustomerRole != null ? data.CustomerRole.toString() : null;
                    ctrl.firstName = data.FirstName;
                    ctrl.lastName = data.LastName;
                    ctrl.phone = data.Phone;
                    ctrl.enabled = data.Enabled;
                    ctrl.headCustomerId = data.HeadCustomerId;
                    ctrl.birthDay = data.BirthDay;
                    ctrl.city = data.City;
                    ctrl.avatar = data.Avatar;
                    ctrl.photoSrc = data.PhotoSrc;
                    ctrl.position = data.Position;
                    ctrl.departmentId = data.DepartmentId;
                    ctrl.headUserId = data.HeadCustomerId;
                    ctrl.selectedRolesIds = data.ManagerRolesIds;
                    ctrl.editHimself = data.EditHimself;
                    ctrl.sign = data.Sign;
                    ctrl.customerFields = ctrl.customerFields || data.CustomerFields.filter((x) => x.ShowInUserEditing);
                    ctrl.fcmToken = data.FcmToken;
                    ctrl.adminAppNotificationsEnabled = data.AdminAppNotificationsEnabled;
                }
                ctrl.addEditUserForm.$setPristine();
                ctrl.formInited = true;

                ctrl.getRolesValidation();
            });
        };

        ctrl.save = function () {
            ctrl.btnSleep = true;

            var params = {
                customerId: ctrl.customerId,
                customerRole: ctrl.customerRole,
                email: ctrl.email,
                firstName: ctrl.firstName,
                lastName: ctrl.lastName,
                phone: ctrl.phone,
                enabled: ctrl.enabled,
                headCustomerId: ctrl.headUserId,
                birthDay: ctrl.birthDay,
                city: ctrl.city,
                departmentId: ctrl.departmentId,
                position: ctrl.position,
                roleActionKeys: ctrl.roleActionKeys,
                managerRolesIds: ctrl.selectedRolesIds,
                avatar: ctrl.avatar,
                photoEncoded: ctrl.photoEncoded,
                sign: ctrl.sign,
                customerFields: ctrl.customerFields,
                adminAppNotificationsEnabled: ctrl.adminAppNotificationsEnabled,
                rnd: Math.random(),
            };

            var url;
            switch (ctrl.mode) {
                case 'add':
                    url = 'users/addUser';
                    break;
                case 'me':
                    url = 'account/updateCurrentUser';
                    break;
                default:
                    url = 'users/updateUser';
                    break;
            }

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop(
                        'success',
                        '',
                        ctrl.mode == 'add'
                            ? $translate.instant('Admin.Js.Settings.AddEditUser.EmployeeAdded')
                            : $translate.instant('Admin.Js.Settings.AddEditUser.ChangesSaved'),
                    );
                    if (ctrl.photoEncoded) {
                        $scope.$emit('avatarupdated', { customerId: ctrl.customerId });
                    }

                    ctrl.getRolesValidation().then(function (result) {
                        if (result) {
                            $uibModalInstance.close(data.obj.customer);
                        }
                    });

                    if (data.obj.reloadPage === true && ctrl.mode != 'me') {
                        window.location.reload();
                    }
                } else {
                    data.errors.forEach(function (error) {
                        toaster.error($translate.instant('Admin.Js.Settings.AddEditUserCtrl.Error'), error);
                    });
                }
                ctrl.btnSleep = false;
            });
        };

        ctrl.getFormData = function () {
            var url = ctrl.mode == 'me' ? 'account/getUserFormData' : 'users/getUserFormData';
            return $http.get(url, { params: { customerId: ctrl.customerId, rnd: Math.random() } }).then(function (response) {
                var data = response.data.obj;
                if (data != null) {
                    ctrl.departments = data.departments;
                    ctrl.users = data.users;
                    ctrl.roles = data.roles;
                    ctrl.roleActionKeys = data.roleActionKeys;
                    ctrl.isAdmin = data.isAdmin;
                    ctrl.moderatorsAvailable = data.moderatorsAvailable;
                    ctrl.managersAvailable = data.managersAvailable;
                    ctrl.hasRoleActionAccess = data.hasRoleActionAccess;
                    ctrl.customerFields = data.customerFields;
                }
            });
        };

        ctrl.selectRoleActions = function (result) {
            ctrl.roleActionKeys = result.roleActionKeys;
        };

        ctrl.changePassword = function () {
            if (ctrl.customerId == null) return;
            SweetAlert.confirm($translate.instant('Admin.Js.Settings.AddEditUserCtrl.LinkToChangePassword'), {
                title: $translate.instant('Admin.Js.Settings.AddEditUserCtrl.ChangePassword'),
            }).then(function (result) {
                if (result === true || result.value) {
                    var url = ctrl.mode == 'me' ? 'account/sendChangePasswordMail' : 'users/sendChangePasswordMail';
                    $http.post(url, { customerId: ctrl.customerId }).then(function (response) {
                        var data = response.data;
                        if (data.result === true) {
                            toaster.pop('success', $translate.instant('Admin.Js.Settings.AddEditUserCtrl.LinkSuccessfullySent'));
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.error($translate.instant('Admin.Js.Settings.AddEditUserCtrl.Error'), error);
                            });
                        }
                    });
                }
            });

            if (isMobileService.getValue()) {
                document.querySelector('.swal2-container').style.zIndex = 2000;
            }
        };

        ctrl.updateAvatar = function (params) {
            if (params != null) {
                ctrl.avatar = params.fileName;
                ctrl.photoSrc = ctrl.photoEncoded = params.base64String;
            }
        };

        ctrl.deleteAvatar = function () {
            $http.post('common/deleteAvatar', { customerId: ctrl.customerId }).then(function (response) {
                ctrl.avatar = null;
                $scope.$emit('avatarupdated', { customerId: ctrl.customerId });
            });
        };

        ctrl.getRolesValidation = function () {
            ctrl.rolesErrors = null;

            if (ctrl.customerId == null || ctrl.mode != 'edit') {
                return $q.resolve(true);
            }
            return $http
                .get('users/getRolesValidation', {
                    params: { customerId: ctrl.customerId, rolesIds: ctrl.selectedRolesIds },
                })
                .then(function (response) {
                    ctrl.rolesErrors = response.data.errors;
                    return ctrl.rolesErrors == null || ctrl.rolesErrors.length == 0;
                });
        };
    };

    ModalAddEditUserCtrl.$inject = ['$http', '$scope', '$q', '$uibModalInstance', 'SweetAlert', 'toaster', '$translate', 'isMobileService'];

    ng.module('uiModal').controller('ModalAddEditUserCtrl', ModalAddEditUserCtrl);
})(window.angular);
