import angular from 'angular';
import 'angular-mocks';
import '../../../_common/PubSub/__mocks__/PubSub.js';
import '../compare.module';
import { mockRandom, resetMockRandom } from 'jest-mock-random';
import { CompareService, compareEndpoints, type ICompareService, type ICompareServiceStatic } from './compareService';
import { type CompareCountResponseType } from '../compare.module';
import { getCompareResponseData, getCompareResponseErrorText, getCompareResponseStatus, getCompareScope, getOfferId } from '../__mocks__/compare';
import { PubSub } from '../../../_common/PubSub/PubSub.js';
import { ICompareCtrl } from '../controllers/compareController';

describe('CompareService', () => {
    let $httpBackend, compareService: ICompareService & ICompareServiceStatic, offerId: number;

    beforeEach(() => {
        offerId = getOfferId();
        mockRandom(0.2);
        angular.mock.module('compare');

        angular.mock.inject(($injector) => {
            $httpBackend = $injector.get('$httpBackend');

            compareService = $injector.get('compareService');
        });
    });

    afterEach(() => {
        resetMockRandom();
        PubSub.clear();
        CompareService.compareScopeList.clear();
    });

    describe('add', () => {
        it('should correctly call', function () {
            const compareData: CompareCountResponseType = getCompareResponseData();
            let result = {};

            $httpBackend.expectPOST(compareEndpoints.ADD, { rnd: Math.random(), offerId }).respond(200, compareData);
            compareService.add(offerId, true).then((res) => {
                result = res;
            });
            $httpBackend.flush();
            expect(result).toEqual(compareData);
            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should correctly handle exception', function () {
            const error = getCompareResponseErrorText();
            let result: Error = new Error();
            $httpBackend.expectPOST(compareEndpoints.ADD, { rnd: Math.random(), offerId }).respond(500, error);
            compareService.add(offerId, true).catch((e: Error) => {
                result = e;
            });
            $httpBackend.flush();
            expect(result.message).toEqual(error);
            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });
    });

    describe('remove', () => {
        it('should correctly call', function () {
            const compareData: CompareCountResponseType = getCompareResponseData();
            let result = {};

            $httpBackend.expectGET(`${compareEndpoints.REMOVE}?offerId=${offerId}&rnd=${Math.random()}`).respond(200, compareData);
            compareService.remove(offerId, false).then((res) => {
                result = res;
            });
            $httpBackend.flush();
            expect(result).toEqual(compareData);
            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should correctly handle exception', function () {
            const error = getCompareResponseErrorText();
            let result: Error = new Error();
            $httpBackend.expectGET(`${compareEndpoints.REMOVE}?offerId=${offerId}&rnd=${Math.random()}`).respond(500, error);
            compareService.remove(offerId, false).catch((e: Error) => {
                result = e;
            });
            $httpBackend.flush();
            expect(result.message).toEqual(error);
            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });
    });

    describe('removeAll', () => {
        it('should correctly call', function () {
            const compareData: CompareCountResponseType = getCompareResponseData();
            let result = {};

            $httpBackend.expectGET(`${compareEndpoints.REMOVE_ALL}`).respond(200, compareData);
            compareService.removeAll().then((res) => {
                result = res;
            });
            $httpBackend.flush();
            expect(result).toEqual(compareData);
            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should correctly handle exception', function () {
            const error = getCompareResponseErrorText();
            let result: Error = new Error();
            $httpBackend.expectGET(`${compareEndpoints.REMOVE_ALL}`).respond(500, error);
            compareService.removeAll().catch((e: Error) => {
                result = e;
            });
            $httpBackend.flush();
            expect(result.message).toEqual(error);
            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });
    });

    describe('getCountObj', () => {
        it('should correctly return data', function () {
            let compareDataResponse: CompareCountResponseType = getCompareResponseData();

            $httpBackend.expectPOST(compareEndpoints.ADD, { rnd: Math.random(), offerId }).respond(200, compareDataResponse);
            compareService.add(offerId, true);
            $httpBackend.flush();
            expect(compareService.getCountObj().count).toBe(compareDataResponse.Count);
            compareDataResponse = getCompareResponseData();
            $httpBackend.expectGET(`${compareEndpoints.REMOVE}?offerId=${offerId}&rnd=${Math.random()}`).respond(200, compareDataResponse);
            compareService.remove(offerId, true);
            $httpBackend.flush();
            expect(compareService.getCountObj().count).toBe(compareDataResponse.Count);

            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });
    });

    describe('getStatus', () => {
        it('should correctly call', () => {
            const compareResponseData: boolean = getCompareResponseStatus();
            let result;

            $httpBackend.expectGET(`${compareEndpoints.STATUS}?offerId=${offerId}&rnd=${Math.random()}`).respond(200, compareResponseData);
            compareService.getStatus(offerId).then((res) => {
                result = res;
            });
            $httpBackend.flush();
            expect(result).toEqual(compareResponseData);
            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should correctly handle exception', () => {
            const compareErrorData: string = getCompareResponseErrorText();
            let result;

            $httpBackend.expectGET(`${compareEndpoints.STATUS}?offerId=${offerId}&rnd=${Math.random()}`).respond(500, compareErrorData);
            compareService.getStatus(offerId).catch((e) => {
                result = e.message;
            });
            $httpBackend.flush();
            expect(result).toEqual(compareErrorData);
            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });
    });

    describe('addCompareScope', () => {
        it('should correctly call', () => {
            const compareScope: ICompareCtrl = getCompareScope();
            compareService.addCompareScope(offerId, compareScope);
            const setCompareScopeList = CompareService.compareScopeList.get(offerId);
            expect(setCompareScopeList?.has(compareScope)).toBe(true);
        });
    });

    describe('removeCompareScope', () => {
        it('should correctly call', () => {
            const compareScope: ICompareCtrl = getCompareScope();
            compareService.addCompareScope(offerId, compareScope);
            const setCompareScopeList = CompareService.compareScopeList.get(offerId);
            expect(setCompareScopeList?.has(compareScope)).toBe(true);
            compareService.removeCompareScope(offerId, compareScope);
            expect(setCompareScopeList?.has(compareScope)).toBe(false);
        });
    });

    describe('changeCompareControlState', () => {
        it('should correctly call', () => {
            const compareScope: ICompareCtrl = getCompareScope();
            compareService.addCompareScope(offerId, compareScope);
            const setCompareScopeList = CompareService.compareScopeList.get(offerId);
            expect(setCompareScopeList?.has(compareScope)).toBe(true);
            compareService.changeCompareControlState(offerId, true);
            expect(compareScope.isAdded).toBe(true);
            compareService.changeCompareControlState(offerId, false);
            expect(compareScope.isAdded).toBe(false);
        });
    });
});
