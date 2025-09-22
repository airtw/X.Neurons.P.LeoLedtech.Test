const i18n = {
    currentLanguage: localStorage.getItem('language') || 'zh-TW',

    setLanguage(lang) {
        if (window.i18n[this.getLangKey(lang)]) {
            this.currentLanguage = lang;
        }
        $('.i18n').each(function () {
            const $el = $(this);
            const key = $el.data('key');

            // 處理不同類型的元素
            if ($el.is('input')) {
                $el.attr('placeholder', t(key));
            } else if ($el.is('span')) {
                $el.html(t(key));  // 使用 html() 來處理 span
            } else if ($el.is('h1')) {
                $el.html(t(key));  // 使用 html() 來處理 span
            }  else {
                $el.text(t(key));
            }
        });
    },

    getLangKey(lang) {
        // 將 'zh-TW' 轉換為 'zhTW' 以匹配對象名稱
        return lang.replace('-', '');
    },

    getText(key) {
        const langKey = this.getLangKey(this.currentLanguage);
        if (!window.i18n[langKey] || !window.i18n[langKey][key]) {
            console.warn(`Missing translation: ${key} for language: ${this.currentLanguage}`);
            return key;
        }
        return window.i18n[langKey][key];
    },

    init() {
        this.setLanguage(this.currentLanguage);
    }
};
// 定義全域的簡短方法
window.t = function (key) {
    return i18n.getText(key);
};
$(document).ready(function () {
    i18n.init();
});