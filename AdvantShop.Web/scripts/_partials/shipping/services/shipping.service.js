const eventReady = `shippingTemplateReady`;
const dataSet = new Set();
class ShippingService {
    whenTemplateReady(scope, fn) {
        scope.$on(eventReady, (event) => {
            fn(event);
        });
    }

    fireTemplateReady(scope, value) {
        scope.$emit(eventReady, value);
    }

    saveTemplateState(templateUrl) {
        dataSet.add(templateUrl);
    }

    isTemplateReady(templateUrl) {
        return dataSet.has(templateUrl);
    }
}

export default ShippingService;
