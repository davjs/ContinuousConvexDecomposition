<template>
    <div>
        <div>
            {{fps}}
        </div>
        <canvas id="myCanvas"
                :width="gameViewModel.width"
                :height="gameViewModel.height"
                v-render-game="gameViewModel"
                ref="canvas">
        </canvas>
    </div>
</template>

<script>
    let width = 640;
    let height = 640;

    export default {
        name: 'HelloWorld',
        data() {
            return {
                running: true,
                prevTimestamp: '',
                fps: 0,
                gameViewModel: {
                    width: width,
                    height: height,
                    polygon: {
                        x: width / 2,
                        y: height / 2,
                        points: [
                            { x: -1, y: -1 },
                            { x: 1, y: -1 },
                            { x: 1, y: 1 },
                            { x: -1, y: 1 },
                        ]
                    },
                    shooter: {
                        x: 0,
                        y: 0
                    },
                    shots: []
                }
            };
        },
        directives: {
            renderGame: function (canvasElement, binding) {
                var ctx = canvasElement.getContext("2d");
                let gameViewModel = binding.value;
                ctx.clearRect(0, 0, gameViewModel.width, gameViewModel.height);
                ctx.fillStyle = "black";
                ctx.fillStyle = '#59C9A5';
                ctx.beginPath();
                let polygon = gameViewModel.polygon;
                let points = polygon.points;
//                ctx.moveTo(polygon.x +points[0].x * 100, points[0].y * 100);
                points.forEach(p => {
                    ctx.lineTo(polygon.x + p.x * 100, polygon.y + p.y * 100);
                });
//                ctx.lineTo(points[0].x * 100, points[0].y * 100);
                ctx.closePath();
                ctx.fill();

                ctx.beginPath();
                ctx.arc(gameViewModel.shooter.x, gameViewModel.shooter.y
                    , 20, 0, 2 * Math.PI);
                ctx.fill();

                gameViewModel.shots.forEach(shot => {
                    ctx.beginPath();
                    ctx.lineTo(shot.x, shot.y);
                    ctx.lineTo(polygon.x, polygon.y);
                    ctx.closePath();
                    ctx.stroke();
                });
            }
        },
        methods: {
            update(timestamp) {
                if (this.running)
                    window.requestAnimationFrame(this.update);
                this.fps = (1000 / (timestamp - this.prevTimestamp)).toFixed(0);
                this.prevTimestamp = timestamp;
                if (this.gameViewModel.shots.length === 0) {
                    this.shootSequence();
                }
            },
            shoot() {
                this.gameViewModel.shots.push({
                    x: this.gameViewModel.shooter.x,
                    y: this.gameViewModel.shooter.y
                });
            },
            async shootSequence() {
                this.updateShooterPosFromDir(Math.random() * Math.PI * 2);
                this.shoot();
                await timeout(300);
                this.updateShooterPosFromDir(Math.random() * Math.PI * 2);
                this.shoot();
                await timeout(300);
                this.updateShooterPosFromDir(Math.random() * Math.PI * 2);
                this.shoot();
                await timeout(1500);
                this.gameViewModel.shots = [];
            },
            updateShooterPosFromDir(dir) {
                this.gameViewModel.shooter.x = this.gameViewModel.polygon.x + Math.cos(dir) * (width / 2);
                this.gameViewModel.shooter.y = this.gameViewModel.polygon.y + Math.sin(dir) * (height / 2);
            },
            mouseMoved(e) {
                let mousePos = getMousePos(this.$refs['canvas'], e);
                let dir = Math.atan2(
                    mousePos.y - this.gameViewModel.polygon.y,
                    mousePos.x - this.gameViewModel.polygon.x);
                this.updateShooterPosFromDir(dir);
            }

        },
        beforeDestroy() {
            this.running = false;
            window.removeEventListener('mousemove', this.mouseMoved, false);
        },
        mounted() {
            this.update();
            window.addEventListener('mousemove', this.mouseMoved, false);
        }
    }

    function timeout(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    function getMousePos(canvas, evt) {
        var rect = canvas.getBoundingClientRect();
        return {
            x: evt.clientX - rect.left,
            y: evt.clientY - rect.top
        };
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    h1, h2 {
        font-weight: normal;
    }

    ul {
        list-style-type: none;
        padding: 0;
    }

    li {
        display: inline-block;
        margin: 0 10px;
    }

    a {
        color: #42b983;
    }
</style>
