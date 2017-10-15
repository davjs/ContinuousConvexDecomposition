module.exports = {

    crossProduct(v1, v2) {
        return (v1.x * v2.y) - (v1.y * v2.x);
    },

    dotProduct(v1, v2) {
        return v1.x * v2.x + v1.y * v2.y;
    },

    vectorSubtract(a, b) {
        return {
            x: a.x - b.x,
            y: a.y - b.y,
        };
    },
    vectorDistanceSquared(a, b) {
        return Math.pow(a.x - b.x, 2) + Math.pow(a.y - b.y, 2);
    },
    vectorAdd(a, b) {
        return {
            x: a.x + b.x,
            y: a.y + b.y,
        };
    },
    vectorMultiply(v1, v2) {
        return {
            x: v1.x * v2.x,
            y: v1.y * v2.y,
        };
    },
    vectorMultiplyWithScalar(v1, scalar) {
        return {
            x: v1.x * scalar,
            y: v1.y * scalar,
        };
    },

    getRayToLineSegmentIntersection(ray, lineSegment) {
        var v1 = this.vectorSubtract(ray.begin, lineSegment.start);
        var v2 = this.vectorSubtract(lineSegment.end, lineSegment.start);
        var v3 = {
            x: -ray.direction.y,
            y: ray.direction.x
        };

        var dot = this.dotProduct(v2, v3);
        if (!(Math.abs(dot) < 0.000001)) {
            var t1 = this.crossProduct(v2, v1) / dot;
            var t2 = this.dotProduct(v1, v3) / dot;
            if (t1 >= 0.0 && (t2 >= 0.0 && t2 <= 1.0)) {
                return {
                    intersects: true,
                    intersectPosition: this.vectorAdd(
                        ray.begin,
                        this.vectorMultiplyWithScalar(ray.direction, t1)
                    )
                };
            }
        }
        return {
            intersects: false
        };
    },

    bulletIntersectsWithConvex(bullet, convex) {
        let ray = {
            begin: {
                x: bullet.x,
                y: bullet.y,
            },
            direction: {
                x: bullet.direction.x,
                y: bullet.direction.y,
            }
        };
        let convexPos = {
            x: convex.x,
            y: convex.y
        };
        let checks = convex.lines.map((lineSegment, index) => (
            Object.assign(
                this.getRayToLineSegmentIntersection(ray, {
                    start: this.vectorAdd(convexPos, lineSegment.start),
                    end: this.vectorAdd(convexPos, lineSegment.end)
                }),
                {
                    index: index
                })
        ));
        var collisions = checks.filter((check) => check.intersects);
        var nCollisions = collisions.length;

        if (nCollisions !== 2 && nCollisions !== 0) {
            console.log('bullet', bullet);
            console.log('convex', convex);
            console.log('convexPonits', convex.getVertices());
            console.log('collisions', collisions);
            console.log('checks', checks);
            throw Error(nCollisions);
        }
        if (nCollisions === 0) {
            return {
                intersects: false
            };
        }
        let dist1 = this.vectorDistanceSquared(bullet,
            collisions[0].intersectPosition);
        let dist2 = this.vectorDistanceSquared(bullet,
            collisions[1].intersectPosition);
        if (dist1 < dist2) {
            return {
                intersects: true,
                enter: collisions[0],
                exit: collisions[1],
                distanceFromOriginSquared: dist1
            };
        }
        else {
            return {
                intersects: true,
                enter: collisions[1],
                exit: collisions[0],
                distanceFromOriginSquared: dist2
            };
        }
    }
};