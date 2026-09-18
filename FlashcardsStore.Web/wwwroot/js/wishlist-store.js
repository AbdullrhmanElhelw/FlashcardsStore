// FlashcardsStore - Alpine.js Wishlist Store
document.addEventListener('alpine:init', () => {
    Alpine.store('wishlist', {
        items: JSON.parse(localStorage.getItem('fc_wishlist') || '[]'),

        has(id) {
            const numId = parseInt(id, 10);
            return this.items.includes(numId);
        },

        toggle(id, title) {
            const numId = parseInt(id, 10);
            const index = this.items.indexOf(numId);
            let added = false;
            if (index > -1) {
                this.items.splice(index, 1);
            } else {
                this.items.push(numId);
                added = true;
            }
            localStorage.setItem('fc_wishlist', JSON.stringify(this.items));

            // Trigger Toast
            const isAr = document.documentElement.getAttribute('dir') === 'rtl';
            const msg = added 
                ? (isAr ? 'تمت إضافة الباقة للمفضلة بنجاح ❤️' : 'Pack added to wishlist ❤️')
                : (isAr ? 'تمت إزالة الباقة من المفضلة' : 'Pack removed from wishlist');
            
            if (window.showToast) {
                window.showToast(msg, added ? 'success' : 'info');
            }
            return added;
        },

        remove(id) {
            const numId = parseInt(id, 10);
            const index = this.items.indexOf(numId);
            if (index > -1) {
                this.items.splice(index, 1);
                localStorage.setItem('fc_wishlist', JSON.stringify(this.items));
            }
        },

        get count() {
            return this.items.length;
        }
    });
});
