(function (ng) {
    'use strict';

    var AdminCommentsCtrl = function (
        $location,
        $window,
        $anchorScroll,
        $timeout,
        toaster,
        SweetAlert,
        adminCommentsService,
        $translate,
        adminWebNotificationsEvents,
        adminWebNotificationsService,
    ) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.fetch();
            ctrl.onInit({ adminCommentsCtrl: ctrl });
            ctrl.removeCallbackUpdateAdminComment = adminWebNotificationsService.addListener(
                adminWebNotificationsEvents.updateAdminComment,
                function (data) {
                    if (data.objId === ctrl.objId && data.objType === ctrl.type) {
                        ctrl.fetch();
                    }
                },
            );
        };

        ctrl.$onDestroy = function () {
            if (ctrl.removeCallbackUpdateAdminComment) {
                ctrl.removeCallbackUpdateAdminComment();
                ctrl.removeCallbackUpdateAdminComment = null;
            }
        };

        ctrl.fetch = function () {
            adminCommentsService.getComments(ctrl.objId, ctrl.type).then(function (result) {
                ctrl.comments = result.Comments;
            });
        };

        ctrl.reply = function (comment) {
            ctrl.adminCommentIdActive = comment.Id;
            ctrl.visibleFormCancelButton = true;
            ctrl.formVisible = true;
            ctrl.form.onReply(comment);
            ctrl.editingComment = null;
        };

        ctrl.showComment = function ($event, id) {
            $event.preventDefault();
            var elId = 'admin-comment-' + id,
                element = $window.document.getElementById(elId);
            if (element) {
                $anchorScroll(elId);
                element.classList.add('highlighted');
                $timeout(function () {
                    element.classList.remove('highlighted');
                }, 2000);
            }
        };

        ctrl.clearForm = function () {
            ctrl.form.reset();
            ctrl.adminCommentIdActive = null;
            ctrl.visibleFormCancelButton = false;
        };

        ctrl.submit = function (form) {
            var url = ctrl.objUrl != null && ctrl.objUrl.length > 0 ? ctrl.objUrl : $location.absUrl();

            adminCommentsService.addComment(ctrl.objId, ctrl.type, form.adminCommentId, form.text, url).then(function (response) {
                if (response.Result === true) {
                    ctrl.fetch();
                    //ctrl.comments.push(response.Comment);

                    if (ctrl.type == 'task') {
                        toaster.success(
                            '',
                            $translate.instant('Admin.Js.Tasks.Tasks.CommentToTask') +
                                ' <a  href="tasks/view/' +
                                ctrl.objId +
                                '">№' +
                                ctrl.objId +
                                '</a> ' +
                                $translate.instant('Admin.Js.Tasks.Tasks.CommentToTaskAdded'),
                        );
                    } else {
                        toaster.success('', $translate.instant('Admin.Js.Partials.CommentAddedSuccessfully'));
                    }
                } else {
                    toaster.error($translate.instant('Admin.Js.Partials.Error'), response.Error);
                }
            });
            ctrl.clearForm();
        };

        ctrl.cancel = function () {
            ctrl.clearForm();
        };

        ctrl.delete = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.Partials.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Partials.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    adminCommentsService.deleteComment(id).then(function (response) {
                        if (response.Result === true) {
                            ctrl.fetch();

                            if (ctrl.type == 'task') {
                                toaster.success(
                                    '',
                                    $translate.instant('Admin.Js.Tasks.Tasks.CommentToTaskDeleted', {
                                        number: ' <a  href="tasks/view/' + ctrl.objId + '">№' + ctrl.objId + '</a> ',
                                    }),
                                );
                            }
                        }
                    });
                }
            });
        };
    };

    AdminCommentsCtrl.$inject = [
        '$location',
        '$window',
        '$anchorScroll',
        '$timeout',
        'toaster',
        'SweetAlert',
        'adminCommentsService',
        '$translate',
        'adminWebNotificationsEvents',
        'adminWebNotificationsService',
    ];

    ng.module('adminComments', []).controller('AdminCommentsCtrl', AdminCommentsCtrl);
})(window.angular);
