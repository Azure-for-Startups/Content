(function () {
    'use strict';

    angular
        .module('app')
        .factory('search', search);

    search.$inject = ['$http'];

    function search($http) {
        var API_VERSION = "2015-02-28-Preview";

        var service = {
            queryIndex: queryIndex
        };

        return service;

        function queryIndex(searchServiceUrl, indexName, phrase, searchQueryKey) {
            var options = {
                params: {
                    "highlight": "content",
                    "highlightPreTag": "<strong>",
                    "highlightPostTag": "</strong>",
                    "$count": "true",
                    "$orderby": "modified desc",
                    "search": phrase,
                    "api-version": API_VERSION
                },
                headers: {
                    "api-key": searchQueryKey
                }
            };

            var url = searchServiceUrl + "/indexes/" + indexName + "/docs";
            return $http.get(url, options);
        }
    }
})();