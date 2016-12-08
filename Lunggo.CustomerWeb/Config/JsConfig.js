//******************************************
// Variables
var SystemConfig = {
    SystemBusy: false
};

var FlightSearchConfig = {
    Url: 'https://api.local.travorama.com/v1/flight/',
    // generate search URL
    GenerateSearchParam: function (params) {
        if (typeof (params) == 'object') {
            // set search result variable
            var url = '';
            var departureParam = '';
            var returnParam = '';
            var passengerParam = '';
            // get variables
            var trip = params.trip || false;
            var departureDate = new Date(params.departureDate) || '';
            var returnDate = new Date(params.returnDate) || '';
            var origin = params.origin;
            var destination = params.destination;
            var passenger = [ params.adult, (params.children || 0), (params.infant || 0) ];
            var cabin = params.cabin.toLowerCase();
            // generate departure param
            departureParam = ( origin + destination ) + ( (('0' + departureDate.getDate()).slice(-2)) + (('0' + (departureDate.getMonth()+1)).slice(-2) ) + (departureDate.getFullYear().toString().substr(2,2)) );
            // generate return param
            if (trip == true) {
                returnParam = (destination + origin) + ((('0' + returnDate.getDate()).slice(-2)) + (('0' + (returnDate.getMonth() + 1)).slice(-2)) + (returnDate.getFullYear().toString().substr(2, 2)));
            }
            // generate passenger param
            if (cabin != 'y' || cabin != 'c' || cabin != 'f') {
                switch (cabin) {
                    case 'economy':
                        cabin = 'y';
                        break;
                    case 'business':
                        cabin = 'c';
                        break;
                    case 'first':
                        cabin = 'f';
                        break;
                }
            }
            passengerParam = passenger[0] + '' + passenger[1] + '' + passenger[2] + '' + cabin ;
            // generate search url
            if (trip == false) {
                url = departureParam + '-' + passengerParam;
            } else {
                url = departureParam + '.' + returnParam + '-' + passengerParam;
            }
            // return the search url
            return url;
        } else {
            console.log('Cannot generate Search URL. Parameter type should be in object. Sample : ');
            console.log('{ trip : true , departureDate : "10-January-2016", returnDate: "11-january-2016", origin : "CGK", destination : "DPS", adult : 1, children : 1, infant : 1 }');
        }
    }
};

var SelectConfig = {
    Url: 'https://api.local.travorama.com/v1/flight/select',
    working: false
};

var RevalidateConfig = {
    Url: 'https://api.local.travorama.com/v1/flight/revalidate',
    working: false
};

var FlightBookConfig = {
    Url: 'https://api.local.travorama.com/v1/flight/book',
    working: false
};

var FlightPayConfig = {
    Url: 'https://api.local.travorama.com/v1/payment/pay',
    working: false
};

var GetRulesConfig = {
    Url: 'https://api.local.travorama.com/v1/flight/rules',
    working: false
};

var HotelAutocompleteConfig = {
    Url: 'https://api.local.travorama.com/v1/autocomplete/hotel/'
};

var FlightAutocompleteConfig = {
    Url: 'https://api.local.travorama.com/v1/autocomplete/airports/'
};

var AirlineAutocompleteConfig = {
    Url: 'https://api.local.travorama.com/v1/autocomplete/airlines/'
};

var CheckVoucherConfig = {
    Url: 'https://api.local.travorama.com/v1/payment/checkvoucher'
};

var CheckBinDiscountConfig = {
    Url: 'https://api.local.travorama.com/v1/payment/checkbindiscount'
};

var SubscribeConfig = {
    Url: 'https://api.local.travorama.com/v1/newsletter/subscribe'
};

var LoginConfig = {
    Url: 'https://api.local.travorama.com/v1/login'
};

var RefundConfig = {
    Url: 'https://api.local.travorama.com/v1/refund'
};
var RescheduleConfig = {
    Url: 'https://api.local.travorama.com/v1/reschedule'
}

var GetProfileConfig = {
    Url: 'https://api.local.travorama.com/v1/profile'
};

var RegisterConfig = {
    Url: 'https://api.local.travorama.com/v1/register'
};

var ResetPasswordConfig = {
    Url: 'https://api.local.travorama.com/v1/resetpassword'
};

var ForgotPasswordConfig = {
    Url: 'https://api.local.travorama.com/v1/forgot'
};

var ChangePasswordConfig = {
    Url: 'https://api.local.travorama.com/v1/changepassword'
};

var ChangeProfileConfig = {
    Url: 'https://api.local.travorama.com/v1/profile'
};

var TrxHistoryConfig = {
    Url: 'https://api.local.travorama.com/v1/trxhistory'
};

var GetReservationConfig = {
    Url: 'https://api.local.travorama.com/v1/rsv/'
};

var ResendConfirmationEmailConfig = {
    Url: 'https://api.local.travorama.com/v1/resendconfirmationemail'
};

var VeritransTokenConfig = {
    Url: 'https://api.sandbox.veritrans.co.id/v2/token',
    ClientKey: 'VT-client-J8i9AzRyIU49D_v3'
};

var uniqueCodePaymentConfig = {
    Url: 'https://api.local.travorama.com/v1/payment/uniquecode'
};

var LoginMobileConfig = {
    Url: 'http://m.local.travorama.com/v1/login'
};

var RefundMobileConfig = {
    Url: 'http://m.local.travorama.com/v1/refund'
};

var RescheduleMobileConfig = {
    Url: 'http://m.local.travorama.com/v1/reschedule'
};

var RegisterMobileConfig = {
    Url: 'http://m.local.travorama.com/v1/register'
};

var ResetPasswordMobileConfig = {
    Url: 'http://m.local.travorama.com/v1/resetpassword'
};

var ForgotPasswordMobileConfig = {
    Url: 'http://m.local.travorama.com/v1/forgot'
};

var ChangePasswordMobileConfig = {
    Url: 'http://m.local.travorama.com/v1/changepassword'
};

var ChangeProfileMobileConfig = {
    Url: 'http://m.local.travorama.com/v1/profile'
};

var ResendConfirmationEmailMobileConfig = {
    Url: 'http://m.local.travorama.com/v1/resendconfirmationemail'
};

var HotelSearchConfig = {
    Url: 'https://api.local.travorama.com/v1/hotel/search'
};

var HotelDetailsConfig = {
    Url: 'https://api.local.travorama.com/v1/hotel/gethoteldetail'
};

var HotelSelectConfig = {
    Url: 'https://api.local.travorama.com/v1/hotel/select'
};

var HotelBookConfig = {
    Url: 'https://api.local.travorama.com/v1/hotel/book',
    working: false
};
function setCookie(cname, cvalue, expTime) {

    if (cname != "accesstoken") {
        var d = new Date();
        d.setTime(d.getTime() + (9999 * 24 * 60 * 60 * 1000));
    }
    else {
        var d = new Date(expTime);
    }
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + "; " + expires + "; path=/";
}

//function setRefreshCookie(cvalue)
//{
//    var d = new Date();
//    d.setTime(d.getTime() + (9999 * 24 * 60 * 60 * 1000));
//    var expires = "expires=" + d.toUTCString();
//    document.cookie = "refreshtoken" + "=" + cvalue + "; " + expires + "; path=/";
//}

//function setAuthCookie(cvalue) {
//    var dAuth = new Date();
//    dAuth.setTime(dAuth.getTime() + (9999 * 24 * 60 * 60 * 1000));
//    var expiresAuth = "expires=" + dAuth.toUTCString();
//    document.cookie = "authKey" + "=" + cvalue + "; " + expiresAuth + "; path=/";
//}

//Get Value from Cookie
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

//Delete Specific value from Cookie
function eraseCookie(name) {
    var d = new Date(-1);
    var expires = "expires=" + d.toUTCString();
    var cvalue = "";
    document.cookie = name + "=" + cvalue + "; " + expires + "; path=/";
}