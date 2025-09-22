window.AppLoader = {
    hideLoading: function() {
        $('.xn-loading-overlay').fadeOut(300, function() {
            $(this).remove();
            $('.xn-header, .xn-sidebar, .xn-main-content').css('visibility', '').hide().fadeIn(300);
        });
    },
    
    showLoading: function() {
        if ($('.xn-loading-overlay').length === 0) {
            const loadingHtml = `
                <div class="xn-loading-overlay">
                    <div class="loading-container">
                        <div class="loading-spinner"></div>
                        <div class="loading-text">載入中...</div>
                    </div>
                </div>
            `;
            $('body').prepend(loadingHtml);
        }
        $('.xn-loading-overlay').show();
        $('.xn-header, .xn-sidebar, .xn-main-content').css('visibility', 'hidden');
    }
};