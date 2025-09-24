import jQuery from 'jquery';

window.jQuery = window.jQuery != null ? window.jQuery : jQuery;
window.$ = window.$ != null ? window.$ : window.jQuery;
export { jQuery };
export default window.jQuery;
