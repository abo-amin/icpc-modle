// كود المؤقت
var questionDuration = @Model.TimerSeconds;
var timerElement = document.getElementById("time-left");
var choicesContainer = document.getElementById("choices-container");
var nextButton = document.getElementById("next-btn");
var form = document.getElementById("examForm");

function updateTimer() {
    var currentTime = new Date();
    var storedQuestionStart = localStorage.getItem("questionStartTime");
    var questionStartTime;

    if (storedQuestionStart) {
        questionStartTime = new Date(storedQuestionStart);
    } else {
        questionStartTime = currentTime;
        localStorage.setItem("questionStartTime", currentTime.toISOString());
    }

    var elapsedTime = (currentTime - questionStartTime) / 1000;
    var timeLeft = questionDuration - elapsedTime;

    if (timeLeft <= 0) {
        timeLeft = 0;
        clearInterval(timerInterval);
        localStorage.removeItem("questionStartTime");

        document.querySelectorAll(".form-check-input").forEach(input => input.disabled = true);
        choicesContainer.classList.add("disabled");
        nextButton.disabled = true;

        setTimeout(() => form.submit(), 1000);
    }

    var minutesLeft = Math.floor(timeLeft / 60);
    var secondsLeft = Math.floor(timeLeft % 60);
    timerElement.textContent = minutesLeft + ":" + (secondsLeft < 10 ? "0" + secondsLeft : secondsLeft);
}

var timerInterval = setInterval(updateTimer, 1000);

// تمكين زر "التالي" عند اختيار إجابة
document.querySelectorAll('.form-check-input').forEach(input => {
    input.addEventListener('change', function () {
        nextButton.disabled = false;
    });
});

// كود الخلفية المتحركة
const canvas = document.getElementById('canvas-background');
const ctx = canvas.getContext('2d');
let width, height;

function resizeCanvas() {
    width = window.innerWidth;
    height = window.innerHeight;
    canvas.width = width;
    canvas.height = height;
}

window.addEventListener('resize', resizeCanvas);
resizeCanvas();

const particles = [];
const properties = {
    bgColor: 'rgba(17, 17, 19, 1)',
    particleColor: 'rgba(255, 40, 40, 1)',
    particleRadius: 3,
    particleCount: 60,
    particleMaxVelocity: 0.5,
    lineLength: 150,
    particleLife: 6,
};

class Particle {
    constructor() {
        this.x = Math.random() * width;
        this.y = Math.random() * height;
        this.velocityX = Math.random() * (properties.particleMaxVelocity * 2) - properties.particleMaxVelocity;
        this.velocityY = Math.random() * (properties.particleMaxVelocity * 2) - properties.particleMaxVelocity;
        this.life = Math.random() * properties.particleLife * 60;
    }

    position() {
        this.x + this.velocityX > width && this.velocityX > 0 || this.x + this.velocityX < 0 && this.velocityX < 0 ? this.velocityX *= -1 : this.velocityX;
        this.y + this.velocityY > height && this.velocityY > 0 || this.y + this.velocityY < 0 && this.velocityY < 0 ? this.velocityY *= -1 : this.velocityY;
        this.x += this.velocityX;
        this.y += this.velocityY;
    }

    reDraw() {
        ctx.beginPath();
        ctx.arc(this.x, this.y, properties.particleRadius, 0, Math.PI * 2);
        ctx.closePath();
        ctx.fillStyle = properties.particleColor;
        ctx.fill();
    }

    reCalculateLife() {
        if (this.life < 1) {
            this.x = Math.random() * width;
            this.y = Math.random() * height;
            this.velocityX = Math.random() * (properties.particleMaxVelocity * 2) - properties.particleMaxVelocity;
            this.velocityY = Math.random() * (properties.particleMaxVelocity * 2) - properties.particleMaxVelocity;
            this.life = Math.random() * properties.particleLife * 60;
        }
        this.life--;
    }
}

function reDrawBackground() {
    ctx.fillStyle = properties.bgColor;
    ctx.fillRect(0, 0, width, height);
}

function drawLines() {
    let x1, y1, x2, y2, length, opacity;
    for (let i in particles) {
        for (let j in particles) {
            x1 = particles[i].x;
            y1 = particles[i].y;
            x2 = particles[j].x;
            y2 = particles[j].y;
            length = Math.sqrt(Math.pow(x2 - x1, 2) + Math.pow(y2 - y1, 2));
            if (length < properties.lineLength) {
                opacity = 1 - length / properties.lineLength;
                ctx.lineWidth = '0.5';
                ctx.strokeStyle = 'rgba(255, 40, 40, ' + opacity + ')';
                ctx.beginPath();
                ctx.moveTo(x1, y1);
                ctx.lineTo(x2, y2);
                ctx.closePath();
                ctx.stroke();
            }
        }
    }
}

function reDrawParticles() {
    for (let i in particles) {
        particles[i].reCalculateLife();
        particles[i].position();
        particles[i].reDraw();
    }
}

function loop() {
    reDrawBackground();
    reDrawParticles();
    drawLines();
    requestAnimationFrame(loop);
}

function init() {
    for (let i = 0; i < properties.particleCount; i++) {
        particles.push(new Particle);
    }
    loop();
}

init();

// تفاعل مع حركة الماوس
let mouse = {
    x: null,
    y: null,
    radius: 100
}

window.addEventListener('mousemove', function (event) {
    mouse.x = event.x;
    mouse.y = event.y;
});

function updateParticlesOnMouseMove() {
    for (let i = 0; i < particles.length; i++) {
        let dx = mouse.x - particles[i].x;
        let dy = mouse.y - particles[i].y;
        let distance = Math.sqrt(dx * dx + dy * dy);
        if (distance < mouse.radius) {
            let forceDirectionX = dx / distance;
            let forceDirectionY = dy / distance;
            let force = (mouse.radius - distance) / mouse.radius;
            let directionX = forceDirectionX * force * 5;
            let directionY = forceDirectionY * force * 5;
            particles[i].x -= directionX;
            particles[i].y -= directionY;
        }
    }
}

function animateParticles() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    updateParticlesOnMouseMove();
    reDrawBackground();
    reDrawParticles();
    drawLines();
    requestAnimationFrame(animateParticles);
}

animateParticles();