import richButtonsTemplate from '../templates/richButtons.html';
import inplaceImageButtonsTemplate from '../templates/inplaceImageButtons.html';
import propertiesNewTemplate from '../templates/propertiesNew.html';
import inplaceAutocompleteButtonsTemplate from '../templates/inplaceAutocompleteButtons.html';
import inplaceAutocompleteTemplate from '../templates/inplaceAutocomplete.html';
import pricePanelTemplate from '../templates/pricePanel.html';
import priceButtonsTemplate from '../templates/priceButtons.html';
let richIdIncrement = 0,
    autocompleteIdIncrement = 0,
    imageIdIncrement = 0,
    priceIdIncrement = 0;

//#region inplaceRich
/* @ngInject */
function inplaceRichDirectives($compile, $document, $locale, $q, $timeout, $window, inplaceService, inplaceRichConfig, $parse) {
    return {
        restrict: 'A',
        scope: {
            inplaceRich: '&',
            //inplaceParams: '@',  //view linking function
            inplaceUrl: '@',
            inplaceOnSave: '&',
        },
        controller: 'InplaceRichCtrl',
        controllerAs: 'inplaceRich',
        bindToController: true,
        priority: 2000,
        link: function (scope, element, attrs, ctrl) {
            ctrl.getParams = function () {
                const params = $parse(element.attr('data-inplace-params'))(scope.$parent);
                return params || {};
            };

            var options = angular.extend(angular.copy(inplaceRichConfig), window._LandingCKeditorConfig || {}, ctrl.inplaceRich() || {}, {
                language: $locale.id === 'ru-ru' ? 'ru' : $locale.id === 'uk-ua' ? 'ua' : 'en',
            });

            //get time for ngBind
            $timeout(function () {
                if (attrs.id == null) {
                    attrs.$set('id', 'inplaceRich_' + richIdIncrement);
                    richIdIncrement += 1;
                }

                if (options.editorSimple === true) {
                    options.removePlugins = 'showborders, magicline';
                    options.enterMode = CKEDITOR.ENTER_BR;
                    options.forcePasteAsPlainText = true;
                    element.addClass('inplace-rich-simple');
                }

                element.find('script').removeAttr('type');

                if (element.html()?.trim().length === 0 && attrs.placeholder != null) {
                    element.addClass('inplace-rich-empty');
                    element.text(attrs.placeholder);
                }

                var wrapinitCKeditor;

                ctrl.initCKeditor = function () {
                    element[0].removeEventListener('mouseover', wrapinitCKeditor);

                    element.attr('contenteditable', 'true');

                    options.basicEntities = false;

                    ctrl.editor = CKEDITOR.inline(attrs.id, options);

                    if (ctrl.editor == null) {
                        return;
                    }

                    ctrl.editor.on('instanceReady', function (e) {
                        if (options.editorSimple === true) {
                            $document[0].getElementById(e.editor.id + '_top').style.display = 'none';
                        }
                    });

                    function dialogBind(e) {
                        var dialogName = e.data.name;
                        if (e.editor === ctrl.editor && dialogName === 'aceDialog') {
                            var dialog = e.data.definition.dialog;
                            dialog.on('ok', function () {
                                ctrl.save(this._.editor.aceEditor.getValue());
                            });
                            dialog.on('hide', function () {
                                CKEDITOR.removeListener('dialogDefinition', dialogBind);
                            });
                        }
                    }

                    ctrl.editor.on('focus', function (e) {
                        CKEDITOR.on('dialogDefinition', dialogBind);

                        if (ctrl.editor.getData().trim() === attrs.placeholder) {
                            element.removeClass('inplace-rich-empty');
                            ctrl.editor.setData('');
                        }

                        scope.$apply(function () {
                            var buttons;

                            ctrl.active();

                            if (ctrl.buttonsRendered == null || ctrl.buttonsRendered === false) {
                                buttons = angular.element('<div inplace-rich-buttons="' + attrs.id + '" is-show="inplaceRich.isShow"></div>');

                                document.body.appendChild(buttons[0]);

                                $compile(buttons)(scope);

                                ctrl.buttonsRendered = true;
                            } else {
                                buttons = ctrl.buttons.element;
                            }

                            ctrl.callCallbacks(ctrl.callbacks);
                        });
                    });

                    ctrl.editor.on('blur', function (e) {
                        setTimeout(function () {
                            //задержка чтобы узнать щелкнули ли на кнопки
                            scope.$apply(function () {
                                ctrl.isShow = false;

                                let data = ctrl.editor.getData();

                                if (attrs.placeholder === data) {
                                    data = '';
                                }

                                $q.when(ctrl.clickedButtons === false || ctrl.clickedButtons == null ? ctrl.save(data) : true).then(() => {
                                    ctrl.clickedButtons = false;

                                    if (ctrl.editor.getData().trim().length === 0 && attrs.placeholder != null) {
                                        element.addClass('inplace-rich-empty');
                                        ctrl.editor.setData(attrs.placeholder);
                                    }
                                });
                            });
                        }, 100);
                    });

                    ctrl.editor.on('key', function (event) {
                        var keyCode = event.data.keyCode;

                        switch (keyCode) {
                            case 13: //enter
                                if (options.editorSimple === true) {
                                    var inputTemp = document.createElement('input'),
                                        pos = element[0].getBoundingClientRect();

                                    inputTemp.className = 'inplace-input-fake';
                                    inputTemp.style.top = pos.top + 'px';
                                    inputTemp.style.left = pos.left + 'px';
                                    document.body.appendChild(inputTemp);
                                    inputTemp.focus();
                                    setTimeout(function () {
                                        inputTemp.parentNode.removeChild(inputTemp);
                                    }, 100);

                                    //event.editor.focusManager.blur(false);

                                    event.stop();
                                    event.cancel();
                                }
                                break;
                            //case 27://esc
                            //    event.editor.focusManager.blur(false);
                            //    event.stop();
                            //    event.cancel();
                            //    break;
                        }
                    });
                };

                wrapinitCKeditor = ctrl.initCKeditor;

                element[0].addEventListener('mouseover', wrapinitCKeditor);

                element.addClass('inplace-initialized');

                inplaceService.addRich(attrs.id, ctrl, element[0]);
            });
        },
    };
}

/* @ngInject */
function inplaceRichButtonsDirective(urlHelper, inplaceConfig) {
    return {
        restrict: 'A',
        scope: {
            inplaceRichButtons: '@',
            isShow: '<?',
            onInit: '&',
        },
        controller: 'InplaceRichButtonsCtrl',
        controllerAs: 'inplaceRichButtons',
        bindToController: true,
        replace: true,
        templateUrl: richButtonsTemplate,
    };
}
//#endregion

//#region inplacePrice
/* @ngInject */
function inplacePriceDirective($compile, $document, $locale, $window, inplaceService, productService, domService) {
    return {
        restrict: 'A',
        scope: {
            inplaceParams: '&',
            inplaceUrl: '@',
        },
        controller: 'InplacePriceCtrl',
        controllerAs: 'inplacePrice',
        bindToController: true,
        link: function (scope, element, attrs, ctrl) {
            var priceNumber, init;

            ctrl.product = productService.getProduct();

            element[0].classList.add('inplace-price-container');
            element[0].classList.add('inplace-offset');

            init = function (event) {
                var priceCurrentBlock = domService.closest(event.target, '.price-current'),
                    priceUnknowBlock = domService.closest(event.target, '.price-unknow'),
                    pricOldBlock = domService.closest(event.target, '.price-old'),
                    pricDiscountPercentBlock = domService.closest(event.target, '.price-discount-percent'),
                    el,
                    options = { language: $locale.id === 'ru-ru' ? 'ru' : $locale.id === 'uk-ua' ? 'ua' : 'en' };

                if (priceCurrentBlock != null) {
                    el = priceCurrentBlock;
                    ctrl.type = 'price';
                } else if (priceUnknowBlock != null) {
                    el = priceUnknowBlock;
                    ctrl.type = 'price';
                } else if (pricOldBlock != null) {
                    el = pricOldBlock;
                    ctrl.type = 'price';
                } else if (pricDiscountPercentBlock != null) {
                    el = pricDiscountPercentBlock;
                    ctrl.type = 'discountPercent';
                }

                if (el == null || (ctrl.needReinit[ctrl.type] === false && priceNumber != null)) {
                    return;
                }

                ctrl.needReinit[ctrl.type] = true;

                priceNumber = el.querySelector('.price-number') || priceUnknowBlock || pricDiscountPercentBlock;

                if (priceNumber == null) {
                    return;
                }

                if (priceNumber.id == null || priceNumber.id.length === 0) {
                    priceNumber.id = 'inplacePrice_' + priceIdIncrement;
                    priceIdIncrement += 1;
                }

                if (CKEDITOR.instances[priceNumber.id] != null) {
                    return;
                }

                options.removePlugins = ' showborders, magicline';
                options.enterMode = CKEDITOR.ENTER_BR;
                options.forcePasteAsPlainText = true;

                priceNumber.classList.add('inplace-rich-simple');

                priceNumber.setAttribute('contenteditable', 'true');

                ctrl.editor = CKEDITOR.inline(priceNumber.id, options);

                ctrl.editor.on('instanceReady', function (e) {
                    $document[0].getElementById(e.editor.id + '_top').style.display = 'none';
                });

                ctrl.editor.on('focus', function (e) {
                    if (
                        priceNumber.classList.contains('price-unknown') === true &&
                        ctrl.convertToFloat(ctrl.editor.getData({ format: 'text' })) == null
                    ) {
                        ctrl.editor.setData('0');
                    }

                    scope.$apply(function () {
                        var buttons, panel;

                        ctrl.active();

                        if (ctrl.buttonsRendered == null || ctrl.buttonsRendered === false) {
                            buttons = angular.element('<div data-inplace-price-buttons="' + priceNumber.id + '"></div>');

                            document.body.appendChild(buttons[0]);

                            $compile(buttons)(scope);

                            panel = angular.element('<div data-inplace-price-panel="' + priceNumber.id + '"></div>');

                            document.body.appendChild(panel[0]);

                            $compile(panel)(scope);

                            ctrl.buttonsRendered = true;
                        } else {
                            buttons = ctrl.buttons.element;
                            panel = ctrl.panel.element;
                        }

                        ctrl.callCallbacks(ctrl.callbacks);
                    });
                });

                ctrl.editor.on('blur', function (e) {
                    setTimeout(function () {
                        //задержка чтобы узнать щелкнули ли на кнопки
                        scope.$apply(function () {
                            ctrl.isShow = false;

                            if (ctrl.clickedButtons === false || ctrl.clickedButtons == null) {
                                ctrl.save();
                            }

                            ctrl.clickedButtons = false;
                        });
                    }, 100);
                });

                ctrl.editor.on('key', function (event) {
                    var keyCode = event.data.keyCode;

                    if (keyCode === 13) {
                        var inputTemp = document.createElement('input'),
                            pos = element[0].getBoundingClientRect();

                        inputTemp.className = 'inplace-input-fake';
                        inputTemp.style.top = pos.top + 'px';
                        inputTemp.style.left = pos.left + 'px';
                        document.body.appendChild(inputTemp);
                        inputTemp.focus();
                        setTimeout(function () {
                            inputTemp.parentNode.removeChild(inputTemp);
                        }, 100);

                        //event.editor.focusManager.blur(false);

                        event.stop();
                        event.cancel();
                    } else {
                        var current = ctrl.convertToFloat(ctrl.editor.getData());

                        if (current === null) {
                            priceNumber.classList.add('inplace-price-error');
                        } else {
                            priceNumber.classList.remove('inplace-price-error');
                        }
                    }
                });

                inplaceService.addInplacePrice(priceNumber.id, ctrl, priceNumber);

                element.addClass('inplace-initialized');
            };

            element[0].addEventListener('mouseover', function (event) {
                var el = this;

                scope.$apply(function () {
                    init(event, el);
                });
            });
        },
    };
}

function inplacePriceButtonsDirective() {
    return {
        restrict: 'A',
        scope: {
            inplacePriceButtons: '@',
            onInit: '&',
        },
        controller: 'InplacePriceButtonsCtrl',
        controllerAs: 'inplacePriceButtons',
        bindToController: true,
        replace: true,
        templateUrl: priceButtonsTemplate,
    };
}

//#region inplacePrice
function inplacePricePanelDirective() {
    return {
        restrict: 'A',
        scope: {
            inplacePricePanel: '@',
            onInit: '&',
        },
        controller: 'InplacePricePanelCtrl',
        controllerAs: 'inplacePricePanel',
        templateUrl: pricePanelTemplate,
        replace: true,
        bindToController: true,
    };
}
//#endregion

//#region modal
function inplaceModalDirective() {
    return {
        restrict: 'A',
        scope: {
            inplaceParams: '&',
            inplaceUrl: '@',
        },
        controller: 'InplaceModalCtrl',
        controllerAs: 'inplaceModal',
        bindToController: true,
        link: function (scope, element, attrs, ctrl) {
            element[0].addEventListener('click', function (event) {
                event.preventDefault();
                scope.$apply(ctrl.modalOpen);
            });
            element.addClass('inplace-initialized');
        },
    };
}
//#endregion

//#region inplaceAutocomplete
/* @ngInject */
function inplaceAutocompleteDirective($compile, $document, $window, inplaceService) {
    return {
        restrict: 'A',
        scope: {
            inplaceParams: '&',
            autocompleteParams: '&inplaceAutocomplete',
            inplaceAutocompleteSelectorBlock: '@',
        },
        controller: 'InplaceAutocompleteCtrl',
        controllerAs: 'inplaceAutocomplete',
        bindToController: true,
        templateUrl: inplaceAutocompleteTemplate,
        replace: true,
        transclude: true,
        link: function (scope, element, attrs, ctrl, transclude) {
            var input = element[0].querySelector('input'),
                transcludeEl = transclude()[0],
                setPosition;

            if (transcludeEl) {
                ctrl.value = transcludeEl.textContent;
            }

            setPosition = function (buttons, rect) {
                buttons.css({
                    top: $window.pageYOffset + rect.bottom,
                    right: $document[0].body.clientWidth - rect.right,
                });
            };

            input.addEventListener('focus', function () {
                var buttons;

                element[0].classList.add('inplace-autocomplete-focus');

                scope.$apply(function () {
                    ctrl.startContent = ctrl.value;

                    ctrl.active();

                    if (ctrl.buttonsRendered == null) {
                        buttons = angular.element('<div inplace-autocomplete-buttons="' + attrs.id + '"></div>');

                        setPosition(buttons, element[0].getBoundingClientRect());

                        document.body.appendChild(buttons[0]);

                        $compile(buttons)(scope);

                        ctrl.buttonsRendered = true;
                    }
                });
            });

            input.addEventListener('blur', function () {
                element[0].classList.remove('inplace-autocomplete-focus');

                setTimeout(function () {
                    //задержка чтобы узнать щелкнули ли на кнопки
                    scope.$apply(function () {
                        ctrl.isShow = false;

                        if (ctrl.clickedButtons === false || ctrl.clickedButtons == null) {
                            ctrl.save();
                        }

                        ctrl.clickedButtons = false;
                    });
                }, 100);
            });

            input.addEventListener('keyup', function (event) {
                var inputTemp, pos;

                if (event.keyCode === 13) {
                    element[0].classList.remove('inplace-autocomplete-focus');

                    inputTemp = document.createElement('input');
                    pos = element[0].getBoundingClientRect();

                    inputTemp.className = 'inplace-input-fake';
                    inputTemp.style.top = pos.top + 'px';
                    inputTemp.style.left = pos.left + 'px';
                    document.body.appendChild(inputTemp);
                    inputTemp.focus();

                    setTimeout(function () {
                        inputTemp.parentNode.removeChild(inputTemp);
                    }, 100);
                }
            });

            if (attrs.id == null) {
                attrs.$set('id', 'inplaceAutocomplete_' + autocompleteIdIncrement);
                autocompleteIdIncrement += 1;
            }

            inplaceService.addInplaceAutocomplete(attrs.id, ctrl);

            element.addClass('inplace-initialized');
        },
    };
}

function inplaceAutocompleteButtonsDirective() {
    return {
        restrict: 'A',
        scope: {
            inplaceAutocompleteButtons: '@',
        },
        controller: 'InplaceAutocompleteButtonsCtrl',
        controllerAs: 'inplaceAutocompleteButtons',
        bindToController: true,
        replace: true,
        templateUrl: inplaceAutocompleteButtonsTemplate,
    };
}

//#endregion

//#region inplaceProperties
function inplacePropertiesNewDirective() {
    return {
        restrict: 'A',
        scope: {
            productId: '@',
        },
        controller: 'InplacePropertiesNewCtrl',
        controllerAs: 'inplacePropertiesNew',
        bindToController: true,
        replace: true,
        templateUrl: propertiesNewTemplate,
    };
}
//#endregion

//#region inplaceImage
/* @ngInject */
function inplaceImageDirective($compile, $document, $parse, $timeout, $window, inplaceService) {
    return {
        restrict: 'A',
        require: ['inplaceImage', '^?carousel', '?^productViewItem', '?^zoomer'],
        scope: true,
        controller: 'InplaceImageCtrl',
        controllerAs: 'inplaceImage',
        bindToController: true,
        replace: true,
        link: function (scope, element, attrs, ctrls) {
            var inplaceImage = ctrls[0],
                carousel = ctrls[1],
                productViewItem = ctrls[2],
                zoomer = ctrls[3],
                documentProduct = document.querySelector('[data-ng-controller="ProductCtrl as product"]'),
                renderButtons,
                mouseenter,
                mouseleave;

            if (attrs.id == null) {
                attrs.$set('id', 'inplaceImage_' + imageIdIncrement);
                imageIdIncrement += 1;
            }

            if (inplaceService.getInplaceImage(attrs.id)) {
                return;
            }

            inplaceImage.carousel = carousel;
            inplaceImage.productViewItem = productViewItem;
            inplaceImage.product = documentProduct != null ? angular.element(documentProduct).controller() : null; //get controller product on details page;
            inplaceImage.inplaceParams = $parse(attrs.inplaceParams)(scope);
            inplaceImage.inplaceUrl = attrs.inplaceUrl;
            inplaceImage.inplaceImageButtonsVisible = angular.extend(
                { add: true, update: true, delete: true, permanentVisible: false },
                new Function('return ' + attrs.inplaceImageButtonsVisible)() || {},
            );
            inplaceImage.accept = attrs.accept;

            renderButtons = function (element) {
                var buttons = angular.element(
                    `<div inplace-image-buttons="${attrs.id}" accept="$parent.inplaceImage.accept" is-buttons-show="$parent.inplaceImage.showButtons"></div>`,
                );

                element.parent().append(buttons);

                $compile(buttons)(scope.$new());
            };

            mouseenter = function () {
                element[0].classList.add('inplace-image-focus');

                inplaceImage.active();

                if (inplaceImage.buttonsRendered == null) {
                    renderButtons(element);
                }

                scope.$apply();
            };

            mouseleave = function () {
                element[0].classList.remove('inplace-image-focus');

                inplaceImage.isActive = false;

                $timeout(function () {
                    if (
                        inplaceImage.buttons != null &&
                        (inplaceImage.buttons.isHoverButtons == false || inplaceImage.buttons.isHoverButtons == null) &&
                        inplaceImage.inplaceImageButtonsVisible.permanentVisible !== true
                    ) {
                        inplaceImage.showButtons = false;
                    }
                }, 100);
            };

            if (inplaceImage.inplaceImageButtonsVisible.permanentVisible === true) {
                renderButtons(element);
            }

            element.on('$destroy', function () {
                inplaceImage.buttonsRendered = null;

                if (zoomer != null) {
                    //bind to zoomer blocks
                    element[0].parentNode.removeEventListener('mouseenter', mouseenter);
                    element[0].parentNode.removeEventListener('mouseleave', mouseleave);
                } else {
                    element[0].removeEventListener('mouseenter', mouseenter);
                    element[0].removeEventListener('mouseleave', mouseleave);
                }

                inplaceService.removeInplaceImage(attrs.id);
            });

            inplaceService.addInplaceImage(attrs.id, inplaceImage);

            if (zoomer != null) {
                //bind to zoomer blocks
                element[0].parentNode.parentNode.addEventListener('mouseenter', mouseenter);
                element[0].parentNode.parentNode.addEventListener('mouseleave', mouseleave);
            } else {
                element[0].addEventListener('mouseenter', mouseenter);
                element[0].addEventListener('mouseleave', mouseleave);
            }

            element.addClass('inplace-initialized');
        },
    };
}
/* @ngInject */
function inplaceImageButtonsDirective($parse, $timeout) {
    return {
        restrict: 'A',
        scope: {
            inplaceImageButtons: '@',
            isButtonsShow: '<?',
            accept: '<?',
        },
        controller: 'InplaceImageButtonsCtrl',
        controllerAs: 'inplaceImageButtons',
        bindToController: true,
        replace: true,
        templateUrl: inplaceImageButtonsTemplate,
        link: function (scope, element, attrs, ctrl) {
            ctrl.element = element;

            if (ctrl.inplaceImage.inplaceImageButtonsVisible.permanentVisible !== true) {
                element[0].addEventListener('mouseenter', function () {
                    scope.$apply(function () {
                        ctrl.isHoverButtons = true;
                    });
                });

                element[0].addEventListener('mouseleave', function () {
                    scope.$apply(function () {
                        ctrl.isHoverButtons = false;
                    });

                    setTimeout(function () {
                        scope.$apply(function () {
                            if (ctrl.inplaceImage.isActive == false || ctrl.inplaceImage.isActive == null) {
                                ctrl.inplaceImage.showButtons = false;
                            }
                        });
                    }, 100);
                });
            } else {
                ctrl.inplaceImage.showButtons = true;
            }

            scope.$watch('inplaceImageButtons.isButtonsShow', (newVal, oldVal) => {
                if (newVal === true) {
                    $timeout(() => {
                        ctrl.inplaceImage.setPositionButtons(element);
                    });
                }
            });

            if (attrs.inplaceImageButtonsOnLoad) {
                $parse(attrs.inplaceImageButtonsOnLoad)(scope, { buttonsElement: element, buttonsCtrl: ctrl });
            }
        },
    };
}
//#endregion

export {
    inplaceRichDirectives,
    inplaceRichButtonsDirective,
    inplacePriceDirective,
    inplacePriceButtonsDirective,
    inplacePricePanelDirective,
    inplaceModalDirective,
    inplaceAutocompleteDirective,
    inplaceAutocompleteButtonsDirective,
    inplacePropertiesNewDirective,
    inplaceImageDirective,
    inplaceImageButtonsDirective,
};
