using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCIA
{
    public class APITransactionDetails
    {
        public string REC_TYPE { get; set; }
        public string TENANT_ID { get; set; }
        public string SITE_ID { get; set; }
        public string LOCATION_CODE { get; set; }
        public string TERMINAL_ID { get; set; }
        public string SHIFT_NO { get; set; }
        public string RECEIPT_NO { get; set; }
        public int RECEIPT_DATE { get; set; }
        public int RECEIPT_DAY { get; set; }
        public int RECEIPT_MONTH { get; set; }
        public int RECEIPT_YEAR { get; set; }
        public string RECEIPT_TIME { get; set; }
        public int RECEIPT_HOUR { get; set; }
        public int RECEIPT_MINUTE { get; set; }
        public int RECEIPT_SECONDS { get; set; }
        public int WEEKNUMBER { get; set; }
        public int WEEK_YEAR { get; set; }
        public string BUSINESS_DT { get; set; }
        public double INV_AMT { get; set; }
        public double TAX_AMT { get; set; }
        public double RET_AMT { get; set; }
        public double RET_TAX_AMT { get; set; }
        public double NET_SALES { get; set; }
        public double DISCOUNT { get; set; }
        public string TRANS_STATUS { get; set; }
        public string Operation_Currency { get; set; }
        public double BaseCurrency_Exchange { get; set; }
        public string DataFlowType { get; set; }
        public APICustomerDetails Customer { get; set; }
        public string FileName { get; set; }
        public int RECEIPT_MONTH_YEAR { get; set; }
        public string SALESPERSON { get; set; }
        public double SERVICECHARGE { get; set; }
    }
    public class APIHeader
    {
        public string TENANT_ID { get; set; }
        public string PORTFOLIO_CODE { get; set; }
        public string SITE_ID { get; set; }
        public string FREQUENCY { get; set; }
        public string FILE_CR_DT { get; set; }
        public string FILE_CR_TM { get; set; }
        public string AGENT_NAME { get; set; }
        public string RELEASE_NO { get; set; }
        public string ACTIVE_STATUS { get; set; }      
    }
    public class APICustomerDetails
    {
        public string CUST_NAME { get; set; }
        public string CUST_GENDER { get; set; }
        public string CUST_NATION { get; set; }
        public string CUST_EMAIL { get; set; }
        public string CUST_CONT { get; set; }
        public string CUST_PAX_TYPE { get; set; }
        public string CUST_PP_NO { get; set; }
        public string CUST_BP_NO { get; set; }
        public string CUST_FL_NO { get; set; }
        public string CUST_FL_CLASS { get; set; }
        public string CUST_BR_GATE_NO { get; set; }
        public string CUST_PNR_NO { get; set; }
        public string CUST_ORIGIN { get; set; }
        public string CUST_DESTINATION { get; set; }
        public string VOUCH_CODE { get; set; }
        public int DATE_OF_DEPARTURE { get; set; }
        public string CustomerMobileNo { get; set; }
        public string CustomerID { get; set; }
        public string OfferCode { get; set; }
        public string GiftVoucherNo { get; set; }
    }
    public class APIItemDetails
    {
        public string TENANT_ID { get; set; }
        public string SITE_ID { get; set; }
        public string POS_TILL { get; set; }
        public string SHIFT_NO { get; set; }
        public string REC_TYPE { get; set; }
        public string RECEIPT_NO { get; set; }
        public int RECEIPT_DATE { get; set; }
        public int RECEIPT_DAY { get; set; }
        public int RECEIPT_MONTH { get; set; }
        public int RECEIPT_YEAR { get; set; }
        public string RECEIPT_TIME { get; set; }
        public int RECEIPT_HOUR { get; set; }
        public int RECEIPT_MINUTE { get; set; }
        public int RECEIPT_SECONDS { get; set; }
        public string BUSINESS_DT { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_NAME { get; set; }
        public double ITEM_QTY { get; set; }
        public double ITEM_PRICE { get; set; }
        public string ITEM_CAT { get; set; }
        public double ITEM_TAX { get; set; }
        public string ITEM_TAX_TYPE { get; set; }
        public double ITEM_NET_AMT { get; set; }
        public string OP_CUR { get; set; }
        public double BC_EXCH { get; set; }
        public string ITEM_STATUS { get; set; }
        public double ITEM_DISCOUNT { get; set; }
        public int CUM_STATUS { get; set; }
        public string DEPARTMENT { get; set; }
        public string SUBCATEGORY { get; set; }
        public string SALESPERSON { get; set; }
        public string HSNCODE { get; set; }
        public string IN_TIME { get; set; }
        public string OUT_TIME { get; set; }
    }
    public class APIPaymentDetails
    {

        public string TENANT_ID { get; set; }
        public string SITE_ID { get; set; }
        public string POS_TILL { get; set; }
        public string SHIFT_NO { get; set; }
        public string REC_TYPE { get; set; }
        public string RECEIPT_NO { get; set; }
        public int RECEIPT_DATE { get; set; }
        public int RECEIPT_DAY { get; set; }
        public int RECEIPT_MONTH { get; set; }
        public int RECEIPT_YEAR { get; set; }
        public string RECEIPT_TIME { get; set; }
        public int RECEIPT_HOUR { get; set; }
        public int RECEIPT_MINUTE { get; set; }
        public int RECEIPT_SECONDS { get; set; }
        public string BUSINESS_DT { get; set; }
        public string PAYMENT_NAME { get; set; }
        public string CURRENCY_CODE { get; set; }
        public double EXCHANGE_RATE { get; set; }
        public double TENDER_AMOUNT { get; set; }
        public string OP_CUR { get; set; }
        public double BC_EXCH { get; set; }
        public string PAYMENT_STATUS { get; set; }
    }
}
