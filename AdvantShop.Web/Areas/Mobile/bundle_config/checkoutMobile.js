import '../../../node_modules/ladda/dist/ladda-themeless.min.css';
import '../../../node_modules/ladda/js/ladda.js';
import '../../../node_modules/angular-ladda/dist/angular-ladda.js';

appDependency.addItem('angular-ladda');

import checkoutModule from '../../../scripts/checkout/checkout.module.js';

import '../styles/views/checkout.scss';

import appDependency from '../../../scripts/appDependency.js';

appDependency.addItem(checkoutModule);
