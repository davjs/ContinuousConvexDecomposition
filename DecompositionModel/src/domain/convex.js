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
        getVertices,
        makeHole
    };

    function getVertices(){
        let vertices = this.lines.map(l => l.start);
        return vertices;
    };
    function getEdgesToLeftOfIntersection(enterIndex, exitIndex){
        return numberSeries
        .circularExclusive(
            enterIndex,
            exitIndex,
            lines.length)
            .map(i => lines[i]);
    };
    function getEdgesToRightOfIntersection(enterIndex, exitIndex){
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
        let leftConvex = Convex({x: this.x,y: this.y});
        let rightConvex = Convex({x: this.x,y: this.y});
        let leftSeries = getEdgesToLeftOfIntersection(intersectionResult.enter.index, intersectionResult.exit.index);
        let rightSeries = getEdgesToRightOfIntersection(intersectionResult.enter.index, intersectionResult.exit.index);

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

        let rightEndEdge = {
            start: lines[intersectionResult.enter.index].start,
            end: {
                x: intersectionResult.enter.intersectPosition.x,
                y: intersectionResult.enter.intersectPosition.y
            },
        };

        let leftSplitEdge = {
            start: intersectionResult.exit.intersectPosition,
            end: intersectionResult.enter.intersectPosition,
        };

        let rightSplitEdge = {
            start: intersectionResult.enter.intersectPosition,
            end: intersectionResult.exit.intersectPosition,
        };

        leftConvex.lines = [
            leftStartEdge,
            ...leftSeries,
            leftEndEdge,
            leftSplitEdge
        ];
        rightConvex.lines = [
            rightStartEdge,
            ...rightSeries,
            rightEndEdge,
            rightSplitEdge
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