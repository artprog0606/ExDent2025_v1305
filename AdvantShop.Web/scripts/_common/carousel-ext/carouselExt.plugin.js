export const SliderDisableToggle = function (Glide, Components, Events) {
    let checkResize = () => {
        if (Components.Html.slides.length <= Glide.settings.perView) {
            Glide.update({ startAt: 0 }).disable();
            Components.Html.root.classList.add('slider-disable');
        } else {
            Components.Html.root.classList.remove('slider-disable');
            Glide.enable();
        }
    };

    Events.on('resize', () => {
        checkResize();
    });

    return {
        mount() {
            Components.Html.collectSlides();
            if (Components.Html.slides.length !== 0) {
                checkResize();
            }
        },
    };
};
