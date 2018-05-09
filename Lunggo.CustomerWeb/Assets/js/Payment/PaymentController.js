import Moment from 'moment';
import 'moment/locale/id';
import { fetchTravoramaApi, AUTH_LEVEL } from '../ApiWrapper';

export function sumTotalBill(...args) {
  return args.reduce((previous, current) => {
    return previous + current;
  }, 0);
}

export function convertToAbsoluteNegative(value) {
  if (value > 0) return (-1) * value;
  else return value;
}

export function isCreditCardNumberFormatValid(value) {
  return !!value && !isNaN(value) && value.toString().length == 16;
}

export function isCVVFormatValid(value) {
  const length = !!value && value.toString().length;
  return !!value && !isNaN(value) && (length == 3 || length == 4);
}

export function validateCreditCardExpiryDate(month_MM, year_YYYY) {
  if (month_MM == 0 || month_MM == '' || year_YYYY == '') {
    return 'Mohon isi tanggal kadaluarsa kartu';
  }
  const value = Moment(`${month_MM}-${year_YYYY}`, 'MM-YYYY').endOf('month');
  if (value.isValid() == false) {
    return 'Mohon isi dengan tanggal yang valid';
  }
  else if (Moment() > value) {
    return 'Kartu telah kadaluarsa';
  }
  else return ''; //// VALID
}

//// check if name only contains alphabetical, empty string, or null
export function isNameAlphabetical(name, acceptEmpty = false) {
  var re = /^[a-zA-Z ]+$/;
  if (acceptEmpty) return name == null || name == "" || re.test(name);
  else return re.test(name);
}

//// check if input is a number or null
export function isNumberOrEmpty(number) {
  var re = /^[0-9]+$/;
  return (number == "" || number == null || re.test(number));
}

export function validateCreditCard({ ccNo, name, cvv, expiry }) {
  const ccNumberErrorMessage = isCreditCardNumberFormatValid(ccNo) ? '' : 'Mohon masukkan nomor kartu yang valid';
  const nameErrorMessage = isNameAlphabetical(name) ? '' : 'Mohon masukkan format nama yang valid';
  const cvvErrorMessage = isCVVFormatValid(cvv) ? '' : 'Gunakan nomor cvv yang tertera di belakang kartu (3-4 digit)';
  const expiryErrorMessage = validateCreditCardExpiryDate(expiry.month, expiry.year);
  if (ccNumberErrorMessage || nameErrorMessage || cvvErrorMessage || expiryErrorMessage) {
    return {
      errorMessages: {
        ccNo: ccNumberErrorMessage,
        name: nameErrorMessage,
        cvv: cvvErrorMessage,
        expiry: expiryErrorMessage
      }
    };
  } else return 'VALID';
}

const fetchPayAPI = async ({ cartId, method, discCd, methodData }) => {
  if (method == 'card') method = `creditCard`;
  const version = 'v1';
  let request = {
    path: `/${version}/payment/cart/checkout`,
    method: 'POST',
    requiredAuthLevel: AUTH_LEVEL.User,
    data: {
      cartId, discCd, [method]: methodData,
      method: method.charAt(0).toUpperCase() + method.slice(1)
    },
  }
  return await fetchTravoramaApi(request);
}

const proceedWithoutVeritransToken = () => {
  throw `not implemented!!`;
  const { formData, totalPrice, voucher, cartId, method, discCd } = paymentData;
  fetchPayAPI({ cartId, method, discCd, methodData });
}

const getVeritransToken = (paymentData, changePaymentStepLayout) => {
  const { formData, totalPrice, voucher = {} } = paymentData;
  // formData = { ccNo, name, cvv, expiry: {month, year} };
  Veritrans.url = VeritransTokenConfig.Url;
  Veritrans.client_key = VeritransTokenConfig.ClientKey;

  const card = () => ({
    'card_number': formData.ccNo,
    'card_exp_month': formData.expiry.month,
    'card_exp_year': formData.expiry.year,
    'card_cvv': formData.cvv,
    //// Set 'secure', 'bank', and 'gross_amount', if the merchant
    //// wants transaction to be processed with 3D Secure
    'secure': true,
    'bank': 'mandiri',
    ///'gross_amount': totalPrice - voucher.amount // + getMdr(),
    'gross_amount': 8
  })
  return new Promise((resolve, reject) => {
    // run the veritrans function to check credit card
    Veritrans.token(card, response => {
      if (response.redirect_url) {
        changePaymentStepLayout('paymentOtp', response.redirect_url); // 3Dsecure transaction. Open 3Dsecure dialog
      } else if (response.status_code == '200') { // success 3d secure or success normal
        changePaymentStepLayout('loading'); //close 3d secure dialog if any
        resolve(response.token_id); // return store token data
      } else {
        changePaymentStepLayout('failed');
        reject(`Terjadi kesalahan pada sistem, mohon menggunakan metode pembayaran lain`);
        /*        $('#submit-button').removeAttr('disabled');
                  // $('#message').text(response.status_message); //// Show status message.
                  $scope.error.message = (response.status_code == 400) ?
                      "Terdapat kesalahan pada pengisian kartu atau kartu tidak terdaftar" :
                      "Terjadi kesalahan pada sistem, mohon menggunakan metode pembayaran lain";
                  scrollPage($('*[data-payment-method="CreditCard"]'));        */
      }
    });
  });
}

export const pay = async (paymentData, errorMessagesHandler, changePaymentStepLayout) => {
  const { cartId, method, discCd, formData } = paymentData;
  let methodData;
  //// VALIDATION
  if (method == 'card') {
    const month = formData.expiry.substr(0, 2);
    const year = `20` + formData.expiry.substr(2, 2);
    formData.expiry = { month, year };
    //const month_MM, year_YY; [month_MM, year_YY] = expiry.split('/');

    const isValid = validateCreditCard(formData);
    if (isValid !== 'VALID') {
      errorMessagesHandler(isValid.errorMessages);
      return `VALIDATION ERROR`;
    }
    try {
      const tokenId = await getVeritransToken(paymentData, changePaymentStepLayout);
      methodData = {
        tokenId,
        holderName: paymentData.formData.name,
        hashedPan: paymentData.formData.ccNo,
        reqBinDiscount: false,
      }
    } catch (e) { console.error(e); }
  } else methodData = proceedWithoutVeritransToken(); //TODO
  const res = await fetchPayAPI({ cartId, method, discCd, methodData });
  if (res.status == 200) {
    if (!!res.redirectionUrl) { /* REDIRECT!! to res.redirectionUrl */ }
    else { changePaymentStepLayout('success'); }
  }
  else changePaymentStepLayout('failed'); //res.error;
}
/*
export getMdr = mdr => {
  var method = 999;
  switch ($scope.paymentMethod) {
      case 'BankTransfer': method = 0; break;
      case 'CreditCard': method = 1; break;
      case 'VirtualAccount': method = 2; break;
  }
  function select(obj) {
      return obj.PaymentMethod == method;
  }
  var a = mdr.filter(select)[0];
  ///// TODO : include discount
  let price = $scope.totalPrice - $scope.voucher.amount;
  return a ? Math.ceil(price * a.Percentage / 100) : 0;
}*/

export const getCreditBalance = async cartId =>
  checkVoucher(cartId, 'REFERRALCREDIT')

export const checkVoucher = async (cartId, code) => {
  const version = 'v1';
  let request = {
    path: `/${version}/payment/checkvoucher`,
    method: 'POST',
    requiredAuthLevel: AUTH_LEVEL.User,
    data: { cartId, code }
  }
  return await fetchTravoramaApi(request);
}
/*
if (voucher.status == 'Success') `Voucher valid`
if (voucher.status == 'ERPVCH01') `Voucher tidak dapat digunakan`
if (voucher.status == 'ERPVCH02') `Kode voucher salah`
if (voucher.status == 'ERPVCH03') `Voucher sudah habis`
if (voucher.status == 'ERPVCH04') `Syarat dan ketentuan voucher tidak terpenuhi`
if (voucher.status == 'ERPVCH05') `Voucher tidak dapat digunakan`
if (voucher.status == 'ERPVCH06') `Voucher sudah digunakan`
if (voucher.status == 'ERPVCH08') `Syarat dan ketentuan voucher tidak terpenuhi`
if (voucher.status == 'ERPVCH09') `Voucher sudah habis`
if (voucher.status == 'ERPVCH10') `Syarat dan ketentuan voucher tidak terpenuhi`
if (voucher.status == 'ERPVCH11') `Syarat dan ketentuan voucher tidak terpenuhi`
if (voucher.status == 'ERPVCH99') `Voucher tidak dapat digunakan`
*/


