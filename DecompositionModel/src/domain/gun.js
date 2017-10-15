let convexHelper = require('../convexHelper.js')

module.exports = {
    shoot(bullet, convexes) {

        let intersectionResultsPerConvex = convexes.map(convex => {
                return {
                    intersectionResult: bullet.intersectsWithConvex(convex),
                    convex: convex
                };
            }
        );

        let collidngIntersections = intersectionResultsPerConvex.filter(
            resultAndConvex => resultAndConvex.intersectionResult.intersects
        );

        let transformations = [];

        if (collidngIntersections.length >= 0) {
            let sorted = collidngIntersections.sort((a, b) =>
                a.intersectionResult.distanceFromOriginSquared
                >= b.intersectionResult.distanceFromOriginSquared
            );
            for (let i = 0; i < sorted.length; i++) {
                let convex = sorted[i].convex;
                let { wentThrough, leftConvex, rightConvex } = convex.makeHole(bullet, sorted[i].intersectionResult);
                if(!convexHelper.isPolygonConvex(leftConvex.getVertices()))
                    Error();
                if(!convexHelper.isPolygonConvex(rightConvex.getVertices()))
                    Error();
                transformations.push({ convex, leftConvex, rightConvex });
                if (!wentThrough)
                    break;
            }
        }

        return transformations;
    }
};

/*
    var bullet = Bullet(
        direction: {
            x,
            y
        },
        x,
        y);
*/