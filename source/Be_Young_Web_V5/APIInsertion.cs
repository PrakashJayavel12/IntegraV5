using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace DCIA
{
    public static class APIInsertion
    {
        public static string _Tenant = "", _SiteID = "", _portFolioCode = "", _AgentName = "", ReleaseNo = "", CreationDate = "", CreationTime = "", _Token = "", _ActiveStatus = "";
        
        public static void APIHeaderInsertion(DataSet DtMaster, DataSet dss)
        {
            foreach (DataRow dr in DtMaster.Tables["Header"].Rows)
            {
                try
                {
                    _Tenant = dr["TENANT_ID"].ToString();
                    _SiteID = dr["SITE_ID"].ToString();
                    _portFolioCode = dr["PORTFOLIO_CODE"].ToString();
                    CreationDate = dr["FILE_CR_DT"].ToString();
                    CreationTime = dr["FILE_CR_TM"].ToString();
                    _AgentName = dr["AGENT_NAME"].ToString();
                    ReleaseNo = dr["RELEASE_NO"].ToString();
                    _ActiveStatus = dr["ACTIVE_STATUS"].ToString();
                 
                Again:
                    DataRow[] tokendt = dss.Tables[1].Select("TenantId='" + _Tenant.ToString() + "' ");
                    if (tokendt.Length > 0)
                    {
                        Encryption EN = new Encryption();
                        _Token = EN.decrypt(tokendt[0][5].ToString());//
                        
                        if (_Token != string.Empty)
                        {
                            APIHeader Header = new APIHeader
                            {
                                TENANT_ID = _Tenant.ToString(),
                                PORTFOLIO_CODE = _portFolioCode,
                                SITE_ID = _SiteID,
                                FREQUENCY = dr["FREQUENCY"].ToString(),
                                FILE_CR_DT = CreationDate,
                                FILE_CR_TM = CreationTime,
                                AGENT_NAME = _AgentName,
                                RELEASE_NO = ReleaseNo,
                                ACTIVE_STATUS = _ActiveStatus
                            };
                            string jsonHeader = GetJson(Header);
                            string HeaderURL = EN.SSH_Locator(dss.Tables[0].Rows[0]["AgentSSH"].ToString().ToLower()) + "/api/v4/agent_details";//.Replace("https", "http")+"/api/v4/agent_details";//"http://10.10.13.20:8082/api/v4/agent_details";
                            string Header_Response = "";
                            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;                
                            using (var client = new WebClient())
                            {
                                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                                client.Headers.Add("PortfolioCode", _portFolioCode.ToString());
                                client.Headers.Add("Token", _Token);
                                client.Headers.Add("Accept", "application/json");
                                client.Headers.Add("Content-Type", "application/json");
                                Header_Response = client.UploadString(HeaderURL, jsonHeader);
                                Writelog(_portFolioCode.ToString() + ":" + _Token + ":" + HeaderURL + ":" + jsonHeader, "hed");
                                //datas
                                //regenerate_Token();
                                //Dataset dss;

                               // dss.Reset();
                               // dss.ReadXml(Application.StartupPath + "\\Settings.xml");
                               // goto cleanup;
                                //TokenVal = EN.decrypt(dss.Tables[1].Rows[irow]["AgentToken"].ToString());
                               // _Token = EN.decrypt(dss.Tables[1].);
                               // Header_Response = client.UploadString(HeaderURL, jsonHeader);
                                if (Header_Response.Contains("Token Expired") || Header_Response.Contains("Un-Authorized Usage"))
                                {
                                    Writelog("Header API Response: " + Header_Response, "Header_Status_" + DateTime.Today.ToString("ddMMMyy"));
                                    regenerate_Token();
                                    dss.Reset();
                                    dss.ReadXml(Application.StartupPath + "\\Settings.xml");
                                    goto Again;
                                    
                                }
                            }
                            Writelog("Header API Response: " + Header_Response, "Header_Status_" + DateTime.Today.ToString("ddMMMyy"));
                        }
                    }
                }
                catch (Exception e)
                {
                    Writelog(e.Message, "insertAPI_error");
                }
            }
        }

        public static void APIWriteData(DataSet DtMaster, DataSet dss, string FileName, string Filepath)
        {
            string custBP = "", paxtype = "";
            Encryption EN = new Encryption();
            string pos = "";
            int DB_TransCount = 0, DB_ItemCount = 0, DB_PayCount = 0;
            int Insert_TransCount = 0, Insert_ItemCount = 0, Insert_PayCount = 0;
            if (DtMaster.Tables.Contains("Transactions"))
            {
                DB_TransCount = DtMaster.Tables["Transactions"].Rows.Count;
            }
            if (DtMaster.Tables.Contains("ItemDetail"))
            {
                DB_ItemCount = DtMaster.Tables["ItemDetail"].Rows.Count;
            }
            if (DtMaster.Tables.Contains("PaymentDetail"))
            {
                DB_PayCount = DtMaster.Tables["PaymentDetail"].Rows.Count;
            }
            foreach (DataRow dr in DtMaster.Tables["Header"].Rows)
            {
                try
                {
                    _Tenant = dr["TENANT_ID"].ToString();
                    _SiteID = dr["SITE_ID"].ToString();
                    _portFolioCode = dr["PORTFOLIO_CODE"].ToString();
                    CreationDate = dr["FILE_CR_DT"].ToString();
                    CreationTime = dr["FILE_CR_TM"].ToString();
                    _AgentName = dr["AGENT_NAME"].ToString();
                    ReleaseNo = dr["RELEASE_NO"].ToString();
                    DataRow[] tokendt = dss.Tables[1].Select("TenantId='" + _Tenant.ToString() + "' ");
                    if (tokendt.Length > 0)
                    {
                        _Token = EN.decrypt(tokendt[0][5].ToString());
                    }
                }
                catch (Exception ex)
                {
                    Writelog(ex.StackTrace.ToString(), "Header_Fetch-Error");
                }
            }
            foreach (DataRow dr in DtMaster.Tables["Transactions"].Rows)
            {
                string billno = "", billdate = "", billtime = "", bussdate = "";
                try
                {
                    pos = dr["TERMINAL_ID"].ToString();
                    billno = dr["RCPT_NUM"].ToString();
                    billdate = dr["RCPT_DT"].ToString();
                    billtime = dr["RCPT_TM"].ToString();
                    bussdate = dr["BUSINESS_DT"].ToString();
                    double taxamt = 0;
                    double rettaxamt = 0;
                    double netsales = 0;
                    string Trantype = dr["TRAN_STATUS"].ToString();
                    if (Trantype == "SALES")
                    {
                        taxamt = Convert.ToDouble(dr["TAX_AMT"].ToString());
                        netsales = Convert.ToDouble(dr["INV_AMT"]) - (Convert.ToDouble(dr["TAX_AMT"]));
                    }
                    if (Trantype == "RETURN")
                    {
                        rettaxamt = Convert.ToDouble(dr["TAX_AMT"].ToString());
                        netsales = (-1) * (Math.Abs(Convert.ToDouble(dr["RET_AMT"])) - (Convert.ToDouble(dr["TAX_AMT"])));
                    }

                    int intwkyear = 0;
                    int wknum = GetISOWeekOfYear(billdate, out intwkyear);
                    APITransactionDetails tran = new APITransactionDetails
                    {
                        TENANT_ID = _Tenant.ToString(),
                        SITE_ID = _SiteID.ToString(),
                        REC_TYPE = "G100",
                        TERMINAL_ID = pos.ToString(),
                        LOCATION_CODE = pos.ToString(),
                        SHIFT_NO = "1",
                        RECEIPT_NO = billno,
                        RECEIPT_DATE = Convert.ToInt32(billdate),
                        RECEIPT_DAY = Convert.ToInt32(billdate.Substring(6, 2)),
                        RECEIPT_MONTH = Convert.ToInt32(billdate.Substring(4, 2)),
                        RECEIPT_MONTH_YEAR = Convert.ToInt32(billdate.Substring(0, 6)),
                        RECEIPT_YEAR = Convert.ToInt32(billdate.Substring(0, 4)),
                        BUSINESS_DT = bussdate,
                        RECEIPT_TIME = billtime,
                        RECEIPT_HOUR = Convert.ToInt32(billtime.Substring(0, 2)),
                        RECEIPT_MINUTE = Convert.ToInt32(billtime.Substring(2, 2)),
                        RECEIPT_SECONDS = Convert.ToInt32(billtime.Substring(4, 2)),
                        WEEK_YEAR = intwkyear,
                        WEEKNUMBER = wknum,
                        INV_AMT = Convert.ToDouble(dr["INV_AMT"].ToString()),
                        TAX_AMT = taxamt,
                        RET_AMT = Convert.ToDouble(dr["RET_AMT"].ToString()),
                        RET_TAX_AMT = rettaxamt,
                        NET_SALES = netsales,
                        Customer = new APICustomerDetails
                        {
                            CUST_NAME = dr["CUST_NAME"].ToString(),
                            CUST_GENDER = dr["CUST_GEN"].ToString(),
                            CUST_NATION = dr["CUST_NATION"].ToString(),
                            CUST_EMAIL = "",
                            CustomerMobileNo = "",
                            CUST_PAX_TYPE = paxtype,
                            CUST_PP_NO = dr["CUST_PP_NO"].ToString(),
                            CUST_BP_NO = custBP.ToString(),
                            CUST_FL_NO = dr["CUST_FL_NO"].ToString(),
                            CUST_FL_CLASS = dr["CUST_FL_CLASS"].ToString(),
                            CUST_BR_GATE_NO = dr["CUST_BR_GATE_NO"].ToString(),
                            CUST_PNR_NO = dr["CUST_PNR_NO"].ToString(),
                            CUST_ORIGIN = dr["CUST_ORIGIN"].ToString(),
                            CUST_DESTINATION = dr["CUST_DESTINATION"].ToString(),
                            CUST_CONT = "1",
                            CustomerID = "",
                            DATE_OF_DEPARTURE = 1,
                            OfferCode = "",
                            GiftVoucherNo = "",
                            VOUCH_CODE = ""
                        },
                        TRANS_STATUS = dr["TRAN_STATUS"].ToString(),
                        Operation_Currency = dr["OP_CUR"].ToString(),
                        BaseCurrency_Exchange = Convert.ToDouble(dr["BC_EXCH"].ToString()),
                        DISCOUNT = Convert.ToDouble(dr["DISCOUNT"].ToString()),
                        //RET_TAX_AMT = 0,
                        SALESPERSON = "",
                        SERVICECHARGE = 0,
                        FileName = FileName,
                        DataFlowType = "API"
                    };
                    DataRow[] tokendt = dss.Tables[1].Select("TenantId='" + _Tenant.ToString() + "' ");
                    if (tokendt.Length > 0)
                    {
                        _Token = EN.decrypt(tokendt[0][5].ToString());
                    }
                    string json = GetJson(tran);
                    string url = EN.SSH_Locator(dss.Tables[0].Rows[0]["AgentSSH"].ToString().ToLower()) + "/api/v4/integra_api";//"http://10.10.13.20:8082/api/v4/integra_api";
                    string responseValue = "";
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    using (var client = new WebClient())
                    {
                        client.Headers[HttpRequestHeader.ContentType] = "application/json";

                        client.Headers.Add("PortfolioCode", _portFolioCode.ToString());
                        client.Headers.Add("Token", _Token);
                        client.Headers.Add("Accept", "application/json");
                        client.Headers.Add("Content-Type", "application/json");
                        responseValue = client.UploadString(url, json);
                        Writelog(json,"dd");
                        if (responseValue.Contains("Token Expired"))
                        {
                            regenerate_Token();
                        }
                        //duplicate
                        if (responseValue.Contains("Successfully Uploaded") || responseValue.Contains("duplicate"))
                        {
                            Insert_TransCount++;
                        }
                        
                       // Writelog(_Token, "");
                     
                    }
                    Writelog("BillNo : " + dr["RCPT_NUM"].ToString() + "Date : " + dr["RCPT_DT"].ToString() + "Status :" + responseValue, "Trans_API_Status");
                }
                catch (Exception e)
                {
                    Writelog("BillNo : " + dr["RCPT_NUM"].ToString() + "Date : " + dr["RCPT_DT"].ToString() + "Status :" + e.ToString(), "Trans_API_Error");
                }
                if (DtMaster.Tables.Contains("ItemDetail"))
                {
                    DataRow[] dritem = DtMaster.Tables["ItemDetail"].Select("RCPT_NUM='" + billno.ToString() + "' and RCPT_DT ='" + billdate.ToString() + "' ");
                    foreach (DataRow dri in dritem)
                    {
                        try
                        {
                            var itm = new APIItemDetails
                            {
                                TENANT_ID = _Tenant,
                                SITE_ID = _SiteID,
                                POS_TILL = "1",
                                SHIFT_NO = "1",
                                REC_TYPE = "G111",
                                RECEIPT_NO = dri["RCPT_NUM"].ToString(),
                                RECEIPT_DATE = Convert.ToInt32(dri["RCPT_DT"]),
                                RECEIPT_DAY = Convert.ToInt16(dri["RCPT_DT"].ToString().Substring(6, 2)),
                                RECEIPT_MONTH = Convert.ToInt16(dri["RCPT_DT"].ToString().Substring(4, 2)),
                                RECEIPT_YEAR = Convert.ToInt16(dri["RCPT_DT"].ToString().Substring(0, 4)),
                                RECEIPT_TIME = billtime,
                                RECEIPT_HOUR = Convert.ToInt16(billtime.Substring(0, 2)),
                                RECEIPT_MINUTE = Convert.ToInt16(billtime.Substring(2, 2)),
                                RECEIPT_SECONDS = Convert.ToInt16(billtime.Substring(4, 2)),
                                BUSINESS_DT = bussdate,
                                ITEM_CODE = dri["ITEM_CODE"].ToString(),
                                ITEM_NAME = dri["ITEM_NAME"].ToString(),
                                ITEM_QTY = Convert.ToDouble(dri["ITEM_QTY"].ToString()),
                                ITEM_PRICE = Convert.ToDouble(dri["ITEM_PRICE"].ToString()),
                                ITEM_CAT = dri["ITEM_CAT"].ToString(),
                                ITEM_TAX = Convert.ToDouble(dri["ITEM_TAX"].ToString()),
                                ITEM_TAX_TYPE = dri["ITEM_TAX_TYPE"].ToString(),
                                ITEM_NET_AMT = Convert.ToDouble(dri["ITEM_NET_AMT"].ToString()),
                                OP_CUR = dri["OP_CUR"].ToString(),
                                BC_EXCH = Convert.ToDouble(dri["BC_EXCH"].ToString()),
                                ITEM_STATUS = dri["ITEM_STATUS"].ToString(),
                                ITEM_DISCOUNT = Convert.ToDouble(dri["ITEM_DISCOUNT"].ToString()),
                                CUM_STATUS = 0,
                                DEPARTMENT = "",
                                SUBCATEGORY = "",
                                SALESPERSON = "",
                                HSNCODE = "",
                                IN_TIME = DateTime.ParseExact(dri["RCPT_DT"].ToString() + billtime.ToString(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss"),
                                OUT_TIME = DateTime.ParseExact(dri["RCPT_DT"].ToString() + billtime.ToString(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss")
                            };
                            DataRow[] tokendt = dss.Tables[1].Select("TenantId='" + _Tenant.ToString() + "' ");
                            if (tokendt.Length > 0)
                            {
                                _Token = EN.decrypt(tokendt[0][5].ToString());
                            }
                            string Ijson = GetJson(itm);
                            string ItmURL = EN.SSH_Locator(dss.Tables[0].Rows[0]["AgentSSH"].ToString().ToLower()) + "/api/v4/item_details";//"http://10.10.13.20:8082/api/v4/item_details";
                            string ItemresponseValue = "";
                            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                            using (var client = new WebClient())
                            {
                                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                                client.Headers.Add("PortfolioCode", _portFolioCode);
                                client.Headers.Add("Token", _Token);
                                client.Headers.Add("Accept", "application/json");
                                client.Headers.Add("Content-Type", "application/json");
                                ItemresponseValue = client.UploadString(ItmURL, Ijson);
                                if (ItemresponseValue.Contains("Token Expired"))
                                {
                                    regenerate_Token();
                                }
                                if (ItemresponseValue.Contains("Successfully Uploaded") || ItemresponseValue.Contains("duplicate"))
                                {
                                    Insert_ItemCount++;
                                }
                                
                            }
                            Writelog("BillNo : " + dri["RCPT_NUM"].ToString() + "Date : " + dri["RCPT_DT"].ToString() + "Status :" + ItemresponseValue, "ItemAPI_Status");
                        }
                        catch (Exception ei)
                        {
                            Writelog("BillNo : " + dri["RCPT_NUM"].ToString() + "Date : " + dri["RCPT_DT"].ToString() + "Status :" + ei.ToString(), "Item_API_Error");
                        }
                    }
                }
                if (DtMaster.Tables.Contains("PaymentDetail"))
                {
                    DataRow[] drp = DtMaster.Tables["PaymentDetail"].Select("RCPT_NUM='" + billno.ToString() + "' and RCPT_DT ='" + billdate.ToString() + "' ");
                    foreach (DataRow drpp in drp)
                    {
                        try
                        {
                            var pay = new APIPaymentDetails
                            {
                                TENANT_ID = _Tenant,
                                SITE_ID = _SiteID,
                                POS_TILL = pos,
                                SHIFT_NO = "1",
                                REC_TYPE = "G115",
                                RECEIPT_NO = drpp["RCPT_NUM"].ToString(),
                                RECEIPT_DATE = Convert.ToInt32(drpp["RCPT_DT"]),
                                RECEIPT_DAY = Convert.ToInt16(drpp["RCPT_DT"].ToString().Substring(6, 2)),
                                RECEIPT_MONTH = Convert.ToInt16(drpp["RCPT_DT"].ToString().Substring(4, 2)),
                                RECEIPT_YEAR = Convert.ToInt16(drpp["RCPT_DT"].ToString().Substring(0, 4)),
                                RECEIPT_TIME = billtime,
                                RECEIPT_HOUR = Convert.ToInt16(billtime.Substring(0, 2)),
                                RECEIPT_MINUTE = Convert.ToInt16(billtime.Substring(2, 2)),
                                RECEIPT_SECONDS = Convert.ToInt16(billtime.Substring(4, 2)),
                                BUSINESS_DT = bussdate,
                                PAYMENT_NAME = drpp["PAYMENT_NAME"].ToString(),
                                CURRENCY_CODE = drpp["CURRENCY_CODE"].ToString(),
                                EXCHANGE_RATE = Convert.ToDouble(drpp["EXCHANGE_RATE"].ToString()),
                                TENDER_AMOUNT = Convert.ToDouble(drpp["TENDER_AMOUNT"].ToString()),
                                OP_CUR = drpp["OP_CUR"].ToString(),
                                BC_EXCH = Convert.ToDouble(drpp["BC_EXCH"].ToString()),
                                PAYMENT_STATUS = dr["TRAN_STATUS"].ToString()
                            };
                            DataRow[] tokendt = dss.Tables[1].Select("TenantId='" + _Tenant.ToString() + "' ");
                            if (tokendt.Length > 0)
                            {
                                _Token = EN.decrypt(tokendt[0][5].ToString());
                            }
                            string pjson = GetJson(pay);
                            string PayURL = EN.SSH_Locator(dss.Tables[0].Rows[0]["AgentSSH"].ToString().ToLower()) + "/api/v4/integra_payments";//"http://10.10.13.20:8082/api/v4/integra_payments";
                            string PayresponseValue = "";
                            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                            using (var client = new WebClient())
                            {
                                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                                client.Headers.Add("PortfolioCode", _portFolioCode);
                                client.Headers.Add("Token", _Token);
                                client.Headers.Add("Accept", "application/json");
                                client.Headers.Add("Content-Type", "application/json");
                                PayresponseValue = client.UploadString(PayURL, pjson);
                                if (PayresponseValue.Contains("Token Expired"))
                                {
                                    regenerate_Token();
                                }
                                if (PayresponseValue.Contains("Successfully Uploaded") || PayresponseValue.Contains("duplicate"))
                                {
                                    Insert_PayCount++;
                                }
                               
                            }
                            Writelog("BillNo : " + drpp["RCPT_NUM"].ToString() + "Date : " + drpp["RCPT_DT"].ToString() + "Status :" + PayresponseValue, "PaymentAPI_Status");
                        }
                        catch (Exception ep)
                        {
                            Writelog("BillNo : " + drpp["RCPT_NUM"].ToString() + "Date : " + drpp["RCPT_DT"].ToString() + "Status :" + ep.ToString(), "Payment_API_Error");
                        }
                    }
                }
            }

            if (DB_TransCount == Insert_TransCount && DB_ItemCount == Insert_ItemCount && DB_PayCount == Insert_PayCount)
            {
                CreateDirectoryInAppPath("DataFiles\\API\\Transferred\\" + Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd"));
                File.Delete(AppPath(Application.StartupPath) + "\\DataFiles\\API\\Transit\\" + Path.GetFileNameWithoutExtension(Filepath) + ".txt");
                File.Move(Filepath, AppPath(Application.StartupPath) + "\\DataFiles\\API\\Transferred\\" + Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "\\" + Path.GetFileName(Filepath));
            }
            else
            {
                Writelog("Data uploaded successfully, but due to Duplicated bills file not moving", "upload status");
            }
        }
        private static string AppPath(string AppPath)
        {
            string path = AppPath + "\\";
            DirectoryInfo parentDir = Directory.GetParent(path);
            var myParentDir = parentDir.Parent.FullName;
            return myParentDir;
        }
        private static void CreateDirectoryInAppPath(string sDirecName)
        {
            string path = Application.StartupPath + "\\";
            DirectoryInfo parentDir = Directory.GetParent(path);
            var myParentDir = parentDir.Parent.FullName;
            if (Directory.Exists(myParentDir + "\\" + sDirecName) == false)
            {
                Directory.CreateDirectory(myParentDir + "\\" + sDirecName);
            }
        }
        private static void Writelog(string txt, string fname)
        {
            //CreateDirectoryInAppPath("DataFiles\\Common\\Log");
            //FileStream fs = new FileStream(AppPath(Application.StartupPath) + "\\DataFiles\\Common\\Log\\" + fname + DateTime.Today.ToString("_ddMMMyy") + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
            //StreamWriter sw = new StreamWriter(fs);
            //sw.BaseStream.Seek(0, SeekOrigin.End);
            //sw.WriteLine(txt);
            //sw.Flush();
            //sw.Close();
        }
        public static void regenerate_Token()
        {
            Encryption EN = new Encryption();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            string sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            DataSet ds = new DataSet();
            ds.ReadXml(Application.StartupPath + "\\Settings.xml");
            string _server = EN.SSH_Locator(ds.Tables[0].Rows[0]["AgentSSH"].ToString().ToLower()) + "/api/v4/regenerate_token";//"http://10.10.13.20:8082/api/v4/regenerate_token";
            for (int irow = 0; irow < ds.Tables[1].Rows.Count; irow++)
            {
                WebClient client1 = new WebClient();
                string urluser = ds.Tables[1].Rows[irow]["AgentUser"].ToString();
                string urlpwd = ds.Tables[1].Rows[irow]["Agentpwd"].ToString();
                string auth = urluser.Trim() + ":" + urlpwd.Trim();
                client1.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(auth));
                System.Collections.Specialized.NameValueCollection formData = new System.Collections.Specialized.NameValueCollection();
                formData["MacId"] = sMacAddress;
                formData["TenantId"] = ds.Tables[1].Rows[irow]["TenantId"].ToString();
                formData["PortfolioCode"] = ds.Tables[1].Rows[irow]["PortfolioCode"].ToString();
                formData["SiteId"] = ds.Tables[1].Rows[irow]["SiteID"].ToString();

                byte[] responseBytes = client1.UploadValues(_server, "POST", formData);
                string result = Encoding.UTF8.GetString(responseBytes);
                int len = result.LastIndexOf(':') + 1;
                ds.Tables[1].Rows[irow]["AgentToken"] = EN.Encrytion(result.Substring(len, result.Length - len).Replace("}", "").Replace("\"", "").Replace("Message:", ""));
            }
            ds.WriteXml(Application.StartupPath + "\\Settings.xml");
        }
        private static string GetJson(object obj)
        {
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            string json = serializer.Serialize(obj);
            return json;

        }
        public static int GetISOWeekOfYear(string strReceiptDate, out int weekyear)
        {
            int _intYear = Convert.ToInt16(strReceiptDate.Substring(0, 4));
            int _intMonth = Convert.ToInt16(strReceiptDate.Substring(4, 2));
            int _intDay = Convert.ToInt16(strReceiptDate.Substring(6, 2));
            DateTime dtReceiptDate = new DateTime(_intYear, _intMonth, _intDay);
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dtReceiptDate);

            int _intweek1 = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dtReceiptDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                dtReceiptDate = dtReceiptDate.AddDays(3);
                // _intYear += 1;
            }

            int _intweek2 = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dtReceiptDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            if (_intweek2 == _intweek1)
            {
                if (_intMonth == 1 && (_intDay >= 1 && _intDay <= 3) && _intweek2 > 51)
                {
                    _intYear -= 1;
                }
            }
            if (_intweek2 < _intweek1)
            {
                _intYear += 1;
            }
            weekyear = Convert.ToInt32(_intYear.ToString() + _intweek2.ToString("#00"));
            return _intweek2;
        }

    }
}
