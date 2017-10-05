let assert = require('assert');
let sinon = require('sinon');
//let convexHelper = require('../../src/convexHelper.js');
let Gun = require('../../src/domain/gun.js');

suite('gun', function () {
    suite('shoot', function () {

        test('creates hole if the direction of bullet intersects with convex', function () {

            let bullet = {
                intersectsWithConvex: () => ({ intersects: true })
            };

            let convex = {
                makeHole: sinon.stub().returns({})
            };

            Gun.shoot(bullet, [convex]);

            assert(convex.makeHole.called);
        });

        test('creates hole in the convex that is closest convex', function () {

            let convexFarAway = {
                makeHole: sinon.stub().returns({})
            };

            let convexClose = {
                makeHole: sinon.stub().returns({})
            };

            let bullet = {
                intersectsWithConvex(convex) {
                    let distance;
                    if (convex == convexFarAway) {
                        distance = 2;
                    }
                    else {
                        distance = 1;
                    }
                    return {
                        intersects: true,
                        distanceFromOriginSquared: distance
                    };
                }
            };

            Gun.shoot(bullet, [convexFarAway, convexClose]);

            assert(convexClose.makeHole.called);
        });

        test('does not create hole in convex that is farther away than an other intersecting convex', function () {

            let convexFarAway = {
                makeHole: sinon.stub().returns({})
            };

            let convexClose = {
                makeHole: sinon.stub().returns({})
            };

            let bullet = {
                intersectsWithConvex(convex) {
                    let distance;
                    if (convex == convexFarAway) {
                        distance = 2;
                    }
                    else {
                        distance = 1;
                    }
                    return {
                        intersects: true,
                        distanceFromOriginSquared: distance
                    };
                }
            };

            Gun.shoot(bullet, [convexClose, convexFarAway]);

            assert(convexFarAway.makeHole.notCalled);
        });
        test('when hole goes through should make hole in next closest convex', function () {

            let convexFarAway = {
                makeHole: sinon.stub().returns({})
            };

            let convexClose = {
                makeHole: () => ({ wentThrough: true })
            };

            let bullet = {
                intersectsWithConvex(convex) {
                    let distance;
                    if (convex == convexFarAway) {
                        distance = 2;
                    }
                    else {
                        distance = 1;
                    }
                    return {
                        intersects: true,
                        distanceFromOriginSquared: distance
                    };
                }
            };

            Gun.shoot(bullet, [convexClose, convexFarAway]);

            assert(convexFarAway.makeHole.called);
        });

        // Should this be in the domain?
        test('returns the transformations required acquire the new model', function () {
            let convex = {
                makeHole: () => ({
                    wentThrough: true,
                    newConvexes: [{}, {}]
                })
            };

            let bullet = {
                intersectsWithConvex() {
                    return {
                        intersects: true,
                        distanceFromOriginSquared: 1
                    };
                }
            };

            let transformations = Gun.shoot(bullet, [convex]);

            assert.equal(transformations.length, 1);
            assert.deepEqual(transformations[0], {
                convex: convex,
                newConvexes: [{}, {}]
            });
        });
    });
});