import galleryCloudTemplate from './galleryCloud.html';
(function (ng) {
    'use strict';

    ng.module('galleryCloud').component('galleryCloud', {
        controller: 'GalleryCloudCtrl',
        templateUrl: galleryCloudTemplate,
        bindings: {
            onSelect: '&',
        },
    });
})(window.angular);
