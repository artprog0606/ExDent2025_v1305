import productAvailabilityMapTemplate from './productAvailabilityMap.html';

export default function ProductAvailabilityMapDirective() {
    return {
        scope: {
            apiKeyMap: '@',
            mobileMode: '<?',
            offerId: '<',
        },
        controller: 'productAvailabilityMapCtrl',
        controllerAs: 'productAvailabilityMap',
        bindToController: true,
        templateUrl: productAvailabilityMapTemplate,
    };
}
