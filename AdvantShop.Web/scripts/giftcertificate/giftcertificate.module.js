import '../../styles/views/giftcertificate.scss';

import GiftCertificateCtrl from './controllers/giftcertificateController.js';
import giftcertificateService from './services/giftcertificateService.js';

const moduleName = 'giftcertificate';

angular.module(moduleName, []).service('giftcertificateService', giftcertificateService).controller('GiftCertificateCtrl', GiftCertificateCtrl);

export default moduleName;
