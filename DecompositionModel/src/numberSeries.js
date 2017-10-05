module.exports = {
    range(start, end) {
        return Array.apply(0, Array(end - start))
            .map(function (element, index) {
                return index + start;
            });
    },
    circularExclusive: function (start, end, size) {
        if (start < end) {
            return this.range(start, size)
                .filter(n => n > start && n < end);
        }
        else {
            return this.range(0, size)
                .filter(n =>
                    (n < start && n < end) ||
                    (n > start && n > end));
        }
    },
};

