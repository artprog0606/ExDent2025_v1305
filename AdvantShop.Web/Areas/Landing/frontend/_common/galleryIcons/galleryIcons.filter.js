const regexpCommon = /(class="([A-Za-z0-9 _-]*)")|(width="\d+")|(height="\d+")/g;
const regexpClass = /class="([A-Za-z0-9 _-]*)"/;
const regexpWidth = /width="\d+"/;
const regexpHeight = /height="\d+"/;

angular.module('galleryIcons').filter('galleryIconsSize', function () {
    return function (value, width, height) {
        let result = value;
        const isSetSizes = width != null || height != null;

        if (isSetSizes) {
            if (width != null) {
                result = result.replace(regexpWidth, ``).replace(`<svg`, `<svg width="${width}"`);
            }

            if (height != null) {
                result = result.replace(regexpHeight, ``).replace(`<svg`, `<svg height="${height}"`);
            }

            result = result.replace(regexpClass, ``);
        } else {
            result = result.replaceAll(regexpCommon, `class="svg-inline--fa fa-arrow-alt-circle-down fa-w-16 fa-fw"`);
        }

        return result;
    };
});
