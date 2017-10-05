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
                let { wentThrough, newConvexes } = convex.makeHole(bullet, sorted[i].intersectionResult);
                transformations.push({ convex, newConvexes });
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