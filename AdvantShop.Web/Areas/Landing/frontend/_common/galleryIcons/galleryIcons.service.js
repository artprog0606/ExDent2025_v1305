import { dataProcess } from './galleryIcons.webworker.js';

const ng = window.angular;
const fontAwesome = window.FontAwesome;
const fontAwesomeStyles = window.___FONT_AWESOME___.styles;

var MODAL_ID_GAllERY = 'modalPictureLoaderGalleryIcons';
var DATA = fontAwesomeStyles;

var DATA_GROUPS = Object.keys(fontAwesomeStyles).filter(function (groupName) {
    return groupName !== 'fa';
}); //fa == fas
var ITEM_PER_PAGE = 144;
var SOURCE = null;
var SOURCE_KEY = null;
var SOURCE_LENGTH = 0;

var galleryIconsService = function ($http, $q, modalService) {
    var service = this;

    service.showModal = function (options) {
        modalService.renderModal(
            MODAL_ID_GAllERY,
            'Выбрать иконку',
            '<gallery-icons class="gallery-icons" on-select="modalData.onSelect(svg, width, height)"></gallery-cloud>',
            null,
            { destroyOnClose: true, modalClass: 'gallery-icons-modal' },
            { modalData: options },
        );

        modalService.getModal(MODAL_ID_GAllERY).then(function (modal) {
            modal.modalScope.open();
        });
    };

    service.closeModal = function () {
        modalService.close(MODAL_ID_GAllERY);
    };

    service.preloadData = function () {
        if (SOURCE == null) {
            service.prepareSource().then(function (sourceFromWorker) {
                SOURCE = sourceFromWorker;
            });
        }
    };

    service.prepareSource = function () {
        var defer = $q.defer();

        const resultProcess = dataProcess(DATA, DATA_GROUPS);
        var result = {};

        Object.keys(resultProcess).forEach(function (key) {
            for (var i = 0, len = resultProcess[key].length; i < len; i++) {
                result[key] = result[key] || [];
                result[key].push(service.renderSVG(resultProcess[key][i].prefix, key, 'currentColor')[0]);
            }
        });

        return $q.when(result);
    };

    service.getData = function (page, term) {
        return $q.when(SOURCE == null ? service.prepareSource() : SOURCE).then(function (sourceFromWorker) {
            SOURCE = SOURCE || sourceFromWorker;
            SOURCE_KEY = SOURCE_KEY || Object.keys(SOURCE);
            SOURCE_LENGTH = SOURCE_KEY.length;

            var iterationResult = service.iterator(SOURCE, page, term);

            return {
                data: iterationResult.data,
                finish: iterationResult.next === false,
                totalCount: (page - 1) * ITEM_PER_PAGE + iterationResult.itemsCount,
            };
        });
    };

    service.iterator = function (data, page, term) {
        var result = {
            data: {},
            next: true,
            itemsCount: 0,
        };

        var counter = 0;

        var limit = ITEM_PER_PAGE;

        var listKeysForInteration = SOURCE_KEY.slice(ITEM_PER_PAGE * (page - 1), term == null ? ITEM_PER_PAGE * page : SOURCE_LENGTH);

        for (var i = 0, len = listKeysForInteration.length; i < len; i++) {
            if (term == null || (term != null && listKeysForInteration[i].toString().toLowerCase().indexOf(term) !== -1)) {
                result.data[listKeysForInteration[i]] = data[listKeysForInteration[i]];
                counter += data[listKeysForInteration[i]].length;

                if (counter === limit) {
                    break;
                }
            }
        }

        result.next = counter > 0;

        result.itemsCount = counter;

        return result;
    };

    service.renderSVG = function (prefix, iconName, color) {
        var objIcon = fontAwesome.findIconDefinition({
            prefix: prefix,
            iconName: iconName,
        });

        return fontAwesome.icon(objIcon, {
            classes: 'fa-fw',
            styles: { color: color },
        }).html;
    };

    service.translate = function (term) {
        return $http.get('landingInplace/translate', { params: { term: term } }).then(function (response) {
            return response.data;
        });
    };
};

ng.module('galleryIcons').service('galleryIconsService', galleryIconsService);

galleryIconsService.$inject = ['$http', '$q', 'modalService'];
