import { faker } from '@faker-js/faker';
import type { CompareCountResponseType, CompareCountType, OfferIdType } from '../compare.module';
import type { ICompareCtrl } from '../controllers/compareController';

export const getOfferId = (): number => {
    return faker.number.int({
        min: 1,
        max: 1000,
    });
};

export const getCompareResponseData = (): CompareCountResponseType => {
    return {
        Count: faker.number.int({
            min: 1,
            max: 1000,
        }),
        isComplete: true,
    };
};

export const getCompareData = (): CompareCountType => {
    return {
        count: faker.number.int({
            min: 1,
            max: 1000,
        }),
        isComplete: true,
    };
};

export const getCompareResponseErrorText = (): string => {
    return faker.hacker.phrase();
};

export const getCompareResponseStatus = (): boolean => {
    return faker.datatype.boolean(Math.random());
};

export const getCompareScope = (): ICompareCtrl => {
    return {
        isAdded: faker.datatype.boolean(),
        add(offerId: OfferIdType, state: boolean) {
            return Promise.resolve(getCompareResponseData());
        },
        remove(offerId: OfferIdType, state: boolean) {
            return Promise.resolve(getCompareResponseData());
        },
        change(offerId: OfferIdType, state: boolean) {
            return Promise.resolve();
        },
        checkStatus(offerId: OfferIdType) {
            return Promise.resolve();
        },
    };
};
