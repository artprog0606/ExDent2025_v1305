import './styles/rating.scss';

import { ratingDirective } from './directives/ratingDirectives.js';
import RatingCtrl from './controllers/ratingController.js';

const moduleName = 'rating';

angular.module(moduleName, []).controller('RatingCtrl', RatingCtrl).directive('rating', ratingDirective);

export default moduleName;
