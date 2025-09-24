/**
 * @jest-environment jsdom
 */

import angular from 'angular';
import 'angular-mocks';
import 'angular-cookies';

import productViewModule from '../productView.module.js';
import { createTestApp } from 'angularjs-jest';
import { mockRandom } from 'jest-mock-random';
import lozadAdvModule from '../../../_common/lozad-adv/lozadAdv.module.js';
import { mockIntersectionObserverInit } from '../../../_common/lozad-adv/__mocks__/lozadAdv.ts';
import { convertToDataAttrs } from '../../../__test__/utils/convertToDataAttrs.js';
import { getInitialPhotoStartJson, getPhotoObj } from '../__mocks__/photos.js';
import { productViewImageListMapperRequest } from '../controllers/productViewImageListController.js';
import { PRODUCT_VIEW_IMAGE_CONTAINER_CLASS } from '../controllers/productViewImageController.js';

describe('productViewImageListDirective', function () {
    let $httpBackend;

    const rnd = 0.123456789;

    const $cookiesMock = () => {
        const cookies = {};
        return {
            put: (key, value) => (cookies[key] = value),
            get: (key) => cookies[key],
        };
    };
    const domServiceMock = () => {
        const cookies = {};
        return {
            put: (key, value) => (cookies[key] = value),
            get: (key) => cookies[key],
        };
    };

    const windowServiceMock = () => {
        const cookies = {};
        return {
            put: (key, value) => (cookies[key] = value),
            get: (key) => cookies[key],
        };
    };
    const urlHelperMock = () => {
        const cookies = {};
        return {
            put: (key, value) => (cookies[key] = value),
            get: (key) => cookies[key],
        };
    };

    const $translateMock = () => {
        const cookies = {};
        return {
            put: (key, value) => (cookies[key] = value),
            get: (key) => cookies[key],
        };
    };
    const getTestApp = () =>
        createTestApp({
            modules: [productViewModule, lozadAdvModule],
            mocks: {
                $cookies: () => $cookiesMock(),
                domService: () => domServiceMock(),
                windowService: () => windowServiceMock(),
                urlHelper: () => urlHelperMock(),
                $translate: () => $translateMock(),
            },
            access: ['$cookies', '$flushPendingTasks', '$q', '$httpBackend', 'productViewService'],
        });

    const firstPhoto = getPhotoObj();
    const secondPhoto = getPhotoObj();
    const photoResult = [firstPhoto, secondPhoto];

    const initialPhotoStartPhotoJson = [getInitialPhotoStartJson(firstPhoto)];

    const data = {
        productId: 1,
        blockProductPhotoHeight: 300,
        productImageType: 'middle',
        photoWidth: 150,
        photoHeight: 150,
        isProductPhotoLazy: false,
        limitPhotoCount: 5,
        renderedPhotoId: firstPhoto.PhotoId,
        viewMode: 'single',
        colorInitialId: firstPhoto.ColorID,
    };

    const triggerProductImageList = (el, event) => {
        el[0].querySelector('[data-product-view-image-list]').dispatchEvent(event);
    };
    beforeEach(() => {
        mockRandom(rnd);
    });

    it('should correct init', async function () {
        const app = getTestApp();
        const el = app.render(
            `
       <div class="mobile-product-view-item cs-t-1 mobile-product-view__item js-products-view-block"
                data-product-view-item=""
                data-offer-id="14510"
                data-product-id="${data.productId}"
                data-offer="{Amount: 10, RoundedPrice: 7990, OfferId: 14510}">
            <a data-product-view-image-list data-product-id="1" data-color-initial-id="22">
                <span id="initialPhoto">
                    <img src="photo.jpg"  onerror="this.src = 'error.png'"data-ng-src="photo.jpg">
                </span>
            </a>
        </div>`,
            app.$scope,
        );
        const elScope = el.scope();

        expect(elScope.$$childHead.productViewImageList.productId).toEqual(1);
    });

    it('should correct add html when dispatch mouseenter', async function () {
        const app = getTestApp();
        app.$httpBackend
            .expectPOST('/mobile/product/ProductViewPhoto', productViewImageListMapperRequest(data))
            .respond(200, "<div id='hasItem'>TEST</div>");
        app.$httpBackend.expectGET(`productExt/getphotos?productId=${data.productId}&rnd=${rnd}`).respond(200, photoResult);

        const el = app.render(
            `
            <div class="mobile-product-view-item cs-t-1 mobile-product-view__item js-products-view-block"
                data-product-view-item=""
                data-offer-id="14510"
                data-product-id="${data.productId}"
                data-offer="{Amount: 10, RoundedPrice: 7990, OfferId: 14510}">
            <a data-product-view-image-list
                ${convertToDataAttrs(data).join('\n')}>
                <span id="initialPhoto">
                    <img src="photo.jpg"  onerror="this.src = 'error.png'" data-ng-src="photo.jpg">
                </span>
            </a>
        </div>`,
            app.$scope,
        );

        triggerProductImageList(el, new MouseEvent('mouseenter'));

        await new Promise((resolve) => {
            setTimeout(() => {
                resolve();
            }, 110);
        });

        app.$httpBackend.flush();
        expect(el[0].querySelector('#initialPhoto')).toBeTruthy();
        expect(el[0].querySelector('#hasItem')).toBeTruthy();
    });

    it('should correct add html when dispatch touchstart', async function () {
        const app = getTestApp();
        app.$httpBackend
            .expectPOST('/mobile/product/ProductViewPhoto', productViewImageListMapperRequest(data))
            .respond(200, "<div id='hasItem'>TEST</div>");
        app.$httpBackend.expectGET(`productExt/getphotos?productId=${data.productId}&rnd=${rnd}`).respond(200, photoResult);

        const el = app.render(
            `
            <div class="mobile-product-view-item cs-t-1 mobile-product-view__item js-products-view-block"
                data-product-view-item=""
                data-offer-id="14510"
                data-product-id="${data.productId}"
                data-offer="{Amount: 10, RoundedPrice: 7990, OfferId: 14510}">
                <a data-product-view-image-list
                data-view-mode="'single'"
                ${convertToDataAttrs(data).join('\n')}>
                    <span id="initialPhoto">
                        <img src="photo.jpg"  onerror="this.src = 'error.png'"data-ng-src="photo.jpg">
                    </span>
                </a>
             </div>`,
            app.scope,
        );

        triggerProductImageList(el, new TouchEvent('touchstart'));

        app.$httpBackend.flush();

        expect(el[0].querySelector('#initialPhoto')).toBeTruthy();
        expect(el[0].querySelector('#hasItem')).toBeTruthy();
    });

    it('should not send multiple requests', async function () {
        const app = getTestApp();
        app.$httpBackend
            .expectPOST('/mobile/product/ProductViewPhoto', productViewImageListMapperRequest(data))
            .respond(200, "<div id='hasItem'>TEST</div>");
        app.$httpBackend.expectGET(`productExt/getphotos?productId=${data.productId}&rnd=${rnd}`).respond(200, photoResult);

        const el = app.render(
            `
        <div class="mobile-product-view-item cs-t-1 mobile-product-view__item js-products-view-block"
                data-product-view-item=""
                data-offer-id="14510"
                data-product-id="${data.productId}"
                data-offer="{Amount: 10, RoundedPrice: 7990, OfferId: 14510}">
                <a data-product-view-image-list
                    data-view-mode="'single'"
                ${convertToDataAttrs(data).join('\n')}>
                    <span id="initialPhoto">
                        <img src="photo.jpg"  onerror="this.src = 'error.png'" data-ng-src="photo.jpg">
                    </span>
                </a>
        </div>`,
            app.scope,
        );

        triggerProductImageList(el, new TouchEvent('touchstart'));

        app.$httpBackend.flush();

        app.$httpBackend
            .expectPOST('/mobile/product/ProductViewPhoto', productViewImageListMapperRequest(data))
            .respond(200, "<div id='hasItem'>TEST</div>");

        triggerProductImageList(el, new TouchEvent('touchstart'));

        expect(() => app.$httpBackend.flush()).toThrow('No pending request to flush');

        expect(el[0].querySelector('#initialPhoto')).toBeTruthy();
        expect(el[0].querySelector('#hasItem')).toBeTruthy();
    });

    it('should changed picture src if change view mode from single to tile', async function () {
        const { mockIntersectionObserver } = mockIntersectionObserverInit();
        Object.defineProperty(window, 'innerHeight', {
            writable: true,
            configurable: true,
            value: 1000,
        });

        document.body.innerHTML = `
        <div data-product-view-mode
            data-default-view-mode="single"
            data-is-mobile="true"
            data-photo-height-by-view-mode-default="'180px'"
            data-photo-height-by-view-mode="{viewName: 'single', value: '300px'}">

            <div class="mobile-product-view-item cs-t-1 mobile-product-view__item js-products-view-block"
                data-product-view-item=""
                data-offer-id="14510"
                data-product-id="${data.productId}"
                data-offer="{Amount: 10, RoundedPrice: 7990, OfferId: 14510}">
                    <a data-product-view-image-list
                     data-lozad-adv="productViewImageList.filterPhotos({isVisible, colorId: productViewImageList.currentColorId})"
                     data-lozad-observer-mode="'observerAlways'"
                     lozad-adv-debounce="false"
                     data-view-mode="'single'"
                    ${convertToDataAttrs(data).join('\n')}>
                        <span id="initialPhoto" class="${PRODUCT_VIEW_IMAGE_CONTAINER_CLASS}">
                            <img id="target" src="${initialPhotoStartPhotoJson[0].PathBig}"  onerror="this.src = 'error.png'"
                            data-product-view-image
                             data-photo-size="'Small'"
                            data-start-photo-json='${JSON.stringify(initialPhotoStartPhotoJson)}'
                            data-photo-id="4328">
                        </span>
                    </a>
            </div>
        </div>`;

        mockIntersectionObserver([
            {
                isIntersecting: true,
                boundingClientRect: {
                    top: 0,
                    height: 100,
                },
                target: document.querySelector('#target'),
            },
        ]);

        const app = getTestApp();
        app.$cookies.put('mobile_viewmode', 'single');
        app.$httpBackend
            .expectPOST('/mobile/product/ProductViewPhoto', productViewImageListMapperRequest(data))
            .respond(200, "<div id='hasItem'>TEST</div>");
        app.$httpBackend.expectGET(`productExt/getphotos?productId=${data.productId}&rnd=${rnd}`).respond(200, photoResult);

        const el = app.render(document.body, app.scope);

        expect(el[0].querySelector('#initialPhoto img').src).toEqual(initialPhotoStartPhotoJson[0].PathBig);
        triggerProductImageList(el, new TouchEvent('touchstart'));

        app.$httpBackend.flush();
        const elScope = el.scope();
        expect(Array.isArray(elScope.$$childHead.$$childHead.$$childHead.productViewImageList.photosStorage)).toBeTruthy();

        app.productViewService.setView('testName', 'tile', [], true);

        await new Promise((resolve) => {
            setTimeout(() => {
                resolve();
            }, 0);
        });

        expect(el[0].querySelector('#initialPhoto img').src).toEqual(initialPhotoStartPhotoJson[0].PathSmall);
    });
});
