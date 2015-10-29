﻿// Travorama Account controller
app.controller('accountController', [
    '$http', '$scope', function($http, $scope) {

        // variables
        $scope.pageLoaded = true;
        $scope.currentSection = 'profile';
        $scope.profileForm = {
            active : false
        };
        $scope.passwordForm = {
            active: false
        };
        
        $scope.userProfile = userProfile;
        $scope.userProfile.edit = false;
        $scope.userProfile.updating = false;

        $scope.password = {}
        $scope.password.edit = false;
        $scope.password.updating = false;

        $scope.countries = ["Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Anguilla", "Antigua &amp; Barbuda", "Argentina", "Armenia", "Aruba", "Australia", "Austria", "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bermuda", "Bhutan", "Bolivia", "Bosnia &amp; Herzegovina", "Botswana", "Brazil", "British Virgin Islands", "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Cambodia", "Cameroon", "Cape Verde", "Cayman Islands", "Chad", "Chile", "China", "Colombia", "Congo", "Cook Islands", "Costa Rica", "Cote D Ivoire", "Croatia", "Cruise Ship", "Cuba", "Cyprus", "Czech Republic", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Estonia", "Ethiopia", "Falkland Islands", "Faroe Islands", "Fiji", "Finland", "France", "French Polynesia", "French West Indies", "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Gibraltar", "Greece", "Greenland", "Grenada", "Guam", "Guatemala", "Guernsey", "Guinea", "Guinea Bissau", "Guyana", "Haiti", "Honduras", "Hong Kong", "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Isle of Man", "Israel", "Italy", "Jamaica", "Japan", "Jersey", "Jordan", "Kazakhstan", "Kenya", "Kuwait", "Kyrgyz Republic", "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Macau", "Macedonia", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Mauritania", "Mauritius", "Mexico", "Moldova", "Monaco", "Mongolia", "Montenegro", "Montserrat", "Morocco", "Mozambique", "Namibia", "Nepal", "Netherlands", "Netherlands Antilles", "New Caledonia", "New Zealand", "Nicaragua", "Niger", "Nigeria", "Norway", "Oman", "Pakistan", "Palestine", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Poland", "Portugal", "Puerto Rico", "Qatar", "Reunion", "Romania", "Russia", "Rwanda", "Saint Pierre &amp; Miquelon", "Samoa", "San Marino", "Satellite", "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "South Africa", "South Korea", "Spain", "Sri Lanka", "St Kitts &amp; Nevis", "St Lucia", "St Vincent", "St. Lucia", "Sudan", "Suriname", "Swaziland", "Sweden", "Switzerland", "Syria", "Taiwan", "Tajikistan", "Tanzania", "Thailand", "Timor L'Este", "Togo", "Tonga", "Trinidad &amp; Tobago", "Tunisia", "Turkey", "Turkmenistan", "Turks &amp; Caicos", "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom", "Uruguay", "Uzbekistan", "Venezuela", "Vietnam", "Virgin Islands (US)", "Yemen", "Zambia", "Zimbabwe"];

        // functions
        $scope.changeSection = function (name) {
            $scope.currentSection = name;
        }

        $scope.editForm = function (name) {
            // edit profile form
            if (name == 'profile') {
                $scope.userProfile.edit = !($scope.userProfile.edit);
            }
            else if (name == 'profileSave') {
                console.log('submitting form');
                // submit form to URL
                $http({
                    url: ChangeProfileConfig.Url,
                    method: 'POST',
                    data: {
                        Address: $scope.userProfile.address,
                        FirstName: $scope.userProfile.firstname,
                        LastName: $scope.userProfile.lastname,
                        PhoneNumber: $scope.userProfile.phone,
                        CountryCd: $scope.userProfile.country
                    }
                }).then(function (returnData) {
                    if (returnData.data.Status == 'Success') {
                        console.log('Success requesting change profile');
                        console.log(returnData);
                        $scope.profileForm.edit = false;
                    }
                    else {
                        console.log(returnData.data.Description);
                        console.log(returnData);
                        $scope.profileForm.edit = true;
                    }
                }, function (returnData) {
                    console.log('Failed requesting change profile');
                    console.log(returnData);
                    $scope.profileForm.edit = true;
                });
            }
            if (name == 'password') {
                $scope.password.edit = !($scope.password.edit);
            }
            else if (name == 'passwordSave') {
                console.log('submitting form');
                // submit form to URL
                $http({
                    url: ChangePasswordConfig.Url,
                    method: 'POST',
                    data: {
                        NewPassword: $scope.passwordForm.newPassword,
                        OldPassword: $scope.passwordForm.currentPassword,
                        ConfirmPassword: $scope.passwordForm.confirmationPassword
                    }
                }).then(function (returnData) {
                    $scope.passwordForm.newPassword = '';
                    $scope.passwordForm.currentPassword = '';
                    $scope.passwordForm.confirmationPassword = '';
                    if (returnData.data.Status == 'Success') {
                        console.log('Success requesting reset password');
                        console.log(returnData);
                        $scope.password.edit = false;
                    }
                    else {
                        console.log(returnData.data.Description);
                        console.log(returnData);
                        $scope.password.edit = true;
                    }
                }, function (returnData) {
                    console.log('Failed requesting reset password');
                    console.log(returnData);
                    $scope.password.edit = true;
                });
            }
        }
        

        $scope.passwordForm.submit = function () {
            $scope.passwordForm.submitting = true;
            console.log('submitting form');
            // submit form to URL
            $http({
                url: ChangePasswordConfig.Url,
                method: 'POST',
                data: {
                    password: $scope.passwordForm.newPassword
                }
            }).then(function (returnData) {
                if (returnData.data.Status == 'Success') {
                    console.log('Success requesting reset password');
                    console.log(returnData);
                    $scope.passwordForm.submitting = false;
                    $scope.passwordForm.submitted = true;
                }
                else {
                    console.log(returnData.data.Description);
                    console.log(returnData);
                    $scope.passwordForm.submitting = false;
                }
            }, function (returnData) {
                console.log('Failed requesting reset password');
                console.log(returnData);
                $scope.passwordForm.submitting = false;
            });
        }
    }
]);// account controller


// Travorama forgot password controller
app.controller('passwordController', [
    '$http', '$scope', function ($http, $scope) {

        $scope.pageLoaded = true;
        $scope.form = {
            submitted: false,
            submitting: false,
            email: ''
        };
        $scope.logConsole = function(data) {
            console.log(data);
        }

        $scope.form.submit = function () {
            $scope.form.submitting = true;
            console.log('submitting form');
            // submit form to URL
            $http({
                url: ForgotPasswordConfig.Url,
                method: 'POST',
                data: {
                    email : $scope.form.email
                }
            }).then(function (returnData) {
                if (returnData.data.Status == 'Success') {
                    console.log('Success requesting reset password');
                    console.log(returnData);
                    $scope.form.submitting = false;
                    $scope.form.submitted = true;
                }
                else {
                    console.log(returnData.data.Description);
                    console.log(returnData);
                    $scope.form.submitting = false;
                }
            }, function (returnData) {
                console.log('Failed requesting reset password');
                console.log(returnData);
                $scope.form.submitting = false;
            });
        }

    }
]);// account controller

// Travorama reset controller
app.controller('resetController', [
    '$http', '$scope', function ($http, $scope) {

        $scope.pageLoaded = true;
        $scope.form = {
            submitted: false,
            submitting: false,
            userEmail: userEmail,
            code : code
        };
        $scope.form.submit = function() {
            $scope.form.submitting = true;

            $http({
                url: ResetPasswordConfig.Url,
                method: 'POST',
                data: {
                    Password: $scope.form.password,
                    ConfirmPassword: $scope.form.password,
                    Email: $scope.form.userEmail,
                    Code: $scope.form.code
                }
            }).then(function (returnData) {
                if (returnData.data.Status == 'Success') {
                    console.log('Success requesting reset password');
                    console.log(returnData);
                    $scope.form.submitting = false;
                    $scope.form.submitted = true;
                }
                else {
                    console.log(returnData.data.Description);
                    console.log(returnData);
                    $scope.form.submitting = false;
                }
            }, function (returnData) {
                console.log('Failed requesting reset password');
                console.log(returnData);
                $scope.form.submitting = false;
            });

        }

    }
]);// reset controller

// Travorama Check Order Controller
app.controller('checkController', [
    '$http', '$scope', function ($http, $scope) {

        $scope.pageLoaded = true;
        $scope.form = {
            orderNo: '',
            lastname: '',
            submitting : false
        };

    }
]);// reset controller

// Travorama Check Order Controller
app.controller('authController', [
    '$scope', function ($scope) {

        $scope.pageLoaded = true;
        $scope.form = {
            email: '',
            password: '',
            submitting: false
        };

    }
]);// reset controller