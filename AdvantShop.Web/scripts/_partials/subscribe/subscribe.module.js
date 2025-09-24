import './styles/subscribe.scss';
import SubscribeCtrl from './controllers/subscribeController.js';

const moduleName = 'subscribe';

angular.module(moduleName, []).controller('SubscribeCtrl', SubscribeCtrl);

export default moduleName;
