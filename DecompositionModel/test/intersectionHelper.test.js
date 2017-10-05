var assert = require('assert');
var Convex = require('../src/domain/convex.js');
var Bullet = require('../src/domain/bullet.js');
var convexHelper = require('../src/convexHelper.js');
var intersectionHelper = require('../src/intersectionHelper.js');

suite('bullet', function () {
    suite('#intersectsWithConvex()', function () {
        test('when only colliding with one line should crash', function () {
            assert.throws(() => {
                let bullet = Bullet({
                    x: 1,
                    y: 0
                }, 0, 0);
                let box = convexHelper.box(0, 0);
                bullet.intersectsWithConvex(box);
            });
        });
        test('when aiming right at box should intersect with box', function () {
            let bullet = Bullet({
                x: 1,
                y: 0
            }, 0, 0);
            let box = convexHelper.box(2, 0);
            assert.equal(true, bullet.intersectsWithConvex(box).intersects);
        });
        test('when aiming away from  box should not intersect with box', function () {
            let bullet = Bullet({
                x: -1,
                y: 0
            }, 0, 0);
            let box = convexHelper.box(2, 0);
            assert.equal(false, bullet.intersectsWithConvex(box).intersects);
        });
        test('should return the distance to the hole enter', function () {
            let bullet = Bullet({
                x: 1,
                y: 0
            }, 0, 0);
            let box = convexHelper.box(3, 0);
            let result = intersectionHelper.bulletIntersectsWithConvex(bullet,box);
            assert.equal(result.distanceFromOriginSquared, 4);
        });

    });
});