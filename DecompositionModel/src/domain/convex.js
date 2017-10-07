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
        console.log(intersectionResult);
        let leftSeries = numberSeries.circularExclusive(
            intersectionResult.enter.index,
            intersectionResult.exit.index,
            lines.length).map(i => lines[i]);

        let rightSeries = numberSeries.circularExclusive(
            intersectionResult.exit.index,
            intersectionResult.enter.index,
            lines.length).map(i => lines[i]);

        let leftStartEdge = {
            start: {
                x: intersectionResult.enter.intersectPosition.x,
                y: intersectionResult.enter.intersectPosition.y
            },
            end: leftSeries[0].start
        };

        let rightStartEdge = {
            start: {
                x: intersectionResult.exit.intersectPosition.x,
                y: intersectionResult.exit.intersectPosition.y
            },
            end: rightSeries[0].start
        };

        leftConvex.lines = [
            leftStartEdge,
            ...leftSeries
        ];
        rightConvex.lines = [
            rightStartEdge,
            ...rightSeries
        ];

        // handle bullet template
        // remove mass

        return {
            wentThrough,
            leftConvex,
            rightConvex
        };
    }
};

module.exports = Convex;