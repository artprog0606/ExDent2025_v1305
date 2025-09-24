/**
 * @jest-environment jsdom
 */
import { fn } from 'jest-mock';

import { urlHelper } from './urlHelper.js';

import { projectsAreas, projectsNames } from '../../../node_scripts/shopVariables.js';

const URL_PARAMS = new URLSearchParams({
    productId: 5,
    q: 'bestproduct',
    param: 'VALUE',
});
const SEARCH = `?${URL_PARAMS.toString()}`;
const DOMAIN = `https://www.example.net`;
const BASE_URL = `${DOMAIN}`;
const EXAMPLE_URL = `${DOMAIN}/search/${SEARCH}&n=`;
const HASH = `testUrlHash`;

const urlHelperInstance = new urlHelper(BASE_URL);

describe('urlHelper', () => {
    beforeEach(() => {
        delete window.location;
        window.location = {
            href: EXAMPLE_URL,
            search: SEARCH,
            origin: DOMAIN,
        };
    });

    test('should return correct value url param', () => {
        expect(urlHelperInstance.getUrlParam('q', true)).toEqual('bestproduct');
        expect(urlHelperInstance.getUrlParam('productId', true)).toEqual('5');
        expect(urlHelperInstance.getUrlParam('param', false)).toEqual('VALUE');
        expect(urlHelperInstance.getUrlParam('param', true)).toEqual('value');
        expect(urlHelperInstance.getUrlParam('null')).toBeNull();
    });

    test('should return correct value url param by name', () => {
        expect(urlHelperInstance.getUrlParamByName('q')).toEqual('bestproduct');
        expect(urlHelperInstance.getUrlParamByName('productId')).toEqual('5');
        expect(urlHelperInstance.getUrlParamByName('param')).toEqual('value');
        expect(urlHelperInstance.getUrlParamByName('n')).toEqual('');
        expect(urlHelperInstance.getUrlParamByName('null')).toBeNull();
    });

    test('should return filtered correct value url param by name', () => {
        const filter = (param) => param != 'key';

        expect(
            urlHelperInstance.getUrlParamDictionaryByNameFunc(filter, {
                key: false,
                params: [1, 2, 3],
                productInfo: 'beautiful product',
            }),
        ).toEqual([
            { name: 'params', value: [1, 2, 3] },
            { name: 'productInfo', value: 'beautiful product' },
        ]);
    });

    test('should return correct params url', () => {
        expect(urlHelperInstance.getUrlParamsAsObject(window.location.search)).toEqual({
            productId: '5',
            q: 'bestproduct',
            param: 'VALUE',
        });

        expect(urlHelperInstance.getUrlParamsAsObject(`${window.location.search}&n=`)).toEqual({
            productId: '5',
            q: 'bestproduct',
            param: 'VALUE',
            n: '',
        });
    });

    test('should return correct base href', () => {
        expect(urlHelperInstance.getBaseHref()).toEqual(DOMAIN);
    });

    test('should validate correct domain', () => {
        expect(urlHelperInstance.hasDomain('https://example/')).toBeTruthy();
        expect(urlHelperInstance.hasDomain('http://example')).toBeTruthy();
        expect(urlHelperInstance.hasDomain('tcp://example')).toBeTruthy();
        expect(urlHelperInstance.hasDomain('ftp://example')).toBeTruthy();
        expect(urlHelperInstance.hasDomain('ip://example')).toBeTruthy();
        expect(urlHelperInstance.hasDomain('pop://example')).toBeTruthy();
        expect(urlHelperInstance.hasDomain('smtp://example')).toBeTruthy();
        expect(urlHelperInstance.hasDomain('https://11.22.33.444/')).toBeTruthy();
    });

    test('should validate fail domain', () => {
        expect(urlHelperInstance.hasDomain('https//11.22.33.444/')).toBeFalsy();
        expect(urlHelperInstance.hasDomain('https//example')).toBeFalsy();
        expect(urlHelperInstance.hasDomain('https//')).toBeFalsy();
        expect(urlHelperInstance.hasDomain('https/')).toBeFalsy();
        expect(urlHelperInstance.hasDomain('http/')).toBeFalsy();
        expect(urlHelperInstance.hasDomain('http:/')).toBeFalsy();
        expect(urlHelperInstance.hasDomain('http11://')).toBeFalsy();
        expect(urlHelperInstance.hasDomain('h9ttp5//')).toBeFalsy();
        expect(urlHelperInstance.hasDomain('http11/')).toBeFalsy();
        expect(urlHelperInstance.hasDomain('://')).toBeFalsy();
        expect(urlHelperInstance.hasDomain('//')).toBeFalsy();
    });

    test('should return correct absolute url', () => {
        expect(urlHelperInstance.getAbsUrl('product/2')).toEqual('https://www.example.net/product/2');

        expect(urlHelperInstance.getAbsUrl('https://www.example.net/test')).toEqual('https://www.example.net/test');

        expect(urlHelperInstance.getAbsUrl('/scripts/_common/modal/templates/modal.html')).toEqual(
            'https://www.example.net/scripts/_common/modal/templates/modal.html',
        );
    });

    test('should return correct url', () => {
        expect(urlHelperInstance.getUrl('product/2')).toEqual('/product/2');
        expect(urlHelperInstance.getUrl('/product/2')).toEqual('/product/2');
        expect(urlHelperInstance.getUrl('https://www.example.net/test')).toEqual('https://www.example.net/test');
    });

    test('should convert correct params to string', () => {
        expect(
            urlHelperInstance.paramsToString({
                key: 1,
                key2: 'test',
                arr: [1, 2, 3],
            }),
        ).toEqual('key=1&key2=test&arr=1,2,3');
    });

    test('should update correct params in url', () => {
        expect(urlHelperInstance.updateQueryStringParameter(window.location.href, 'productId', 999)).toEqual(
            'https://www.example.net/search/?productId=999&q=bestproduct&param=VALUE&n=',
        );
    });

    test('should update correct params in url with hash', () => {
        Object.defineProperty(window, 'location', {
            value: {
                href: `${DOMAIN}/#${HASH}`,
                search: null,
                hash: HASH,
            },
        });

        expect(urlHelperInstance.updateQueryStringParameter(window.location.href, 'productId', 999)).toEqual(`${DOMAIN}/?productId=999#${HASH}`);
    });

    test('should remove correct params in url', () => {
        expect(urlHelperInstance.updateQueryStringParameter(window.location.href, 'productId')).toEqual(
            'https://www.example.net/search/?q=bestproduct&param=VALUE&n=',
        );
    });

    test('should add correct params in url', () => {
        expect(urlHelperInstance.updateQueryStringParameter(window.location.href, 'newParam', 'newValue')).toEqual(
            'https://www.example.net/search/?productId=5&q=bestproduct&param=VALUE&n=&newParam=newValue',
        );
    });

    test('should set correct params in url and push state in history', () => {
        const NEW_URL = 'https://www.example.net/search/?productId=100&q=bestproduct&param=VALUE&n=';

        window.history.pushState = fn(() => (window.location.href = NEW_URL));
        urlHelperInstance.setLocationQueryParams('productId', 10);

        expect(window.location.href).toEqual(NEW_URL);
    });

    test('should set correct params in url and replace state in history', () => {
        const NEW_URL = 'https://www.example.net/search/?productId=100&q=bestproduct&param=VALUE&n=';

        window.history.replaceState = fn(() => (window.location.href = NEW_URL));
        urlHelperInstance.setLocationQueryParams('productId', 10, true);

        expect(window.location.href).toEqual(NEW_URL);
    });

    test('should return correct string', () => {
        expect(urlHelperInstance.getHashFromString('search#id1')).toEqual('id1');
        expect(urlHelperInstance.getHashFromString('search#id1', true)).toEqual('#id1');
    });

    test('should return physical path', () => {
        expect(urlHelperInstance.getPhysicalPath(projectsNames.store, false)).toEqual(projectsAreas.store);
        expect(urlHelperInstance.getPhysicalPath(projectsNames.store, true)).toEqual(projectsAreas.storeMobile);
        expect(urlHelperInstance.getPhysicalPath(projectsNames.admin, false)).toEqual(projectsAreas.admin);
        expect(urlHelperInstance.getPhysicalPath(projectsNames.admin, true)).toEqual(projectsAreas.adminMobile);
    });

    it.each(
        Object.values(projectsNames)
            .map((x) =>
                [projectsNames.store, projectsNames.admin].includes(x)
                    ? [true, false].map((isMobile) => ({
                          projectName: x,
                          isMobile,
                          result: projectsAreas[isMobile ? `${x}Mobile` : x],
                      }))
                    : {
                          projectName: x,
                          isMobile: false,
                          result: projectsAreas[x],
                      },
            )
            .flat(),
    )(
        'should return correct project areas "$result", when isMobile "$isMobile" and project name "$projectName"',
        ({ projectName, isMobile, result }) => {
            expect(urlHelperInstance.getPhysicalPath(projectName, isMobile)).toEqual(result);
        },
    );

    test('should return error because unknown area', () => {
        expect(() => urlHelperInstance.getPhysicalPath('bad_area')).toThrow(`urlHelper: unknown area is "bad_area"`);
    });

    const exampleUrl = ['assets/cart.html', 'chunks/cart.html', 'entries/cart.html'];
    it.each(
        Object.values(projectsAreas)
            .map((x) =>
                exampleUrl.map((example) => ({
                    projectArea: x,
                    url: `../${example}`,
                    result: `${x}dist/${example}`,
                })),
            )
            .flat(),
    )('should return correct normalize url "$result", when url "$url" and project area "$projectArea"', ({ projectArea, url, result }) => {
        expect(urlHelperInstance.normalizeBundleFilesUrl(projectArea, url)).toEqual(result);
    });

    test('should return old url when is absolute', () => {
        expect(urlHelperInstance.normalizeBundleFilesUrl(projectsAreas.store, 'http://localhost/dist/assets/cart.html')).toEqual(
            'http://localhost/dist/assets/cart.html',
        );
    });

    test.each([
        {
            href: BASE_URL,
        },
        {
            href: `${BASE_URL}/adminv3`,
        },
    ])('should return subfolder "/" for $href', ({ href }) => {
        urlHelper.instance = null;
        const _urlHelperInstance = new urlHelper(href);

        expect(_urlHelperInstance.getSubfolder(href)).toEqual('/');
    });

    test.each([
        {
            href: `${BASE_URL}/dev`,
        },
        {
            href: `${BASE_URL}/dev/adminv3`,
        },
    ])('should return subfolder "/dev" for $href', ({ href }) => {
        urlHelper.instance = null;
        const _urlHelperInstance = new urlHelper(href);

        expect(_urlHelperInstance.getSubfolder()).toEqual('/dev');
    });

    test.each([
        {
            url: `${BASE_URL}/dev`,
            result: `${BASE_URL}/dev/`,
        },
        {
            url: `${BASE_URL}`,
            result: `${BASE_URL}/`,
        },
        {
            url: `${BASE_URL}/dev/adminv3`,
            result: `${BASE_URL}/dev/adminv3/`,
        },
        {
            url: `${BASE_URL}/dev/adminv3/`,
            result: `${BASE_URL}/dev/adminv3/`,
        },
    ])('should add "/" for url when not contains', ({ url, result }) => {
        expect(urlHelperInstance.addSlash(url)).toEqual(result);
    });

    test.each([
        {
            href: BASE_URL,
            result: 'example_net',
        },
        {
            href: `${BASE_URL}/adminv3`,
            result: 'example_net_adminv3',
        },
        {
            href: `${BASE_URL}/dev`,
            result: 'example_net_dev',
        },
        {
            href: `${BASE_URL}/dev/adminv3`,
            result: 'example_net_dev_adminv3',
        },
    ])('should return uri only words or numbers with delimiter "_"', ({ href, result }) => {
        urlHelper.instance = null;
        const _urlHelperInstance = new urlHelper();

        expect(_urlHelperInstance.transformUrlToKey(href)).toEqual(result);
    });

    describe('should return base uri only words or numbers with delimiter "_"', () => {
        test.each([
            {
                href: BASE_URL,
                result: 'example_net',
            },
            {
                href: `${BASE_URL}/adminv3`,
                result: 'example_net',
            },
            {
                href: `${BASE_URL}/dev`,
                result: 'example_net_dev',
            },
            {
                href: `${BASE_URL}/dev/adminv3`,
                result: 'example_net_dev',
            },
        ])('without "admin" part', ({ href, result }) => {
            urlHelper.instance = null;
            const _urlHelperInstance = new urlHelper(href);

            expect(_urlHelperInstance.transformBaseUriToKey()).toEqual(result);
        });

        test.each([
            {
                href: `${BASE_URL}/adminv3`,
                result: 'example_net_adminv3',
            },
            {
                href: `${BASE_URL}/dev/adminv3`,
                result: 'example_net_dev_adminv3',
            },
        ])('with "admin" part', ({ href, result }) => {
            urlHelper.instance = null;
            const _urlHelperInstance = new urlHelper(href);

            expect(_urlHelperInstance.transformBaseUriToKey(false)).toEqual(result);
        });
    });
});
