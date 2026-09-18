// FlashcardsStore - Dedicated 3D Flip Card Mobile Interaction Handler

document.addEventListener('DOMContentLoaded', () => {
    // Click / Tap Flip Handler across desktop and mobile
    document.addEventListener('click', (e) => {
        const flipCard = e.target.closest('.flip-card');
        if (!flipCard) return;

        // If clicked on an action button, link, or input inside the card, do not toggle the flip
        if (e.target.closest('button') || e.target.closest('a') || e.target.closest('input')) {
            return;
        }

        // Deliberate click on card canvas flips the card smoothly
        flipCard.classList.toggle('is-flipped');
    });

    // Reset flip when clicking outside
    document.addEventListener('click', (e) => {
        if (!e.target.closest('.flip-card')) {
            document.querySelectorAll('.flip-card.is-flipped').forEach(card => {
                card.classList.remove('is-flipped');
            });
        }
    });
});
