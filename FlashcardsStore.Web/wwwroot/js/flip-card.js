// FlashcardsStore - 3D Flip Card Handler
// Flipping ONLY occurs when user clicks the dedicated flip button ([data-action="flip-card"], .flip-hint-badge, .flip-back-btn).
// Standard clicks on card image, title, or buttons navigate to product details or perform actions.

document.addEventListener('DOMContentLoaded', () => {
    // Dedicated Flip Button Click Handler
    document.addEventListener('click', (e) => {
        const flipBtn = e.target.closest('[data-action="flip-card"], .flip-hint-badge, .flip-back-btn');
        if (flipBtn) {
            e.preventDefault();
            e.stopPropagation();
            const flipCard = flipBtn.closest('.flip-card');
            if (flipCard) {
                flipCard.classList.toggle('is-flipped');
            }
            return;
        }

        // Reset any flipped cards when clicking outside all cards
        if (!e.target.closest('.flip-card')) {
            document.querySelectorAll('.flip-card.is-flipped').forEach(card => {
                card.classList.remove('is-flipped');
            });
        }
    });
});

