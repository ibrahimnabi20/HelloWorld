var ViewModel = function() {

    const me = this;
    const apiUrl = "http://" + window.location.hostname + ":81/api/";

    // Properties
    me.languages = ko.observableArray();
    me.language = ko.observable();
    me.greeting = ko.observable();
    me.planet = ko.observable();
    me.loaderCounter = ko.observable(0);
    me.loaderVisible = ko.computed(() => {
        return me.loaderCounter() !== 0;
    });

    // Subscribers
    me.language.subscribe(() => {
        me.loaderCounter(me.loaderCounter()+1);
        $.ajax({
            url: apiUrl + "Text?languageCode=" + me.language(),
            success: (data) => {
                me.greeting(data.greeting);
                me.planet(data.planet);
                me.loaderCounter(me.loaderCounter()-1);
            }
        });
    });

    // Constructor
    me.loaderCounter(me.loaderCounter()+1);
    $.ajax({
        url: apiUrl + "Language",
        success: (data) => {
            ko.utils.arrayPushAll(me.languages, data.languages);
            me.language(data.defaultLanguage);
            me.loaderCounter(me.loaderCounter()-1);
        }
    });
}
ko.applyBindings(new ViewModel(), $("body")[0]);