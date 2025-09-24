import PagesStorage from '../node_scripts/pagesStorage.js';
import { getDirname } from '../node_scripts/shopPath.js';
let obj = new PagesStorage(getDirname(import.meta.url));

obj.addItem('yandexnewdelivery', '../scripts/_partials/shipping/extend/yandexnewdelivery/yandexnewdelivery.js');
obj.addItem('yandexWidget', '../scripts/_partials/shipping/extend/Yandex/yandexWidget/yandexWidget.js');
obj.addItem('yandex', '../scripts/_partials/shipping/extend/Yandex/yandex.js');
obj.addItem('shiptor', '../scripts/_partials/shipping/extend/shiptor/shiptor.js');
obj.addItem('sdek', '../scripts/_partials/shipping/extend/sdek/sdek.js');
obj.addItem('sberlogistic', '../scripts/_partials/shipping/extend/sberlogistic/sberlogistic.js');
obj.addItem('pointdelivery', '../scripts/_partials/shipping/extend/pointdelivery/pointdeliverymap.js');
obj.addItem('pickpoint', '../scripts/_partials/shipping/extend/pickpoint/pickpoint.js');
obj.addItem('ozonRocket', '../scripts/_partials/shipping/extend/ozon-rocket/ozon-rocket.js');
obj.addItem('hermes', '../scripts/_partials/shipping/extend/hermes/hermes.js');
obj.addItem('grastin', '../scripts/_partials/shipping/extend/grastin/grastin.js');
obj.addItem('ddelivery', '../scripts/_partials/shipping/extend/ddelivery/ddelivery.js');
obj.addItem('boxberry', '../scripts/_partials/shipping/extend/boxberry/boxberry.js');
obj.addItem('deliverybyzones', '../scripts/_partials/shipping/extend/deliveryByZones/deliveryByZones.js');
obj.addItem('fivePostWidget', '../scripts/_partials/shipping/extend/fivePost/fivePostWidget.js');

export default obj;
