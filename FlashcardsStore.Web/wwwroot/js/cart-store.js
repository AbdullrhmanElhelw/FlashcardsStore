// FlashcardsStore - Alpine.js Cart Store & Drawer Management

document.addEventListener('alpine:init', () => {
    Alpine.store('cart', {
        isOpen: false,
        count: 0,

        syncScroll() {
            if (this.isOpen) {
                document.body.classList.add('overflow-hidden');
            } else {
                document.body.classList.remove('overflow-hidden');
            }
        },

        toggle() {
            this.isOpen = !this.isOpen;
            this.syncScroll();
        },

        open() {
            this.isOpen = true;
            this.syncScroll();
        },

        close() {
            this.isOpen = false;
            this.syncScroll();
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

    // Handle Escape key to close cart and release scroll
    window.addEventListener('keydown', (e) => {
        if (e.key === 'Escape' && Alpine.store('cart') && Alpine.store('cart').isOpen) {
            Alpine.store('cart').close();
        }
    });
});
