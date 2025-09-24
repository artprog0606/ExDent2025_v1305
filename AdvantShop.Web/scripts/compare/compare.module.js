import ratingModule from '../_common/rating/rating.module.js';

import '../../styles/views/compareproducts.scss';

import './compare.js';

const moduleName = 'comparePage';

angular.module(moduleName, [ratingModule]).controller('ComparePageCtrl', function () {});

export default moduleName;
