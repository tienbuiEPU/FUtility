var myApp = angular.module('DragonApp', ['ngPrint', 'ngJsonExportExcel', 'ngDialog', 'ngMd5', 'ngRoute', 'wu.masonry', 'ui.bootstrap', 'ngFileUpload', 'ui.bootstrap.datetimepicker', 'angular-loading-bar', 'yaru22.angular-timeago', 'chart.js', 'ngMaterial', 'firebase']);
myApp.value('config', { domain: 'http://localhost:11762/' });

myApp.config(['cfpLoadingBarProvider', function (cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeSpinner = false;
}]);

myApp.config(function () {
    var config = {
        apiKey: "AIzaSyCfL5CEdAUbIbWM4LnCo91ZF1IF6XnQR1E",
        authDomain: "fsale-36773.firebaseapp.com",
        databaseURL: "https://fsale-36773.firebaseio.com",
        projectId: "fsale-36773",
        storageBucket: "fsale-36773.appspot.com",
        messagingSenderId: "483587726805"
    };
    firebase.initializeApp(config);
});

myApp.filter('iif', function () {
    return function (input, trueValue, falseValue) {
        return input ? trueValue : falseValue;
    };
});

myApp.config(['timeAgoSettings', function (timeAgoSettings) {
    timeAgoSettings.strings.vi_VN = {
        prefixAgo: null,
        prefixFromNow: null,
        suffixAgo: 'trước',
        suffixFromNow: 'từ bây giờ',
        seconds: 'nhỏ hơn 1 phút',
        minute: 'khoảng 1 phút',
        minutes: '%d phút',
        hour: 'khoảng 1 giờ',
        hours: 'khoảng %d giờ',
        day: 'một ngày',
        days: '%d ngày',
        month: 'khoảng 1 tháng',
        months: '%d tháng',
        year: 'khoảng 1 năm',
        years: '%d năm',
        numbers: []
    };
}]);

var _fileReader = function ($q, $log) {

    var onLoad = function (reader, deferred, scope) {
        return function () {
            scope.$apply(function () {
                deferred.resolve(reader.result);
            });
        };
    };

    var onError = function (reader, deferred, scope) {
        return function () {
            scope.$apply(function () {
                deferred.reject(reader.result);
            });
        };
    };

    var onProgress = function (reader, scope) {
        return function (event) {
            scope.$broadcast("fileProgress",
                {
                    total: event.total,
                    loaded: event.loaded
                });
        };
    };

    var getReader = function (deferred, scope) {
        var reader = new FileReader();
        reader.onload = onLoad(reader, deferred, scope);
        reader.onerror = onError(reader, deferred, scope);
        reader.onprogress = onProgress(reader, scope);
        return reader;
    };

    var readAsDataURL = function (file, scope) {
        var deferred = $q.defer();

        var reader = getReader(deferred, scope);
        reader.readAsDataURL(file);

        return deferred.promise;
    };

    return {
        readAsDataUrl: readAsDataURL
    };
};

myApp.factory("fileReader",
    ["$q", "$log", _fileReader]);

myApp.directive('attributeset', function () {
    return {
        restrict: 'E',
        replace: false,

        scope: {
            attributes: "="
        },
        templateUrl: '/Scripts/directive/attribute.html',
        link: function (scope, element, attrs) {
            scope.attrs = { "data": [] };
            scope.internalControl = scope.control || {};
            scope.addAttr = function () {
                //check exist
                var isDup = false;
                for (var i = 0; i < scope.attrs.data.length; i++) {
                    if (scope.attrs.data[i].AttributeId == parseInt(scope.newAttr.AttributeId) || scope.newAttr.AttributeId == "-1")
                        isDup = true;
                }
                if (isDup)
                    return;
                if (!scope.newAttr.Value || /^\s*$/.test(scope.newAttr.Value))
                    return;
                scope.attrs.data.push(scope.newAttr);
                scope.newAttr = { "AttributeId": "-1", "Value": "" };
            }
            scope.deleteAttr = function (id) {
                //check exist                
                for (var i = 0; i < scope.attrs.data.length; i++) {
                    if (scope.attrs.data[i].AttributeId == id)
                        scope.attrs.data.splice(i, 1);
                }
            }
            scope.findName = function (id) {
                var name = "";
                for (var i = 0; i < scope.attributes.length; i++) {
                    if (scope.attributes[i].AttributeId == parseInt(id))
                        name = scope.attributes[i].AttributeName;
                }
                return name;
            }
            scope.Delete = function (e) {
                //remove element and also destoy the scope that element
                element.remove();
                scope.$destroy();
            }
            //scope.$on('Delete', function (e) {
            //    //remove element and also destoy the scope that element
            //    element.remove();
            //    scope.$destroy();
            //})
        }
    };
});

myApp.directive('productunit', function () {
    return {
        restrict: 'E',
        replace: false,
        scope: {
        },
        templateUrl: '/Scripts/directive/unit.html',
        link: function (scope, element, attrs) {
            scope.Delete = function (e) {
                //remove element and also destoy the scope that element
                element.remove();
                scope.$destroy();
            }
        }
    };
});

myApp.directive('productimage', ['fileReader', function (fileReader) {
    return {
        restrict: 'E',
        replace: false,
        templateUrl: '/Scripts/directive/productImage.html',
        link: function ($scope, element, attrs) {
            $scope.Delete = function (e, name) {
                if ($scope.files == undefined || $scope.files.length > 0) {
                    var index = 0;
                    for (var i = 0; i < $scope.files.length; i++) {
                        if (name == $scope.files[i].name)
                            index = i;
                    }
                    //remove
                    $scope.files.splice(index, 1);
                    if ($scope.isEditing)
                        $scope.edit.addedImages.splice(index, 1);
                    else
                        $scope.new.addedImages.splice(index, 1);
                }
            }

            $scope.loadImage = function (file) {
                fileReader.readAsDataUrl(file, $scope)
                      .then(function (result) {
                          file.base64String = result;
                      });
            }
        }
    };
}]);

myApp.directive('format', ['$filter', function ($filter) {
    return {
        require: '?ngModel',
        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;

            ctrl.$formatters.unshift(function (a) {
                return $filter(attrs.format)(ctrl.$modelValue, "", 0)
            });

            elem.bind('blur', function (event) {
                var plainNumber = elem.val().replace(/[^\d|\-+|\.+]/g, '');
                elem.val($filter(attrs.format)(plainNumber, "", 0));
            });
        }
    };
}]);

myApp.directive('num', ['$filter', function ($filter) {
    return {
        require: '?ngModel',
        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;

            ctrl.$formatters.unshift(function (a) {
                return $filter(attrs.format)(ctrl.$modelValue, "", 0)
            });

            elem.bind('blur', function (event) {
                var plainNumber = elem.val().replace(/[^\d|\-+|\.+]/g, '');
                elem.val($filter(attrs.format)(plainNumber, "", 0));
            });
        }
    };
}]);

myApp.directive("ngFileSelect", function () {

    return {
        link: function ($scope, el) {

            el.bind("change", function (e) {
                //check file exists
                var currentFiles = $scope.files;
                //check ext                
                var selectedFiles = (e.srcElement || e.target).files;
                var isValid = true;
                for (var i = 0; i < selectedFiles.length; i++) {
                    var ext = selectedFiles[i].name.split('.').pop();
                    if ("jpg|jpeg|png|bmp|gif|JPG|JPEG|PNG|bmp|GIF".indexOf(ext) == -1)
                        isValid = false;
                }
                if (!isValid)
                    return;
                if (currentFiles != undefined && currentFiles.length > 0) {
                    if (selectedFiles != undefined && selectedFiles.length > 0) {
                        for (var i = 0; i < selectedFiles.length; i++) {
                            var isDup = false;
                            for (var j = 0; j < currentFiles.length; j++) {
                                if (selectedFiles[i].name == currentFiles[j].name) {
                                    isDup = true;
                                    break;
                                }
                            }
                            if (!isDup) {
                                $scope.files.push(selectedFiles[i]);
                                if ($scope.isEditing)
                                    $scope.edit.addedImages.push(selectedFiles[i]);
                                else
                                    $scope.new.addedImages.push(selectedFiles[i]);
                                $scope.$apply();
                            }
                        }
                    }
                }
                else {
                    var files = (e.srcElement || e.target).files;
                    $scope.files = new Array();
                    for (var i = 0; i < files.length; i++) {
                        //$scope.getFile(files[i]);
                        $scope.files.push(files[i]);
                        if ($scope.isEditing)
                            $scope.edit.addedImages.push(selectedFiles[i]);
                        else
                            $scope.new.addedImages.push(selectedFiles[i]);
                        $scope.$apply();
                    }
                }
            })

        }

    }
})

myApp.directive('inputEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.inputEnter);
                });

                event.preventDefault();
            }
        });
    };
});

//Xuất excel
myApp.factory('Excel', ['$window', function ($window) {
    var uri = 'data:application/vnd.ms-excel;base64,',
        template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>',
        base64 = function (s) { return $window.btoa(unescape(encodeURIComponent(s))); },
        format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) };
    return {
        report: function (tableId, worksheetName) {
            var table = $(tableId),
                ctx = { worksheet: worksheetName, table: table.html() },
                href = uri + base64(format(template, ctx));
            return href;
        }
    };
}]);

//Export Table
myApp.directive('exportTable', function () {
    var link = function ($scope, elm, attr) {
        $scope.$on('export-pdf', function (e, d) {
            elm.tableExport({ type: 'pdf', escape: false });
        });
        $scope.$on('export-excel', function (e, d) {
            elm.tableExport({ type: 'excel', escape: false });
        });
        $scope.$on('export-doc', function (e, d) {
            elm.tableExport({ type: 'doc', escape: false });
        });
        $scope.$on('export-csv', function (e, d) {
            elm.tableExport({ type: 'csv', escape: false });
        });
    }
    return {
        restrict: 'C',
        link: link
    }
});

//Đinh dạng giá
myApp.filter("displayprice", function () {
    return function (input) {
        //console.log("displayprice: " + input);
        input = (typeof input === 'undefined' || input === '') ? "" : input + "";
        if (parseInt(input) === 0) {
            input = 0 + "";
        }
        var comma = ".";
        var num = parseInt(input) ? parseInt(input.replace(/\./g, '')) : input;
        //var num = parseInt(input) ? parseInt(input.replace(/[^\d|\-+|\.+]/g, '')) : input;

        //console.log("displayprice2: " + input);
        var nums = 0;
        if (num >= 0) {
            nums = num;
        }
        else {
            nums = 0 - num;
        }
        nums = nums + "";

        var str = "";

        var k = (nums.length % 3);
        if (k > 0) {
            str += nums.substring(0, k) + comma;
        }

        while (k < nums.length) {

            str += nums.substring(k, k + 3) + comma;
            k = k + 3;
        }
        if (num >= 0) {
            str = str.substring(0, str.length - 1);
        }
        else {
            str = "-" + str.substring(0, str.length - 1);
        }
        return str;
    }
});

myApp.filter("formatPrice", function () {
    return function (price, digits, thoSeperator, decSeperator, bdisplayprice) {
        var i;
        price = (typeof price === "undefined") ? 0 : price;
        digits = (typeof digits === "undefined") ? 0 : digits;
        bdisplayprice = (typeof bdisplayprice === "undefined") ? true : bdisplayprice;
        thoSeperator = (typeof thoSeperator === "undefined") ? "." : thoSeperator;
        decSeperator = (typeof decSeperator === "undefined") ? "," : decSeperator;
        price = (typeof price === "") ? "0" : price;

        if (price != 0) {
            var prices = 0 - price;
            if (price > 0) {
                prices = price;
            }
            prices = prices + "";
            var _temp = prices.split('.');
            var dig = (typeof _temp[1] === "undefined") ? "00" : _temp[1];
            if (bdisplayprice && parseInt(dig, 10) === 0) {
                dig = "";
            } else {
                dig = dig + "";
                if (dig.length > digits) {
                    dig = (Math.round(parseFloat("0." + dig) * Math.pow(10, digits))) + "";
                }
                for (i = dig.length; i < digits; i++) {
                    dig += "0";
                }
            }
            var num = _temp[0];
            var s = "",
                ii = 0;
            for (i = num.length - 1; i > -1; i--) {
                s = ((ii++ % 3 === 2) ? ((i > 0) ? thoSeperator : "") : "") + num.substr(i, 1) + s;
            }
        }
        else {
            s = 0;
        }

        if (price < 0) {
            s = '- ' + s;;
        }
        if (dig > 0) {
            return s + decSeperator + dig;
        }
        else {
            return s;
        }
    }
});

//Đinh dạng độ dài chuỗi
myApp.filter("displaystring", function () {
    return function (input, k) {
        input = (typeof input === 'undefined' || input === '') ? "" : input + "";
        var str = input.length > k ? input.substring(0, input.substring(0, k).lastIndexOf(' ')) + "..." : input
        return str;
    }
});
myApp.directive("fileread", [function () {
    return {
        scope: {
            fileread: "="
        },
        link: function (scope, element, attributes) {
            element.bind("change", function (changeEvent) {
                scope.$apply(function () {
                    scope.fileread = changeEvent.target.files[0];
                    // or all selected files:
                    // scope.fileread = changeEvent.target.files;
                });
            });
        }
    }
}]);

// chuyen chu hoa thanh chu thuong va chuyen tieng viet khong dau thanh co dau
myApp.formatSpecialchar = function (ystring) {
    if (ystring) {
        ystring = ystring.toLowerCase();
        ystring = ystring.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a").replace(/đ/g, "d").replace(/đ/g, "d").replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y").replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u").replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ.+/g, "o").replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ.+/g, "e").replace(/ì|í|ị|ỉ|ĩ/g, "i");
    }
    else {
        ystring = '';
    }
    return ystring;
}

myApp.controller('UploadController', ['$scope', '$http', 'config', 'ngDialog', 'md5', '$window', 'fileReader', 'Excel', '$timeout', '$compile', 'Upload', 'cfpLoadingBar', '$mdDialog', '$mdToast', '$sce', '$interval', function UploadController($scope, $http, config, ngDialog, md5, $window, fileReader, Excel, $timeout, $compile, Upload, cfpLoadingBar, $mdDialog, $mdToast, $sce, $interval) {
    
    var domain = config.domain;
    $scope.new = {};
    $scope.new.addedImages = new Array();
    $scope.files = new Array();
    $scope.result = [];
    $scope.imgSource = "";
    $scope.imgMark = "";

    $scope.upload = function ()
    {
        if ($scope.new.addedImages && $scope.new.addedImages.length) {      
            var images = new Array();
            for (var i = 0; i < $scope.new.addedImages.length; i++) {
                images.push($scope.new.addedImages[i]);
            }
            for (var i = 0; i < images.length; i++) {
                Upload.upload({
                    url: domain + "api/watermark/uploadImage",
                    data: { file: images[i] }
                }).then(function (resp) {
                    $scope.result.push(resp.data);
                }, function (resp) {
                }, function (evt) {
                });
            }
        }

        var interval = $interval(function () {
            if ($scope.result[0] != undefined && $scope.result[1] != undefined) {
                var obj = {};
                obj.url1 = $scope.result[0].data[0];
                obj.url2 = $scope.result[1].data[0];
                console.log(obj);
                var post = $http({
                    method: "POST",
                    url: "/api/app/createWatermark",
                    data: obj
                });

                post.success(function successCallback(data) {
                    console.log(data);
                    $scope.linkResult = data.data;
                    //$scope.new.addedImages = new Array();
                    //$scope.files = new Array();
                });
                $interval.cancel(interval);
            }
        }, 100);
    }

    $scope.actionWaterMark = function()
    {
        console.log($scope.imgSource);
        console.log($scope.imgMark);

        var post = $http({
            method: "POST",
            url: domain + "api/imageCp?imageSource=" + $scope.imgSource + "&imageMark=" + $scope.imgMark
        });

        post.success(function successCallback(data) {
            $scope.linkResult = data;
        });
    }
}]);


