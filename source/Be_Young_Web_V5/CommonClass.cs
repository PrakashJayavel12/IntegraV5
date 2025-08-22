using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DCIA
{
    public class Itemdetails
    {
        public string REC_TYPE { get; set; }
        public string RCPT_NUM { get; set; }
        public string RCPT_DT { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_NAME { get; set; }
        public string ITEM_QTY { get; set; }
        public string ITEM_PRICE { get; set; }
        public string ITEM_CAT { get; set; }
        public string ITEM_TAX { get; set; }
        public string ITEM_TAX_TYPE { get; set; }
        public string ITEM_NET_AMT { get; set; }
        public string OP_CUR { get; set; }
        public string BC_EXCH { get; set; }
        public string ITEM_STATUS { get; set; }
        public string ITEM_DISCOUNT { get; set; }

    }
    public class PaymentDeatils
    {
        public string REC_TYPE { get; set; }
        public string RCPT_NUM { get; set; }
        public string RCPT_DT { get; set; }
        public string PAYMENT_NAME { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string EXCHANGE_RATE { get; set; }
        public string TENDER_AMOUNT { get; set; }
        public string OP_CUR { get; set; }
        public string BC_EXCH { get; set; }
        public string PAYMENT_STATUS { get; set; }
    }
    public class Transactions
    {
        public string REC_TYPE { get; set; }
        public string LOCATION_CODE { get; set; }
        public string TERMINAL_ID { get; set; }
        public string SHIFT_NO { get; set; }
        public string RCPT_NUM { get; set; }
        public string RCPT_DT { get; set; }
        public string BUSINESS_DT { get; set; }
        public string RCPT_TM { get; set; }
        public string INV_AMT { get; set; }
        public string TAX_AMT { get; set; }
        public string RET_AMT { get; set; }
        public string CUST_NAME { get; set; }
        public string CUST_GEN { get; set; }
        public string CUST_NATION { get; set; }
        public string CUST_EMAIL { get; set; }
        public string CUST_CONT { get; set; }
        public string CUST_PAX_TYPE { get; set; }
        public string CUST_PP_NO { get; set; }
        public string CUST_BP { get; set; }
        public string CUST_FL_NO { get; set; }
        public string CUST_FL_CLASS { get; set; }
        public string CUST_BR_GATE_NO { get; set; }
        public string CUST_PNR_NO { get; set; }
        public string CustomerMobileNo { get; set; }
        public string CustomerID { get; set; }
        public string OfferCode { get; set; }
        public string GiftVoucherNo { get; set; }
        public string TRAN_STATUS { get; set; }
        public string OP_CUR { get; set; }
        public string BC_EXCH { get; set; }
        public string DISCOUNT { get; set; }
        public string CUST_ORIGIN { get; set; }
        public string CUST_DESTINATION { get; set; }
        public string VOUCH_CODE { get; set; }
        public int DATE_OF_DEPARTURE { get; set; }
        public Itemdetails[] ItemDetail { get; set; }
        public PaymentDeatils[] PaymentDetail { get; set; }
    }
    public class Header
    {
        public string REC_TYPE { get; set; }
        public string FILE_TYPE { get; set; }
        public string TENANT_ID { get; set; }
        public string PORTFOLIO_CODE { get; set; }
        public string SITE_ID { get; set; }
        public string FREQUENCY { get; set; }
        public string FILE_CR_DT { get; set; }
        public string FILE_CR_TM { get; set; }
        public string[] TRANS_DATE { get; set; }
        public int FILE_SEQ_NUM { get; set; }
        public string FILE_NAME { get; set; }
        public string AGENT_NAME { get; set; }
        public string RELEASE_NO { get; set; }
        public string ACTIVE_STATUS { get; set; }
    }
    public class Footer
    {
        public string REC_TYPE { get; set; }
        public string NO_OF_RECORDS { get; set; }
        public double HASH_TOTAL { get; set; }

    }
    public class Root
    {
        public Header Header { get; set; }
        public Transactions[] Transactions { get; set; }
        public Footer Footer { get; set; }
    }
    public static class CommonClass
    {
        public static DataTable dtItem = new DataTable("Item");
        public static DataTable dtTrans = new DataTable("Transaction");
        public static DataTable dtPay = new DataTable("Payment");
        public static DataTable dtHdr = new DataTable("Header");

        public static void CreateTables()
        {
            dtHdr.Columns.Clear();
            dtHdr.Columns.Add("fileid", System.Type.GetType("System.String"));
            dtHdr.Columns.Add("filename", System.Type.GetType("System.String"));
            dtHdr.Columns.Add("sitecode", System.Type.GetType("System.String"));
            dtHdr.Columns.Add("tenantid", System.Type.GetType("System.String"));
            dtHdr.Columns.Add("fileseqno", System.Type.GetType("System.String"));
            dtHdr.Columns.Add("businessdate", System.Type.GetType("System.String"));
            dtHdr.Columns.Add("timestamp", System.Type.GetType("System.String"));
            dtHdr.Columns.Add("noofrecords", System.Type.GetType("System.Int16"));
            dtHdr.Columns.Add("hashtotal", System.Type.GetType("System.Decimal"));
            dtHdr.Columns.Add("status", System.Type.GetType("System.Int16"));
            dtHdr.Columns.Add("failremarks", System.Type.GetType("System.String"));

            dtItem.Columns.Clear();
            dtItem.Columns.Add("REC_TYPE", System.Type.GetType("System.String"));
            dtItem.Columns.Add("RCPT_NUM", System.Type.GetType("System.String"));
            dtItem.Columns.Add("RCPT_DT", System.Type.GetType("System.String"));
            dtItem.Columns.Add("ITEM_CODE", System.Type.GetType("System.String"));
            dtItem.Columns.Add("ITEM_NAME", System.Type.GetType("System.String"));
            dtItem.Columns.Add("ITEM_QTY", System.Type.GetType("System.String"));
            dtItem.Columns.Add("ITEM_PRICE", System.Type.GetType("System.String"));
            dtItem.Columns.Add("ITEM_CAT", System.Type.GetType("System.String"));
            dtItem.Columns.Add("ITEM_TAX", System.Type.GetType("System.String"));
            dtItem.Columns.Add("ITEM_TAX_TYPE", System.Type.GetType("System.String"));
            dtItem.Columns.Add("ITEM_NET_AMT", System.Type.GetType("System.String"));
            dtItem.Columns.Add("OP_CUR", System.Type.GetType("System.String"));
            dtItem.Columns.Add("BC_EXCH", System.Type.GetType("System.String"));
            dtItem.Columns.Add("ITEM_STATUS", System.Type.GetType("System.String"));
            dtItem.Columns.Add("ITEM_DISCOUNT", System.Type.GetType("System.String"));


            dtTrans.Columns.Clear();
            dtTrans.Columns.Add("REC_TYPE", typeof(string));                  // 1
            dtTrans.Columns.Add("LOCATION_CODE", typeof(string));             // 2
            dtTrans.Columns.Add("TERMINAL_ID", typeof(string));               // 3
            dtTrans.Columns.Add("SHIFT_NO", typeof(string));                  // 4
            dtTrans.Columns.Add("RCPT_NUM", typeof(string));                  // 5
            dtTrans.Columns.Add("RCPT_DT", typeof(string));                   // 6
            dtTrans.Columns.Add("BUSINESS_DT", typeof(string));               // 7
            dtTrans.Columns.Add("RCPT_TM", typeof(string));                   // 8
            dtTrans.Columns.Add("INV_AMT", typeof(string));                   // 9
            dtTrans.Columns.Add("TAX_AMT", typeof(string));                   // 10
            dtTrans.Columns.Add("RET_AMT", typeof(string));                   // 11
            dtTrans.Columns.Add("CUST_NAME", typeof(string));                 // 12
            dtTrans.Columns.Add("CUST_GEN", typeof(string));                  // 13
            dtTrans.Columns.Add("CUST_NATION", typeof(string));               // 14
            dtTrans.Columns.Add("CUST_EMAIL", typeof(string));                // 15
            dtTrans.Columns.Add("CUST_CONT", typeof(string));                 // 16
            dtTrans.Columns.Add("CUST_PAX_TYPE", typeof(string));             // 17
            dtTrans.Columns.Add("CUST_PP_NO", typeof(string));                // 18
            dtTrans.Columns.Add("CUST_BP", typeof(string));                   // 19
            dtTrans.Columns.Add("CUST_FL_NO", typeof(string));                // 20
            dtTrans.Columns.Add("CUST_FL_CLASS", typeof(string));             // 21
            dtTrans.Columns.Add("CUST_BR_GATE_NO", typeof(string));           // 22
            dtTrans.Columns.Add("CUST_PNR_NO", typeof(string));               // 23
            dtTrans.Columns.Add("CustomerMobileNo", typeof(string));          // 24
            dtTrans.Columns.Add("CustomerID", typeof(string));                // 25
            dtTrans.Columns.Add("OfferCode", typeof(string));                 // 26
            dtTrans.Columns.Add("GiftVoucherNo", typeof(string));             // 27
            dtTrans.Columns.Add("TRAN_STATUS", typeof(string));               // 28
            dtTrans.Columns.Add("OP_CUR", typeof(string));                    // 29
            dtTrans.Columns.Add("BC_EXCH", typeof(string));                   // 30
            dtTrans.Columns.Add("DISCOUNT", typeof(string));                  // 31
            dtTrans.Columns.Add("CUST_ORIGIN", typeof(string));               // 32
            dtTrans.Columns.Add("CUST_DESTINATION", typeof(string));          // 33
            dtTrans.Columns.Add("VOUCH_CODE", typeof(string));                // 34
            dtTrans.Columns.Add("DATE_OF_DEPARTURE", typeof(string));         // 35

            dtPay.Columns.Clear();
            dtPay.Columns.Add("REC_TYPE", System.Type.GetType("System.String"));
            dtPay.Columns.Add("RCPT_NUM", System.Type.GetType("System.String"));
            dtPay.Columns.Add("RCPT_DT", System.Type.GetType("System.String"));
            dtPay.Columns.Add("PAYMENT_NAME", System.Type.GetType("System.String"));
            dtPay.Columns.Add("CURRENCY_CODE", System.Type.GetType("System.String"));
            dtPay.Columns.Add("EXCHANGE_RATE", System.Type.GetType("System.String"));
            dtPay.Columns.Add("TENDER_AMOUNT", System.Type.GetType("System.String"));
            dtPay.Columns.Add("OP_CUR", System.Type.GetType("System.String"));
            dtPay.Columns.Add("BC_EXCH", System.Type.GetType("System.String"));
            dtPay.Columns.Add("PAYMENT_STATUS", System.Type.GetType("System.String"));

            

           

        }
    }
}
