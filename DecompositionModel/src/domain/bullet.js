let intersectionHelper = require('../intersectionHelper.js');

module.exports = function (direction, x, y) {

    return {
        direction: direction,
        speed: 1,
        density: 1,
        x: x,
        y: y,

        intersectsWithConvex: function (convex) {
            return intersectionHelper.bulletIntersectsWithConvex(this,convex);
        }
    };

};