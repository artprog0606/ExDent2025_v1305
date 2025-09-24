/**
 * @jest-environment jsdom
 */

import angular from 'angular';
import 'angular-mocks';

import '../../../../node_modules/angular-translate/dist/angular-translate.js';
import '../spinbox.module.js';
import fs from 'node:fs';
import path from 'path';
import { getDirname } from '../../../../node_scripts/shopPath.js';

const dispatchBlur = (el) => {
    el.dispatchEvent(new FocusEvent('blur'));
};
describe('Spinbox', function () {
    let $controller, $rootScope, $parse, $scope, $compile, $httpBackend, $templateCache;

    beforeEach(() => {
        angular.mock.module('pascalprecht.translate');
        angular.mock.module('spinbox');
        angular.mock.inject(($injector) => {
            $parse = $injector.get('$parse');
            $compile = $injector.get('$compile');
            $controller = $injector.get('$controller');
            $controller = $injector.get('$controller');
            $rootScope = $injector.get('$rootScope');
            $httpBackend = $injector.get('$httpBackend');
            $templateCache = $injector.get('$templateCache');
        });
        $scope = $rootScope.$new();

        $templateCache.put('test', fs.readFileSync(path.join(getDirname(import.meta.url), '../templates/spinbox.html')).toString());
    });

    it('should correct init', function () {
        $scope.exampleValue = 5;
        const el = $compile(`<div data-spinbox
                            data-validation-text="validationTextTest"
                            data-need-comma="true"
                            data-disable-correct="true"
                            data-value="exampleValue"
                            data-step="1"
                            data-max="1000"
                            data-min="0"></div>`)($scope);
        $scope.$digest();
        expect(el[0].querySelector('input').value).toEqual($scope.exampleValue.toString());
    });

    it('should not correcting value and do validate input, when enable onlyValidation', function () {
        $scope.exampleValue = 5;
        const el = $compile(`<div data-spinbox
                            data-validation-text="validationTextTest"
                            data-need-comma="true"
                            data-only-validation="true"
                            data-value="exampleValue"
                            data-step="1"
                            data-max="1000"
                            data-min="0"></div>`)($scope);
        $scope.$digest();
        expect(el[0].querySelector('input').value).toEqual($scope.exampleValue.toString());

        $scope.exampleValue = 5.5;
        $scope.$digest();
        const input = el[0].querySelector('input');
        dispatchBlur(input);
        expect(input.value).toEqual('5,5');
        expect(input.classList.contains('ng-invalid')).toBeTruthy();
        expect(input.classList.contains('ng-invalid-spinbox-input')).toBeTruthy();
    });

    it('should correcting value when disable onlyValidation', function () {
        $scope.exampleValue = 5;
        const el = $compile(`<div data-spinbox
                            data-validation-text="validationTextTest"
                            data-need-comma="true"
                            data-only-validation="false"
                            data-value="exampleValue"
                            data-step="1"
                            data-max="1000"
                            data-min="0"></div>`)($scope);
        $scope.$digest();
        expect(el[0].querySelector('input').value).toEqual($scope.exampleValue.toString());

        $scope.exampleValue = 5.5;
        $scope.$digest();
        const input = el[0].querySelector('input');
        dispatchBlur(input);
        expect(input.value).toEqual('6');
        expect(input.classList.contains('ng-invalid')).toBeFalsy();
        expect(input.classList.contains('ng-invalid-spinbox-input')).toBeFalsy();
    });

    it('should update button status when change min or min', function () {
        $scope.exampleValue = 5;
        $scope.minOut = 1;
        $scope.maxOut = 10;
        const el = $compile(`<div data-spinbox
                            data-validation-text="validationTextTest"
                            data-need-comma="true"
                            data-only-validation="false"
                            data-value="exampleValue"
                            data-step="1"
                            data-max="maxOut"
                            data-min="minOut"></div>`)($scope);
        $scope.$digest();
        expect(el[0].querySelector('input').value).toEqual($scope.exampleValue.toString());

        $scope.exampleValue = 5.5;
        $scope.$digest();
        const input = el[0].querySelector('input');
        const btnMore = el[0].querySelector('.spinbox-more');
        const btnLess = el[0].querySelector('.spinbox-less');
        dispatchBlur(input);
        expect(input.value).toEqual('6');
        expect(input.classList.contains('ng-invalid')).toBeFalsy();
        expect(input.classList.contains('ng-invalid-spinbox-input')).toBeFalsy();

        expect(btnMore.classList.contains('spinbox-button-disabled')).toBeFalsy();
        expect(btnLess.classList.contains('spinbox-button-disabled')).toBeFalsy();

        expect($scope.exampleValue).toEqual('6');

        $scope.maxOut = 6;
        $scope.$digest();

        expect(btnMore.classList.contains('spinbox-button-disabled')).toBeTruthy();
        expect(btnLess.classList.contains('spinbox-button-disabled')).toBeFalsy();

        $scope.minOut = 7;
        $scope.maxOut = 10;

        $scope.$digest();

        expect(btnMore.classList.contains('spinbox-button-disabled')).toBeFalsy();
        expect(btnLess.classList.contains('spinbox-button-disabled')).toBeTruthy();
    });

    it('should revalidate, when change step, max or min in onlyValidation mode', function () {
        $scope.exampleValue = 5;
        $scope.step = 1;
        $scope.min = 0;
        $scope.max = 10;
        const el = $compile(`<div data-spinbox
                            data-validation-text="validationTextTest"
                            data-need-comma="true"
                            data-only-validation="true"
                            data-value="exampleValue"
                            data-step="step"
                            data-max="max"
                            data-min="min"></div>`)($scope);

        $scope.$digest();
        const input = el[0].querySelector('input');

        expect(input.value).toEqual($scope.exampleValue.toString());

        $scope.exampleValue = 5.5;
        $scope.$digest();
        expect(input.classList.contains('ng-invalid')).toBeTruthy();
        expect(input.classList.contains('ng-invalid-spinbox-input')).toBeTruthy();

        $scope.step = 0.5;
        $scope.$digest();

        expect(input.classList.contains('ng-invalid')).toBeFalsy();
        expect(input.classList.contains('ng-invalid-spinbox-input')).toBeFalsy();
        expect(input.classList.contains('ng-valid')).toBeTruthy();
        expect(input.classList.contains('ng-valid-spinbox-input')).toBeTruthy();
        expect(input.classList.contains('ng-dirty')).toBeTruthy();

        $scope.min = 6;
        $scope.$digest();
        expect(input.classList.contains('ng-invalid')).toBeTruthy();
        expect(input.classList.contains('ng-invalid-spinbox-input')).toBeTruthy();
        expect(input.classList.contains('ng-valid')).toBeFalsy();
        expect(input.classList.contains('ng-valid-spinbox-input')).toBeFalsy();

        $scope.exampleValue = 9;
        $scope.$digest();

        $scope.max = 8;
        $scope.$digest();

        expect($scope.exampleValue).toBeUndefined();
        expect(input.value).toEqual('9');
        expect(input.classList.contains('ng-invalid')).toBeTruthy();
        expect(input.classList.contains('ng-invalid-spinbox-input')).toBeTruthy();
        expect(input.classList.contains('ng-valid')).toBeFalsy();
        expect(input.classList.contains('ng-valid-spinbox-input')).toBeFalsy();
    });
});
