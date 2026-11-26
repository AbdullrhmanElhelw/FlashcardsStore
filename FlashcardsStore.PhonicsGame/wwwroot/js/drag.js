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
