(function () {
    var value;

	function addCombo( editor, comboName, styleType, lang, entries, defaultLabel, styleDefinition, order ) {
		var config = editor.config,style = new CKEDITOR.style( styleDefinition );		
		var names = entries.split( ';' ),values = [];		
		var styles = {};
		for ( var i = 0; i < names.length; i++ ) {
			var parts = names[ i ];
			if ( parts ) {
				parts = parts.split( '/' );
				var vars = {},name = names[ i ] = parts[ 0 ];
				vars[ styleType ] = values[ i ] = parts[ 1 ] || name;
				styles[ name ] = new CKEDITOR.style( styleDefinition, vars );
				styles[ name ]._.definition.name = name;
			} else
				names.splice( i--, 1 );
		}
		editor.ui.addRichCombo( comboName, {
		    label: editor.lang.bold.title,
			title: editor.lang.bold.title,
			toolbar: 'styles,' + order,
			allowedContent: style,
			requiredContent: style,
			panel: {
				css: [ CKEDITOR.skin.getPath( 'editor' ) ].concat( config.contentsCss ),
				multiSelect: false,
				attributes: { 'aria-label': editor.lang.bold.title }
			},
			init: function() {
			    this.startGroup(editor.lang.bold.title);
				for ( var i = 0; i < names.length; i++ ) {
					var name = names[ i ];					
					this.add( name, styles[ name ].buildPreview(), name );
				}
			},
			onClick: function( value ) {
				editor.focus();
				editor.fire( 'saveSnapshot' );
                var style = styles[value];
                var startElement = editor.getSelection().getStartElement();
                if (startElement != null) {
                    //style.applyToObject(startElement, editor);
                    editor.applyStyle(style);
                    this.setValue(value);
                } else {
                    editor[this.getValue() == value ? 'removeStyle' : 'applyStyle'](style);
                }
				editor.fire( 'saveSnapshot' );
			},
			onRender: function() {
				editor.on( 'selectionChange', function( ev ) {
                    var currentValue = this.getValue();
					var elementPath = ev.data.path,elements = elementPath.elements;
					for ( var i = 0, element; i < elements.length; i++ ) {
						element = elements[ i ];
						for ( var value in styles ) {
						if ( styles[ value ].checkElementMatch( element, true, editor ) ) {
                            if (value != currentValue)
                                    this.setValue(value);
								return;
							}
						}
					}
					this.setValue( '', defaultLabel );
				}, this );
			},
			refresh: function() {
				if ( !editor.activeFilter.check( style ) )
					this.setState( CKEDITOR.TRISTATE_DISABLED );
			}
		} );
	}
	CKEDITOR.plugins.add('bold', {
		requires: 'richcombo',
		lang: 'ru,en',
		init: function( editor ) {
			var config = editor.config;
			addCombo(editor, 'bold', 'size', editor.lang.bold.title, config.bold_values, editor.lang.bold.title, config.bold_style, 40);
		}
	} );
} )();
CKEDITOR.config.bold_values = 'Тонкий/300;Обычный/400;Полужирный/600;Жирный/700;Очень жирный/800';
CKEDITOR.config.bold_style = {
	element: 'span',
	styles: { 'font-weight': '#(size)' },
	overrides: [ {
	    element: 'font-weight'
	} ]
};
