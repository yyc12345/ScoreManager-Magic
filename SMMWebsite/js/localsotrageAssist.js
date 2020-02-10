function smmw_localstorageAssist_Get(index, defaultValue) {
    var cache = localStorage.getItem(index);
    if (cache == null) {
        smmw_localstorageAssist_Set(index, defaultValue);
        return defaultValue;
    } else return cache;
}

function smmw_localstorageAssist_Set(index, value) {
    localStorage.setItem(index, value);
}