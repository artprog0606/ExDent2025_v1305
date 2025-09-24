(function (ng) {
    'use strict';

    var GalleryCloudCtrl = function ($q, $timeout, galleryCloudService, toaster) {
        var ctrl = this,
            timerSearch;

        ctrl.$onInit = function () {
            ctrl.inProgress = false;
            ctrl.data = {};
            ctrl.page = 0;
            ctrl.prevTerm = '';
            //ctrl.getData();
        };

        ctrl.$postLink = function () {
            ctrl.showContent = true;
        };

        ctrl.getData = function (term) {
            var _page = null;

            if (ctrl.itemsLoading === true) {
                return;
            }

            if (term !== ctrl.prevTerm) {
                ctrl.data = {};
                ctrl.page = 0;
            } else if (ctrl.data != null && ctrl.data.next_page == null) {
                return;
            }

            _page = ctrl.page += 1;

            ctrl.itemsLoading = true;

            return $q
                .when(
                    term == null || term.length === 0
                        ? galleryCloudService.getPopularImages(_page)
                        : galleryCloudService.getSearchImages(_page, term),
                )
                .then(function (data) {
                    if (data.result == false) {
                        return $q.reject(data.errors);
                    } else {
                        ctrl.prevTerm = term;
                        ctrl.mergeData(ctrl.data, data);
                        ctrl.isLoaded = true;
                        return ctrl.data;
                    }
                })
                .catch(function (error) {
                    if (ng.isArray(error)) {
                        error.forEach(function (error) {
                            toaster.pop('error', '', error);
                        });
                    } else {
                        toaster.pop('error', '', error);
                    }
                })
                .finally(function () {
                    $timeout(function () {
                        ctrl.itemsLoading = false;
                    }, 500);
                });
        };

        ctrl.mergeData = function (dataDist, data) {
            var photos;
            if (dataDist == null) {
                dataDist = data;
            } else {
                photos = ng.copy(dataDist.photos);
                dataDist = ng.extend(dataDist, data);
                if (dataDist.photos != null) {
                    Array.prototype.unshift.apply(dataDist.photos, photos);
                }
            }
            return dataDist;
        };

        ctrl.search = function (term) {
            if (timerSearch != null) {
                clearTimeout(timerSearch);
            }

            timerSearch = setTimeout(function () {
                ctrl.page = 0;
                ctrl.getData(term);
            }, 700);
        };

        ctrl.selectSearchExample = function (term) {
            ctrl.page = 0;
            ctrl.term = term;
            ctrl.search(term);
        };

        ctrl.select = function (photo) {
            if (ctrl.onSelect != null && ctrl.inProgress === false) {
                photo.isSelected = true;
                ctrl.inProgress = true;

                $q.when(ctrl.onSelect({ photo: photo }) || true)
                    .then(function () {
                        galleryCloudService.closeModal();
                    })
                    .finally(function () {
                        photo.isSelected = false;
                        ctrl.inProgress = false;
                    });
            }
        };
    };

    ng.module('galleryCloud').controller('GalleryCloudCtrl', GalleryCloudCtrl);

    GalleryCloudCtrl.$inject = ['$q', '$timeout', 'galleryCloudService', 'toaster'];
})(window.angular);
