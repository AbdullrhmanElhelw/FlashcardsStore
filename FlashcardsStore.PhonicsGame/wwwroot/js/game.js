window.phonicsDom = {
    setLocale(lang, dir) {
        document.documentElement.lang = lang || 'ar';
        document.documentElement.dir = dir || 'rtl';
    }
};

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
            const colors = ['#F59E0B', '#10B981', '#3B82F6', '#EC4899', '#8B5CF6', '#FDE047'];
            const container = document.createElement('div');
            container.className = 'confetti-layer';
            document.body.appendChild(container);
            for (let i = 0; i < 28; i++) {
                const p = document.createElement('span');
                p.className = 'confetti-piece';
                p.style.left = (10 + Math.random() * 80) + 'vw';
                p.style.backgroundColor = colors[i % colors.length];
                p.style.animationDelay = (Math.random() * 0.25) + 's';
                p.style.transform = `rotate(${Math.random() * 360}deg)`;
                container.appendChild(p);
            }
            setTimeout(() => container.remove(), 1400);
        }
    };
})();

window.phonicsDrag = (() => {
    let dotNetRef = null;
    let ghost = null;
    let activeTileIndex = -1;
    let startX = 0;
    let startY = 0;
    let moved = false;

    function clearHover() {
        document.querySelectorAll('.letter-slot.drop-hover').forEach(el => el.classList.remove('drop-hover'));
    }

    function findSlotAt(x, y) {
        const els = document.elementsFromPoint(x, y);
        for (const el of els) {
            if (el.dataset && el.dataset.slotIndex !== undefined) {
                return el;
            }
        }
        return null;
    }

    function onPointerMove(e) {
        if (activeTileIndex < 0) return;
        const dx = Math.abs(e.clientX - startX);
        const dy = Math.abs(e.clientY - startY);
        if (!moved && (dx > 6 || dy > 6)) {
            moved = true;
            if (ghost) ghost.style.display = 'flex';
        }
        if (moved && ghost) {
            ghost.style.left = e.clientX + 'px';
            ghost.style.top = e.clientY + 'px';
            clearHover();
            const slot = findSlotAt(e.clientX, e.clientY);
            if (slot) slot.classList.add('drop-hover');
        }
    }

    function onPointerUp(e) {
        if (activeTileIndex < 0) return;
        const tileIdx = activeTileIndex;
        const wasMoved = moved;

        window.removeEventListener('pointermove', onPointerMove);
        window.removeEventListener('pointerup', onPointerUp);
        window.removeEventListener('pointercancel', onPointerUp);
        clearHover();

        if (ghost) {
            ghost.remove();
            ghost = null;
        }
        activeTileIndex = -1;
        moved = false;

        if (wasMoved && dotNetRef) {
            const slot = findSlotAt(e.clientX, e.clientY);
            if (slot) {
                const slotIndex = parseInt(slot.dataset.slotIndex, 10);
                dotNetRef.invokeMethodAsync('OnTileDroppedOnSlot', tileIdx, slotIndex);
            }
        } else if (!wasMoved && dotNetRef) {
            dotNetRef.invokeMethodAsync('OnTileTapped', tileIdx);
        }
    }

    return {
        register(ref) {
            dotNetRef = ref;
        },
        startDrag(tileIdx, letter, isVowel, clientX, clientY) {
            activeTileIndex = tileIdx;
            startX = clientX;
            startY = clientY;
            moved = false;

            ghost = document.createElement('div');
            ghost.className = 'letter-tile drag-ghost' + (isVowel ? ' vowel-tile' : '');
            ghost.textContent = letter;
            ghost.style.display = 'none';
            ghost.style.left = clientX + 'px';
            ghost.style.top = clientY + 'px';
            document.body.appendChild(ghost);

            window.addEventListener('pointermove', onPointerMove, { passive: true });
            window.addEventListener('pointerup', onPointerUp);
            window.addEventListener('pointercancel', onPointerUp);
        }
    };
})();
