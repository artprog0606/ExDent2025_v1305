import deliveryByZonesListTemplate from './deliveryByZonesList.html';
(function (ng) {
    'use strict';

    var DeliveryByZonesListCtrl = function ($window, $document, $timeout) {
        var ctrl = this,
            emptyString = '';
        ctrl.addZone = function (point) {
            if (!point) {
                return;
            }
            ctrl.zones.push(point);
            ctrl.updateZones();
        };
        ctrl.editZone = function (newZone, oldZone) {
            if (!newZone || !oldZone) {
                return;
            }
            var index = ctrl.zones.indexOf(oldZone);
            if (index !== -1) {
                ctrl.zones[index] = newZone;
                ctrl.updateZones();
            }
        };
        ctrl.deleteZone = function (item) {
            var index = ctrl.zones.indexOf(item);
            if (index !== -1) {
                ctrl.zones.splice(index, 1);
                ctrl.updateZones();
            }
        };
        ctrl.updateZones = function () {
            ctrl.update = true;
        };
        ctrl.changeFiles = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $files != null && $files.length > 0) {
                const file = $files[0];
                //check: extention is .geojson, .json or .txt

                const reader = new FileReader();
                reader.onload = function (event) {
                    if (event.target.result) {
                        const json_text = event.target.result;
                        $timeout(function () {
                            ctrl.loadGeoObject(JSON.parse(json_text));
                        }, 0);
                    }
                };
                reader.readAsText(file);
            }
        };
        ctrl.loadGeoObject = function (geoObject) {
            if (!geoObject) {
                return;
            }
            if (!geoObject.features || !Array.isArray(geoObject.features)) {
                return;
            }
            const currentZones = ctrl.zones || [];
            let newZones = geoObject.features
                .filter((item) => item.geometry && item.geometry.coordinates && item.geometry.type === 'Polygon')
                .map(function (geoItem) {
                    let id = geoItem.id || 0;
                    let currentZone = currentZones.find((item) => item.Id === id) || {};
                    let temp;
                    // переварачиваем координаты с [долгота, широта] на [широта, долгота]
                    geoItem.geometry.coordinates.forEach((item) =>
                        item.forEach((point) => {
                            temp = point[1];
                            point[1] = point[0];
                            point[0] = temp;
                        }),
                    );
                    return {
                        Id: id,
                        Name: geoItem.properties.description || currentZone.Name || '',
                        Description: currentZone.Description || '',
                        Rate: currentZone.Rate || 0.0,
                        DeliveryTime: currentZone.DeliveryTime || '',
                        Coordinates: geoItem.geometry.coordinates,
                        FillColor: geoItem.properties.fill || currentZone.FillColor || '#ed4543',
                        FillOpacity: geoItem.properties['fill-opacity'] || currentZone.FillOpacity || 0.2,
                        StrokeColor: geoItem.properties.stroke || currentZone.StrokeColor || '#ed4543',
                        StrokeWidth: geoItem.properties['stroke-width'] || currentZone.StrokeWidth || '5',
                        StrokeOpacity: geoItem.properties['stroke-opacity'] || currentZone.StrokeOpacity || 0.5,
                    };
                });
            ctrl.zones = newZones;
            ctrl.updateZones();
        };
        ctrl.downloadGeoJson = function (event) {
            const geoObject = {
                type: 'FeatureCollection',
                metadata: {
                    name: 'map_zones_' + ctrl.methodId,
                    creator: 'Advantshop',
                },
                features: null,
            };
            geoObject.features = ctrl.zones.map(function (zone) {
                // переварачиваем координаты с [широта, долгота] на [долгота, широта]
                let coordinates = zone.Coordinates.map((item) => item.map((point) => [point[1], point[0]]));
                return {
                    type: 'Feature',
                    id: zone.Id,
                    geometry: {
                        type: 'Polygon',
                        coordinates: coordinates,
                    },
                    properties: {
                        description: zone.Name,
                        fill: zone.FillColor,
                        'fill-opacity': zone.FillOpacity,
                        stroke: zone.StrokeColor,
                        'stroke-width': zone.StrokeWidth,
                        'stroke-opacity': zone.StrokeOpacity,
                    },
                };
            });
            const blob = new Blob([JSON.stringify(geoObject)], {
                type: 'application/json',
            });
            const toDay = new Date();
            const filename =
                'map_zones_' +
                ('0' + toDay.getDate()).slice(-2) +
                '-' +
                ('0' + (toDay.getMonth() + 1)).slice(-2) +
                '-' +
                toDay.getFullYear() +
                '-' +
                ('0' + toDay.getHours()).slice(-2) +
                '-' +
                ('0' + toDay.getMinutes()).slice(-2) +
                '-' +
                ('0' + toDay.getSeconds()).slice(-2) +
                '-' +
                '.geojson';
            if ($window.navigator.msSaveOrOpenBlob) {
                $window.navigator.msSaveOrOpenBlob(blob, filename);
            } else {
                let a = $document[0].createElement('a'),
                    url = $window.URL.createObjectURL(blob);
                a.style.display = 'none';
                a.href = url;
                a.download = filename;
                a.target = '_blank';
                $document[0].body.appendChild(a);
                a.click(event);
                $timeout(function () {
                    $document[0].body.removeChild(a);
                    $window.URL.revokeObjectURL(url);
                }, 0);
            }
        };
        ctrl.sortableOptions = {
            containment: '#deliveryByZonesSortingContainer',
            scrollableContainer: '#deliveryByZonesSortingContainer',
            containerPositioning: 'relative',
            accept: function (sourceItemHandleScope, destSortableScope) {
                return sourceItemHandleScope.itemScope.sortableScope.$id === destSortableScope.$id;
            },
            orderChanged: function (event) {
                ctrl.updateZones();
            },
        };
    };
    DeliveryByZonesListCtrl.$inject = ['$window', '$document', '$timeout'];
    ng.module('shippingMethod')
        .controller('DeliveryByZonesListCtrl', DeliveryByZonesListCtrl)
        .component('deliveryByZonesList', {
            templateUrl: deliveryByZonesListTemplate,
            controller: 'DeliveryByZonesListCtrl',
            bindings: {
                onInit: '&',
                methodId: '@',
                zones: '<?',
                currencyLabel: '@',
                yaMapsApiKey: '@',
                warehousesActive: '<?',
            },
        });
})(window.angular);
