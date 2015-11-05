﻿//******************************************
// Variables
var SystemConfig = {
    SystemBusy: false
};

var SearchHotelConfig = {
    Url: '$apiUrl$$hotelPath$',
    ResultCount: 24
};

var SearchRoomConfig = {
    Url: '$apiUrl$$roomPath$'
};

var FlightSearchConfig = {
    Url: '$apiUrl$$flightPath$'
};

var RevalidateConfig = {
    Url: '$apiUrl$$flightRevalidatePath$',
    working: false
};

var FlightBookConfig = {
    Url: '$apiUrl$$flightBookPath$',
    working: false
};

var GetRulesConfig = {
    Url: '$apiUrl$$flightRulesPath$',
    working: false
};

var HotelAutocompleteConfig = {
    Url: '$apiUrl$$autocompleteHotelLocationPath$'
};

var FlightAutocompleteConfig = {
    Url: '$apiUrl$$autocompleteAirportPath$'
};

var AirlineAutocompleteConfig = {
    Url: '$apiUrl$$autocompleteAirlinePath$'
};

var CheckVoucherConfig = {
    Url: '$apiUrl$$checkVoucherPath$'
};

var SubscribeConfig = {
    Url: '$apiUrl$$subscribePath$'
};

var ResetPasswordConfig = {
    Url: '$rootUrl$$resetPasswordPath$'
};

var ForgotPasswordConfig = {
    Url: '$rootUrl$$forgotPasswordPath$'
};

var ChangePasswordConfig = {
    Url: '$rootUrl$$changePasswordPath$'
};

var ChangeProfileConfig = {
    Url: '$rootUrl$$changeProfilePath$'
};