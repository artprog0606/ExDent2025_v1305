import { IScope, IComponentController } from 'angular';
import { IAbstractShippingOption, IBaseShippingOption, IGeoModeDeliveries } from '../types';

interface IShippingPointsListCtrl extends IComponentController {
    getShippingPoints(): Promise<{ options: IAbstractShippingOption[] }>;
    selectShippingPoint(shippingPoint: IAbstractShippingOption, point?: IBaseShippingOption): void;
}

class ShippingPointsListCtrl implements IShippingPointsListCtrl {
    items: IAbstractShippingOption[] = [];
    onChange: (({ shippingPoint, point }: { shippingPoint: IAbstractShippingOption; point?: IBaseShippingOption }) => void) | undefined;
    selectedShippingPoint: IAbstractShippingOption | undefined;
    isLoadedShippingPoints: boolean = false;
    /* @ngInject */
    constructor(
        private readonly checkoutService: any,
        private readonly $scope: IScope,
    ) {
        this.checkoutService = checkoutService;
        this.$scope = $scope;
    }

    async $onInit() {
        try {
            const { options } = await this.getShippingPoints().finally(() => {
                this.isLoadedShippingPoints = true;
            });
            if (options != null) {
                this.items = options;
                this.$scope.$apply();
            }
        } catch (e) {
            this.isLoadedShippingPoints = true;
            console.error(e.message);
        }
    }

    selectShippingPoint(shippingPoint: IAbstractShippingOption, point?: IBaseShippingOption) {
        this.selectedShippingPoint = shippingPoint;
        if (point) {
            this.selectedShippingPoint.SelectedPoint = point;
        }
        if (this.onChange) {
            this.onChange({ shippingPoint, point: point });
        }
    }

    async getShippingPoints(): Promise<IGeoModeDeliveries> {
        return await this.checkoutService.getGeoModeDeliveries('self-delivery');
    }
}

export default ShippingPointsListCtrl;
