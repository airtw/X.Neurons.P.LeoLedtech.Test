//basic.menu.js
$(document).ready(function () {

    // 全域常量定義
    var UPDATE_INTERVAL = 1000; // 更新間隔（毫秒）

    // 全域狀態管理
    const state = {
        isPauseAutoScroll: false,
        pauseTimer: null,
        isManualScroll: false,
        currentActiveStation: null
    };

    // 服務初始化
    const sessionService = window.sessionDashboardService;
    const connection = sessionService.getConnection();



    // 站點資訊更新函數
    // async function renderStationInfo() {
    //     try {
    //         const response = await connection.invoke("StationsInfo");
    //         if (!response) {
    //             throw new Error('無效的站點資料格式');
    //         }
    //         console.log('資料',response);
    //         // 尋找並更新活動站點的資訊
    //         for (const station of response) {
    //             if (!station.name) continue;

    //             // const isActive = isActiveStation(station);
    //             // if (isActive) {
    //             //     // updateStationDisplay(station);
    //             //     break; // 找到活動站點後就停止搜尋
    //             // }
    //         }
    //     } catch (error) {
    //         handleError(error, 'renderStationInfo');
    //     }
    // }

    // 設置事件監聽器
    function setupEventListeners() {

        // 添加SignalR重連事件監聽
        connection.onreconnected(async connectionId => {
            try {
                console.log('SignalR reconnected:', connectionId);
                // 重新獲取設定
                // await updateConfig();
            } catch (error) {
                handleError(error, 'reconnectionHandler');
            }
        });
        // SignalR斷線重連相關事件
        connection.onreconnecting(error => {
            try {
                console.log('SignalR reconnecting:', error);
                // clearAllData();
            } catch (error) {
                handleError(error, 'reconnectingHandler');
            }
        });
        connection.onclose(error => {
            try {
                console.log('SignalR connection closed:', error);
                // clearAllData();
            } catch (error) {
                handleError(error, 'closeHandler');
            }
        });
    }



    // 錯誤處理輔助函數
    function handleError(error, context) {
        console.error(`Error in ${context}:`, error);
    }

    // 主程式初始化
    async function initialize() {
        try {

            DevExpress.localization.locale('zh-tw');
            // 啟動連接
            await sessionService.start();
            console.log('Session service started successfully.');


            // 初始渲染
            // await Promise.all([
            //     renderStationInfo()
            // ]);

            // 設置事件監聽器
            setupEventListeners();

            // 設置定時更新
            // setupIntervalUpdates();

        } catch (error) {
            handleError(error, 'initialize');
            throw error; // 重拋錯誤以通知調用者初始化失敗
        }
    }

    // 啟動主程式
    initialize().catch(error => {
        console.error('程式初始化失敗:', error);
        // 這裡可以添加更多錯誤處理邏輯，如顯示錯誤提示等
    });
});