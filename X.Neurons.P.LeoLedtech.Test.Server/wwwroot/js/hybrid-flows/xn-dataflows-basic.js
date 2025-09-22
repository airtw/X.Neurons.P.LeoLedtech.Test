$(() => {
//----------------------------
// 初始化 Drawflow
var container = document.getElementById("drawflow");
var editor = new Drawflow(container);
editor.useuuid = true;

editor.start();

// 監聽連接事件
editor.on('connectionCreated', function (connection) {
    console.log('Connection created', connection);
});


// 監聽節點選擇事件
editor.on('nodeSelected', function (nodeId) {
    console.log('Node selected', nodeId);
    const nodeData = editor.getNodeFromId(nodeId);
    if (nodeData && nodeData.data.customData) {
        console.log('Custom Data:', nodeData.data.customData);
    }
});

editor.container.addEventListener('contextmenu', function (e) {
    const targetNode = e.target.closest('.drawflow-node');

    if (targetNode) {
        e.preventDefault(); // 阻止原本的右鍵行為
        const nodeId = targetNode.getAttribute('id');
        console.log(`右鍵點擊節點: ${nodeId}`);

        // 添加自定義的 dxContextMenu
        $("#customContextMenu").dxContextMenu({
            items: [
                { text: "查看詳情" },
                { text: "刪除節點" },
            ],
            target: targetNode,
            onItemClick: function (e) {
                if (e.itemData.text === "刪除節點") {
                    console.log(`刪除節點: ${nodeId}`);
                    editor.removeNodeId(nodeId); // 移除節點
                } else if (e.itemData.text === "查看詳情") {
                    console.log(`查看節點: ${nodeId}`);
                }
            }
        }).dxContextMenu("instance").show();
    } else {
        console.log("未點擊節點，保留原功能");
    }
});

// 添加連接成功的回調
editor.on('translate', function (connection) {
    // console.log('拖曳節點', connection);
});

// 添加連接成功的回調
editor.on('connectionCreated', function (connection) {
    console.log('成功連接節點！', connection);
});

var elements = document.getElementsByClassName('drag-drawflow');
for (var i = 0; i < elements.length; i++) {
    elements[i].addEventListener('touchend', drop, false);
    elements[i].addEventListener('touchmove', positionMobile, false);
    elements[i].addEventListener('touchstart', drag, false);
}

var mobile_item_selec = '';
var mobile_last_move = null;
function positionMobile(ev) {
    mobile_last_move = ev;
}

function allowDrop(ev) {

    ev.preventDefault();
}

function drag(ev) {
    if (ev.type === "touchstart") {
        mobile_item_selec = ev.target.closest(".drag-drawflow").getAttribute('data-node');
    } else {
        console.log('drag ----------');
        // 拖動開始時，設置資料
        ev.dataTransfer.setData("node", ev.target.getAttribute('data-node'));
        // 可以設置拖動時的效果
        ev.dataTransfer.effectAllowed = "move";
    }
}

function drop(ev) {
    if (ev.type === "touchend") {
        var parentdrawflow = document.elementFromPoint(mobile_last_move.touches[0].clientX, mobile_last_move.touches[0].clientY).closest("#drawflow");
        if (parentdrawflow != null) {
            console.log('drop ----------');
            addNodeToDrawFlow(mobile_item_selec, mobile_last_move.touches[0].clientX, mobile_last_move.touches[0].clientY);
        }
        mobile_item_selec = '';
    } else {
        ev.preventDefault();
        // 獲取拖動的節點類型
        const nodeType = ev.dataTransfer.getData("node");

        // 獲取畫布元素
        const drawflow = document.getElementById("drawflow");
        const rect = drawflow.getBoundingClientRect();

        // 計算相對於畫布的位置
        const x = ev.clientX - rect.left;
        const y = ev.clientY - rect.top;

        addNodeToDrawFlow(nodeType, ev.clientX, ev.clientY);
    }
}

function myFunction() {
    var exportdata = editor.export();
    const jsonString = JSON.stringify(exportdata);
    console.log(jsonString);
}

function addNodeToDrawFlow(name, pos_x, pos_y) {
    if (editor.editor_mode === 'fixed') {
        return false;
    }

    // 獲取畫布的位置和縮放資訊
    const rect = editor.precanvas.getBoundingClientRect();
    const zoom = editor.zoom;

    // 計算實際位置
    pos_x = (pos_x - rect.left) / zoom;
    pos_y = (pos_y - rect.top) / zoom;

    switch (name) {
        // #region::System
        case 'system-start-process':
            var nodeName = 'system-start-process';
            var content = `
            <div class="node-content">
                <svg class="node-icon" viewBox="0 0 24 24">
                    <use href="../images/data-flows/widget/system-start.svg#uuid-0fbaa938-5cf6-479a-99f3-41ea40ca24ab" />
                </svg>
                <span class="node-title">開始</span>
            </div>
            `;
            editor.addNode(nodeName, 0, 1, pos_x, pos_y, nodeName, {}, content);
            break;
        case 'print-console':
            var nodeName = 'print-console';
            var inputdata = [
                { text: 'IIII' },
                { text: 'IIII2' }
            ];
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
            <div class="node-content">
                <svg class="node-icon" viewBox="0 0 24 24">
                    <use href="../images/data-flows/widget/system-print.svg#uuid-968568b6-66b7-4176-9be5-815c7bba629b" />
                </svg>
                <span class="node-title">程式印出</span>
            </div>
                `;
            editor.addNode(nodeName, 1, 3, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
        case 'system-logs':
            var nodeName = 'system-logs';
            var content = `
                <div class="node-content">
                    <svg class="node-icon" viewBox="0 0 24 24">
                        <use href="../images/data-flows/widget/system-logs.svg#uuid-e5eaf2cd-365c-4fb6-a2a7-cd1ebf7e9db2" />
                    </svg>
                    <span class="node-title">系統Logs</span>
                </div>
                `;
            editor.addNode(nodeName, 1, 1, pos_x, pos_y, nodeName, {}, content);
            break;
        // #endregion::System
        // #region::Import
        case 'import-api':
            var nodeName = 'import-api';
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
            <div class="node-content">
                <svg class="node-icon" viewBox="0 0 24 24">
                    <use href="../images/data-flows/widget/import-api.svg#uuid-ef83fe61-4c19-4fd3-847a-6cfa3324cef4" />
                </svg>
                <span class="node-title">來源API</span>
            </div>
            `;
            editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
        case 'import-sftp':
            var nodeName = 'import-sftp';
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
            <div class="node-content">
                <svg class="node-icon" viewBox="0 0 24 24">
                    <use href="../images/data-flows/widget/import-sftp.svg#uuid-f25c702e-56e1-44dc-a9bb-90e9ebd7bd41" />
                </svg>
                <span class="node-title">來源SFTP</span>
            </div>
                `;
            editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
        case 'import-ftp':
            var nodeName = 'import-ftp';
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
                <div class="node-content">
                    <svg class="node-icon" viewBox="0 0 24 24">
                        <use href="../images/data-flows/widget/import-ftp.svg#uuid-c6b107fc-1a48-409f-bd9b-f85c574e078e" />
                    </svg>
                    <span class="node-title">來源FTP</span>
                </div>
                `;
                editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
        case 'import-file':
            var nodeName = 'import-file';
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
                    <div class="node-content">
                        <svg class="node-icon" viewBox="0 0 24 24">
                            <use href="../images/data-flows/widget/import-file.svg#uuid-d0531f85-d0ce-46c5-a29f-ddc1e48c50c1" />
                        </svg>
                        <span class="node-title">來源檔案</span>
                    </div>
                    `;
                    editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
        // #endregion::Import
        // #region::Export
        case 'export-api':
            var nodeName = 'export-api';
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
            <div class="node-content">
                <svg class="node-icon" viewBox="0 0 24 24">
                    <use href="../images/data-flows/widget/export-api.svg#uuid-9c390665-16d7-42f3-a8a9-512d452ca46c" />
                </svg>
                <span class="node-title">目標API</span>
            </div>
            `;
            editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
        case 'export-sftp':
            var nodeName = 'export-sftp';
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
            <div class="node-content">
                <svg class="node-icon" viewBox="0 0 24 24">
                    <use href="../images/data-flows/widget/export-sftp.svg#uuid-5feaa0df-0f0d-40fb-97e4-0af78078cdc3" />
                </svg>
                <span class="node-title">目標SFTP</span>
            </div>
                `;
            editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
        case 'export-ftp':
            var nodeName = 'export-ftp';
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
                <div class="node-content">
                    <svg class="node-icon" viewBox="0 0 24 24">
                        <use href="../images/data-flows/widget/export-ftp.svg#uuid-9b1c1d2f-7288-42af-9a48-ed7ac97b5f5f" />
                    </svg>
                    <span class="node-title">目標FTP</span>
                </div>
                `;
                editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
        case 'export-file':
            var nodeName = 'export-file';
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
                    <div class="node-content">
                        <svg class="node-icon" viewBox="0 0 24 24">
                            <use href="../images/data-flows/widget/export-file.svg#uuid-8c75a45b-7e6e-449b-8e29-2d5ce1ef667b" />
                        </svg>
                        <span class="node-title">目標檔案</span>
                    </div>
                    `;
                    editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
        // #endregion::Export
        // #region::Format
        case 'format-xml':
            var nodeName = 'format-xml';
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
            <div class="node-content">
                <svg class="node-icon" viewBox="0 0 24 24">
                    <use href="../images/data-flows/widget/format-xml.svg#uuid-27e06798-8621-44da-bc1a-30d86bfceef1" />
                </svg>
                <span class="node-title">XML</span>
            </div>
            `;
            editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
        case 'format-json':
            var nodeName = 'format-json';
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
            <div class="node-content">
                <svg class="node-icon" viewBox="0 0 24 24">
                    <use href="../images/data-flows/widget/format-json.svg#uuid-930a4ea0-6e65-4544-8e3d-24f18a5f3284" />
                </svg>
                <span class="node-title">JSON</span>
            </div>
                `;
            editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
        case 'format-xlsx':
            var nodeName = 'format-xlsx';
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
                <div class="node-content">
                    <svg class="node-icon" viewBox="0 0 24 24">
                        <use href="../images/data-flows/widget/format-xlsx.svg#uuid-a74f76e7-5fdf-442e-af32-fa8894550db0" />
                    </svg>
                    <span class="node-title">目標XLSX</span>
                </div>
                `;
                editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
        case 'format-csv':
            var nodeName = 'format-csv';
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
                    <div class="node-content">
                        <svg class="node-icon" viewBox="0 0 24 24">
                            <use href="../images/data-flows/widget/format-csv.svg#uuid-1ccea502-429e-48ce-9159-e1ca06fa9a7e" />
                        </svg>
                        <span class="node-title">CSV</span>
                    </div>
                    `;
                    editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
            case 'format-txt':
                var nodeName = 'format-txt';
                var outputdata = [
                    { text: 'BBBBB', type: 'success' },
                    { text: 'BBBB', type: 'fail' }
                ];
                var content = `
                        <div class="node-content">
                            <svg class="node-icon" viewBox="0 0 24 24">
                                <use href="../images/data-flows/widget/format-txt.svg#uuid-5ddfd778-5ffe-4926-bafd-cd2578b16c2b" />
                            </svg>
                            <span class="node-title">TXT</span>
                        </div>
                        `;
                        editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
                break;
        // #endregion::Format
        // #region::Transform
        case 'transform-sort':
            var nodeName = 'transform-sort';
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
            <div class="node-content">
                <svg class="node-icon" viewBox="0 0 24 24">
                    <use href="../images/data-flows/widget/transform-sort.svg#uuid-4aba36d6-a02d-422f-b27b-4376fdb7fc5e" />
                </svg>
                <span class="node-title">排序</span>
            </div>
            `;
            editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
        case 'transform-filters':
            var nodeName = 'transform-filters';
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
            <div class="node-content">
                <svg class="node-icon" viewBox="0 0 24 24">
                    <use href="../images/data-flows/widget/transform-filters.svg#uuid-56d667d7-38f0-4edb-861e-449197d184ef" />
                </svg>
                <span class="node-title">篩選</span>
            </div>
                `;
            editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
        case 'transform-calculate':
            var nodeName = 'transform-calculate';
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
                <div class="node-content">
                    <svg class="node-icon" viewBox="0 0 24 24">
                        <use href="../images/data-flows/widget/transform-calculate.svg#uuid-07832d82-56a4-42e7-afca-bdd9fe58cc6b" />
                    </svg>
                    <span class="node-title">計算</span>
                </div>
                `;
                editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
        case 'transform-validator':
            var nodeName = 'transform-validator';
            var outputdata = [
                { text: 'BBBBB', type: 'success' },
                { text: 'BBBB', type: 'fail' }
            ];
            var content = `
                    <div class="node-content">
                        <svg class="node-icon" viewBox="0 0 24 24">
                            <use href="../images/data-flows/widget/transform-validator.svg#uuid-0946a06b-4f4b-4d30-8f36-0f98e54de6f4" />
                        </svg>
                        <span class="node-title">驗證</span>
                    </div>
                    `;
                    editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
            break;
            case 'transform-replace':
                var nodeName = 'transform-replace';
                var outputdata = [
                    { text: 'BBBBB', type: 'success' },
                    { text: 'BBBB', type: 'fail' }
                ];
                var content = `
                        <div class="node-content">
                            <svg class="node-icon" viewBox="0 0 24 24">
                                 <use href="../images/data-flows/widget/transform-replace.svg#uuid-37bf9059-a81f-48fc-ad5e-5e90a426432d" />
                            </svg>
                            <span class="node-title">文字替換</span>
                        </div>
                        `;
                        editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
                break;
                case 'transform-remove-duplicates':
                    var nodeName = 'transform-remove-duplicates';
                    var outputdata = [
                        { text: 'BBBBB', type: 'success' },
                        { text: 'BBBB', type: 'fail' }
                    ];
                    var content = `
                            <div class="node-content">
                                <svg class="node-icon" viewBox="0 0 24 24">
                                     <use href="../images/data-flows/widget/transform-remove-duplicates.svg#uuid-81a127b2-7410-4f53-9c1f-969a17fe17bd" />
                                </svg>
                                <span class="node-title">排除重複</span>
                            </div>
                            `;
                            editor.addNode(nodeName, 1, 2, pos_x, pos_y, nodeName, {}, content, false, inputdata, outputdata);
                    break;
        // #endregion::transform
    }
}
window.allowDrop = allowDrop;
window.drag = drag;
window.drop = drop;
});