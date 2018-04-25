'use strict';
import React from 'react';
import { observable, action } from "mobx";
import { observer } from "mobx-react";
import Layout from './PaymentPage';
import { getCreditBalance, sumTotalBill } from './PaymentController';

export default const PaymentPageStateContainer = observer(
class PaymentPageStateContainer extends React.Component {

  method = null
  creditBalance = this.props.creditBalance
  discountVoucherAmount = ''
  discountVoucherCode = ''
  errorMessage = ''
  isLoadingCreditBalance = false
  isLoadingDiscountVoucher = false

  setMethod(method) { this.method = method }

  componentDidMount() {
    this.isLoadingCreditBalance = true;
    getCreditBalance()
      .then( r => {
        if (r.status=200) this.creditBalance = r.discount;
        else this.errorMessage = r.error;
      })
      .finally( () => this.isLoadingCreditBalance = false);
  }

  applyDiscountVoucher = () => {
    this.isLoadingDiscountVoucher = true;
    getCreditBalance()
      .then( r => {
        if (r.status=200) this.discountVoucherAmount = r.discount;
        else this.errorMessage = r.error;
      })
      .finally( () => this.isLoadingDiscountVoucher = false);
  }

  onChangedVoucherCode = e => {
    this.discountVoucherCode = e.target.value;
  }

	render() {
	  return(
      <Layout
        method={this.method}
        setMethod={this.setMethod}
        creditBalance={this.creditBalance}
        discountVoucherAmount={this.discountVoucherAmount}
        discountVoucherCode={this.discountVoucherCode}
        onChangedVoucherCode={this.onChangedVoucherCode}
        applyDiscountVoucher={this.applyDiscountVoucher}

        rsvNo={this.props.rsvNo}
        discCd={this.props.discCd}
        headerTitle={this.props.headerTitle}
        pricingDetails={this.props.pricingDetails}
        refund='tidak bisa refund untuk aktivitas ini'
        originalPrice={this.props.originalPrice}
        termsUrl={this.props.termsUrl}
        privacyUrl={this.props.privacyUrl}
        // mandiriClickpayToken={this.props.mandiriClickpayToken
        // cartId='0'
      />
	  );
	}
}
decorate(PaymentPageStateContainer, {
  method: observable,
  creditBalance: observable,
  discountVoucherAmount: observable,
  discountVoucherCode: observable,
  errorMessage: observable,
  isLoadingCreditBalance: observable,
  isLoadingDiscountVoucher: observable,
  setMethod: action,
  componentDidMount: action,
  applyDiscountVoucher: action,
  onChangedVoucherCode: action,
});
