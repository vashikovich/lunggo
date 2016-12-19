﻿// home controller
app.controller('hotelDetailController', ['$scope', '$log', '$http', '$resource', '$timeout', 'hotelSearchSvc', 'imageSvc', function ($scope, $log, $http, $resource, $timeout, hotelSearchSvc, imageSvc) {

    $scope.destinationName = "";
    $scope.hotel = {};
    $scope.searchId = '';
    $scope.searchParam = '';
    

    $scope.hotel.location = '';
    $scope.hotel.checkinDate = "";
    $scope.hotel.checkoutDate = "";
    $scope.hotel.adultCount = 3;
    $scope.hotel.childCount = 1;
    $scope.hotel.nightCount = "";
    $scope.hotel.roomCount = 2;
    $scope.returnUrl = "/";
    $scope.roomCount = 3;
    $scope.minRoomCount = 3;
    $scope.maxRoomCount = 100;
    $scope.pageLoaded = true;
    $scope.searchDone = false;
    $scope.loc = loc;
    $scope.selectedRoom = '';
    $scope.loading = false;

    

    $scope.init = function (model) {
        $log.debug(model);
        $scope.searchId = model.searchId;
        $scope.searchParam = model.searchParam;
        $scope.loading = true;
        
        var mydata = $scope.searchParam.split('.');
        var cekin = mydata[2];
        var cekout = mydata[3];
        var nightcount = mydata[4];
        var roomcount = mydata[5];
        var occupancies = mydata[6].split('|');

        var searchParamObject = {
            nightCount: nightcount,
            roomCount: roomcount,
            checkinDate: moment(cekin, "YYYY-MM-DD"),
            checkoutDate: moment(cekout, "YYYY-MM-DD"),
            occupancies:[]
        }

        for (var x = 0; x < occupancies.length; x++) {
            var occdata = occupancies[x].split('~');
            var ages = [];
            if (occdata.length == 3) {
                var agedata = occdata[2].split(',');
                for (var a = 0; a < agedata.length; a++) {
                    ages.push(parseInt(agedata[a]));
                }
                for (var b = 0; b < 4 - agedata.length; b++) {
                    ages.push(0);
                }
            } else {
                ages = [0, 0, 0, 0];
            }
            searchParamObject.occupancies.push({
                adultCount: occdata[0],
                childCount: occdata[1],
                childrenAges: occdata[1] == 0 ? [0, 0, 0, 0] : ages
            });
        }

        for (var m = 0; m < 8 - roomcount; m++) {
            searchParamObject.occupancies.push({
                adultCount: 1,
                childCount: 0,
                childrenAges: [0, 0, 0, 0]
            });
        }
        hotelSearchSvc.initializeSearchForm($scope, searchParamObject);
        
        

        //$scope.hotelSearch.checkinDate = moment(cekin, "YYYY-MM-DD");
        //$scope.hotelSearch.checkoutDate = moment(cekout, "YYYY-MM-DD");
        //$scope.hotelSearch.checkinDateDisplay = moment(cekin,"YYYY-MM-DD").locale("id").format('LL');
        //$scope.hotelSearch.checkoutDateDisplay = moment(cekout, "YYYY-MM-DD").locale("id").format('LL');

        //$scope.hotelSearch.destinationCheckinDate = moment(cekin, "YYYY-MM-DD").locale("id").format('LL');
        //$scope.hotelSearch.destinationCheckoutDate = moment(cekout, "YYYY-MM-DD").locale("id").format('LL');


        var resource = $resource(HotelDetailsConfig.Url + '/:searchId/:hotelCd',
            {},
            {
                query: {
                    method: 'GET',
                    params: { searchId: model.searchId, hotelCd: model.hotelCd },
                    isArray: false
                }
            }
        );



        resource.query({}, {}).$promise.then(function (data) {
            $scope.loading = false;
            validateResponse(data);
            $scope.searchDone = true;
            $scope.hotel = data.hotelDetails;
            //$scope.destinationName = data.destinationName;
            $scope.hotelSearch.locationDisplay = $scope.hotel.destinationName;
            $scope.hotelSearch.destinationName = $scope.hotel.destinationName;

            var loadedImages = 0;
            var maxImages = 6;
            var tempHotelImages = [];

            //Remove broken images
            var imageCount = $scope.hotel.images.length;
            var imageIndex = 0;
            var finishedSlider = 0;
            $.each($scope.hotel.images, function (key, value) {
                imageSvc.isImage(value).then(function () {
                    if (loadedImages < maxImages) {
                        loadedImages++;
                        tempHotelImages.push(value);
                        $scope.hotel.images = tempHotelImages;
                    }
                    else return false;
                }, function () {
                    return; //equivalent of continue
                }).finally(function () {
                    imageIndex++;
                    if (!finishedSlider && loadedImages == 1 || (imageIndex + 1 == imageCount && loadedImages < maxImages)) {
                        $timeout(function () { initiateSlider(); }, 0);
                        finishedSlider = true;
                    }
                });
            });

            // apakah code di bawah ini sudah efektif?
            $.each($scope.hotel.room, function (roomKey, room) {
                $.each(room.roomImages, function (imageKey, roomImage) {
                    $scope.hotel.room[roomKey].roomImages[imageKey] = roomImage;
                });
            });
            
            var cekin = $scope.hotel.room[0].rate.regsId.split(',')[2].split('|')[0];
            var cekout = $scope.hotel.room[0].rate.regsId.split(',')[2].split('|')[1];
            $scope.hotel.checkinDate = new Date(parseInt(cekin.substring(0, 4)), parseInt(cekin.substring(4, 6)) - 1, parseInt(cekin.substring(6, 8)));
            $scope.hotel.checkoutDate = new Date(parseInt(cekout.substring(0, 4)), parseInt(cekout.substring(4, 6)) - 1, parseInt(cekout.substring(6, 8)));
            $scope.hotel.nightCount = (new Date($scope.hotel.checkoutDate) - new Date($scope.hotel.checkinDate)) / (3600 * 24 * 1000);
            
            setFacilityDisplay();
            //setTncDisplay();
            setDescriptionDisplay();
            $timeout(function() { hotelDetailFunctions(); }, 0);
            //$timeout(function () { initiateSlider(); }, 0);
            $timeout(function () {  accordionFunctions(); }, 0);

            $log.debug($scope.hotel);


            $timeout(function () {
                // **********
                // Search Detail Tab
                $('body .detail-tab').each(function (index, item) {
                    if (index > 0) {
                        $(item).hide();
                    }
                });
                $('body .hotel-detail-menu-action a').on('click touchstart', function () {
                    var id = $(this).attr('attr-link');
                    $('body .detail-tab').hide();
                    $(id).show();
                });

                // **********
                // Shorten Area

                // Search Detail
                $('body .sh-desc a').on('click touchstart', function () {
                    $('body .sh-desc a').toggleClass('active');
                    $('body .sh-txt').toggleClass('opened');
                });

                // **********
                // Open Detail Room
                $('body .dh-list').on('click', function () {
                    var id = $(this).parent();

                    id.toggleClass('active');
                    id.siblings().removeClass('active');

                    id.find('.dh-list-detail').toggleClass('active');
                    id.siblings().find('.dh-list-detail').removeClass('active');
                });

                // **********
                // Slick Slider Detail Hotel
                $('.dh-slider').slick({
                    autoplay: true,
                    autoplaySpeed: 2500,
                    dots: false
                });
            }, 0);

        }, function (error) {
            $log.debug(error);

        });
        $scope.hotelSearch.location = $scope.getLocationCode();
    }

    //$scope.getLocationCode = function () {
    //    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    //    var locationCode = hashes[2].split('.')[1];
    //    return locationCode;
    //}
    
    var setDescriptionDisplay = function () {
        if ($scope.hotel.description != null && $scope.hotel.description.length > 0) {
            var description = [];
            var descriptionArray = $scope.hotel.description.split('.');
            var tempDescription = '';
            $.each(descriptionArray, function (key, value) {
                value = value + '.';
                tempDescription = tempDescription + value;
                if (key % 3 == 0) {
                    description.push(tempDescription);
                    tempDescription = '';
                }
            });
            $scope.hotel.description = description;
        }
    }

    var validateResponse = function (data) {
        //searchId expired
        if (data.error == "ERHGHD02") {
            $log.debug('searchId is expired. (' + $scope.searchId + ') \n redirecting to search with ' + $scope.searchParam)
            alert('searchId is expired. Redirecting to search.');
            hotelSearchSvc.gotoHotelSearch($scope.hotelSearch);
            //location.href = location.href = '/id/Hotel/Search/?' + $scope.searchParam;
        }
    }

    //var setTncDisplay = function () {
    //    $.each($scope.hotel.room, function (key, room) {
    //        var tncArray = room.rate.tnc.split('↵');
    //        $log.debug(tncArray);
    //    })
    //}
    $scope.getFacilityOrder = function (facilityGroup) {
        switch (facilityGroup) {
            case 'general': return 0;
            case 'health': return 1;
            case 'sport': return 2;
            case 'business': return 3;
            case 'meal': return 4;
            case 'entertainment': return 5;
            case 'other': return 6;
            default: $log.debug('unknown facility type deteted.'); break;
        }
    }

    var setFacilityDisplay = function () {
        var hotelFacilityReplacement = [];

        var facilityOrder = 0;
        var tempFacilityList = null;
        $.each($scope.hotel.facilities, function (facilityGroup, facility) {
            tempFacilityList = [[], [], [], []];
            $.each(facility, function (index, facilityName) {
                if (index % 4 == 0) {
                    tempFacilityList[0].push(facilityName);
                } else if (index % 4 == 1) {
                    tempFacilityList[1].push(facilityName);
                } else if (index % 4 == 2) {
                    tempFacilityList[2].push(facilityName);
                } else if (index % 4 == 3) {
                    tempFacilityList[3].push(facilityName);
    }
            });

            facilityOrder = $scope.getFacilityOrder(facilityGroup);
            hotelFacilityReplacement[facilityOrder] = { 'facilityGroup': facilityGroup, 'facilityList': tempFacilityList };
        });

        $scope.hotel.facilities = hotelFacilityReplacement;
    };

    $scope.model = {};
    $scope.hotels = [];
    $scope.totalActualHotel = '';
    $scope.hotelFilterDisplayInfo = undefined;
    $scope.filterDisabled = true;

    //=============== hotel start ======================

    $scope.hotel.searchHotel = function () {
        $log.debug('searching hotel');
        //$http({
        //   url: "/id/Hotel/Search", 
        //   method: "GET",
        //   params: {aaa : "kode123", bbb : "silubab" }
        //   })
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
        )
    }

    var selectService = $resource(HotelSelectConfig.Url,
          {},
          {
              query: {
                  method: 'POST',
                  params: {},
                  isArray: false
              }
          }
      );

    $scope.seeRoomDetail = function (room) {
        $scope.selectedRoom = room;
    }
    $scope.selectFailed = false;
    $scope.booking = false;
    $scope.bookRoom = function (room) {
        var childrenages = room.rate.regsId.split(',')[2].split('|')[10];
        var ages = [];
        if (childrenages != null && childrenages.length != 0) {
            var age = childrenages.split('~');
            for (var i = 0; i < age.length; i++) {
                ages.push(parseInt(age[i]));
            }
        }
        $scope.booking = true;
        
        selectService.query({}, {
            "searchId": $scope.searchId,
            "regs": [
                  {
                      "regsId": room.rate.regsId,
                      "rateCount": room.rate.roomCount,
                      "adultCount": room.rate.adultCount,
                      "childCount": room.rate.childCount,
                      "childrenAges": ages
                  }
            ]
        }).$promise.then(function (data) {
            $scope.booking = false;
            if (data.status != 200 || data.token == null) {
                $scope.selectFailed = true;
            } else {
                $scope.token = data.token;
                $scope.tokenLimit = data.timeLimit;
                $log.debug('selected, token = ' + $scope.selectToken);

                $log.debug('going to checkout');

                location.href = '/id/Hotel/Checkout/?token=' + $scope.token;
            }
            
        });
    }

    $scope.HotelSearchForm = {
        AutoComplete: {
            Keyword: '',
            MinLength: 3,
            GetLocation: function () {
                $scope.getLocation($scope.HotelSearchForm.AutoComplete.Keyword);
                // function in hotelSearchService.js
            },
        },
    }

    var accordionFunctions = function () {
        //Accordion Help Section by W3School
        $timeout(function () {
            var acc = document.getElementsByClassName("accordion");
            var i;

            for (i = 0; i < acc.length; i++) {
                acc[i].onclick = function () {
                    this.classList.toggle("active");
                    this.nextElementSibling.classList.toggle("show");
                }
            }
        }, 0);
    }

    var hotelDetailFunctions = function (parameters) {
        // Open Room Detail
        $('body .room-rl').on('click', function () {
            var parent1 = $(this).closest('.room-list').find('.room-left');
            var parent2 = parent1.closest('.room-list-container li').find('.hotel-detail');
            parent2.slideToggle('.4s');
            $(this).closest('.room-list-container li').siblings().find('.hotel-detail, .option').hide();

            $(this).each(function () {
                $(this).closest('.room-list-container li').find('.room-gallery').lightSlider({
                    gallery: true,
                    item: 1,
                    thumbItem: 6,
                    slideMargin: 0,
                    loop: true,
                    keyPress: true,
                    onSliderLoad: function () {
                        $('.room-gallery').removeClass('cS-hidden');
                    }
                });
            });

            var list = $(this).closest('.room-list-container li');

            list.find('.room-list').toggleClass('active');
            list.siblings().find('.room-list').removeClass('active');

            list.toggleClass('active');
            list.siblings().removeClass('active');
        });

        $("body .change-room").click(function () {
            var parent1 = $(this).closest('.room-list').find('.room-left');
            var parent2 = parent1.closest('.room-list-container li');
            parent2.find('.option').toggle();
            parent2.siblings().find('.hotel-detail, .option').hide();
        });
    }

    $scope.togleDisplay = function () {
        if ($scope.roomCount == $scope.maxRoomCount) {
            $scope.roomCount = $scope.minRoomCount;
        } else {
            $scope.roomCount = $scope.maxRoomCount;
        }
    }
    //=============== hotel end ======================

    var initiateSlider = function () {
        $('#image-gallery').lightSlider({
            gallery: true,
            item: 1,
            thumbItem: 6,
            slideMargin: 0,
            loop: true,
            keyPress: true,
            onSliderLoad: function () {
                $('#image-gallery').removeClass('cS-hidden');
            }
        });

        $timeout(function () {
            var altImagePath = document.location.origin + '/Assets/travorama20/images/Hotel/no-hotel.png';
            var elements = angular.element(document.querySelectorAll('#imgtum'));

            //angular.each(elements, function(key, value) {
            //    elements[key].removeAttr('src');
            //    elements[key].attr('src', altImagePath); // set default image
            //});
        }, 0);

    }

    $scope.isFreeRefund = function (room, index) {
        if (room.rate.isRefundable) {
            if (room.rate.cancellation !== undefined && room.rate.cancellation.length > 0) {
                var cancellation = room.rate.cancellation;

                var cancelTRUE = moment(cancellation[0].startTime).isAfter(moment(), 'day');


                if (true) {

                }
                return cancelTRUE;
            }
            else {
                console.log('room ' + index + ' is refundable but no cancellation rate');
                return false;
            }
        }
        else {
            return false;
        }
    };

    $scope.toTitleCase = function (str) {
        return str.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
    }

    $('.change-hotel.form-control').click(function () {
        $(this).select();
    });
}]);
