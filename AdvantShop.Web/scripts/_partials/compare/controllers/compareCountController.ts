import type { IController } from 'angular';
import type { ICompareService } from '../services/compareService';
import { CompareCountType } from '../compare.module';

export interface ICompareCountCtrl extends IController {}

export default class CompareCountCtrl implements ICompareCountCtrl {
    countObj: CompareCountType | null = null;
    /*@ngInject*/
    constructor(private readonly compareService: ICompareService) {
        this.countObj = compareService.getCountObj();
    }
}
