let assert = require('assert');
let sinon = require('sinon');
let convexHelper = require('../../src/convexHelper.js');
let Convex = require('../../src/domain/convex.js');

suite('convex', function () {
    suite('makeHole', function () {
        test('//returns correct object?', function () {
            // let convex = Convex();
            // let { wentThrough, newConvexes } = convex.makeHole({},
            //     {
            //         enter: {
            //             intersectPosition: {},
            //             index: 0
            //         },
            //         exit: { index: 1 }
            //     });
        });
        test('all edges appearant in the original convex ' +
            'that did not intersect with the bullet ' +
            'should exist in either of the new convexes', function () {

            let line0 = {};
            let line1 = {};
            let line2 = {};
            // {
            //     start: {x: 0, y: 0},
            //     end: {x: 0, y: 0}
            // }
            let convex = Convex({
                lines: [
                    line0, line1, line2
                ]
            });
            let { leftConvex, rightConvex } = convex.makeHole(
                {},
                createIntersectionResult(0, 2)
            );

            assert(leftConvex.lines.includes(line1) ||
                rightConvex.lines.includes(line1));

            assert(!leftConvex.lines.includes(line0));
            assert(!leftConvex.lines.includes(line0));
            assert(!rightConvex.lines.includes(line2));
            assert(!rightConvex.lines.includes(line2));
        });

        test('one of the convexes should contain' +
            ' all edges to the left of the bullet ray', function () {

            let leftEdge = {};
            let topEdge = {end: {}};
            let rightEdge = {};
            let bottomEdge = {end: {}};

            let box = Convex({
                lines: [
                    leftEdge, topEdge, rightEdge, bottomEdge
                ]
            });
            let { leftConvex, rightConvex } = box.makeHole({}, 
                createIntersectionResult(3, 1)
            );

            assert(leftConvex.lines.includes(leftEdge));
        });

        test('one of the convexes should contain' +
            ' all edges to the right of the bullet ray', function () {

            let leftEdge = {};
            let topEdge = {};
            let rightEdge = {};
            let bottomEdge = {};

            let box = Convex({
                lines: [
                    leftEdge, topEdge, rightEdge, bottomEdge
                ]
            });
            let { leftConvex, rightConvex } = box.makeHole({}, 
                createIntersectionResult(3, 1)
            );

            assert(rightConvex.lines.includes(rightEdge));
        });

        test('the left convex should start ' +
            'with an edge that starts at the intersection enter', function () {

            let bottomEdge = {
                end: { x: 0, y: 0 }                
            };
            let leftEdge = {
            };
            let topEdge = {};
            let rightEdge = {};

            let box = Convex({
                lines: [
                    bottomEdge, leftEdge, topEdge, rightEdge
                ]
            });
            let { leftConvex, rightConvex } = box.makeHole({}, {
                enter: {
                    intersectPosition: { x: 0, y: 1 },
                    index: 0
                },
                exit: { 
                    intersectPosition: {},
                    index: 2 
                }
            });
            
            assert.equal(leftConvex.lines[0].start.x, 0);
            assert.equal(leftConvex.lines[0].start.y, 1);
            assert.deepEqual(leftConvex.lines[0].end, bottomEdge.end);
        });

        test('the right convex should start ' +
            'with an edge that starts at the intersection exit', function () {

            let leftEdge = {
                start: { x: 0, y: 0 },
                end: { x: 0, y: 0 }
            };
            let topEdge = {};
            let rightEdge = {};
            let bottomEdge = {};

            let box = Convex({
                lines: [
                    bottomEdge, leftEdge, topEdge, rightEdge
                ]
            });
            let { leftConvex, rightConvex } = box.makeHole({}, {
                enter: {
                    intersectPosition: {},
                    index: 0
                },
                exit: {
                    intersectPosition: { x: 0, y: 1 },
                    index: 2
                }
            });
            console.log(rightConvex.lines[0]);
            assert.equal(rightConvex.lines[0].start.x, 0);
            assert.equal(rightConvex.lines[0].start.y, 1);
            assert.equal(rightConvex.lines[0].end, rightEdge.start);
        });
        test('the next last edge in the left convex ' +
            'should end at the intersection exit', function () {

                let bottomEdge = {};
                let leftEdge = {
                start: { x: 0, y: 0 },
                end: { x: 0, y: 0 }
            };
            let rightEdge = {};
            
            let box = Convex({
                lines: [
                    bottomEdge, leftEdge, rightEdge
                ]
            });
            let { leftConvex, rightConvex } = box.makeHole({}, {
                enter: {
                    intersectPosition: { x: 0, y: 1 },
                    index: 0
                },
                exit: { 
                    intersectPosition: {x: 2, y: 3},
                    index: 2
                 }
            });

            //assert.equal(leftConvex.lines[2].start, rightEdge.end);
            assert.equal(leftConvex.lines[2].end.x, 2);
            assert.equal(leftConvex.lines[2].end.y, 3);
        });

        // Take care of triangle!!

    });
});

function createIntersectionResult(enterIndex, exitIndex) {
    return {
        enter: {
            intersectPosition: {},
            index: enterIndex
        },
        exit: {
            index: exitIndex,
            intersectPosition: {},
        }
    };
}
