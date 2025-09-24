const baseUrl = window.location.origin;

import { ContentRouteService } from './content-loader.routing.js';

const adminUrl = `adminv3/`;
const adminUrlLength = adminUrl.length - 1;
class ContentLoaderService {
    /*@ngInject*/
    constructor($http, $ocLazyLoad, $q, $location) {
        this.$http = $http;
        this.$ocLazyLoad = $ocLazyLoad;
        this.$q = $q;
        this.$location = $location;
    }

    addPlace(ctrl) {
        this.place = ctrl;
    }

    #urlSubstring(url) {
        const index = url.toLowerCase().lastIndexOf(adminUrl);
        return url.slice(index + adminUrlLength);
    }

    #wrapFnModuleLoad(fn) {
        if (typeof fn === 'undefined') {
            return this.$q.resolve(null);
        }

        return this.$q.when(fn()).then((module) => this.$ocLazyLoad.inject(module.default));
    }

    getData(url) {
        return fetch(`${url}?${new URLSearchParams({ useMobileLayout: true })}`, {
            method: 'GET',
        });
    }

    getContent(url) {
        if (typeof this.place === 'undefined') {
            // eslint-disable-next-line no-console
            console.error(`Place for the content wasn't found`);
            return undefined;
        }

        const route = ContentRouteService.findByUrl(url);

        if (typeof route === 'undefined') {
            window.location = url;
            return undefined;
            // console.error(`Not found route by ${url}`);
            // return;
        }

        this.place.showLoading();

        return this.#wrapFnModuleLoad(route[1].fnModuleLoad)
            .then(
                () =>
                    new Promise((resolve, reject) => {
                        const setFetchUrls = new Set();
                        const getContent = (_url) => {
                            if (setFetchUrls.has(_url)) {
                                return reject(new Error('Too many redirects'));
                            }
                            setFetchUrls.add(_url);
                            const promise = this.getData(_url);
                            return promise.then(async (response) => {
                                if (response.redirected) {
                                    getContent(response.url);
                                } else {
                                    const data = await response.text();
                                    resolve(data);
                                }
                            });
                        };
                        getContent(url.startsWith('http') ? url : `${baseUrl}${url}`);
                    }),
            )
            .then((data) => {
                this.place.setContent(data);
                this.$location.url(this.#urlSubstring(url));
            })
            .catch((error) => {
                // eslint-disable-next-line no-console
                console.error(error);
            })
            .finally(() => this.place.hideLoading());
    }
}

export { ContentLoaderService };
