/**
 * @jest-environment jsdom
 */

import angular from 'angular';
import 'angular-mocks';

import '../../../_common/urlHelper/urlHelperService.module.js';

import '../customOptions.module.js';
import { evaluatedCustomOptionsToCustomOptionItemMapper } from '../../cart/__mocks__/cart.js';

describe('customOptionsService', function () {
    let customOptionsService;

    document.head.innerHTML = `<base href="http://example.net/" />`;

    beforeEach(() => {
        angular.mock.module('urlHelper');
        angular.mock.module('customOptions');
        angular.mock.inject(($injector) => {
            customOptionsService = $injector.get('customOptionsService');
        });
    });

    describe('isEqualCustomOptions', function () {
        it('should return true, when options with once id', function () {
            expect(
                customOptionsService.isEqualCustomOptions(
                    [
                        {
                            CustomOptionId: 1,
                            OptionAmount: 2,
                            OptionTitle: 'test1',
                            OptionId: 2,
                        },
                        {
                            CustomOptionId: 1,
                            OptionAmount: 3,
                            OptionTitle: 'test1',
                            OptionId: 3,
                        },
                    ],
                    [
                        {
                            CustomOptionId: 1,
                            OptionAmount: 2,
                            OptionTitle: 'test1',
                            OptionId: 2,
                        },
                        {
                            CustomOptionId: 1,
                            OptionAmount: 3,
                            OptionTitle: 'test1',
                            OptionId: 3,
                        },
                    ],
                ),
            ).toBeTruthy();
        });
        it('should compare title if amount is null', function () {
            expect(
                customOptionsService.isEqualCustomOptions(
                    [
                        {
                            CustomOptionId: 1,
                            OptionTitle: 'test1',
                            OptionId: 2,
                        },
                        {
                            CustomOptionId: 1,
                            OptionTitle: 'test2',
                            OptionId: 3,
                        },
                    ],
                    [
                        {
                            CustomOptionId: 1,
                            OptionTitle: 'test1',
                            OptionId: 2,
                        },
                        {
                            CustomOptionId: 1,
                            OptionTitle: 'test2',
                            OptionId: 3,
                        },
                    ],
                ),
            ).toBeTruthy();
        });
        it('should return false', function () {
            expect(customOptionsService.isEqualCustomOptions([{ CustomOptionId: 1 }], [{ CustomOptionsId: 99 }])).toBeFalsy();
        });

        it('should return false, when options with once id and different title or amount', function () {
            expect(
                customOptionsService.isEqualCustomOptions(
                    [
                        {
                            CustomOptionId: 1,
                            OptionTitle: 'test1',
                            OptionId: 3,
                        },
                        {
                            CustomOptionId: 1,
                            OptionTitle: 'test2',
                            OptionId: 4,
                        },
                    ],
                    [
                        {
                            CustomOptionId: 1,
                            OptionTitle: 'test1',
                            OptionId: 3,
                        },
                        {
                            CustomOptionId: 1,
                            OptionTitle: 'test3',
                            OptionId: 4,
                        },
                    ],
                ),
            ).toBeFalsy();

            expect(
                customOptionsService.isEqualCustomOptions(
                    [
                        {
                            CustomOptionId: 1,
                            OptionTitle: 'test1',
                            OptionAmount: 2,
                            OptionId: 3,
                        },
                        {
                            CustomOptionId: 1,
                            OptionTitle: 'test2',
                            OptionAmount: 2,
                            OptionId: 4,
                        },
                    ],
                    [
                        {
                            CustomOptionId: 1,
                            OptionTitle: 'test1',
                            OptionAmount: 5,
                            OptionId: 3,
                        },
                        {
                            CustomOptionId: 1,
                            OptionTitle: 'test2',
                            OptionAmount: 5,
                            OptionId: 4,
                        },
                    ],
                ),
            ).toBeFalsy();
        });
    });
});
