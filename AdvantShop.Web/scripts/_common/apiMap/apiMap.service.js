export default function apiMapService() {
    const service = this;
    let mapApiKey = null;

    service.setMapApiKey = (apiKey) => {
        mapApiKey = apiKey;
    };

    service.getMapApiKey = () => {
        return mapApiKey;
    };
}
