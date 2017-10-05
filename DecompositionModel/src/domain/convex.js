let numberSeries = require('../numberSeries.js');
Convex = function (createOptions = {}) {

    let lines = createOptions.lines || [];
    let y = createOptions.y || 0;
    let x = createOptions.x || 0;
    let density = createOptions.density || 1;

    return {
        density,
        x,
        y,
        lines,
        makeHole
    };

    function makeHole(bullet, intersectionResult) {
        //TODO: calculate depth
        let wentThrough = false;
        let leftConvex = Convex();
        let rightConvex = Convex();

        let leftSeries = numberSeries.circularExclusive(
            intersectionResult.enter.index,
            intersectionResult.exit.index,
            lines.length);

        let rightSeries = numberSeries.circularExclusive(
            intersectionResult.exit.index,
            intersectionResult.enter.index,
            lines.length);

        leftConvex.lines = leftSeries.map(i => lines[i]);
        rightConvex.lines = rightSeries.map(i => lines[i]);

        // handle bullet template
        // remove mass

        return {
            wentThrough,
            newConvexes: [
                leftConvex, rightConvex
            ]
        };
    }
};

module.exports = Convex;