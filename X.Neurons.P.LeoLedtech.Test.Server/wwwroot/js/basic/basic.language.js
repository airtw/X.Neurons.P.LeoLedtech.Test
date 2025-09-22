$(document).ready(function () {
    // console.log(localStorage.getItem('language') || 'zh-TW');

    // 語言選項配置
    const languages = [
        { id: 'zh-TW', text: '繁體中文' },
        { id: 'en-US', text: 'English' },
        { id: 'zh-CN', text: '簡體中文' }
    ];

    // 初始化 DropDownBox
    $(".xn-language-dropdown").dxDropDownBox({
        dataSource: languages,
        value: localStorage.getItem('language') || 'zh-TW',
        valueExpr: 'id',
        displayExpr: 'text',
        dropDownOptions: {
            minHeight: 100,
        },
        contentTemplate: function (e) {
            return $("<div>").dxList({
                dataSource: languages,
                selectionMode: "single",
                selectedItemKeys: [e.component.option("value")],
                onItemClick: function (args) {
                    e.component.option("value", args.itemData.id);
                    localStorage.setItem('language', args.itemData.id);
                    i18n.setLanguage(args.itemData.id);
                    e.component.close();
                }
            });
        }
    });
  
});
