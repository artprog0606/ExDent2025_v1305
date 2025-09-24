import { createTestApp } from 'angularjs-jest';
import compareModule from '../compare.module';
import { CompareService as CompareServiceMock } from '../__mocks__/compareServiceMock';
import type { IQService } from 'angular';
import { getOfferId } from '../__mocks__/compare';
import { spyOn } from 'jest-mock';

describe('compareControl', () => {
    const getTestApp = () =>
        createTestApp({
            modules: [compareModule],
            mocks: { compareService: ($q: IQService) => CompareServiceMock($q) },
            access: ['$parse'],
        });
    it('should render compareControl', async () => {
        const app = getTestApp();
        const offerId = getOfferId();
        const element = app.render(`<div compare-control="${offerId}"></div>`, app.$scope);
        const removeCompareScopeMock = spyOn(app.compareService, 'removeCompareScope');
        app.compareService.removeCompareScope = removeCompareScopeMock;
        element.scope().$destroy();
        expect(removeCompareScopeMock).toHaveBeenCalled();
    });
});
