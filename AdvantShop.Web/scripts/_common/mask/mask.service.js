export default function () {
    const service = this;
    let maskControlConfig = {};

    service.setMaskControlConfig = function (config) {
        maskControlConfig = Object.assign({}, config);
    };

    service.getMaskControlConfig = function () {
        return maskControlConfig;
    };
}
