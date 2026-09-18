// FlashcardsStore - Alpine.js Cart Store & Drawer Management

document.addEventListener('alpine:init', () => {
    Alpine.store('cart', {
        isOpen: false,
        count: 0,

        toggle() {
            this.isOpen = !this.isOpen;
        },

        open() {
            this.isOpen = true;
        },

        close() {
            this.isOpen = false;
        },

        setCount(val) {
            this.count = val;
            const badge = document.getElementById('navbar-cart-badge');
            if (badge) {
                badge.classList.remove('cart-badge-pulse');
                void badge.offsetWidth; // Trigger reflow
                badge.classList.add('cart-badge-pulse');
            }
        }
    });

    // Listen for custom HTMX events to auto-open drawer or update counter
    document.body.addEventListener('cartUpdated', (e) => {
        if (e.detail && typeof e.detail.count !== 'undefined') {
            Alpine.store('cart').setCount(e.detail.count);
        }
        if (e.detail && e.detail.openDrawer) {
            Alpine.store('cart').open();
        }
    });
});
