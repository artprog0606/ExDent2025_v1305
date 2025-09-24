import './src/variables.scss';
import './src/sweetalert2.scss';
import './ext/sweet-alert.ext.scss';

import Sweetalert2 from './src/sweetalert2.cjs';
globalThis.Sweetalert2 = globalThis.Sweetalert = globalThis.swal = Sweetalert2;
export default globalThis.Sweetalert2.mixin({});
