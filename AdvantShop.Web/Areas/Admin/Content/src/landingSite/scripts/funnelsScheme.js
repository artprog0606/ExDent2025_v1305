$(function () {
    jsPlumb.ready(function () {
        var instance = jsPlumb.getInstance({
            Connector: ['Flowchart', { gap: 5, cornerRadius: 5, alwaysRespectStubs: true }], // stub: [40, 40],
            DragOptions: { cursor: 'pointer', zIndex: 2000 },
            PaintStyle: {
                strokeWidth: 2,
                stroke: '#61B7CF',
                joinstyle: 'round',
                outlineStroke: 'transparent',
                outlineWidth: 2,
            },
            Endpoint: ['Dot', { radius: 9, cssClass: 'item-connection-dot' }],
            EndpointStyle: { fill: '#7AB02C', stroke: '#7AB02C', radius: 7, strokeWidth: 1 },
            HoverPaintStyle: { stroke: '#216477' },
            EndpointHoverStyle: { fill: '#216477' },
            ConnectionOverlays: [['Arrow', { location: 1, visible: true, width: 14, length: 11, id: 'ARROW' }]],
            MaxConnections: -1,
            Container: 'funnelSheme',
        });

        instance.batch(function () {
            var model = getData();

            console.log(model);

            for (var i = 0; i < model.Nodes.length; i++) {
                var item = model.Nodes[i];

                var left = (i == 0 ? 20 : 0) + item.Level * 240;
                var top = 20 + item.SubLevel * 100 + getParentSublevelTop(item.Id, model);

                if (item.SubLevel != 0 && (item.TypeStr == 'page' || item.TypeStr == 'trigger')) {
                    top += 100;
                }

                $('.funnels-scheme').append(
                    '<div class="funnels-scheme-item funnel-scheme-type-' +
                        item.TypeStr +
                        '" id="' +
                        item.Id +
                        '" style="left:' +
                        left +
                        'px; top:' +
                        top +
                        'px;">' +
                        (item.Title != null ? '<div class="funnels-scheme-item-header">' + item.Title + '</div>' : '') +
                        '<div class="funnels-scheme-item-content">' +
                        getContent(item.TypeStr, item.Content) +
                        '</div>' +
                        (item.Name != null ? '<div class="funnels-scheme-item-name">' + item.Name + '</div>' : '') +
                        '</div>',
                );

                item.top = top;
                item.left = left;
            }

            for (let i = 0; i < model.Edges.length; i++) {
                let item = model.Edges[i];

                instance.connect({
                    source: item.Source,
                    target: item.Target,
                    anchors: ['Right', 'Left'],
                });
            }

            var windows = jsPlumb.getSelector('.funnels-scheme .funnels-scheme-item');
            instance.draggable(windows);
        });

        jsPlumb.fire('jsPlumbDemoLoaded', instance);

        function getData() {
            var res = '';

            $.ajax({
                dataType: 'json',
                cache: false,
                type: 'GET',
                async: false,
                url: '../../landings/getFunnelScheme?siteId=' + $('.siteId').attr('data-id'),
                success: function (data) {
                    res = data;
                },
            });

            return res;
        }

        function getContent(type, content) {
            if (content == null) return '';

            return '<img src="' + content + '" />';
        }

        function getParentSublevelTop(id, model) {
            var parent = model.Edges.filter(function (x) {
                return x.Target == id;
            });
            if (parent == null || parent.length == 0) return 0;

            var node = model.Nodes.filter(function (x) {
                return x.Id == parent[0].Source;
            });
            if (node == null || node.length == 0) return 0;

            var top = 0; //node[0].SubLevel * 100;

            if (node[0].SubLevel > 0 && node[0].top != null) {
                top += node[0].top / 2;
            }

            return top;
        }
    });
});
