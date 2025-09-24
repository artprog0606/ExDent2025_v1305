(function (ng) {
    'use strict';

    var NewsItemCtrl = function ($http, $window, SweetAlert, $translate) {
        var ctrl = this;
        ctrl.PhotoId = 0;

        ctrl.deleteNewsItem = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.NewsItem.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.NewsItem.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('News/DeleteNewsItem', { newsId: id }).then(function (response) {
                        $window.location.assign('news');
                    });
                }
            });
        };

        ctrl.changePhoto = function (result) {
            ctrl.PhotoId = result.pictureId;
        };
    };

    NewsItemCtrl.$inject = ['$http', '$window', 'SweetAlert', '$translate'];

    ng.module('newsItem', ['uiGridCustom', 'urlGenerator', 'newsProducts', 'ngCkeditor']).controller('NewsItemCtrl', NewsItemCtrl);
})(window.angular);
