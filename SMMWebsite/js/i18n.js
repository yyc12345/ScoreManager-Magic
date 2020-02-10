var smmw_i18n_i18nSupported = ['en-US', 'zh-CN'];
var smmw_i18n_currentLanguage = 'en-US';

// judge current language
smmw_i18n_currentLanguage = smmw_localstorageAssist_Get('smmw-i18n', 'en-US');
if (smmw_i18n_i18nSupported.indexOf(smmw_i18n_currentLanguage) == -1){
    smmw_localstorageAssist_Set('smmw-i18n', 'en-US');
    smmw_i18n_currentLanguage = 'en-US';
}
//execute
smmw_i18n_ApplyLanguage();


function smmw_i18n_ChangeLanguage(newLang) {
    if (smmw_i18n_i18nSupported.indexOf(newLang) == -1) return false;
    smmw_i18n_currentLanguage = newLang;
    smmw_localstorageAssist_Set('smmw-i18n', newLang);
    return true;
}

function smmw_i18n_ApplyLanguage() {
    $.i18n.properties({
        name: 'strings_' + smmw_i18n_currentLanguage,
        path: 'i18n/',
        mode: 'map',
        language: smmw_i18n_currentLanguage,
        callback: function() {
            //set usual block
            var cache = $(".smmw-i18n");
            cache.each(function() {
                $(this).html($.i18n.prop($(this).attr('name')));
            });

            //set unusual block
            //set title
            switch(smmw_pages_currentPage) {
                case smmw_pages_enumPages.home:
                    $('#smmw-pageName').html($.i18n.prop('smmw-pageName-home'))
                    break;
                case smmw_pages_enumPages.user:
                    $('#smmw-pageName').html($.i18n.prop('smmw-pageName-user'))
                    break;
                case smmw_pages_enumPages.userinfo:
                    $('#smmw-pageName').html($.i18n.prop('smmw-pageName-userinfo'))
                    break;
                case smmw_pages_enumPages.map:
                    $('#smmw-pageName').html($.i18n.prop('smmw-pageName-map'))
                    break;
                case smmw_pages_enumPages.mapinfo:
                    $('#smmw-pageName').html($.i18n.prop('smmw-pageName-mapinfo'))
                    break;
                case smmw_pages_enumPages.about:
                    $('#smmw-pageName').html($.i18n.prop('smmw-pageName-about'))
                    break;
            }
        }
    })
}
