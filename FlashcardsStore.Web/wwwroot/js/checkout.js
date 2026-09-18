// FlashcardsStore - Checkout & Payment Proof Logic

document.addEventListener('DOMContentLoaded', () => {
    // Customer Type Card Selection
    const customerCards = document.querySelectorAll('.customer-type-card');
    customerCards.forEach(card => {
        card.addEventListener('click', () => {
            customerCards.forEach(c => c.classList.remove('selected'));
            card.classList.add('selected');
            const radio = card.querySelector('input[type="radio"]');
            if (radio) radio.checked = true;
        });
    });

    // Payment Method Card Selection
    const methodCards = document.querySelectorAll('.payment-method-card');
    methodCards.forEach(card => {
        card.addEventListener('click', () => {
            methodCards.forEach(c => c.classList.remove('selected'));
            card.classList.add('selected');
            const radio = card.querySelector('input[type="radio"]');
            if (radio) {
                radio.checked = true;
                togglePaymentInfo(radio.value);
            }
        });
    });

    function togglePaymentInfo(value) {
        const walletSection = document.getElementById('wallet-transfer-info');
        const whatsappSection = document.getElementById('whatsapp-direct-info');

        if (value === '1' || value === 'WalletTransfer') {
            if (walletSection) walletSection.style.display = 'block';
            if (whatsappSection) whatsappSection.style.display = 'none';
        } else {
            if (walletSection) walletSection.style.display = 'none';
            if (whatsappSection) whatsappSection.style.display = 'block';
        }
    }

    // Dynamic Shipping Calculator
    const govSelect = document.getElementById('governorate-select');
    const shippingDisplay = document.getElementById('shipping-cost-display');
    const grandTotalDisplay = document.getElementById('grand-total-display');
    const subtotalEl = document.getElementById('subtotal-amount');

    function updateShipping() {
        if (!govSelect || !shippingDisplay || !grandTotalDisplay || !subtotalEl) return;
        const gov = govSelect.value.trim();
        const subtotal = parseFloat(subtotalEl.dataset.subtotal || '0');
        const isLtr = document.documentElement.dir === 'ltr' || document.documentElement.lang === 'en';
        const currency = isLtr ? ' EGP' : ' ج.م';

        let shipping = 60;
        if (['القاهرة', 'الجيزة', 'Cairo', 'Giza'].includes(gov)) {
            shipping = 50;
        } else if (['الفيوم', 'بني سويف', 'المنيا', 'أسيوط', 'سوهاج', 'قنا', 'الأقصر', 'أسوان', 'البحر الأحمر', 'مطروح', 'الوادي الجديد', 'شمال سيناء', 'جنوب سيناء',
                    'Fayoum', 'Beni Suef', 'Minya', 'Asyut', 'Sohag', 'Qena', 'Luxor', 'Aswan', 'Red Sea', 'Matrouh', 'New Valley', 'North Sinai', 'South Sinai'].includes(gov)) {
            shipping = 75;
        }

        shippingDisplay.innerText = shipping.toFixed(0) + ' ' + currency;
        const total = subtotal + shipping;
        grandTotalDisplay.innerText = total.toFixed(0) + ' ' + currency;
    }

    if (govSelect) {
        govSelect.addEventListener('change', updateShipping);
        updateShipping();
    }

    // Copy to Clipboard Utility
    window.copyToClipboard = function(text, btnElement) {
        const isLtr = document.documentElement.dir === 'ltr' || document.documentElement.lang === 'en';
        navigator.clipboard.writeText(text).then(() => {
            const originalText = btnElement.innerText;
            btnElement.innerText = isLtr ? 'Copied! ✓' : 'تم النسخ! ✓';
            btnElement.classList.add('bg-green-100', 'text-green-700');
            setTimeout(() => {
                btnElement.innerText = originalText;
                btnElement.classList.remove('bg-green-100', 'text-green-700');
            }, 2500);
        }).catch(() => {
            const errorMsg = isLtr ? 'Failed to copy text' : 'فشل نسخ النص، يرجى نسخه يدوياً';
            window.showToast?.(errorMsg, 'error');
        });
    };

    // Receipt File Preview Handler
    const receiptInput = document.getElementById('receipt-file-input');
    const previewContainer = document.getElementById('receipt-preview-container');
    const previewImage = document.getElementById('receipt-preview-img');

    if (receiptInput && previewContainer && previewImage) {
        receiptInput.addEventListener('change', (e) => {
            const file = e.target.files?.[0];
            if (file && file.type.startsWith('image/')) {
                const reader = new FileReader();
                reader.onload = (re) => {
                    previewImage.src = re.target?.result;
                    previewContainer.style.display = 'block';
                };
                reader.readAsDataURL(file);
            } else {
                previewContainer.style.display = 'none';
            }
        });
    }
});
