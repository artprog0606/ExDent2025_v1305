(function (namespace) {
    'use strict';

    var increment = 0;

    // set sticky module and directive
    angular.module(namespace, [])
        .service(namespace + 'Service', function () {
            var service = this,
                storage = {},
                callbacks = {},
                observer;

            service.addGroup = function (group, element, data) {
                storage[group] = storage[group] || [];

                storage[group].push({
                    element: element,
                    data: data
                });
            };

            service.updateDataAll = function () {
                Object.keys(storage).forEach(function (group) {
                    if (storage[group][0].data.state == null || (storage[group][0].data.state.activate == false && storage[group][0].data.state.activeTop == false)) {
                        var offset = service.getOffsetByElement(storage[group][0].element, storage[group][0].data.container);
                        storage[group][0].data.startOffsetTop = offset.top;
                        storage[group][0].data.startOffsetLeft = offset.left;
                    }
                })
            };

            service.addListener = function (eventName, element, callback, group) {
                var isExistElement = false,
                    eventCallback;

                callbacks[eventName] = callbacks[eventName] || {};

                if (callbacks[eventName][group] == null) {

                    eventCallback = function eventCallback() {
                        callbacks[eventName][group].fn.apply(null, [storage[group]].concat(arguments));
                    };

                    element.addEventListener(eventName, eventCallback);

                    callbacks[eventName][group] = {
                        fn: callback,
                        unbind: function () {
                            element.removeEventListener(eventName, eventCallback);
                        }
                    };
                }
            };

            service.clearListeners = function (group) {
                var callbackKeys = Object.keys(callbacks),
                    groupKeys;

                for (var key in callbacks) {
                    if (callbacks.hasOwnProperty(key) === true) {
                        for (var groupKey in callbacks[key]) {
                            if (callbacks[key].hasOwnProperty(groupKey) === true && groupKey === group) {
                                callbacks[key][groupKey].unbind();
                                delete callbacks[key][groupKey];
                                delete storage[groupKey];
                            }
                        }
                    }
                }

                callbackKeys = {};
            }

            service.calc = function (item, container, activeTop, isHorizontalSpy) {
                var top = item.data.top,
                    left = item.data.left,
                    startOffsetTop = item.data.startOffsetTop,
                    isPositionAbs = item.data.isPositionAbs,
                    media = item.data.media,
                    result = { activate: false, activateHorizontal: false, deactivate: false };

                // if activated
                if (activeTop) {
                    activeTop = !isNaN(top) && startOffsetTop < top + container.scrollTop;

                    result.activate = isPositionAbs && activeTop;
                    result.activateHorizontal =  isHorizontalSpy && container.scrollLeft > 0;
                    result.deactivate = !activeTop;
                }
                    // if not activated
                else if (media.matches) {

                    activeTop = !isNaN(top) && startOffsetTop < top + container.scrollTop;
                    result.activate = activeTop;
                }

                result.activeTop = activeTop;

                return result;
            }

            service.getOffsetByElement = function getOffsetByElement(element, parentElementOffset) {
                var rectElement = element.getBoundingClientRect(),
                    rectParent = parentElementOffset.getBoundingClientRect();

                return {
                    top: rectElement.top - rectParent.top,
                    left: rectElement.left - rectParent.left
                };

            };

            service.createObserver = function () {
                if ('MutationObserver' in window && observer == null) {
                    observer = new MutationObserver(function (records) {
                        var isChangesNotStickky = records.some(function (item) {
                            return item.target.children.length === 0 || !item.target.children[0].classList.contains('sticky-wrapper');
                        });

                        if (isChangesNotStickky === true) {
                            service.updateDataAll();
                        }
                    });

                    observer.observe(document.body, {
                        'childList': true,
                        'subtree': true
                    });
                }
            }
        })
        .directive(namespace, ['$timeout', '$parse', namespace + 'Service', function ($timeout, $parse, service) {
            return {
                require: '?^^uibModalWindow',
                link: function (scope, angularElement, attrs, uibModalWindow) {

                    $timeout(function () {
                        var
                            // get element
                            element = angularElement[0],

                            // get document
                            document = element.ownerDocument,

                            // get window
                            window = document.defaultView,

                            // get wrapper
                            wrapper = document.createElement('span'),

                            // cache style
                            style = element.getAttribute('style'),

                            // get options
                            media = window.matchMedia(attrs[namespace + 'Media'] || 'all'),
                            top = $parse(attrs[namespace + 'Top'])(scope),
                            left = $parse(attrs[namespace + 'Top'])(scope) || 0,
                            //position type

                            isPositionAbs = attrs.stickyPositionType === 'absolute',

                            isHorizontalSpy = attrs.stickyHorizontalSpy == null || attrs.stickyHorizontalSpy === 'true',

                            //containers
                            container = attrs.stickySpy ? document.querySelector(attrs.stickySpy) : document.documentElement,

                            //startOffsetTop = element.getBoundingClientRect().top + container.scrollTop,
                            startOffsetTop = 0,

                            startOffsetLeft = 0,

                            group = attrs.stickyGroup ? attrs.stickyGroup : 'single_' + (increment += 1),

                            data,

                            // initialize states
                            activeTop = false,

                            offset = {};

                        var offsetParent = service.getOffsetByElement(element, container);
                        startOffsetTop = offsetParent.top;
                        startOffsetLeft = offsetParent.left;

                        data = getData(element);

                        wrapper.classList.add('sticky-wrapper');

                        function getData(element) {
                            var computedStyle = getComputedStyle(element);

                            return {
                                computedStyle: computedStyle,
                                offsetHeight: element.offsetHeight,
                                wrapper: wrapper,
                                container: container,
                                top: top,
                                left: left,
                                startOffsetTop: startOffsetTop,
                                startOffsetLeft: startOffsetLeft,
                                isPositionAbs: isPositionAbs,
                                media: media
                            };
                        }

                        // activate sticky
                        function activate(items) {

                            // get element computed style
                            var
                                data = items[0].data,
                                computedStyle = data.computedStyle,
                                isPositionAbs = data.isPositionAbs,
                                position = 'top:' + (data.top + (isPositionAbs ? data.container.scrollTop - data.startOffsetTop : 0)),
                                positionLeft = isHorizontalSpy && data.state.activateHorizontal ? 'transform:translateX(' + (isPositionAbs ? 0 : (data.container.scrollLeft) * -1) + 'px)' : '',
                                element,
                                parentNode,
                                nextSibling;

                            for (var i = 0, len = items.length; i < len; i++) {
                                element = items[i].element;
                                wrapper = items[i].data.wrapper;
                                computedStyle = items[i].data.computedStyle;
                                parentNode = element.parentNode;
                                nextSibling = element.nextSibling;

                                if (parentNode !== wrapper) {
                                    // replace element with wrapper containing element
                                    wrapper.appendChild(element);

                                    //if (parentNode && nextSibling) {
                                    parentNode.insertBefore(wrapper, nextSibling);
                                    //}
                                }

                                // style wrapper
                                wrapper.setAttribute('style', 'display:' + computedStyle.display + ';height:' + items[i].data.offsetHeight + 'px;margin:' + computedStyle.margin + (isPositionAbs ? ';position:relative' : '')); // + ';width:' + element.offsetWidth + 'px'

                                // style element
                                element.setAttribute('style', positionLeft + ';margin:0;transition:none;' + position + 'px;width:' + computedStyle.width + ';position:' + (attrs.stickyPositionType || 'fixed') + ';');

                                // configure wrapper
                                wrapper.classList.add('is-' + namespace);
                            }
                        }

                        // deactivate sticky
                        function deactivate(items) {
                            //  var
                            // parentNode = wrapper.parentNode,
                            //nextSibling = wrapper.nextSibling;

                            // replace wrapper with element
                            //parentNode.removeChild(wrapper);

                            // parentNode.insertBefore(element, nextSibling);

                            for (var i = 0, len = items.length; i < len; i++) {
                                element = items[i].element;
                                wrapper = items[i].data.wrapper;

                                // unstyle element
                                if (style === null) {
                                    element.removeAttribute('style');
                                } else {
                                    element.setAttribute('style', style);
                                }

                                wrapper.classList.remove('is-sticky')

                                // unstyle wrapper
                                wrapper.removeAttribute('style');
                            }

                            activeTop = false;
                        }

                        // window scroll listener
                        function onscroll(items, container) {


                            var result = service.calc(items[0], container, activeTop, isHorizontalSpy);

                            items[0].data.state = result;

                            activeTop = result.activeTop;

                            if (result.activate || (activeTop && result.activateHorizontal)) {

                                activate(items);
                            } else if (result.deactivate) {
                                deactivate(items);
                            }
                        }

                        // window resize listener
                        function onresize(items) {
                            // conditionally deactivate sticky
                            if (activeTop) {
                                deactivate(items);
                            }

                            // re-initialize sticky
                            onscroll(items, container);
                        }

                        // destroy listener
                        function ondestroy() {
                            service.clearListeners(group);
                        }

                        // bind listeners

                        service.addGroup(group, element, data);

                        service.addListener('scroll', container !== document.documentElement ? container : window, function (items) {
                            onscroll(items, container);
                        }, group);

                        service.addListener('resize', window, onresize, group);

                        angularElement.on('$destroy', ondestroy);

                        // initialize sticky
                        onscroll([{ element: element, data: data }], container);

                        service.createObserver();
                    });
                }
            };
        }]);
})('sticky');
