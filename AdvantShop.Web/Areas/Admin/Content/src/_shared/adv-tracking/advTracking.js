function advTrack(eventKey, eventKeyPostfix) {
    var x = new XMLHttpRequest();
    x.open(
        'GET',
        'advantshopTracking/trackEvent?eventKey=' +
            eventKey +
            (eventKeyPostfix && eventKeyPostfix.length ? '&eventKeyPostfix=' + eventKeyPostfix : ''),
        false,
    );
    x.send();
    return true;
}

window.advTrack = advTrack;
