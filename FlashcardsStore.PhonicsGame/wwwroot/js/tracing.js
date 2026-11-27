window.phonicsTracing = (() => {
    let canvas = null;
    let ctx = null;
    let maskCanvas = null;
    let maskCtx = null;
    let targetWord = '';
    let drawing = false;
    let strokeColor = '#7C3AED';
    let lastPoint = null;
    let history = [];
    let demoRaf = null;
    let textBounds = { left: 0, right: 340, top: 0, bottom: 210 };

    function getPos(e) {
        const rect = canvas.getBoundingClientRect();
        return {
            x: (e.clientX - rect.left) * (canvas.width / rect.width),
            y: (e.clientY - rect.top) * (canvas.height / rect.height)
        };
    }

    function buildWordMask(text) {
        targetWord = (text || '').trim().toLowerCase();
        if (!canvas) return;
        maskCanvas = document.createElement('canvas');
        maskCanvas.width = canvas.width;
        maskCanvas.height = canvas.height;
        maskCtx = maskCanvas.getContext('2d', { willReadFrequently: true });
        maskCtx.clearRect(0, 0, maskCanvas.width, maskCanvas.height);

        if (!targetWord) return;

        const cRect = canvas.getBoundingClientRect();
        const cells = document.querySelectorAll('#wordTraceGuide .lingo-letter-cell');
        maskCtx.textAlign = 'center';
        maskCtx.textBaseline = 'middle';
        maskCtx.lineWidth = 18;
        maskCtx.strokeStyle = '#000';
        maskCtx.fillStyle = '#000';

        if (cells.length > 0 && cRect.width > 0 && cRect.height > 0) {
            const scaleX = canvas.width / cRect.width;
            const scaleY = canvas.height / cRect.height;
            const fontSize = targetWord.length > 4 ? 76 : 96;
            maskCtx.font = `800 ${fontSize}px Nunito, Fredoka, sans-serif`;

            cells.forEach(cell => {
                const r = cell.getBoundingClientRect();
                const cx = (r.left - cRect.left + r.width / 2) * scaleX;
                const cy = (r.top - cRect.top + r.height / 2 + 4) * scaleY;
                const ch = cell.getAttribute('data-char') || '';
                maskCtx.strokeText(ch, cx, cy);
                maskCtx.fillText(ch, cx, cy);
            });
        } else {
            const fontSize = targetWord.length > 4 ? 76 : 96;
            maskCtx.font = `800 ${fontSize}px Nunito, Fredoka, sans-serif`;
            const cx = maskCanvas.width / 2;
            const cy = maskCanvas.height / 2 + 2;
            maskCtx.strokeText(targetWord, cx, cy);
            maskCtx.fillText(targetWord, cx, cy);
        }

        const data = maskCtx.getImageData(0, 0, maskCanvas.width, maskCanvas.height).data;
        let minX = maskCanvas.width, maxX = 0, minY = maskCanvas.height, maxY = 0;
        for (let y = 0; y < maskCanvas.height; y += 2) {
            for (let x = 0; x < maskCanvas.width; x += 2) {
                if (data[(y * maskCanvas.width + x) * 4 + 3] > 20) {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }
        textBounds = minX < maxX
            ? { left: minX, right: maxX, top: minY, bottom: maxY }
            : { left: 20, right: maskCanvas.width - 20, top: 20, bottom: maskCanvas.height - 20 };
    }

    function computeEvaluation() {
        if (!ctx || !canvas) return { ok: false, percent: 0, missing: '' };
        if (!maskCtx || !targetWord) {
            return { ok: true, percent: 100, missing: '' };
        }

        const w = canvas.width;
        const h = canvas.height;
        const userData = ctx.getImageData(0, 0, w, h).data;
        const maskData = maskCtx.getImageData(0, 0, w, h).data;

        const len = Math.max(1, targetWord.length);
        const span = Math.max(1, textBounds.right - textBounds.left);
        const sliceW = span / len;

        const maskCounts = new Array(len).fill(0);
        const hitCounts = new Array(len).fill(0);
        let totalUserInk = 0;
        let inkOnMask = 0;

        for (let y = 0; y < h; y += 3) {
            for (let x = 0; x < w; x += 3) {
                const idx = (y * w + x) * 4 + 3;
                const isMask = maskData[idx] > 20;
                const isUser = userData[idx] > 20;

                if (isUser) {
                    totalUserInk++;
                    if (isMask) inkOnMask++;
                }

                if (isMask) {
                    const charIdx = Math.min(len - 1, Math.max(0, Math.floor((x - textBounds.left) / sliceW)));
                    maskCounts[charIdx]++;
                    if (isUser) hitCounts[charIdx]++;
                }
            }
        }

        let coveredSlices = 0;
        const missingChars = [];
        for (let i = 0; i < len; i++) {
            const ratio = maskCounts[i] > 0 ? hitCounts[i] / maskCounts[i] : 1;
            if (ratio >= 0.28) {
                coveredSlices++;
            } else {
                missingChars.push(targetWord[i]);
            }
        }

        const totalMask = maskCounts.reduce((a, b) => a + b, 0);
        const totalHit = hitCounts.reduce((a, b) => a + b, 0);
        const overallRatio = totalMask > 0 ? totalHit / totalMask : 0;
        const percent = Math.min(100, Math.round((coveredSlices / len) * 70 + Math.min(1, overallRatio / 0.45) * 30));
        const precision = totalUserInk > 0 ? inkOnMask / totalUserInk : 0;

        const ok = missingChars.length === 0 && totalUserInk > 25 && precision >= 0.25;
        return { ok, percent, missing: missingChars.join(', ') };
    }

    function updateProgressBar() {
        const res = computeEvaluation();
        const fill = document.getElementById('traceProgressFill');
        const txt = document.getElementById('traceProgressText');
        if (fill) fill.style.width = `${res.percent}%`;
        if (txt) txt.textContent = `${res.percent}%`;
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

        lastPoint = pos;
    }

    function onUp() {
        if (!drawing) return;
        drawing = false;
        lastPoint = null;
        updateProgressBar();
    }

    return {
        init(canvasId, word) {
            if (demoRaf) cancelAnimationFrame(demoRaf);
            canvas = document.getElementById(canvasId);
            if (!canvas) return;
            ctx = canvas.getContext('2d', { willReadFrequently: true });
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            history = [];
            buildWordMask(word || '');
            updateProgressBar();

            canvas.onpointerdown = onDown;
            canvas.onpointermove = onMove;
            canvas.onpointerup = onUp;
            canvas.onpointercancel = onUp;
        },
        clear() {
            if (ctx && canvas) {
                saveSnapshot();
                ctx.clearRect(0, 0, canvas.width, canvas.height);
                updateProgressBar();
            }
        },
        undo() {
            if (!ctx || history.length === 0) return;
            const prev = history.pop();
            ctx.putImageData(prev, 0, 0);
            updateProgressBar();
        },
        setColor(color) {
            strokeColor = color || '#7C3AED';
        },
        evaluateTrace() {
            return computeEvaluation();
        },
        playDemo(guideId) {
            const container = document.getElementById(guideId);
            if (!container) return;
            const cells = container.querySelectorAll('.lingo-letter-cell');
            if (cells.length > 0) {
                cells.forEach((cell, i) => {
                    setTimeout(() => {
                        cell.classList.remove('demo-pulse');
                        void cell.offsetWidth;
                        cell.classList.add('demo-pulse');
                        setTimeout(() => cell.classList.remove('demo-pulse'), 650);
                    }, i * 380);
                });
            }
        }
    };
})();
