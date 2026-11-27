window.phonicsTracing = (() => {
    let canvas = null;
    let ctx = null;
    let drawing = false;
    let strokeColor = '#2B071C';

    function getPos(e) {
        const rect = canvas.getBoundingClientRect();
        return {
            x: (e.clientX - rect.left) * (canvas.width / rect.width),
            y: (e.clientY - rect.top) * (canvas.height / rect.height)
        };
    }

    function onDown(e) {
        if (!ctx) return;
        drawing = true;
        const pos = getPos(e);
        ctx.beginPath();
        ctx.moveTo(pos.x, pos.y);
    }

    function onMove(e) {
        if (!drawing || !ctx) return;
        const pos = getPos(e);
        ctx.lineTo(pos.x, pos.y);
        ctx.strokeStyle = strokeColor;
        ctx.lineWidth = 14;
        ctx.lineCap = 'round';
        ctx.lineJoin = 'round';
        ctx.stroke();
    }

    function onUp() {
        drawing = false;
    }

    return {
        init(canvasId) {
            canvas = document.getElementById(canvasId);
            if (!canvas) return;
            ctx = canvas.getContext('2d');
            ctx.clearRect(0, 0, canvas.width, canvas.height);

            canvas.onpointerdown = onDown;
            canvas.onpointermove = onMove;
            canvas.onpointerup = onUp;
            canvas.onpointercancel = onUp;
        },
        clear() {
            if (ctx && canvas) {
                ctx.clearRect(0, 0, canvas.width, canvas.height);
            }
        },
        setColor(color) {
            strokeColor = color || '#2B071C';
        }
    };
})();
