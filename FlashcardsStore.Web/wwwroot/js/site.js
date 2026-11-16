// Kholoud Elnady English Academy - Global Application Scripts & HTMX Integration
// Zero inline scripts / Strict modular file

document.addEventListener('DOMContentLoaded', () => {
    // Toast Notification System with SVG Icons
    window.showToast = function(message, type = 'success') {
        const container = document.getElementById('toast-container');
        if (!container) return;

        const toast = document.createElement('div');
        toast.className = `toast ${type === 'error' ? 'toast-error' : 'toast-success'}`;
        
        const iconSvg = type === 'error' 
            ? `<svg class="w-5 h-5 text-rose-400 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/></svg>`
            : `<svg class="w-5 h-5 text-emerald-400 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>`;

        toast.innerHTML = `${iconSvg}<span class="text-sm font-bold text-white">${message}</span>`;
        
        container.appendChild(toast);

        setTimeout(() => {
            toast.style.opacity = '0';
            toast.style.transform = 'translateX(-100%)';
            setTimeout(() => toast.remove(), 300);
        }, 4000);
    };

    // Global HTMX Event Listeners
    // Attach CSRF RequestVerificationToken automatically to every HTMX request
    document.body.addEventListener('htmx:configRequest', (event) => {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        if (tokenInput && tokenInput.value) {
            event.detail.headers['RequestVerificationToken'] = tokenInput.value;
        }
    });

    document.body.addEventListener('htmx:afterOnLoad', (event) => {
        // Look for custom toast headers
        const toastMsg = event.detail.xhr.getResponseHeader('HX-Toast-Message');
        const toastType = event.detail.xhr.getResponseHeader('HX-Toast-Type') || 'success';
        if (toastMsg) {
            window.showToast(decodeURIComponent(toastMsg), toastType);
        }
    });

    document.body.addEventListener('htmx:responseError', (event) => {
        window.showToast('حدث خطأ أثناء تنفيذ العملية، يرجى المحاولة مرة أخرى.', 'error');
    });
});
