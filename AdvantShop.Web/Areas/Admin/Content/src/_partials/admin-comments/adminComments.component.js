import adminCommentsFormTemplate from './templates/admin-comments-form.html';
import adminCommentItemTemplate from './templates/admin-comment-item.html';
import adminCommentsTemplate from './templates/admin-comments.html';
(function (ng) {
    'use strict';

    ng.module('adminComments')
        .component('adminComments', {
            templateUrl: adminCommentsTemplate,
            controller: 'AdminCommentsCtrl',
            bindings: {
                objId: '<',
                objUrl: '<?',
                type: '<?',
                formVisible: '=',
                onInit: '&',
            },
        })
        .component('adminCommentItem', {
            require: {
                parent: '^adminComments',
            },
            controller: 'AdminCommentsItemCtrl',
            templateUrl: adminCommentItemTemplate,
            bindings: {
                comment: '<',
                objId: '<',
                type: '<?',
            },
        })
        .component('adminCommentsForm', {
            require: {
                parent: '^adminComments',
            },
            controller: 'AdminCommentsFormCtrl',
            templateUrl: adminCommentsFormTemplate,
            bindings: {
                visibleFormCancelButton: '=',
                adminCommentId: '=',
                submitFn: '&',
                cancelFn: '&',
                formVisible: '=',
            },
        });
})(window.angular);
