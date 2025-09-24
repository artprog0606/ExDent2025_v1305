/* @ngInject */
export function apiMapKeyDirective(apiMapService) {
    return {
        scope: {
            mapApiKey: '@',
        },
        link(scope, element, attrs, ctrl) {
            if (attrs.mapApiKey) {
                apiMapService.setMapApiKey(attrs.mapApiKey);
            } else {
                console.error('ApiKey required for geo-location-api-map-key');
            }
        },
    };
}
