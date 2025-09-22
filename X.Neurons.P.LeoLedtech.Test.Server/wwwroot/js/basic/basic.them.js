$(document).ready(function () {
    // 主題相關功能
    function applyTheme(theme) {
        $('body').attr('data-theme', theme);
        // const icon = theme === 'dark' ? 'fa-sun' : 'fa-moon';
        // $('#theme-toggle i').removeClass('fa-sun fa-moon').addClass(icon);

        // console.log();
        DevExpress.ui.themes.current(theme === 'dark' ? 'generic.dark' : 'generic.light');
    }

    // 從 localStorage 讀取主題設置
    const savedTheme = localStorage.getItem('theme') || 'light';
    applyTheme(savedTheme);

    // 主題切換
    $('#theme-toggle').click(function () {
        const currentTheme = $('body').attr('data-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        localStorage.setItem('theme', newTheme);
        applyTheme(newTheme);
    });
});