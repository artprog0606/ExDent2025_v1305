import galleryIconsTemplate from './galleryIcons.html';
(function (ng) {
    'use strict';

    ng.module('galleryIcons').component('galleryIcons', {
        controller: 'GalleryIconsCtrl',
        templateUrl: galleryIconsTemplate,
        bindings: {
            onSelect: '&',
        },
    });
})(window.angular);
