/* @ngInject */
function RatingCtrl($http, $q) {
    var ctrl = this;

    ctrl.items = [];

    ctrl.select = function (val) {
        if (ctrl.readonly === false) {
            ctrl.current = val;

            for (var i = 0; i < val; i++) {
                ctrl.items[i].isSelected = true;
            }

            if (ctrl.url) {
                return $http.post(ctrl.url, { objId: ctrl.objId, rating: ctrl.current }).then(function (response) {
                    return (ctrl.current = response.data);
                });
            } else {
                return $q.resolve(ctrl.current);
            }
        }
    };
}

export default RatingCtrl;
