/**
 * Based on https://github.com/gregjacobs/Autolinker.js/
 */

(function () {
    'use strict';


    function getAutolinkerOptions(options, data){
        return angular.extend({}, options, {
            stripPrefix: false,
            stripTrailingSlash: false,
            replaceFn: function (match) {
                var regexp = new RegExp('<span class=".*autolinker-ignore.*">' + match.matchedText + '\s*|(&nbsp;)*<\/span>');
                return regexp.test(data) === false;
            }
        });
    };

    CKEDITOR.plugins.add("autolinker", {
        init: function (editor) {

            var pathPlugin = this.path,
                bookmark,
                prevData,
                keyupTimer,
                currentElement;

            function autolink(data, callback) {
                var newData = Autolinker.link(data, getAutolinkerOptions(null, data));

                if (data !== newData) {
                    //editor.setData(data);
                    editor.setData('', {
                        callback: function () {
                            editor.insertHtml(newData);

                            if (callback != null) {
                                callback(newData)
                            }
                        }
                    });
                }
            };

            function updateData(editor) {

                var data = editor.getData();

                storeCursorLocation(editor);

                autolink(data, function (newData) {
                    prevData = newData;
                    restoreCursorLocation(editor);
                });

            }

            var storeCursorLocation = function (editor) {
                bookmark = editor.getSelection().createBookmarks2();
            };

            var restoreCursorLocation = function (editor) {
                editor.getSelection().selectBookmarks(bookmark);
            };

            editor.addContentsCss(pathPlugin + 'autolinker.css');

            CKEDITOR.scriptLoader.load([pathPlugin + 'Autolinker.min.js']);

            editor.on('instanceReady', function () {

                prevData = editor.getData();

                editor.document.$.addEventListener('mouseleave', function (event) {
                    var data = editor.getData();
                    if (data !== prevData) {
                        updateData(editor);
                    }
                });
            });

            editor.on('afterCommandExec', function (event) {
                var elementDirty,
                    element;

                if (event.data.name === 'unlink') {

                    elementDirty = editor.getSelection().getStartElement();

                    if (elementDirty != null) {
                        element = elementDirty.$;

                        element.innerHTML = '<span class="autolinker-ignore">' + element.innerHTML + '</span>';
                    }
                }
            });

            editor.on('key', function (event) {
                var data = editor.getData();

                if (data !== prevData && [13, 32, 9].indexOf(event.data.keyCode) !== -1) {
                    updateData(editor);
                }
            });

            editor.on('paste', function (event) {
                var data = event.data.dataValue;

                if (event.data.dataTransfer.getTransferType(editor) == CKEDITOR.DATA_TRANSFER_INTERNAL) {
                    return;
                }

                // If we found "<" it means that most likely there's some tag and we don't want to touch it.
                if (data.indexOf('<') > -1) {
                    return;
                }

                // http://dev.ckeditor.com/ticket/13419
                //data = data.replace(validUrlRegex, '<a href="' + data.replace(doubleQuoteRegex, '%22') + '">$&</a>');
                data = Autolinker.link(data, getAutolinkerOptions(data));

                // If link was discovered, change the type to 'html'. This is important e.g. when pasting plain text in Chrome
                // where real type is correctly recognized.
                if (data != event.data.dataValue) {
                    event.data.type = 'html';
                }

                event.data.dataValue = data;
            });
        }
    });

})();
