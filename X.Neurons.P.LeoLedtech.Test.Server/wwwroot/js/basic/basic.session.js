// basic.session.js
class SessionService {
    static instance = null;

    constructor() {
        if (SessionService.instance) {
            return SessionService.instance;
        }

        // 初始化 toast
        this.toast = $(".system-message-toast").dxToast({
            displayTime: 3000
        }).dxToast("instance");
        console.log('初始化 toast');

        // 設定連線保持時間和重試策略
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/authHub")
            .withAutomaticReconnect([0, 2000, 5000, 10000]) // 重試間隔（毫秒）
            .build();

        this.setupEventHandlers();
        this.setupHeartbeat();
        SessionService.instance = this;
    }

    setupEventHandlers() {
        this.connection.onreconnecting(error => {
            console.log('正在重新連線:', error);
            this.showMessage("正在嘗試重新連線...", "warning", 0, true);
        });

        this.connection.onreconnected(connectionId => {
            console.log('已重新連線:', connectionId);
            this.showMessage("已重新連線", "success", 20000, false);
        });

        this.connection.onclose(error => {
            console.log('連線已關閉:', error);
            // 嘗試重新連線
            this.start();
        });

        this.connection.on("HeartbeatResponse", () => {
            console.log('收到心跳響應');
            this.lastHeartbeat = Date.now();
        });
    }

    setupHeartbeat() {
        this.heartbeatInterval = setInterval(async () => {
            if (this.connection.state === signalR.HubConnectionState.Connected) {
                try {
                    await this.connection.invoke("Heartbeat");
                    window.authService.validateTokenEx();
                } catch (error) {
                    console.error('心跳檢測失敗:', error);
                    // 嘗試重新連線
                    await this.start();
                }
            }
        }, 1000); // 改為 30 秒
    }

    showMessage(message, type, displayTime, isCancel) {
        if (this.toast) {
            this.toast.option({
                message: message,
                type: type,
                displayTime: displayTime,
                onHiding: function (e) {  
                    e.cancel = isCancel;  
                }  
            });
            this.toast.show();
        }
    }

    dispose() {
        if (this.heartbeatInterval) {
            clearInterval(this.heartbeatInterval);
            this.heartbeatInterval = null;
        }
        if (this.connection) {
            this.connection.stop();
        }
    }

    async start() {
        if (this.connection.state === "Connected") {
            return;
        }
        try {
            await this.connection.start();
            console.log("SignalR Connected");
        } catch (err) {
            console.error("SignalR Connection Error:", err);
            // 如果連線失敗，5秒後重試
            setTimeout(() => this.start(), 5000);
        }
    }

    getConnection() {
        return this.connection;
    }
}

// 創建全局實例
window.sessionService = new SessionService();