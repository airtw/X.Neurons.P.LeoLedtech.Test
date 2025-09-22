// basic.auth.js
class AuthService {
    constructor() {
        if (AuthService.instance) {
            return AuthService.instance;
        }

        this.connection = window.sessionService.getConnection();
        this.setupEventHandlers();
        this.setupLoadingControl();
        // 在constructor中初始化事件監聽
        this.initializeUserControls();
        AuthService.instance = this;
    }
    // 添加新方法處理用戶控制項
    initializeUserControls() {
        document.addEventListener('DOMContentLoaded', () => {
            const userInfo = document.querySelector('.user-info');
            const userDropdown = document.querySelector('.user-dropdown');
            const logoutButton = document.querySelector('.logout-button');
            const userDropdownName = document.querySelector('.user-dropdown-name');

            if (!userInfo || !userDropdown || !logoutButton || !userDropdownName) {
                return; // 如果元素不存在就返回
            }

            // 設置用戶名
       
            const userName = localStorage.getItem('user');
            if (userName) {
                const firstChar = userName.charAt(0).toUpperCase();
                document.querySelector('.user-avatar-small span').textContent = firstChar;
                userDropdownName.textContent = localStorage.getItem('user');
            }

            // 點擊用戶資訊時顯示/隱藏下拉選單
            userInfo.addEventListener('click', e => {
                userDropdown.classList.toggle('show');
                e.stopPropagation();
            });

            // 點擊其他地方時關閉下拉選單
            document.addEventListener('click', e => {
                if (!userInfo.contains(e.target)) {
                    userDropdown.classList.remove('show');
                }
            });

            // 處理登出 - 使用現有的logout方法
            logoutButton.addEventListener('click', () => {
                DevExpress.ui.notify({
                    message: '已成功登出',
                    position: {
                        my: 'center top',
                        at: 'center top',
                    },
                }, 'success', 2000);

                // 延遲後登出
                setTimeout(() => this.logout(), 1000);
            });
        });
    }
    setupLoadingControl() {
        // 隱藏主內容
        $('.xn-header, .xn-sidebar, .xn-main-content').css('visibility', 'hidden');

        // 確保loading overlay存在
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
    }

    hideLoading() {
        $('.xn-loading-overlay').fadeOut(300, function () {
            $(this).remove();
            $('.xn-header, .xn-sidebar, .xn-main-content').css('visibility', '').hide().fadeIn(300);
        });
    }

    setupEventHandlers() {
        this.connection.on("ForceLogout", () => {
            this.logout();
        });
    }

    updateUserInfo(fullName, role) {
        if ($('.user-name').length != 0) {
            document.querySelector('.user-name').textContent = fullName;
        }
        if ($('.user-role').length != 0) {
            document.querySelector('.user-role').textContent = role;
        }
        if ($('.user-avatar span').length != 0) {
            if (fullName != null) {
                document.querySelector('.user-avatar span').textContent = fullName.charAt(0).toUpperCase();
                // document.querySelector('.user-avatar-small span').textContent = fullName.charAt(0).toUpperCase();
            }

        }
        // document.querySelector('.user-name').textContent = fullName;
        // document.querySelector('.user-role').textContent = role;
        // document.querySelector('.user-avatar span').textContent = fullName.charAt(0).toUpperCase();
    }

    async validateToken(token) {
        try {
            const result = await this.connection.invoke("ValidateToken", token);

            if (!result || !result.isValid) {
                authService.logout();
                return false;
            }
            return true;
        } catch (error) {
            console.error('Token validation error:', error);
            return false;
        }
    }

    async validateTokenEx() {
        const authService = window.authService;
        const sessionService = window.sessionService;
        const currentPath = window.location.pathname;
        const token = authService.getToken();

        // 先啟動連線
        await sessionService.start();

        // if (currentPath === '/') {
        //     window.location.href = '/test.html';
        //     return;
        // }

        // this.updateUserInfo('John Doe', 'Administrator');

        this.updateUserInfo(localStorage.getItem('user'), localStorage.getItem('role'));

        switch (currentPath) {
            case '/':
                window.location.href = '/index.html';
                return
            case '/dashboard':
                window.location.href = '/dashboard.html';
                return
            // 如果是登入頁面
            case '/login.html':
                if (token) {
                    try {
                        const isValid = await authService.validateToken(token);
                        if (isValid) {
                            const urlParams = new URLSearchParams(window.location.search);
                            const returnUrl = urlParams.get('returnUrl');
                            window.location.href = returnUrl || '/index.html';
                        }
                    } catch (error) {
                        console.error('Token validation error:', error);
                        this.hideLoading();
                    }
                } else {
                    // console.log('在登入頁面且沒有token');
                    this.hideLoading();
                }
                return;
            case '/login':
                if (token) {
                    try {
                        const isValid = await authService.validateToken(token);
                        if (isValid) {
                            window.location.href = '/index.html';
                        }
                    } catch (error) {
                        console.error('Token validation error:', error);
                    }
                } else {
                    // console.log('在登入頁面且沒有token');
                }
                return;
            case '/admin':
                if (!token) {

                    window.location.href = '/login.html';
                    return;
                } else {
                    const isValid = await authService.validateToken(token);
                    this.hideLoading();
                }
                return;
            case '/main':
                if (!token) {
                    window.location.href = '/login.html';
                    return;
                } else {
                    const isValid = await authService.validateToken(token);
                    this.hideLoading();
                }
                return;
            case '/index.html':
                if (!token) {
                    window.location.href = '/login.html';
                    return;
                } else {
                    const isValid = await authService.validateToken(token);
                    this.hideLoading();
                }
                return;
            case '/xn-system-config':
                if (!token) {
                    window.location.href = '/login.html';
                    return;
                } else {
                    const isValid = await authService.validateToken(token);
                    this.hideLoading();
                }
                return;
            case '/xn-warn-message':
                if (!token) {
                    window.location.href = '/login.html';
                    return;
                } else {
                    const isValid = await authService.validateToken(token);
                    this.hideLoading();
                }
                return;
            case '/xn-equipment-history':
                if (!token) {
                    window.location.href = '/login.html';
                    return;
                } else {
                    const isValid = await authService.validateToken(token);
                    this.hideLoading();
                }
                return;
            case '/xn-equipment-operate':
                if (!token) {
                    window.location.href = '/login.html';
                    return;
                } else {
                    const isValid = await authService.validateToken(token);
                    this.hideLoading();
                }
                return;
            default:
                // console.log('跳走的路徑', currentPath);
                window.location.href = '/dashboard.html';
        }
        try {
            const isValid = await authService.validateToken(token);
            // console.log('驗證結果',isValid);
            if (!isValid) {
                authService.logout();
            } else {
                this.hideLoading();
            }
        } catch (error) {
            console.error('Authentication error:', error);
            this.hideLoading();
        }
    }



    getToken() {
        return localStorage.getItem('token');
    }

    setToken(token) {
        localStorage.setItem('token', token);
    }

    removeToken() {
        localStorage.removeItem('token');
    }

    logout() {
        console.log('登出');
        this.removeToken();
        window.location.href = '/login.html';
    }

    isTokenExpired(token) {
        if (!token) {
            console.log('沒有找到 token');
            return true;
        }

        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            const currentTime = Date.now();
            const expirationTime = payload.exp * 1000;
            const isExpired = expirationTime < currentTime;

            console.log('Token 資訊:', {
                當前時間: new Date(currentTime).toLocaleString(),
                過期時間: new Date(expirationTime).toLocaleString(),
                是否過期: isExpired
            });

            return isExpired;
        } catch (error) {
            console.error('Token 解析錯誤:', error);
            return true;
        }
    }

    getConnection() {
        return this.connection;
    }
}

// 創建全局實例
window.authService = new AuthService();

// 初始化權限驗證
$(document).ready(async function () {
    const authService = window.authService;
    await authService.validateTokenEx();

});