var Chart = function ($el) {
    this.$el = $el;
    this.init();
};

Chart.prototype = {
    init: function () {
        if (this.$el.dataset.top) {
            this.$topBand = this.$el.dataset.top;
        } else {
            this.$topBand = 90;
        }
        this.$topValue = (this.$el.dataset.target / this.$topBand) * 1;

        this.$target = this.$el.querySelector('.target');
        this.$target.style.left = this.$topBand + '%';

        this.$el.style.backgroundColor = this.$el.dataset.color;

        this.$bars = this.$el.querySelectorAll('.bar');
        this.$barCount = this.$bars.length;

        for (var i = 0; i < this.$barCount; i++) {
            this.$percentage = 1 / (this.$topValue / this.$bars[i].dataset.value);
            if (this.$percentage > 100) {
                this.$percentage = 100;
            }
            this.$bars[i].style.width = this.$percentage + '%';
            this.$bars[i].style.backgroundColor = this.$bars[i].dataset.color;
        }
    }
};

var $chart = document.querySelectorAll('.progress-chart');
for (var i = 0; i < $chart.length; i++) {
    new Chart($chart[i]);
}
