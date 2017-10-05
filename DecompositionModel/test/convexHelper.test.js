var assert = require('assert');
var convexHelper = require('../src/convexHelper.js');

suite('convexHelper', function () {
    suite('#generateConvex()', function () {
        test('generates convex polygons', function () {
            var convex = convexHelper.randomConvex();
            let points = convex.lines.map(line => line.start);
            assert(convexHelper.isPolygonConvex(points));
        });
        test('generates polygons whoose end is connected to the start', function () {
            var convex = convexHelper.randomConvex();
            assert.equal(
                convex.lines[convex.lines.length - 1].end,
                convex.lines[0].start);
        });
    });
    suite('#generateBox()', function () {
        test('all lines length are 2', function () {
            var box = convexHelper.box();
            box.lines.forEach(line => {
                let hdist = line.start.x - line.end.x;
                let vdist = line.start.y - line.end.y;
                assert.equal(hdist * hdist + vdist * vdist, 2 * 2);
            });
        });
    });
});