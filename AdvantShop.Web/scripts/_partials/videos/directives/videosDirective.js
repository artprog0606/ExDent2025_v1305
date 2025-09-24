import videosTemplateTemplate from '../templates/videosTemplate.html';
function videosDirective() {
    return {
        restrict: 'A',
        controller: 'VideosCtrl',
        controllerAs: 'videos',
        scope: {
            productId: '@',
            onReceive: '&',
        },
        bindToController: true,
        replace: true,
        templateUrl: videosTemplateTemplate,
    };
}

export { videosDirective };
