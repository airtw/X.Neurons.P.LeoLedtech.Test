// 等待 DOM 載入完成
$(document).ready(function () {
    // 獲取搜尋輸入框
    const searchInput = document.querySelector('.xn-dataflows-left-search input');

    // 搜尋功能
    function searchComponents() {
        const keyword = searchInput.value.toLowerCase();
        const blocks = document.querySelectorAll('.xn-component-block');
        
        blocks.forEach(block => {
            // 在每個區塊中尋找元件名稱
            const componentItems = block.querySelectorAll('.xn-component-text');
            let hasResults = false;

            // 檢查該區塊中是否有符合的元件
            componentItems.forEach(item => {
                const componentItem = item.closest('.xn-component-item');
                if (componentItem) {
                    if (item.textContent.toLowerCase().includes(keyword)) {
                        componentItem.style.display = '';
                        hasResults = true;
                    } else {
                        componentItem.style.display = 'none';
                    }
                }
            });

            // 根據是否有結果來顯示或隱藏整個區塊
            block.style.display = hasResults || keyword === '' ? '' : 'none';
        });

        // 處理無任何結果的情況
        const hasAnyResults = Array.from(blocks).some(block => 
            block.style.display !== 'none'
        );

        // 移除舊的無結果訊息
        const oldMsg = document.querySelector('.xn-no-results-message');
        if (oldMsg) oldMsg.remove();

        // 如果完全沒有結果且有搜尋關鍵字，顯示訊息
        if (!hasAnyResults && keyword) {
            const containerElement = document.querySelector('.xn-dataflows-components');
            const noResultsMsg = document.createElement('div');
            noResultsMsg.className = 'xn-no-results-message';
            noResultsMsg.style.cssText = `
                padding: 1rem;
                text-align: center;
                color: var(--text-color);
                opacity: 0.7;
            `;
            noResultsMsg.textContent = `找不到符合「${keyword}」的元件`;
            containerElement.appendChild(noResultsMsg);
        }
    }

    // 註冊搜尋輸入事件
    if (searchInput) {
        searchInput.addEventListener('input', searchComponents);
    }
});