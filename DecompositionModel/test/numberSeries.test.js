var assert = require('assert');
var numberSeries = require('../src/numberSeries.js');

suite('numberSeries', function () {
    suite('#circular()', function () {
        test('0..1', function () {
            assert.deepEqual(numberSeries.circularExclusive(0, 1, 1),
                []);
        });
        test('0..1 (5)', function () {
            assert.deepEqual(numberSeries.circularExclusive(0, 1, 5),
                []);
        });
        test('0..5', function () {
            assert.deepEqual(numberSeries.circularExclusive(0, 5, 5),
                [1, 2, 3, 4]);
        });
        test('5..10', function () {
            assert.deepEqual(numberSeries.circularExclusive(5, 10, 10),
                [6, 7, 8, 9]);
        });
        test('1..0', function () {
            assert.deepEqual(numberSeries.circularExclusive(1, 0, 1),
                []);
        });
        test('1..0 (5)', function () {
            assert.deepEqual(numberSeries.circularExclusive(1, 0, 5),
                [2, 3, 4]);
        });
        test('5..0', function () {
            assert.deepEqual(numberSeries.circularExclusive(5, 0, 5),
                []);
        });
        test('10..5', function () {
            assert.deepEqual(numberSeries.circularExclusive(10, 5, 10),
                [0, 1, 2, 3, 4]);
        });
    });
});