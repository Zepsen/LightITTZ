var app = angular.module('myApp', []).config(function ($httpProvider) {});

app.controller('myController', function ($scope, $http) {
    $scope.inputText = '';
    $scope.outputText = '';
    $scope.inputStep = 0;
    $scope.systemMessage = '';

    //Ajax 
    $scope.postAjax = function (method) {

        var ajaxSetting = {
            method: 'POST',
            url: '/Home/Main',
            params: {
                method: method,
                data: $scope.inputText,
                step: $scope.inputStep,
                message: $scope.systemMessage
            },
            cache: false
        };

        var response = $http(ajaxSetting);

        response.success(function (resp) {
            var data = JSON.parse(resp);

            $scope.outputText = data.Data;
            $scope.systemMessage = data.Message;
            $scope.Diagram(data.Letters);
        });

        response.error(function (data) {
            $scope.systemMessage = data;
        });
    };

    //Diagram
    $scope.Diagram = function (jsonLettets) {
        var js = JSON.parse(jsonLettets);

        var revenueChart = new FusionCharts({
            "type": "column2d",
            "renderAt": "diagramContainer",
            "width": "100%",
            "height": "300",
            "dataFormat": "json",
            "dataSource": {
                "chart": {
                    "caption": "Letters diagramm",
                    "xAxisName": "Letters",
                    "yAxisName": "Counts",
                    "theme": "Carboon"
                },
                "data": js
            }
        });
        revenueChart.render();
    }

    //Clear inputs
    $scope.clear = function() {
        $scope.inputText = '';
        $scope.outputText = '';
        $scope.inputStep = 0;
        $scope.systemMessage = '';
    }
});
