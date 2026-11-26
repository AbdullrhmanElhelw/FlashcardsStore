window.phonicsDom = {
    setLocale(lang, dir) {
        document.documentElement.lang = lang || 'ar';
        document.documentElement.dir = dir || 'rtl';
    }
};
