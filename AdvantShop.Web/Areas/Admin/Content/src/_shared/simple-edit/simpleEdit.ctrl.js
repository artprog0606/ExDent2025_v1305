(function (ng) {
    'use strict';

    var SimpleEditCtrl = function ($document, $window) {
        var ctrl = this;

        ctrl.getContent = function () {
            return ctrl.content;
        };

        ctrl.addContent = function (element) {
            ctrl.content = element;
        };

        ctrl.addTrigger = function (element) {
            ctrl.trigger = element;
        };

        ctrl.saveAsOldValue = function (oldValue) {
            ctrl.oldValue = oldValue;
        };

        ctrl.getValue = function () {
            return ctrl.content.textContent.trim();
        };

        ctrl.setValue = function (value) {
            ctrl.content.innerText = value.trim();
        };

        ctrl.revertValue = function () {
            ctrl.setValue(ctrl.oldValue);
        };

        ctrl.change = function (value) {
            if (value === ctrl.emptyText) {
                value = '';
            }

            if (ctrl.onChange != null) {
                ctrl.onChange({ value: value.trim() });
            }
        };

        ctrl.setCursorPosition = function (el, position) {
            var range = document.createRange();
            var sel = window.getSelection();
            var hasChild = el.childNodes.length > 0;
            var childLength = hasChild === true ? el.childNodes[0].length : 0;
            var pos = position != null && position < childLength ? position : childLength;
            range.setStart(hasChild === true ? el.childNodes[0] : el, pos);
            range.collapse(true);
            sel.removeAllRanges();
            sel.addRange(range);
        };

        ctrl.getCaretPosition = function () {
            var editableDiv = ctrl.getContent();

            var caretPos = 0,
                sel,
                range;

            if ($window.getSelection) {
                sel = $window.getSelection();
                if (sel.rangeCount) {
                    range = sel.getRangeAt(0);
                    if (range.commonAncestorContainer.parentNode == editableDiv) {
                        caretPos = range.endOffset;
                    }
                }
            } else if ($document.selection && $document.selection.createRange) {
                range = $document.selection.createRange();
                if (range.parentElement() == editableDiv) {
                    var tempEl = $document.createElement('span');
                    editableDiv.insertBefore(tempEl, editableDiv.firstChild);
                    var tempRange = range.duplicate();
                    tempRange.moveToElementText(tempEl);
                    tempRange.setEndPoint('EndToEnd', range);
                    caretPos = tempRange.text.length;
                }
            }
            return caretPos;
        };
    };

    SimpleEditCtrl.$inject = ['$document', '$window'];

    ng.module('simpleEdit').controller('SimpleEditCtrl', SimpleEditCtrl);
})(window.angular);
