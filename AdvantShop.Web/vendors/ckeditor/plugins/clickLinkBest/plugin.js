/**
 * @license Copyright (c) 2003-2017, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

( function() {
	'use strict';

	var eventName = 'click';

	CKEDITOR.plugins.add('clicklinkbest', {
	    init: function (editor) {
	        editor.on('instanceReady', function () {

	            var callback = function (event) {
	                if (event.target.tagName === 'A' && event.target.getAttribute('href') != null && event.target.getAttribute('href').length > 0) {
	                    window.open(event.target.href, 'new' + event.screenX);
	                }
	            };

	            editor.document.$.addEventListener(eventName, callback);

	            ['afterInsertHtml', 'afterSetData'].forEach(function (eventCKEName) {
	                editor.on(eventCKEName, function () {
	                    editor.document.$.removeEventListener(eventName, callback);
	                    editor.document.$.addEventListener(eventName, callback);
	                })
	            })
	        });

	    }
	});
} )();
