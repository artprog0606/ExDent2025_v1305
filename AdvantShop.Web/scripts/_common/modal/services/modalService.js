(function (ng) {
    'use strict';

    angular.module('modal').service(
        'modalService',
        /* @ngInject*/
        function ($compile, $rootScope, $q, modalDefaultOptions) {
            var modals = {},
                promises = {},
                queue = [],
                working = true,
                modalService = this;

            modalService.stopWorking = function () {
                working = false;
            };

            modalService.startWorking = function () {
                working = true;

                if (queue.length > 0) {
                    modalService.open(queue[0].id);
                }
            };

            modalService.isWorking = function () {
                return working === true;
            };

            modalService.checkQueue = function (modal) {
                return queue.length === 0 || modalService.existInQueue(modal);
            };

            modalService.existInQueue = function (modal) {
                return queue.indexOf(modal) !== -1;
            };

            modalService.addQueue = function (modal) {
                queue.push(modal);
            };

            modalService.removeItemQueue = function (modal) {
                var index = queue.indexOf(modal);

                if (index !== -1) {
                    queue.splice(index, 1);
                }

                if (queue.length > 0) {
                    modalService.open(queue[queue.length - 1].id);
                }
            };

            modalService.open = function (modalId, skipQueue, modalDataAdditional) {
                if (angular.isDefined(modals[modalId])) {
                    modals[modalId].modalScope.open(skipQueue, modalDataAdditional);
                }
            };

            modalService.close = function (modalId) {
                if (angular.isDefined(modals[modalId])) {
                    modals[modalId].modalScope.close();
                }
            };

            modalService.destroy = function (modalId) {
                if (angular.isDefined(modals[modalId])) {
                    modals[modalId].modalScope.destroy();
                    modalService.removeFromStorage(modalId);
                }
            };

            modalService.setVisibleFooter = function (modalId, visible) {
                if (angular.isDefined(modals[modalId])) {
                    modals[modalId].modalScope.setVisibleFooter(visible);
                }
            };

            modalService.addStorage = function (modalId, modalElement, modalScope) {
                modals[modalId] = {
                    modalElement: modalElement,
                    modalScope: modalScope,
                };

                if (promises[modalId] != null) {
                    promises[modalId].resolve(modals[modalId]);
                    delete promises[modalId];
                }

                return modals[modalId];
            };

            modalService.hasModal = function (modalId) {
                return modals[modalId] != null;
            };

            modalService.getModal = function (modalId) {
                var defer = $q.defer();

                if (modals[modalId] != null) {
                    defer.resolve(modals[modalId]);
                } else {
                    promises[modalId] = defer;
                }

                return defer.promise;
            };

            modalService.renderAttrubutes = function (attrubutes) {
                var arrStrings = [],
                    tempString,
                    keyFormatted;

                for (var key in attrubutes) {
                    if (Object.prototype.hasOwnProperty.call(attrubutes, key)) {
                        keyFormatted = key.replace(/[A-Z]/g, function (str) {
                            return '-' + str.toLowerCase();
                        });

                        tempString = [keyFormatted, '=', '"', attrubutes[key], '"'].join('');
                        arrStrings.push(tempString);
                    }
                }

                return arrStrings.join(' ');
            };
            /**
             *
             * @param {string} modalId Unique id for modal
             * @param {string} modalHeader String as html for header
             * @param {string} modalContent String as html for content
             * @param {string} modalFooter String as html for footer
             * @param {object} options Options for modal
             * @param {$scope} parentScope Parent scope for compile
             * @returns {JqueryElement} Form Element
             */
            modalService.renderModal = function (modalId, modalHeader, modalContent, modalFooter, options, parentScope) {
                if (angular.isUndefined(modalId) || modalId.length === 0) {
                    throw Error('Modal "id" is required');
                }

                if (angular.isDefined(modals[modalId])) {
                    return modals[modalId].modalElement;
                }

                var parentScopeAsAngularScope = parentScope != null && parentScope instanceof $rootScope.constructor;

                options = options || {};

                options = angular.extend({}, modalDefaultOptions, options, { id: modalId });

                var scope = parentScopeAsAngularScope ? parentScope : $rootScope.$new(),
                    blockStart = ['<modal-control ', modalService.renderAttrubutes(options), '>'],
                    header = modalHeader != null ? ['<div class="modal-header" data-modal-header>', modalHeader, '</div>'] : [' '],
                    content = modalContent != null ? ['<div class="modal-content">', modalContent, '</div>'] : [' '],
                    footer = modalFooter != null ? ['<div class="modal-footer" data-modal-footer>', modalFooter, '</div>'] : [' '],
                    blockEnd = ['</modal-control>'],
                    compileString = blockStart.join('') + header.join('') + footer.join('') + content.join('') + blockEnd.join(''),
                    modalElement;

                modalElement = angular.element(compileString).css('z-index', modalService.getNewZIndex(options.zIndex));

                angular.element(document.body).append(modalElement);

                if (parentScope != null && parentScopeAsAngularScope === false) {
                    angular.extend(scope, parentScope);
                }

                return $compile(modalElement)(scope);
            };

            modalService.getNewZIndex = function (zIndex) {
                return (zIndex != null ? zIndex : modalDefaultOptions.zIndex) * (queue.length + 1);
            };

            modalService.removeFromStorage = function (modalId) {
                delete modals[modalId];
            };
        },
    );
})(angular);
