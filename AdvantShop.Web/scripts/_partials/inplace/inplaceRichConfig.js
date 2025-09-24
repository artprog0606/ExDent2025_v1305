export default {
    title: false,
    allowedContent: true,
    autoParagraph: false,
    removePlugins: 'dragdrop, basket',
    floatSpaceDockedOffsetY: 5,
    toolbar: [
        { name: 'source', items: ['Ace', 'Templates'] },
        {
            name: 'elements',
            items: ['NumberedList', 'BulletedList', 'Link', 'Unlink', '-', 'Image', 'VideoYTEmbed', 'Table', 'HorizontalRule'],
        },
        { name: 'styles', items: ['Styles', 'Font', 'FontSize', 'lineheight'] },
        '/',
        { name: 'text', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'RemoveFormat'] },
        { name: 'text', items: ['TextColor', 'BGColor'] },
        { name: 'align', items: ['Outdent', 'Indent', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'] },
        { name: 'document', items: ['Paste', 'PasteText', 'PasteFromWord', 'Undo', 'Redo'] },
        { name: 'icons', items: ['Emojione'] },
    ],
    extraPlugins: 'scriptencode,lineheight,bold,emojione,fontSizeExtend,videoytembed,ace',
    stylesSet: 'inplaceStyles',
    customConfig: window.CKEDITOR_BASEPATH + 'config.js?p=' + (window.v || Math.random()),
};
