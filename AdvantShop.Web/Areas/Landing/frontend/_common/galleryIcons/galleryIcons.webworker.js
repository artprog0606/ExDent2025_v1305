function mergeStyles(items) {
    var result = {};
    var destCopy = Object.assign({}, items[0]);
    var listCopy = items.slice(1);

    var keyList = Object.keys(destCopy.iconList);

    keyList.forEach(function (key) {
        result[key] = [{ prefix: destCopy.prefix, icon: destCopy.iconList[key] }];

        if (listCopy.length > 0) {
            listCopy.forEach(function (item) {
                if (item.iconList[key] != null) {
                    result[key].push({ prefix: item.prefix, icon: item.iconList[key] });

                    delete item.iconList[key];
                }
            });
        }

        delete destCopy.iconList[key];
    });

    return Object.assign(result, listCopy.length > 0 ? mergeStyles(listCopy) : null);
}

export const dataProcess = (data, dataGroups) => {
    var dataSorted = [];
    dataGroups.forEach(function (groupName) {
        var obj = {
            prefix: groupName,
            iconList: Object.assign({}, data[groupName]),
        };

        dataSorted.push(obj);
    });

    dataSorted.sort(function (a, b) {
        return Object.keys(a.iconList).length - Object.keys(b.iconList).length;
    });

    return mergeStyles(dataSorted);
};
