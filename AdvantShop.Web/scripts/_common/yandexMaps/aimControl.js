import './aimControl.scss';
/**
 * Класс контрола "центр карты".
 * @class
 * @name AimControl
 * устанавливаем шаблон-верстку для контрола
 *
 *@param {string} tpl
 **/

function AimControl(options) {
    this.events = new ymaps.event.Manager();
    this.options = new ymaps.option.Manager();
    this.Layout = ymaps.templateLayoutFactory.createClass(
        '<div class="aim-control" style="right:$[options.position.right]px; top:$[options.position.top]px;">' +
            '<svg xmlns="http://www.w3.org/2000/svg" width="30" height="30" fill="currentColor"' +
            'viewBox="0 0 16 16">' +
            '<path d="M8 16s6-5.686 6-10A6 6 0 0 0 2 6c0 4.314 6 10 6 10zm0-7a3 3 0 1 1 0-6 3 3 0 0 1 0 6z"/>' +
            '</svg>' +
            '<div class="aim-control__dot"></div>' +
            '</div>',
    );
}

/**
 * @lends AimControl.prototype
 */
AimControl.prototype = {
    /**
     * @constructor
     */
    constructor: AimControl,

    /**
     * Устанавливает родительский объект.
     * @function
     * @name AimControl.setParent
     * @param {IControlParent} parent Родительский объект.
     * @returns {AimControl} Возвращает ссылку на себя.
     */
    setParent: function (parent) {
        this.parent = parent;

        if (parent) {
            this.map = parent.getMap();

            this._setPosition();
            this.map.container.events
                .add('sizechange', this._setPosition, this)
                .add('actionbegin', () => {
                    const element = this.layout.getElement();
                    if (element) {
                        const aimControl = element.querySelector('.aim-control');
                        if (aimControl) {
                            aimControl.classList.add('aim-control--move');
                        }
                    }
                })
                .add('actionend', () => {
                    const element = this.layout.getElement();
                    if (element) {
                        const aimControl = element.querySelector('.aim-control');
                        if (aimControl) {
                            aimControl.classList.remove('aim-control--move');
                        }
                    }
                });

            /**
             * Передаем в макет контрола данные о его опциях.
             * @see http://api.yandex.ru/maps/doc/jsapi/2.x/ref/reference/ILayout.xml#constructor-summary
             */
            // this.layout = new this.constructor.Layout({ options: this.options });
            this.layout = new this.Layout({ options: this.options });
            /**
             * Контрол будет добавляться в pane событий, чтобы исключить интерактивность.
             * @see http://api.yandex.ru/maps/doc/jsapi/2.x/ref/reference/ILayout.xml#setParentElement
             */
            this.layout.setParentElement(this.map.panes.get('events').getElement());
        } else {
            this.layout.setParentElement(null);
            this.map.container.events.remove('sizechange').remove('actionbegin').remove('actionend');
        }

        return this;
    },
    /**
     * Возвращает ссылку на родительский объект.
     * @see http://api.yandex.ru/maps/doc/jsapi/2.x/ref/reference/IControl.xml#getParent
     * @function
     * @name AimControl.getParent
     * @returns {IControlParent} Ссылка на родительский объект.
     */
    getParent: function () {
        return this.parent;
    },
    /**
     * Устанавливает контролу опцию "position".
     * @function
     * @private
     * @name AimControl._setPosition
     * @param {Array} size Размер контейнера карты.
     */
    _setPosition: function () {
        const size = this.map.container.getSize();
        this.options.set('position', {
            top: size[1] / 2 - 30,
            right: size[0] / 2 - 15,
        });
    },
};

export default AimControl;
