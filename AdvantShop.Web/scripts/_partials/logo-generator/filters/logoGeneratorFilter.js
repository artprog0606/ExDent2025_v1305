/*@ngInject*/
function logoGeneratorFontSupportFilter(logoGeneratorService) {
    return function (input, langs) {
        var result = input;

        if (langs != null && langs.length === 1) {
            if (langs.indexOf('cyrillic') === -1 && logoGeneratorService.isCyrillic(input) === true) {
                result = logoGeneratorService.replaceUnsupportOnSymbol(input, 'cyrillic');
            } else if (langs.indexOf('latin') === -1 && logoGeneratorService.isLatin(input) === true) {
                result = logoGeneratorService.replaceUnsupportOnSymbol(input, 'latin');
            }
        }

        return result;
    };
}

export default logoGeneratorFontSupportFilter;
