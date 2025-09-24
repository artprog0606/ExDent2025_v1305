/*@ngInject*/
function yandexMapsService(urlHelper, $q) {
    const service = this;
    let deferReady = $q.defer();
    let isInitialized = false;

    // дополнительная функция проверки признаков что янекс.карта уже как-то загружена
    service.isLoadedYandexMap = function () {
        return window.ymaps != null;
    };

    service.loadYandexMap = function (params) {
        if (service.isLoadedYandexMap() && isInitialized) {
            return $q.resolve();
        } else {
            if (document.querySelector('script[src*="api-maps.yandex.ru"]') == null) {
                var script = document.createElement('script');
                script.onload = function () {
                    waitingInitYmaps();
                };
                script.src = 'https://api-maps.yandex.ru/2.1/?' + urlHelper.paramsToString(angular.extend({ lang: 'ru-RU' }, params || {}));
                document.body.appendChild(script);
            } else {
                waitingInitYmaps();
            }
        }

        return deferReady.promise;
    };

    function waitingInitYmaps() {
        const _defer = $q.defer();
        if (service.isLoadedYandexMap()) {
            ymaps.ready(() => _defer.resolve());
        } else {
            loop(_defer);
        }

        return _defer.promise.then(() => {
            isInitialized = true;
            deferReady.resolve();
        });
    }

    const delay = 50;
    const retryMax = 3000 / 50;
    let tryCount = 0;

    function loop(defer) {
        if (service.isLoadedYandexMap()) {
            defer.resolve();
        } else if (tryCount < retryMax) {
            tryCount += 1;
            setTimeout(() => loop(defer), delay);
        } else {
            console.warn('Yandex map not found');
            defer.reject('Yandex map not found');
        }
        return defer.promise;
    }
}

export default yandexMapsService;
