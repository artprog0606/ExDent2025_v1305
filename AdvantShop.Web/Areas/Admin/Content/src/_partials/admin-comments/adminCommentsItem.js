(function (ng) {
    'use strict';

    var AdminCommentsItemCtrl = function ($timeout, toaster, adminCommentsService, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {};

        ctrl.edit = function () {
            ctrl.text = ctrl.comment.Text;
            ctrl.parent.editingComment = ctrl.comment.Id;
            //ctrl.editMode = true;
            ctrl.setFocus();
        };

        ctrl.save = function () {
            if (ctrl.text == null) {
                ctrl.setFocus();
                return;
            }
            adminCommentsService.updateComment(ctrl.comment.Id, ctrl.text).then(function (response) {
                if (response.Result === true) {
                    ctrl.comment.Text = ctrl.text;
                    ctrl.cancelEdit();

                    if (ctrl.type == 'task') {
                        toaster.success(
                            '',
                            $translate.instant('Admin.Js.Tasks.Tasks.CommentToTaskUpdated', {
                                number: ' <a  href="tasks/view/' + ctrl.objId + '">№' + ctrl.objId + '</a> ',
                            }),
                        );
                    } else {
                        toaster.success('', $translate.instant('Admin.Js.Partials.ChangesSaved'));
                    }
                } else {
                    toaster.error($translate.instant('Admin.Js.Partials.Error'), response.Error);
                }
            });
        };

        ctrl.cancelEdit = function () {
            ctrl.parent.editingComment = null;
            //ctrl.editMode = false;
        };

        ctrl.setFocus = function () {
            ctrl.texFocus = false;
            $timeout(function () {
                ctrl.texFocus = true;
            }, 0);
        };
    };

    AdminCommentsItemCtrl.$inject = ['$timeout', 'toaster', 'adminCommentsService', '$translate'];

    ng.module('adminCommentsItem', []).controller('AdminCommentsItemCtrl', AdminCommentsItemCtrl);
})(window.angular);
