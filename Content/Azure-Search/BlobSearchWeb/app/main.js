(function () {
    'use strict';

    angular
        .module('app')
        .controller('main', main);

    main.$inject = ['$location', 'settings', 'search'];

    function main($location, settings, search) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'main';
        vm.settings = settings;
        vm.query = query;
        vm.reset = reset;
        vm.phrase = "";
        vm.items = "";

        activate();

        function activate() {
        }

        function query() {
            if (!vm.settings.searchServiceUrl || !vm.settings.indexName || !vm.settings.searchQueryKey)
            {
                alert("Please correct settings");
                return;
            }

            search.queryIndex(vm.settings.searchServiceUrl, vm.settings.indexName, vm.phrase, vm.settings.searchQueryKey)
                .then(function mySucces(response) {
                    vm.items = response.data;
                }, function myError(response) {
                    vm.items = null;
                    alert("Connection error");
                })
        }

        function apply() {
            this.settings.save();
        }

        function reset() {
            vm.settings.reset();
            vm.settings.save();
        }
    }
})();
