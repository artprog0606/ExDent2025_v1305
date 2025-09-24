/* @ngInject */
function ReviewItemRatingCtrl($attrs, $http, $parse, $scope, toaster) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.actionUrl = $parse($attrs.actionUrl)($scope);
        ctrl.reviewId = $parse($attrs.reviewId)($scope);

        ctrl.likeData = {
            Dislikes: $parse($attrs.countDislikes)($scope),
            Likes: $parse($attrs.countLikes)($scope),
        };
    };

    ctrl.like = function () {
        ctrl.voteReview(ctrl.reviewId, true, ctrl.actionUrl);
    };

    ctrl.dislike = function () {
        ctrl.voteReview(ctrl.reviewId, false, ctrl.actionUrl);
    };

    ctrl.voteReview = function (reviewId, like, actionUrl) {
        return $http.post(actionUrl, { reviewId: reviewId, vote: like }).then(function (response) {
            if (response.data.error) {
                toaster.pop('error', response.data.errors);
            } else {
                angular.extend(ctrl.likeData, response.data.likeData);
            }
            return response.data;
        });
    };
}

export default ReviewItemRatingCtrl;
