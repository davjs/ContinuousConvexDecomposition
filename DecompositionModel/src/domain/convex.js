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

    function GetEdgesToLeftOfIntersection(enterIndex, exitIndex){
        return numberSeries
        .circularExclusive(
            enterIndex,
            exitIndex,
            lines.length)
            .map(i => lines[i]);
    };
    function GetEdgesToRightOfIntersection(enterIndex, exitIndex){
        return numberSeries
        .circularExclusive(
            exitIndex,
            enterIndex,
            lines.length)
            .map(i => lines[i]);
    };
    function makeHole(bullet, intersectionResult) {
        //TODO: calculate depth
        let wentThrough = false;
        let leftConvex = Convex();
        let rightConvex = Convex();
        let leftSeries = GetEdgesToLeftOfIntersection(intersectionResult.enter.index, intersectionResult.exit.index);
        let rightSeries = GetEdgesToRightOfIntersection(intersectionResult.enter.index, intersectionResult.exit.index);

        let leftStartEdge = {
            start: {
                x: intersectionResult.enter.intersectPosition.x,
                y: intersectionResult.enter.intersectPosition.y
            },
            end: lines[intersectionResult.enter.index].end
        };

        let leftEndEdge = {
            start: lines[intersectionResult.exit.index].start,
            end: {
                x: intersectionResult.exit.intersectPosition.x,
                y: intersectionResult.exit.intersectPosition.y
            },
        };
        
        let rightStartEdge = {
            start: {
                x: intersectionResult.exit.intersectPosition.x,
                y: intersectionResult.exit.intersectPosition.y
            },
            end: lines[intersectionResult.exit.index].end
        };

        leftConvex.lines = [
            leftStartEdge,
            ...leftSeries,
            leftEndEdge
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