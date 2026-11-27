window.phonicsTracing = (() => {
    let canvas = null;
    let ctx = null;
    let drawing = false;
    let strokeColor = '#7C3AED';
    let lastPoint = null;
    let totalDistance = 0;
    let history = [];
    let demoRaf = null;

    function getPos(e) {
        const rect = canvas.getBoundingClientRect();
        return {
            x: (e.clientX - rect.left) * (canvas.width / rect.width),
            y: (e.clientY - rect.top) * (canvas.height / rect.height)
        };
    }

    function saveSnapshot() {
        if (!ctx || !canvas) return;
        if (history.length >= 25) history.shift();
        history.push(ctx.getImageData(0, 0, canvas.width, canvas.height));
    }

    function onDown(e) {
        if (!ctx) return;
        e.preventDefault();
        if (canvas.setPointerCapture && e.pointerId !== undefined) {
            try { canvas.setPointerCapture(e.pointerId); } catch {}
        }
        saveSnapshot();
        drawing = true;
        lastPoint = getPos(e);
        ctx.beginPath();
        ctx.arc(lastPoint.x, lastPoint.y, 7, 0, Math.PI * 2);
        ctx.fillStyle = strokeColor;
        ctx.fill();
    }

    function onMove(e) {
        if (!drawing || !ctx || !lastPoint) return;
        e.preventDefault();
        const pos = getPos(e);
        const midX = (lastPoint.x + pos.x) / 2;
        const midY = (lastPoint.y + pos.y) / 2;

        ctx.beginPath();
        ctx.moveTo(lastPoint.x, lastPoint.y);
        ctx.quadraticCurveTo(lastPoint.x, lastPoint.y, midX, midY);
        ctx.lineTo(pos.x, pos.y);
        ctx.strokeStyle = strokeColor;
        ctx.lineWidth = 14;
        ctx.lineCap = 'round';
        ctx.lineJoin = 'round';
        ctx.stroke();

        const dx = pos.x - lastPoint.x;
        const dy = pos.y - lastPoint.y;
        totalDistance += Math.hypot(dx, dy);
        lastPoint = pos;
    }

    function onUp() {
        drawing = false;
        lastPoint = null;
    }

    return {
        init(canvasId) {
            if (demoRaf) cancelAnimationFrame(demoRaf);
            canvas = document.getElementById(canvasId);
            if (!canvas) return;
            ctx = canvas.getContext('2d', { willReadFrequently: true });
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            history = [];
            totalDistance = 0;

            canvas.onpointerdown = onDown;
            canvas.onpointermove = onMove;
            canvas.onpointerup = onUp;
            canvas.onpointercancel = onUp;
        },
        clear() {
            if (ctx && canvas) {
                saveSnapshot();
                ctx.clearRect(0, 0, canvas.width, canvas.height);
                totalDistance = 0;
            }
        },
        undo() {
            if (!ctx || history.length === 0) return;
            const prev = history.pop();
            ctx.putImageData(prev, 0, 0);
        },
        setColor(color) {
            strokeColor = color || '#7C3AED';
        },
        hasDrawnEnough() {
            return totalDistance > 45;
        },
        playDemo(guideId) {
            const el = document.getElementById(guideId);
            if (!el) return;
            el.classList.remove('demo-pulse');
            void el.offsetWidth;
            el.classList.add('demo-pulse');
        }
    };
})();
