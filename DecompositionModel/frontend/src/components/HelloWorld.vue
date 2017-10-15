<template>
    <div>
        <div>
            {{fps}}
        </div>
        <!--v-render-game="gameViewModel"-->
        <canvas id="myCanvas"
                :width="gameViewModel.width"
                :height="gameViewModel.height"
                ref="canvas">
        </canvas>
        <div>
            <input type="checkbox" v-model="isPaused">Paused<br>
            <button v-if="nextSequence.length" v-on:click="useNextSequence">
                Next
            </button>
            <button v-if="prevSequence.length" v-on:click="usePrevSequence">
                Prev
            </button>
            <button v-on:click="GenerateNewSequence">
                New
            </button>
        </div>
    </div>
</template>

<script>
    window.decomp = require('poly-decomp');
    let Matter = require('matter-js');
    let Convex = require('../../../src/domain/convex.js');
    let Bullet = require('../../../src/domain/bullet.js');
    let Gun = require('../../../src/domain/gun.js');
    let convexHelper = require('../../../src/convexHelper.js');

    // module aliases
    var Engine = Matter.Engine,
        Render = Matter.Render,
        World = Matter.World,
        Bodies = Matter.Bodies;

    let width = 640;
    let height = 640;

    export default {
        name: 'HelloWorld',
        data() {
            return {
                runLog: [],
                isShooting: false,
                isPaused: false,
                engine: Engine.create(),
                convexes: [],
                firstUpdate: true,
                running: true,
                prevTimestamp: '',
                fps: 0,
                shooterBody: {},
                mainBody: {},
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
                },
                currentSequence: [0.1, 0.5, 0.8],
                prevSequence: [],
                nextSequence: [],
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
                if (!this.isShooting && !this.isPaused) {
                    this.shootSequence();
                }
            },
            usePrevSequence() {
                this.nextSequence = this.currentSequence;
                this.currentSequence = this.prevSequence;
                this.prevSequence = [];
            },
            useNextSequence() {
                this.prevSequence = this.currentSequence;
                this.currentSequence = this.nextSequence;
                this.nextSequence = [];
            },
            GenerateNewSequence() {
                this.prevSequence = this.currentSequence.slice(0);
                this.currentSequence[0] = Math.random() * Math.PI * 2;
                this.currentSequence[1] = Math.random() * Math.PI * 2;
                this.currentSequence[2] = Math.random() * Math.PI * 2;
                this.nextSequence = [];
            },
            updateShooterPosFromDir(dir) {
                this.gameViewModel.shooter.x = this.gameViewModel.polygon.x + Math.cos(dir) * (width / 2);
                this.gameViewModel.shooter.y = this.gameViewModel.polygon.y + Math.sin(dir) * (height / 2);
                Matter.Body.setPosition(this.shooterBody, this.gameViewModel.shooter);
            },
            shootFromDir(angle) {
                this.updateShooterPosFromDir(angle);
                this.gameViewModel.shots.push({
                    x: this.gameViewModel.shooter.x,
                    y: this.gameViewModel.shooter.y
                });
                let { x, y } = this.mainBody.position;
                let vertices2 = this.convexes[0].getVertices();
                vertices2[0].y += 20;
                let inverseAngle = angle - Math.PI;
                let bullet = Bullet(
                    {
                        x: 1,
                        y: 0,
                    },
                    this.gameViewModel.shooter.x,
                    this.gameViewModel.shooter.y);
//
                let transformations = Gun.shoot(bullet, this.convexes);
                transformations.forEach(t => {
                    console.log(t.convex);
                    console.log(this.gameViewModel.shooter.x);
                    console.log(this.gameViewModel.shooter.y);
                    World.remove(this.engine.world, t.convex.body);
                    let oldIndex = this.convexes.findIndex(c => c == t.convex);
                    this.convexes.splice(oldIndex, 1);
                    this.convexes.push(t.leftConvex);
                    this.convexes.push(t.rightConvex);
                    let leftVertices = t.leftConvex.getVertices();
                    let rightVertices = t.rightConvex.getVertices();
                    console.log(leftVertices);
                    t.leftConvex.x = width /2;
                    t.leftConvex.y = height /2;
                    t.rightConvex.x = width /2;
                    t.rightConvex.y = height /2;
                    t.leftConvex.body = Bodies.fromVertices(width /2, height/2, leftVertices);
                    t.rightConvex.body = Bodies.fromVertices(width /2, height/2, rightVertices);
                    World.add(this.engine.world, t.leftConvex.body);
                    World.add(this.engine.world, t.rightConvex.body);
                });

//                World.remove(this.engine.world, this.mainBody);
//                this.convexes[0].body = this.mainBody = Bodies.fromVertices(width / 2, height / 2, vertices2);
//                World.add(this.engine.world, this.mainBody);
            },
            async shootSequence() {
                this.isShooting = true;
                this.runLog = [];
                this.convexes.forEach(c => {
                    Matter.World.remove(this.engine.world, c.body);
                });
                this.convexes = [];
                this.mainBody = this.createMainBody();
                Matter.World.add(this.engine.world, this.mainBody);
                await timeout(100);
                this.shootFromDir(this.currentSequence[0] * Math.PI * 2);
                await timeout(100);
                this.shootFromDir(this.currentSequence[1] * Math.PI * 2);
                await timeout(100);
                this.shootFromDir(this.currentSequence[2] * Math.PI * 2);
                await timeout(500);
                this.gameViewModel.shots = [];
                this.isShooting = false;
                console.log('----');
                console.log(this.runLog);
            },
            mouseMoved(e) {
                let mousePos = getMousePos(this.$refs['canvas'], e);
                let dir = Math.atan2(
                    mousePos.y - this.gameViewModel.polygon.y,
                    mousePos.x - this.gameViewModel.polygon.x);
                this.updateShooterPosFromDir(dir);
            },
            createMainBody() {
                let convex = convexHelper.box({ x: width / 2, y: height / 2, size: 100 });
                let originalVertices = convex.getVertices();
                convex.body = Bodies.fromVertices(width / 2, height / 2, originalVertices);
                this.convexes[0] = convex;
                return convex.body;
            },
        },
        beforeDestroy() {
            this.running = false;
            window.removeEventListener('mousemove', this.mouseMoved, false);
            Engine.clear(this.engine);
        },
        mounted() {
            window.addEventListener('mousemove', this.mouseMoved, false);
            // create two boxes and a ground
            this.mainBody = this.createMainBody();
            this.shooterBody = Bodies.circle(450, 100, 40, { isStatic: true });
            var ground = Bodies.rectangle(400, 610, 810, 60, { isStatic: true });
            var render = Render.create({
                canvas: this.$refs['canvas'],
                engine: this.engine
            });

            World.add(this.engine.world, [this.mainBody, this.shooterBody, ground]);
            this.engine.world.gravity.y = 0;
            Engine.run(this.engine);
            Render.run(render);
            this.update();
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

    button {
        font-size: 1.5em;
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
