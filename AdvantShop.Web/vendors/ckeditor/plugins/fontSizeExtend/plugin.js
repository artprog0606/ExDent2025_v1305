(function () {
    'use strict';

    function addCombo(editor, comboName, entries, defaultLabel, order, format, parse, findClasses ) {
        var config = editor.config;
        var names = entries.split(';').filter(function (item) { return item != null && item.length > 0});
        var values = names.map(function (item) {
            var temp = item.split('/');
            return { value: temp[0], name: temp[1] };
        });

        editor.ui.addRichCombo(comboName, {
            label: editor.lang.fontSizeExtend.title,
            title: editor.lang.fontSizeExtend.title,
			toolbar: 'styles,' + order,
			allowedContent: true,
			//requiredContent: true,
			panel: {
				css: [ CKEDITOR.skin.getPath( 'editor' ) ].concat( config.contentsCss ),
				multiSelect: false,
                attributes: { 'aria-label': editor.lang.fontSizeExtend.title }
			},
			init: function() {
                this.startGroup(editor.lang.fontSizeExtend.title);
                for (var i = 0; i < values.length; i++ ) {
                    var item = values[ i ];					
                    this.add(item.value, item.name, item.name);
				}
			},
			onClick: function( value ) {
				editor.focus();
                editor.fire('saveSnapshot');
                var selection = editor.getSelection();
                var isCollapsed = selection.isCollapsed();
                var startElement = selection.getStartElement();
                var style;

                if (startElement != null) {

                    if (isCollapsed) {
                        selection.selectElement(startElement);
                    }

                    style = new CKEDITOR.style({
                        element: selection.isCollapsed() ? startElement.$.nodeName.toLowerCase() : 'span',
                        attributes: { 'class': format(value) }
                    });

                    var classExist = findClasses(startElement.$.className);
                    if (classExist != null) {
                        startElement.removeClass(classExist);
                    }

                    editor.applyStyle(style);
                    this.setValue(value);
                } else {

                    style = new CKEDITOR.style({
                        attributes: { 'class': format(value) }
                    });

                    editor[this.getValue() === value ? 'removeStyle' : 'applyStyle'](style);
                }

				editor.fire( 'saveSnapshot' );
            },
            onRender: function () {
                editor.on('selectionChange', function (ev) {
                    var currentValue = this.getValue();
                    var elementPath = ev.data.path, elements = elementPath.elements;
                    for (var i = 0, element; i < elements.length; i++) {
                        element = elements[i];
                        for (var j = 0, lenJ = values.length; j < lenJ; j++) {
                            var size = parse(element.$.className);
                            if (size != null && size === values[j].value && currentValue !== values[j].value) {
                                this.setValue(values[j].value);
                                return;
                            }
                        }
                    }
                    this.setValue('', defaultLabel);
                }, this);
            },
		} );
	}
    CKEDITOR.plugins.add( 'fontSizeExtend', {
		requires: 'richcombo',
		lang: 'ru,en',
		init: function( editor ) {
			var config = editor.config;
            addCombo(editor, 'fontSizeExtend', config.font_size_extend, editor.lang.fontSizeExtend.title, 40, config.font_size_extend_template_format, config.font_size_extend_template_parse, config.font_size_extend_find_classes);
        }
	} );
} )();
CKEDITOR.config.font_size_extend = '';
CKEDITOR.config.font_size_extend_find_classes = null;
CKEDITOR.config.font_size_extend_template_format = null;
CKEDITOR.config.font_size_extend_template_parse = null;
