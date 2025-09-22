//basic.menu.js
$(document).ready(function () {

    let currentPage = '/main';

    // 封裝 active 狀態處理函數
    function setActiveMenuItem($item) {
        $('.xn-menu-item').removeClass('active');
        $item.addClass('active');
    }

    // 封裝內容載入函數
    function loadContent(url, pushState = true) {
        // 檢查並清理 Drawflow 實例
        if (window.Drawflow) {
            if (window.editor) {
                window.editor.clear();
                window.editor = null;
            }
        }
        // AJAX 載入內容
        const isExternalUrl = url.toLowerCase().includes('http');

        if (isExternalUrl) {
            // 顯示載入中狀態
            $('.xn-main-content').html(`
                <div class="loading-container">
                    <div class="loading-spinner"></div>
                    <div class="loading-text">載入中...</div>
                </div>
            `);

            // 創建iframe容器和樣式
            const iframeContainer = $('<div>').addClass('iframe-container');
            const style = $('<style>').text(`
                .iframe-container {
                    position: relative;
                    width: 100%;
                    height: calc(100vh - 72px);  /* 減去header高度 */
                    overflow: hidden;
                    background: #fff;
                    border-radius: 4px;
                    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                }
                .content-iframe {
                    width: 100%;
                    height: 100%;
                    border: none;
                }
            `);

            // 創建iframe
            const iframe = $('<iframe>').attr({
                'src': url,
                'class': 'content-iframe',
                'frameborder': '0'
            });

            // 監聽iframe載入完成
            iframe.on('load', function () {
                $('.loading-container').remove();
            });

            // 監聽iframe載入錯誤
            iframe.on('error', function () {
                $('.xn-main-content').html(`
                    <div class="error-container">
                        <svg class="error-icon" viewBox="0 0 24 24">
                            <use href="../images/system-icons/system-error.svg#uuid-b2050fff-d4a6-424d-abec-b415173bc239" />
                        </svg>
                        <div class="error-text">載入失敗</div>
                    </div>
                `);
            });

            // 添加到頁面
            $('.xn-main-content')
                .empty()
                .append(style)
                .append(iframeContainer.append(iframe));

        } else {
            // 內部頁面使用原有的Ajax載入方式
            $.ajax({
                url: url + '.html',
                method: 'GET',
                beforeSend: function () {
                    $('.xn-main-content').html(`
                        <div class="loading-container">
                            <div class="loading-spinner"></div>
                            <div class="loading-text">載入中...</div>
                        </div>
                    `);
                },
                success: function (response) {
                    $('.xn-main-content').html(response);
                    i18n.init();
                    currentPage = url;
                },
                error: function (xhr, status, error) {
                    $('.xn-main-content').html(`
                        <div class="error-container">
                            <svg class="error-icon" viewBox="0 0 24 24">
                                <use href="../images/system-icons/system-error.svg#uuid-b2050fff-d4a6-424d-abec-b415173bc239" />
                            </svg>
                            <div class="error-text">載入失敗</div>
                        </div>
                    `);
                    console.error('載入失敗:', error);
                }
            });
        }
    }

    // 點擊選單
    $('.xn-menu-item').click(function () {
        const $this = $(this);
        const url = $this.data('url');

        if (url && url !== currentPage) {
            setActiveMenuItem($this);
            loadContent(url);
        }
    });

    // 處理瀏覽器的前進/後退
    $(window).on('popstate', function (e) {
        if (e.originalEvent.state) {
            const url = e.originalEvent.state.page;
            const $menuItem = $(`.xn-menu-item[data-url="${url}"]`);
            if ($menuItem.length) {
                setActiveMenuItem($menuItem);
                loadContent(url, false);
            }
        }
    });

    // 初始載入：檢查當前 URL 並載入對應內容
    const path = window.location.pathname;
    if (path === '/' || path === '/index.html') {
        // 找到並設置 main 頁面的選單項目為 active
        const $mainMenuItem = $('.xn-menu-item[data-url="/main"]');
        if ($mainMenuItem.length) {
            setActiveMenuItem($mainMenuItem);
            loadContent('/main');
        }
    } else {
        const $menuItem = $(`.xn-menu-item[data-url="${path}"]`);
        if ($menuItem.length) {
            setActiveMenuItem($menuItem);
            loadContent(path, false);
        } else {
            // 如果找不到對應的選單項目，預設載入 main
            const $mainMenuItem = $('.xn-menu-item[data-url="/main"]');
            setActiveMenuItem($mainMenuItem);
            loadContent('/main');
        }
    }
});