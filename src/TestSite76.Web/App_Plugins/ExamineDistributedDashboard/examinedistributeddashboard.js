angular.module("umbraco").controller("ExamineDistributedDashboard", function ($scope, $http) {
    var url = "/umbraco/backoffice/api/examinedistributeddashboard/serverscheckin";
    $scope.success = false;
    $scope.successMessage = "Complete";
    $scope.failure = false;
    $scope.buttonState = "init";

    $scope.serversCheckIn = function () {
        $scope.buttonState = "busy";
        $http.get(url).then(
            function (response) {
                console.log(response);
                $scope.success = true;
                $scope.successMessage = response;
                $scope.buttonState = "success";
            },
            function (data) {
                console.log(data);
                $scope.failure = true;
                $scope.buttonState = "error";
            }
        );
    };

    $scope.sendRebuildRequest = function () {
        console.log($scope.targetAppDomain);

        $http({
            method: 'POST',
            url: '/umbraco/backoffice/api/examinedistributeddashboard/sendrebuildrequest',
            params: {
                'targetMachineName': $scope.targetMachineName,
                'targetProcessId': $scope.targetProcessId,
                'targetIndexName': $scope.targetIndexName
            },
            data: {}
        }).then(
            function (response) {
                console.log(response);
                $scope.requestSuccess = true;
                $scope.requestSuccessMessage = response;
            },
            function (data) {
                console.log(data);
                $scope.requestFailure = true;
            }
        );
    };
});