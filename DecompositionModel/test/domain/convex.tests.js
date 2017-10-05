let assert = require('assert');
let sinon = require('sinon');
let convexHelper = require('../../src/convexHelper.js');
let Convex = require('../../src/domain/convex.js');

suite('convex', function () {
    suite('makeHole', function () {
        test('splits the convex into two', function () {
            let convex = Convex();
            let { wentThrough, newConvexes } = convex.makeHole({},
                {
                    enter: {index: 0},
                    exit: {index: 1}
                });

            assert.equal(newConvexes.length, 2);
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
            let { wentThrough, newConvexes } = convex.makeHole({}, {
                enter: { index: 0 },
                exit: { index: 2 }
            });

            assert(newConvexes[0].lines.includes(line1) ||
                newConvexes[1].lines.includes(line1));

            assert(!newConvexes[0].lines.includes(line0));
            assert(!newConvexes[0].lines.includes(line0));
            assert(!newConvexes[1].lines.includes(line2));
            assert(!newConvexes[1].lines.includes(line2));

            assert.equal(newConvexes.length, 2);
        });

        test('one of the convexes should contain' +
            ' all edges to the left of the bullet ray', function () {

            let leftEdge = {};
            let topEdge = {};
            let rightEdge = {};
            let bottomEdge = {};

            let box = Convex({
                lines: [
                    leftEdge, topEdge, rightEdge, bottomEdge
                ]
            });
            let { _, newConvexes } = box.makeHole({}, {
                enter: { index: 3 },
                exit: { index: 1 }
            });

            assert(newConvexes[0].lines.includes(leftEdge));

            assert.equal(newConvexes.length, 2);
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
            let { _, newConvexes } = box.makeHole({}, {
                enter: { index: 3 },
                exit: { index: 1 }
            });

            assert(newConvexes[1].lines.includes(rightEdge));

            assert.equal(newConvexes.length, 2);
        });
    });
});