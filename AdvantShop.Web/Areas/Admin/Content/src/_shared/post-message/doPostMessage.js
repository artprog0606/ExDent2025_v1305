(function (document, window) {
    'use strict';

    var deferList = {};

    document.addEventListener('DOMContentLoaded', function () {
        var callbacks = {
            scrollTop: function () {
                window.scrollTo(0, 0);
            },
            openChat: function () {
                /* eslint-disable no-undef*/
                jivo_api.open();
                setTimeout(function () {
                    document.querySelector('#jivo-iframe-container').classList.add('shown');
                }, 30);
            },
            //Модальное окно (затемнение)
            removeModalBackground: function () {
                var iframeWrap = document.querySelector('.js-iframe-wrap');
                var iframe = document.querySelector('iframe');
                var modalBackground = document.querySelectorAll('.post-modal-background');

                if (iframeWrap !== null && modalBackground !== null && modalBackground.length > 0 && iframe !== null) {
                    if (modalBackground.length === 1) {
                        iframe.classList.remove('iframe');
                    }

                    iframeWrap.removeChild(modalBackground[0]);
                }
            },
            //Открыть модальное окно
            openModal: function () {
                var el = document.createElement('div');
                var iframe = document.querySelector('iframe');
                var iframeWrap = document.querySelector('.js-iframe-wrap');
                if (iframeWrap !== null && iframe !== null) {
                    iframe.classList.add('iframe');
                    el.classList.add('post-modal-background');
                    //el.addEventListener('click', function removeBackgroundModal(e) {
                    //    doPostMessage(iframe, 'closeModal');
                    //    iframe.classList.remove('iframe');
                    //    iframeWrap.removeChild(el);
                    //    el.removeEventListener('click', removeBackgroundModal);
                    //});

                    iframeWrap.appendChild(el);
                }
            },
            //Высота Iframe
            iframeHeight: function (postData) {
                //doPostMessage(window, 'iframeHeight');

                var iframe = document.querySelector('iframe');

                if (iframe !== null) {
                    iframe.style.height = postData.height + 'px';
                }
            },
            openSupportModel: function () {
                var iframe = document.querySelector('iframe');

                if (iframe !== null) {
                    doPostMessage(
                        iframe,
                        JSON.stringify({
                            name: 'modalPosition',
                            windowScrollHeight: window.pageYOffset,
                        }),
                    );
                }
            },
            tariffs: function () {
                var iframe = document.querySelector('iframe');

                if (iframe !== null) {
                    iframe.addEventListener('load', function () {
                        window.scrollTo(0, 0);
                    });
                }
            },
            getPageYOffset: function () {
                var iframe = document.querySelector('iframe');

                if (iframe !== null) {
                    doPostMessage(
                        iframe,
                        JSON.stringify({
                            name: 'pageYOffset',
                            pageYOffset: window.pageYOffset,
                        }),
                    );
                }
            },
        };

        window.addEventListener(
            'message',
            function (event) {
                var postData = getDataAsJSON(event.data);

                var postDataIsString = postData == null && typeof event.data === 'string' && event.data != null && event.data.length > 0;

                if (postDataIsString === false && postData != null && postData.name != null && callbacks[postData.name] != null) {
                    callbacks[postData.name](postData);
                } else if (postDataIsString === true && callbacks[event.data] != null) {
                    callbacks[event.data]();
                }
                if (postData != null && postData.name != null) {
                    checkDefer(postData.name, postData);
                } else if (postDataIsString === true) {
                    checkDefer(event.data);
                }
            },
            false,
        );

        doPostMessage(document.querySelector('iframe'), 'readyPost');

        doPostMessageWait('iframeHeight', callbacks['iframeHeight']);

        window.addEventListener('resize', function () {
            doPostMessage(document.querySelector('iframe'), 'readyPost');
        });
    });

    function getDataAsJSON(data) {
        var result;

        try {
            result = JSON.parse(data);
        } catch (e) {
            result = null;
        }

        return result;
    }

    function getEl(element) {
        return element != null && typeof element === 'string' ? document.querySelector(element) : element;
    }

    function doPostMessage(otherWindow, message, targetOrigin) {
        var origin = targetOrigin || '*';
        var obj = getEl(otherWindow);

        if (obj != null) {
            //window может быть iframe
            (obj.contentWindow || obj).postMessage(message, origin);
        }
    }

    function doPostMessageWait(message, callback) {
        if (deferList[message] != null) {
            callback();
        } else {
            deferList[message] = deferList[message] || { callbackList: [] };
            deferList[message].callbackList.push(callback);
        }
    }

    function checkDefer(messageName, data) {
        if (deferList[messageName] != null && deferList[messageName].callbackList != null && deferList[messageName].callbackList.length > 0) {
            deferList[messageName].callbackList.forEach(function (callback) {
                callback(data);
            });
        } else {
            deferList[messageName] = { name: messageName };
        }
    }

    function deleteCallback(message) {
        delete deferList[message];
    }

    window.doPostMessage = doPostMessage;
    window.doPostMessageWait = doPostMessageWait;
    window.doPostMessageDeleteCallback = deleteCallback;
})(document, window);
