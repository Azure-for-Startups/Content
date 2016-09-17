(function () {
    'use strict';

    angular
        .module('app')
        .service('settings', settings);

    settings.$inject = ['$cookies'];

    function settings($cookies) {
        this.indexName = $cookies.get("indexName");
        this.searchServiceUrl = $cookies.get("searchServiceUrl");
        this.searchQueryKey = $cookies.get("searchQueryKey");
        this.save = save;
        this.reset = reset;

        function save() {
            $cookies.put("indexName", this.indexName);
            $cookies.put("searchServiceUrl", this.searchServiceUrl);
            $cookies.put("searchQueryKey", this.searchQueryKey);
        }

        function reset() {
            this.indexName = "logset-idx";
            this.searchServiceUrl = "_SERVICE_URL_"
            this.searchQueryKey = "_QUERY_KEY_";
        }
    }
})();
