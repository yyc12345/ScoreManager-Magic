//bind language switcher
$(document).ready(function() {
    $('#smmw-hf-language-en-US').click(function(){ smmw_headerFooter_ChangeLanguage('en-US'); })
    $('#smmw-hf-language-zh-CN').click(function(){ smmw_headerFooter_ChangeLanguage('zh-CN'); })
});

function smmw_headerFooter_ChangeLanguage(newLang) {
    smmw_i18n_ChangeLanguage(newLang);
    smmw_i18n_ApplyLanguage();    
}