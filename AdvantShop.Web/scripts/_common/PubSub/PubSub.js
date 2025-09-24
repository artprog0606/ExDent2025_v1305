/**
 * Class implementing publish/subscribe.
 * Class is singleton
 */

// TODO rewrite to TS
let instance;
class PubSubClass {
    constructor() {
        if (instance || window.PubSub) {
            return instance || window.PubSub;
        }

        instance = this;
        this.events = {};
    }
    /**
     * Subscribes callback to the event
     * @param  {string} eventName
     * @param  {Function} callback
     * @returns {() => void} function for remove it callback
     */
    subscribe(eventName, callback) {
        if (!this.events[eventName]) {
            this.events[eventName] = [];
        }

        const id = this.events[eventName].push(callback) - 1;

        return () => {
            this.events[eventName].splice(id, 1);
        };
    }
    /**
     * Notify subscribes callbacks
     * @param  {string} eventName
     * @param  {any} data - data to params callback.
     * @returns {boolean} True, if publish success. False, if fail
     */
    publish(eventName, ...data) {
        const event = this.events[eventName];
        if (!event) {
            return false;
        }

        event.forEach((callback) => callback(...data));

        //TODO: remove when we give up JQ
        $(document).trigger(eventName, [...data]);

        return true;
    }
    /**
     * Get all subscribes on event
     * @param  {string} eventName
     * @returns {Array} array subscribes
     */
    getSubscribes(eventName) {
        return this.events[eventName];
    }
    /**
     * Clear all events.
     */
    clear() {
        this.events = {};
    }
}

const PubSub = new PubSubClass();
globalThis.PubSub = PubSub;
export { PubSub };
