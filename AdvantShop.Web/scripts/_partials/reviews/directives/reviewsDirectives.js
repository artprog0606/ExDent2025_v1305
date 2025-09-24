import reviewFormTemplate from '../templates/reviewForm.html';
/* @ngInject */
function reviewsDirective($parse) {
    return {
        restrict: 'A',
        scope: true,
        controller: 'ReviewsCtrl',
        controllerAs: 'reviews',
        bindToController: true,
        link: function (scope, element, attrs, ctrl) {
            ctrl.moderate = attrs.moderate === 'true';
            ctrl.isAdmin = attrs.isAdmin === 'true';
            ctrl.entityId = attrs.entityId;
            ctrl.entityType = attrs.entityType;
            ctrl.name = attrs.name;
            ctrl.email = attrs.email;
            ctrl.currentRating = attrs.currentRating;
            ctrl.actionUrl = attrs.actionUrl;
            ctrl.formVisible = attrs.formVisible !== 'false';
            ctrl.allowImageUpload = attrs.allowImageUpload === 'true';
            ctrl.readonly = attrs.readonly === 'true';
            ctrl.onAddComment = attrs.onAddComment != null ? $parse(attrs.onAddComment) : null;
            ctrl.onDeleteComment = attrs.onDeleteComment != null ? $parse(attrs.onDeleteComment) : null;
            ctrl.showFormAfterDo = attrs.showFormAfterDo != null ? $parse(attrs.showFormAfterDo) : true;
        },
    };
}

function reviewItemDirective() {
    return {
        require: '^reviews',
        restrict: 'A',
        scope: true,
        link: function (scope, element, attrs, ctrl) {
            ctrl.addItemInStorage(attrs.reviewId, element);
        },
    };
}

function reviewsFormDirective() {
    return {
        require: ['^reviewsForm', '^reviews'],
        restrict: 'A',
        scope: {
            visibleFormCancelButton: '=',
            reviewId: '=',
            name: '=',
            email: '=',
            submitFn: '&',
            cancelFn: '&',
            formVisible: '=',
            allowImageUpload: '=',
            isShowUserAgreementText: '=',
            agreementDefaultChecked: '<?',
            userAgreementText: '@',
            moderate: '=',
            currentRating: '=',
        },
        controller: 'ReviewsFormCtrl',
        controllerAs: 'reviewsForm',
        bindToController: true,
        transclude: true,
        templateUrl: reviewFormTemplate,
        replace: true,
        link: function (scope, element, attrs, ctrls) {
            ctrls[1].addForm(ctrls[0], element);
        },
    };
}

function reviewReplyDirective() {
    return {
        require: '^reviews',
        restrict: 'A',
        replace: true,
        transclude: true,
        scope: {
            reviewId: '@',
        },
        template: '<a href="javascript:void(0)" class="review-item-button" data-ng-transclude data-ng-click="parentScope.reply(reviewId)"></a>',
        link: function (scope, element, attrs, ctrl) {
            scope.parentScope = ctrl;
        },
    };
}

function reviewDeleteDirective() {
    return {
        require: '^reviews',
        restrict: 'A',
        replace: true,
        transclude: true,
        scope: {
            reviewId: '@',
            actionUrl: '@',
        },
        template:
            '<a href="javascript:void(0)" class="review-item-button" data-ng-transclude data-ng-click="parentScope.delete(reviewId, actionUrl)"></a>',
        link: function (scope, element, attrs, ctrl) {
            scope.parentScope = ctrl;
        },
    };
}

function reviewItemRatingDirective() {
    return {
        scope: true,
        controller: 'ReviewItemRatingCtrl',
        controllerAs: 'reviewItemRating',
    };
}

export { reviewsDirective, reviewItemDirective, reviewsFormDirective, reviewReplyDirective, reviewDeleteDirective, reviewItemRatingDirective };
