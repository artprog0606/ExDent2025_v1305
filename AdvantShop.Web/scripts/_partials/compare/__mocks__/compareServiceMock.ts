import type { ICompareService } from '../services/compareService';
import type { CompareCountResponseType, CompareCountType, OfferIdType } from '../compare.module';
import { getCompareData, getCompareResponseData, getCompareResponseStatus } from './compare';
import type { IQService, IPromise } from 'angular';
import { ICompareCtrl } from '../controllers/compareController';

export const CompareService = ($q: IQService): ICompareService => ({
    add(offerId: OfferIdType, state: boolean) {
        return $q<CompareCountResponseType>((resolve) => resolve(getCompareResponseData()));
    },
    remove(offerId: OfferIdType, state: boolean): IPromise<CompareCountResponseType> {
        return $q<CompareCountResponseType>((resolve) => resolve(getCompareResponseData()));
    },
    removeAll(): IPromise<CompareCountResponseType> {
        return $q<CompareCountResponseType>((resolve) => resolve(getCompareResponseData()));
    },
    removeCompareScope: (id: OfferIdType, ctrl: ICompareCtrl) => {},
    getCountObj(): CompareCountType {
        return getCompareData();
    },
    getStatus(offerId: OfferIdType): IPromise<boolean> {
        return $q<boolean>((resolve) => resolve(getCompareResponseStatus()));
    },
    addCompareScope(id: OfferIdType, ctrl: ICompareCtrl) {},
    changeCompareControlState(id: OfferIdType, state: boolean) {},
});
