/* ==========================================================================
   Admin SPA Navigation, Active Sidebar Sync & Per-Page Pagination Handler
   ========================================================================== */

(function () {
    'use strict';

    function resolveNavGroup(pathname) {
        var path = (pathname || '').toLowerCase();
        if (path.includes('/admin/orders') || path.includes('/admin/orderdetails')) return 'orders';
        if (path.includes('/admin/products') || path.includes('/admin/createproduct') || path.includes('/admin/editproduct')) return 'products';
        if (path.includes('/admin/categories') || path.includes('/admin/createcategory') || path.includes('/admin/editcategory')) return 'categories';
        if (path.includes('/admin/customers')) return 'customers';
        if (path.includes('/admin/reviews')) return 'reviews';
        if (path.includes('/admin/phonics')) return 'phonics';
        if (path.includes('/admin/settings')) return 'settings';
        return 'dashboard';
    }

    function syncAdminShellAfterSwap() {
        var group = resolveNavGroup(window.location.pathname);

        document.querySelectorAll('#admin-sidebar-nav .sidebar-link').forEach(function (el) {
            if (el.dataset.navGroup === group) {
                el.classList.add('is-active');
                el.classList.remove('text-slate-300');
            } else {
                el.classList.remove('is-active');
                el.classList.add('text-slate-300');
            }
        });

        var bodyData = document.body._x_dataStack && document.body._x_dataStack[0];
        if (bodyData) {
            bodyData.sidebarOpen = false;
        }

        var titleEl = document.getElementById('admin-page-title');
        if (titleEl && document.title) {
            titleEl.textContent = document.title.split(' - لوحة التحكم')[0];
        }
    }

    document.addEventListener('htmx:beforeSwap', function (e) {
        var target = e.detail && e.detail.target;
        if (!target || target.id !== 'admin-spa-content') return;

        var xhr = e.detail.xhr;
        var responseUrl = (xhr && xhr.responseURL ? xhr.responseURL : '').toLowerCase();
        var serverHtml = e.detail.serverResponse || '';

        if (responseUrl.includes('/admin/login') || serverHtml.indexOf('id="admin-spa-content"') === -1) {
            e.detail.shouldSwap = false;
            window.location.href = (xhr && xhr.responseURL) ? xhr.responseURL : '/Admin/Login';
        }
    });

    document.addEventListener('htmx:afterSettle', syncAdminShellAfterSwap);

    document.addEventListener('change', function (e) {
        var select = e.target && e.target.closest('[data-admin-page-size]');
        if (!select) return;

        var url = select.value;
        if (!url) return;

        if (window.htmx) {
            window.htmx.ajax('GET', url, {
                target: '#admin-spa-content',
                select: '#admin-spa-content',
                swap: 'innerHTML show:window:top'
            });
            window.history.pushState({}, '', url);
        } else {
            window.location.href = url;
        }
    });
})();
