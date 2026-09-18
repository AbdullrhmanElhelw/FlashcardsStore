// FlashcardsStore - Order Tracker Client Logic

document.addEventListener('DOMContentLoaded', () => {
    const trackForm = document.getElementById('order-track-form');
    if (trackForm) {
        trackForm.addEventListener('submit', (e) => {
            const orderNo = document.getElementById('track-order-number')?.value?.trim();
            const phone = document.getElementById('track-phone')?.value?.trim();

            if (!orderNo || !phone) {
                e.preventDefault();
                window.showToast?.('يرجى إدخال رقم الطلب ورقم الهاتف المسجل', 'error');
                return false;
            }
        });
    }
});
