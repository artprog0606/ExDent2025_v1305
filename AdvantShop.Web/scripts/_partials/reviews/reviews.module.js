import ngFileUploadModule from '../../../node_modules/ng-file-upload/index.js';

import './styles/reviews.scss';

import ReviewsCtrl from './controllers/reviewsController.js';
import ReviewsFormCtrl from './controllers/reviewsFormController.js';
import ReviewItemRatingCtrl from './controllers/reviewItemRatingController.js';
import {
    reviewsDirective,
    reviewItemDirective,
    reviewsFormDirective,
    reviewReplyDirective,
    reviewDeleteDirective,
    reviewItemRatingDirective,
} from './directives/reviewsDirectives.js';

const moduleName = 'reviews';

angular
    .module(moduleName, [ngFileUploadModule])
    .controller('ReviewsCtrl', ReviewsCtrl)
    .controller('ReviewItemRatingCtrl', ReviewItemRatingCtrl)
    .controller('ReviewsFormCtrl', ReviewsFormCtrl)
    .directive('reviews', reviewsDirective)
    .directive('reviewItem', reviewItemDirective)
    .directive('reviewsForm', reviewsFormDirective)
    .directive('reviewReply', reviewReplyDirective)
    .directive('reviewDelete', reviewDeleteDirective)
    .directive('reviewItemRating', reviewItemRatingDirective);

export default moduleName;
