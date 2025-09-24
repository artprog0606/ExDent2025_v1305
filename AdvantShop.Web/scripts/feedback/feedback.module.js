import '../../styles/views/feedback.scss';
import checkOrderModule from '../_partials/checkorder/checkorder.module.js';

import FeedbackCtrl from './controllers/feedbackController.js';

const moduleName = 'feedback';

angular.module(moduleName, [checkOrderModule]).controller('FeedbackCtrl', FeedbackCtrl);

export default moduleName;
