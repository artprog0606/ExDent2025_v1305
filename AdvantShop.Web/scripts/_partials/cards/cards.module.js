import CardsFormCtrl from './controllers/cardsFormController.js';
import CardsRemoveCtrl from './controllers/cardsRemoveController.js';
import { cardsFormDirective, cardsRemoveDirective } from './directives/cardsDirectives.js';
import cardsService from './services/cardsService.js';

const moduleName = 'cards';

angular
    .module(moduleName, [])
    .service('cardsService', cardsService)
    .controller('CardsFormCtrl', CardsFormCtrl)
    .controller('CardsRemoveCtrl', CardsRemoveCtrl)
    .directive('cardsForm', cardsFormDirective)
    .directive('cardsRemove', cardsRemoveDirective);

export default moduleName;
