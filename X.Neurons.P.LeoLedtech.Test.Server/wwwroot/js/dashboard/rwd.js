$(document).ready(function () {
    const newUrl = window.location.href.replace('.html', '');
    history.replaceState({}, '', newUrl);

    if (window.innerWidth <= 768) {
        const stationList = document.querySelector('.station-list');
        const leftArrow = document.querySelector('.scroll-arrow.left');
        const rightArrow = document.querySelector('.scroll-arrow.right');
        const scrollAmount = 200; // 每次滾動的距離

        leftArrow.addEventListener('click', () => {
            stationList.scrollBy({
                left: -scrollAmount,
                behavior: 'smooth'
            });
        });

        rightArrow.addEventListener('click', () => {
            stationList.scrollBy({
                left: scrollAmount,
                behavior: 'smooth'
            });
        });

        // 檢查滾動位置來顯示/隱藏箭頭
        stationList.addEventListener('scroll', () => {
            leftArrow.style.display =
                stationList.scrollLeft <= 0 ? 'none' : 'flex';
            rightArrow.style.display =
                stationList.scrollLeft >= (stationList.scrollWidth - stationList.clientWidth)
                    ? 'none' : 'flex';
        });

        // 初始檢查
        leftArrow.style.display = 'none';
    }

});