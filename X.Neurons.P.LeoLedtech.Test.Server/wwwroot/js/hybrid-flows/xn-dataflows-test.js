function createComponentsPopup() {
  // console.log('按下');
  // createRestApi();
  createRestApi('Restful API');
}
function initializeConfigPopup(componentName) {
  if($('.xn-components-config-content') != null){
    $('.xn-components-config-content').remove();
  }
  const $popup = $("<div>").addClass("xn-components-config-popup");
  const $popupContent = $("<div>").addClass("xn-components-config-content");
  const $widgetName = $("<div>").addClass("xn-components-name");
  $popupContent.append(
    $widgetName,
  );
  $popup.append($popupContent);
  $('#xn-components-config').append($popup);
  $popup.dxPopup({
    width: '50%',
    height: "auto",
    visible: false,
    title: componentName,
    hideOnOutsideClick: false,
    showCloseButton: true,
    onHidden: function (e) {
      $popup.remove();
    }
  }).dxPopup('instance');
  // URL Input
  $widgetName.dxTextBox({
    placeholder: t('data_flows_components_name_placeholder'),
    value: "",
    width: "100%"
  });
  //彈出視窗
  $popup.dxPopup('instance').show();

  return $popupContent;
}