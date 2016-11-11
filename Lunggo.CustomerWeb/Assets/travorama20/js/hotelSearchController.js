﻿// home controller
app.controller('hotelSearchController', ['$scope', '$log', '$http', '$resource', '$timeout', function ($scope, $log, $http, $resource, $timeout) {

    $scope.model = {};
    $scope.hotels = [];
    $scope.totalActualHotel = '';
    $scope.hotelFilterDisplayInfo = undefined;

    $scope.filterDisabled = true;
    var isFirstload = true;

    //@Url.Action("Search", "Hotel")?zzz={{departureDate}}" method="POST"
    //=============== hotel start ======================
    $scope.hotel = {};
    $scope.hotel.searchHotelType = { "location": 'Location', searchId: 'SearchId'};
    $scope.hotel.searchId = null;
    //$scope.hotel.searchId = "d3eaa926-7bb1-4a08-bfed-824b14968a50";
    $scope.hotel.location = "BALI";
    $scope.hotel.locationDisplay = "";
    $scope.hotel.checkinDate = "12/10/2016";
    $scope.hotel.checkoutDate = "12/11/2016";
    $scope.hotel.adultCount = 3;
    $scope.hotel.childCount = 1;
    $scope.hotel.nightCount = 1;
    $scope.hotel.roomCount = 2;
    $scope.hotel.childrenAges = [];
    $scope.searchDone = false;
    $scope.filter = {};
    $scope.filter.minPrice = 0;
    $scope.filter.maxPrice = 0;
    $scope.filter.zones = null;
    $scope.filter.stars = null;
    $scope.filter.facilities = null;
    $scope.sorting = '';

    $scope.page = 1;
    $scope.perPage = 10;
    $scope.pageCount = 1;
    $scope.searchHeader = {};
    $scope.view = {};
    $scope.view.showHotelSearch = false;

    var resource = $resource(HotelSearchConfig.Url,
            {},
            {
                query: {
                    method: 'POST',
                    params: $scope.Hotel,
                    isArray: false
                }
            }
        );
    var resourcex = $resource(HotelAutocompleteConfig.Url + '/:prefix',
           { prefix: '@prefix' },
           {
               query: {
                   method: 'GET',
                   params: {},
                   isArray: false
               }
           }
       );

    $scope.$watch('hotel.locationDisplay', function (newValue, oldValue, ccc) {
        if (newValue.length >= 3) {
            resourcex.query({ prefix: newValue }).$promise.then(function (data) {
                $timeout(function () {
                    $scope.hotel.hotelAutocomplete = data.hotelAutocomplete;
                    $log.debug($scope.hotel.hotelAutocomplete);
                }, 0);
            });
        }
    });
    $scope.init = function (model) {
        $scope.model = model;
        $log.debug($scope.model);
        //$scope.hotel.searchId = $scope.model.searchId;
        $scope.hotel.location = $scope.model.location;
        $scope.hotel.checkinDate = $scope.model.checkinDate;
        $scope.hotel.checkoutDate = $scope.model.checkoutDate;
        $scope.hotel.adultCount = $scope.model.adultCount;
        $scope.hotel.childCount = $scope.model.childCount;
        $scope.hotel.nightCount = new Date($scope.hotel.checkoutDate).getDate() - new Date($scope.hotel.checkinDate).getDate();
        $scope.hotel.roomCount = $scope.model.roomCount;
        $scope.hotel.childrenAges = $scope.model.childrenAges;

        $scope.searchHotel();
    }

    $scope.searchHotel = function (filter, sort) {
        $log.debug('searching hotel: ');
        $log.debug({
            "searchHotelType": $scope.hotel.searchId == null ? $scope.hotel.searchHotelType.location : $scope.hotel.searchHotelType.searchId,
            "searchId": $scope.hotel.searchId,

            "location": $scope.hotel.location,
            "checkinDate": $scope.hotel.checkinDate,
            "checkoutDate": moment($scope.model.checkinDate).add($scope.hotel.nightCount, 'days').format("YYYY-MM-DD"),
            "nightCount": $scope.hotel.nightCount,
            "occupancies":
                [{
                    "adultCount": $scope.hotel.adultCount,
                    "childCount": $scope.hotel.childCount,
                    "roomCount": $scope.hotel.roomCount,
                    "childrenAges": $scope.hotel.childrenAges
                }],
            "hotelFilter":
            {
                "priceFilter":
                {
                    "minPrice": $scope.filter.minPrice,
                    "maxPrice": $scope.filter.maxPrice
                },
                "zoneFilter":
                {
                    "zones": $scope.filter.zones
                },
                "starFilter":
                {
                    "stars": $scope.filter.stars
                },
                "facilityFilter": {
                    "facilities": $scope.filter.facilities
                }
            },
            "hotelSorting": $scope.sorting,//"DESCENDINGPRICE",
            "page": $scope.page,
            "perPage": $scope.perPage,
            "regsId": $scope.regsId
            //"hotelSorting": "ASCENDINGPRICE"
        });
        $scope.searchDone = false;


        resource.query( {}, {
            "searchHotelType": $scope.hotel.searchId == null ? $scope.hotel.searchHotelType.location : $scope.hotel.searchHotelType.searchId,
            "searchId": $scope.hotel.searchId,

            "location": $scope.hotel.location,
            "checkinDate": $scope.hotel.checkinDate,
            "checkoutDate": moment($scope.model.checkinDate).add($scope.hotel.nightCount, 'days').format("YYYY-MM-DD"),
            "nightCount": $scope.hotel.nightCount,
            "occupancies":
                [{
                    "adultCount": $scope.hotel.adultCount,
                    "childCount": $scope.hotel.childCount,
                    "roomCount": $scope.hotel.roomCount,
                    "childrenAges": $scope.hotel.childrenAges
                }], 
            "hotelFilter":
            {
                "priceFilter":
                {
                    "minPrice": $scope.filter.minPrice,
                    "maxPrice": $scope.filter.maxPrice
                },
                "zoneFilter":
                {
                    "zones": $scope.filter.zones
                },
                "starFilter":
                {
                    "stars": $scope.filter.stars
                },
                "facilityFilter": {
                    "facilities": $scope.filter.facilities
                }
            },
            "hotelSorting": $scope.sorting,//"DESCENDINGPRICE",
            "page": $scope.page,
            "perPage": $scope.perPage,
            "regsId": $scope.regsId
            //"hotelSorting": "ASCENDINGPRICE"
        }).$promise.then(function (data) {

            $scope.searchDone = true;
            if (data.searchId !== undefined) $scope.hotel.searchId = data.searchId;

            $scope.hotels = data.hotels;
            $scope.totalActualHotel = data.returnedHotelCount;
            $scope.returnedHotelCount = data.returnedHotelCount;
            $scope.filteredHotelCount = data.filteredHotelCount;
            
            $scope.page = data.page;
            $scope.perPage = data.perPage;
            $scope.pageCount = data.pageCount;


            if (isFirstload) {
                $scope.filter.minPrice = data.minPrice;
                $scope.filter.maxPrice = data.maxPrice;
                initiatePriceSlider();

                $scope.hotelFilterDisplayInfo = data.hotelFilterDisplayInfo;
                isFirstload = false;
            }

            $timeout(function () { customCheckbox(); }, 0);
            $log.debug(data);
        });
    };

    $scope.hotel.searchParam = function () {
        return ("?info=" +
            [$scope.hotel.location,
             $scope.hotel.checkinDate,
             $scope.hotel.checkoutDate,
             $scope.hotel.adultCount,
             $scope.hotel.childCount,
             $scope.hotel.nightCount,
             $scope.hotel.roomCount].join('.')
        );
    }

    $scope.GotoDetailHotel = function (hotelCd) {
        $log.debug('redirect to detail hotel with hotelCd: ' + hotelCd);
        location.href = '/id/Hotel/DetailHotel/?' +
            "searchId=" + $scope.hotel.searchId + "&" +
            "hotelCd=" + hotelCd;
    }

    $scope.changeFilter = function (filterType, value) {
        if (filterType == 'star') {
            if ($scope.filter.stars == null) {
                $scope.filter.stars = [value];
            }
            else {
                var starIndex = $scope.filter.stars.indexOf(value);
                if (starIndex < 0) {
                    $scope.filter.stars.push(value);
                }
                else {
                    $scope.filter.stars.splice(starIndex, 1);
                }
            }
        }
        else if (filterType == 'zone') {
            if ($scope.filter.zones == null) {
                $scope.filter.zones = [value.code];
            }
            else {
                var zoneCodeIndex = $scope.filter.zones.indexOf(value.code);
                if (zoneCodeIndex < 0) {
                    $scope.filter.zones.push(value.code);
                }
                else {
                    $scope.filter.zones.splice(zoneCodeIndex, 1);
                }
            }
        }
        
    }

    var initiatePriceSlider = function (minPrice, maxPrice) {
        $scope.filter.minPrice = $scope.filter.minPrice - ($scope.filter.minPrice % 100000);
        if ($scope.filter.maxPrice % 100000 == 0) {
            $scope.filter.maxPrice = $scope.filter.maxPrice;
        } else $scope.filter.maxPrice = $scope.filter.maxPrice + (100000 - $scope.filter.maxPrice % 100000);
        // activate price filter
        $('.slider-range').slider({
            range: true,
            min: $scope.filter.minPrice,
            max: $scope.filter.maxPrice,
            step: 100000,
            values: [$scope.filter.minPrice, $scope.filter.maxPrice],
            create: function (event, ui) {
                $('.slider-range-min').val($scope.filter.minPrice);
                $('.slider-range-min').trigger('input');
                $('.slider-range-max').val($scope.filter.maxPrice);
                $('.slider-range-max').trigger('input');
            },
            slide: function (event, ui) {
                //$('.slider-range-min').val(ui.values[0]);
                $scope.filter.minPrice = ui.values[0];
                $('.slider-range-min').trigger('input');
                $scope.filter.maxPrice = ui.values[1];
                $('.slider-range-max').trigger('input');
                $scope.$apply();
            }
        });
    }
    
    //=============== hotel end ======================
    $scope.$watch('hotels', function (newValue, oldValue, ccc) {
        $timeout(function () {
            $("body .col-left-hotel .img-list").each(function (i, elem) {
                var img = $(elem);
                var div = $("<div />").css({
                    background: "url(" + img.attr("src") + ") no-repeat",
                    width: "143px",
                    height: "180px",
                    "background-size": "cover",
                    "background-position": "center"
                });
                img.replaceWith(div);
            });
        }, 0);
    });

    $scope.prevPage = function () {
        $scope.page--;
        $scope.searchHotel();
    }

    $scope.nextPage = function () {
        $scope.page++;
        $scope.searchHotel();
    }

    $scope.changePage = function (pageNumber) {
        $scope.page = pageNumber;
        $scope.searchHotel();
    }

}]);
