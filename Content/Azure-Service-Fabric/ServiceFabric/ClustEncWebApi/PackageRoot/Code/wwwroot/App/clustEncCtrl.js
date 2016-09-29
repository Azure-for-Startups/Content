(function() {
    "use strict";

    angular
        .module("app", ["ngResource", "ngCookies"])
        .controller("clustEncCtrl", clustEncCtrl);

    clustEncCtrl.$inject = ["$scope", "$resource", "$cookies", "$location", "$timeout"];

    function clustEncCtrl($scope, $resource, $cookies, $location, $timeout) {
        $scope.title = "Sample Angular JS client for ClustEnc API";

        // Retrieving a user cookie
        var userCookieName = "userId";
        $scope.userId = $cookies.get(userCookieName);
        if (!$scope.userId) {
            // Generating a new userId
            $scope.userId = uuid.v4().replace(/-/g, '');
            // Setting a user cookie
            $cookies.put(userCookieName, $scope.userId);
        }

        // Resoving service endpoint
        $scope.endpoint = "/api/encodertask";
        if ($location.protocol() !== "http") {
            // Local file system hosting is detected. Use a local cluster.
            $scope.endpoint = "http://localhost:8844" + $scope.endpoint;
        }

        // Service endpoint client
        $scope.api = $resource($scope.endpoint);

        $scope.getTasks = function() {
            $scope.encoderTasks = $scope.api.query({ userId: $scope.userId });
        };
        $scope.scheduleTask = function(error) {
            if (error) return;
            $scope.api.save({ sourceUrl: $scope.sourceUrl, userId: $scope.userId });
            $scope.encoderTasks = null;
            $timeout(function() { $scope.getTasks(); }, 1000);
        };
        $scope.deleteTask = function(name) {
            $scope.api.delete({ name: name });
            $scope.encoderTasks = null;
            $timeout(function() { $scope.getTasks(); }, 1000);
        };

        // Initial data loading
        $scope.getTasks();
    }
})();