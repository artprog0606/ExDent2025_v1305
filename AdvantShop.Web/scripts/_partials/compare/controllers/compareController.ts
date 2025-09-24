import type { ICompareService } from '../services/compareService';
import type { CompareCountResponseType, OfferIdType } from '../compare.module';
import type { IController } from 'angular';

export interface ICompareCtrl extends IController {
    isAdded: boolean | null;
    add(offerId: OfferIdType, state: boolean): Promise<CompareCountResponseType>;
    remove(offerId: OfferIdType, state: boolean): Promise<CompareCountResponseType>;
    change(offerId: OfferIdType, state: boolean): Promise<void>;
    checkStatus(offerId: OfferIdType): Promise<void>;
}

export default class CompareCtrl implements ICompareCtrl {
    /* @ngInject */
    constructor(private readonly compareService: ICompareService) {}
    isAdded: boolean | null = null;
    add = async (offerId: OfferIdType, state: boolean): Promise<CompareCountResponseType> => {
        return await this.compareService.add(offerId, state);
    };

    remove = async (offerId: OfferIdType, state: boolean): Promise<CompareCountResponseType> => {
        return await this.compareService.remove(offerId, state);
    };

    change = async (offerId: OfferIdType, state: boolean): Promise<void> => {
        this.isAdded ? await this.add(offerId, state) : await this.remove(offerId, state);
    };

    checkStatus = async (offerId: OfferIdType): Promise<void> => {
        const isAdded = await this.compareService.getStatus(offerId);
        if (isAdded != null) {
            this.isAdded = isAdded;
        }
    };
}
