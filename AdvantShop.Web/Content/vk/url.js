class Url {
    toQueryString(obj) {
        return Object.keys(obj)
            .map(function (key) {
                return encodeURIComponent(key) + '=' + encodeURIComponent(obj[key] != null ? obj[key] : '');
            })
            .join('&');
    }

    parseLocation(url) {
        //  create a link in the DOM and set its href
        let link = document.createElement('a');
        link.setAttribute('href', url);

        //  return an easy-to-use object that breaks apart the path
        return {
            host: link.hostname,
            port: link.port,
            search: this.processSearchParams(link.search),
            path: link.pathname,
            protocol: link.protocol,
        };
    }

    processSearchParams(search, preserveDuplicates) {
        //  option to preserve duplicate keys (e.g. 'sort=name&sort=age')
        preserveDuplicates = preserveDuplicates || false; //  disabled by default

        var outputNoDupes = {};
        var outputWithDupes = []; //  optional output array to preserve duplicate keys

        //  sanity check
        if (!search) throw new Error('processSearchParams: expecting "search" input parameter');

        //  remove ? separator (?foo=1&bar=2 -> 'foo=1&bar=2')
        search = search.split('?')[1];

        //  split apart keys into an array ('foo=1&bar=2' -> ['foo=1', 'bar=2'])
        search = search.split('&');

        //  separate keys from values (['foo=1', 'bar=2'] -> [{foo:1}, {bar:2}])
        //  also construct simplified outputObj
        outputWithDupes = search.map(function (keyval) {
            var out = {};
            keyval = keyval.split('=');
            out[keyval[0]] = keyval[1];
            outputNoDupes[keyval[0]] = keyval[1]; //  might as well do the no-dupe work too while we're in the loop
            return out;
        });

        return preserveDuplicates ? outputWithDupes : outputNoDupes;
    }
}
