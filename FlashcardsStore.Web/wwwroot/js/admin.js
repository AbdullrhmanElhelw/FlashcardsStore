// FlashcardsStore - Admin Portal Interactive Controls

document.addEventListener('DOMContentLoaded', () => {
    // Receipt Modal Viewer
    window.openReceiptModal = function(imageUrl, title, details) {
        const modal = document.getElementById('receipt-modal');
        const img = document.getElementById('modal-receipt-img');
        const titleEl = document.getElementById('modal-receipt-title');
        const detailsEl = document.getElementById('modal-receipt-details');

        if (modal && img) {
            img.src = imageUrl;
            if (titleEl) titleEl.innerText = title || 'إيصال التحويل البنكي / المحفظة';
            if (detailsEl) detailsEl.innerText = details || '';
            modal.style.display = 'flex';
        }
    };

    window.closeReceiptModal = function() {
        const modal = document.getElementById('receipt-modal');
        if (modal) {
            modal.style.display = 'none';
        }
    };

    // Close on Escape key
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            window.closeReceiptModal();
        }
    });

    // Close on clicking backdrop
    const modalBackdrop = document.getElementById('receipt-modal');
    if (modalBackdrop) {
        modalBackdrop.addEventListener('click', (e) => {
            if (e.target === modalBackdrop) {
                window.closeReceiptModal();
            }
        });
    }

    // Confirmation on Approve/Reject
    window.confirmOrderAction = function(actionName, orderNumber) {
        return confirm(`هل أنت متأكد من رغبتك في ${actionName} الطلب رقم ${orderNumber}؟`);
    };
});

// Alpine.js components for Product Card Image Uploads & Management
function registerProductComponents() {
    if (typeof Alpine === 'undefined') return;

    Alpine.data('productImageUploader', () => ({
        previews: [],
        isDragging: false,

        handleFiles(event) {
            const files = event.target.files || (event.dataTransfer ? event.dataTransfer.files : []);
            if (!files || files.length === 0) return;
            
            for (let i = 0; i < files.length; i++) {
                const file = files[i];
                if (file.type && file.type.startsWith('image/')) {
                    const reader = new FileReader();
                    reader.onload = (e) => {
                        this.previews.push({
                            name: file.name,
                            size: (file.size / 1024).toFixed(1) + ' KB',
                            url: e.target.result
                        });
                    };
                    reader.readAsDataURL(file);
                }
            }
        },

        clearAll() {
            this.previews = [];
            const input = document.getElementById('imageFilesInput');
            if (input) input.value = '';
        },

        removePreview(index) {
            this.previews.splice(index, 1);
        }
    }));

    Alpine.data('productImageEditor', (initialExisting) => ({
        existingImages: initialExisting ? initialExisting.split(',').map(s => s.trim()).filter(Boolean) : [],
        newPreviews: [],
        isDragging: false,

        removeExisting(index) {
            this.existingImages.splice(index, 1);
        },

        removeNewPreview(index) {
            this.newPreviews.splice(index, 1);
        },

        handleFiles(event) {
            const files = event.target.files || (event.dataTransfer ? event.dataTransfer.files : []);
            if (!files || files.length === 0) return;
            
            for (let i = 0; i < files.length; i++) {
                const file = files[i];
                if (file.type && file.type.startsWith('image/')) {
                    const reader = new FileReader();
                    reader.onload = (e) => {
                        this.newPreviews.push({
                            name: file.name,
                            size: (file.size / 1024).toFixed(1) + ' KB',
                            url: e.target.result
                        });
                    };
                    reader.readAsDataURL(file);
                }
            }
        },

        clearNewFiles() {
            this.newPreviews = [];
            const input = document.getElementById('editImageFilesInput');
            if (input) input.value = '';
        },

        get existingImagesJoined() {
            return this.existingImages.join(',');
        }
    }));
}

document.addEventListener('alpine:init', registerProductComponents);
if (typeof Alpine !== 'undefined') {
    registerProductComponents();
}
