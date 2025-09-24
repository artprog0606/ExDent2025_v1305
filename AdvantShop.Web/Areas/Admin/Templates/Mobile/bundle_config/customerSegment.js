import appDependency from '../../../../../scripts/appDependency.js';

import '../../../Content/src/customerSegment/customerSegment.js';
import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';
appDependency.addItem(`swipeLine`);

import '../../../Content/src/_partials/customer-fields/customerFields.component.js';
appDependency.addItem(`customerFields`);

import '../../../Content/src/_shared/modal/sendSms/ModalSendSmsAdvCtrl.js';
import '../../../Content/src/_shared/modal/sendLetterToCustomer/ModalSendLetterToCustomerCtrl.js';
import '../../../Content/src/_shared/modal/sendSocialMessage/ModalSendSocialMessageCtrl.js';

appDependency.addItem('customerSegment');
