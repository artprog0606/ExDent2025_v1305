const PagesStorage = require('../../../../../node_scripts/pagesStorage.js');

let pages = new PagesStorage();

pages.addItem('commonTemplate', __dirname + '/common.js');

module.exports = pages;