window.phonicsAudio = (() => {
    let ctx = null;
    function getCtx() {
        if (!ctx) {
            const AudioCtx = window.AudioContext || window.webkitAudioContext;
            if (AudioCtx) ctx = new AudioCtx();
        }
        if (ctx && ctx.state === 'suspended') {
            ctx.resume().catch(() => {});
        }
        return ctx;
    }

    function tone(freq, duration, type, delay = 0, vol = 0.18) {
        const c = getCtx();
        if (!c) return;
        const osc = c.createOscillator();
        const gain = c.createGain();
        osc.type = type;
        osc.frequency.setValueAtTime(freq, c.currentTime + delay);
        gain.gain.setValueAtTime(vol, c.currentTime + delay);
        gain.gain.exponentialRampToValueAtTime(0.001, c.currentTime + delay + duration);
        osc.connect(gain);
        gain.connect(c.destination);
        osc.start(c.currentTime + delay);
        osc.stop(c.currentTime + delay + duration);
    }

    return {
        pop() {
            tone(520, 0.08, 'sine', 0, 0.14);
        },
        success() {
            tone(523.25, 0.12, 'triangle', 0, 0.2);
            tone(659.25, 0.12, 'triangle', 0.1, 0.2);
            tone(783.99, 0.18, 'triangle', 0.2, 0.22);
            tone(1046.5, 0.28, 'sine', 0.32, 0.22);
        },
        wrong() {
            tone(240, 0.14, 'sawtooth', 0, 0.1);
            tone(190, 0.2, 'sawtooth', 0.12, 0.1);
        },
        confetti() {
            if (typeof window.confetti === 'function') {
                window.confetti({
                    particleCount: 55,
                    spread: 70,
                    origin: { y: 0.65 },
                    colors: ['#D4AF37', '#10B981', '#3B82F6', '#F59E0B', '#EC4899']
                });
            }
        }
    };
})();
