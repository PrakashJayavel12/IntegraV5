using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Renci.SshNet;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using WebSync;
using System.Linq;
using System.Text.RegularExpressions;
//using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace DCIA
{
    public class clsGenerate : IAdsr
    {
        private string sline = "";
        private string sdb = "";
        private string scmp = "";
        private string sapp = "";
        private string cpitm = "";
        private string TerminalwithReceiptno = "";
        private string _DataFlowType = "";
        private string psptrlapi = "";
        private string[] dbs;
        private string[] sposs;
        private string[] center;
        private int count = 0;
        private double dtotinv = 0.0;
        private DataSet datas = new DataSet();
        private string seqdts = "";
        private string chkBoarding = "";
        private string BoardingPath = "";
        private string _portfolioCode = "";
        private string _SiteID = "";
        private string _AgentName = "";
        private string _ReleaseNo = "201910173200030";
        private string ssftp = "";
        private string ssftpuser = "";
        private string ssftpwd = "";
        private string ssftpfldr = "";
        Decimal dtot = 0;
        private string ssftport = "";
        private string winauth = "";
        private string TokenVal = "";
        private string RegStatus = "";
        private List<string> Ls = new List<string>();
        private List<DateTime> allDates = new List<DateTime>();
        private FtpConnection Ftpconn = new FtpConnection();

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool GenerateOldData { get; set; }

        public string LastPooledDate { get; set; }

        public string TittleHeader { get; set; }

        public string SegmentHeader { get; set; }

        public void GenerateFile()
        {
            CommonClass.CreateTables();
            DataSet dataSet1 = new DataSet();
            int num1 = (int)dataSet1.ReadXml(Application.StartupPath + "\\Settings.xml");
            if (dataSet1.Tables.Count == 0 || dataSet1.Tables[0].Rows.Count == 0)
                return;
            string str1 = "";
            string str2 = "";
            string spos = "";
            DataSet dataSet2 = new DataSet();
            this.chkBoarding = dataSet1.Tables[0].Rows[0]["IntegrateBoarding"].ToString();
            string str3 = "";
            string str4 = "";
            string suser = "";
            string spwd = "";
            string str5 = "";
            string str6 = "";
            string str7 = "";
            string str8 = "";
            string str9 = "";
            string AccountName = "";
            string storeid = "";
            string server = "";
            string Server2 = "";
            this._DataFlowType = dataSet1.Tables[0].Rows[0]["DataFlowType"].ToString();//PasptrlAPI
            psptrlapi = dataSet1.Tables[0].Rows[0]["PasptrlAPI"].ToString();
            string input = dataSet1.Tables[0].Rows[0]["Frequency"].ToString();
            this.cpitm = dataSet1.Tables[0].Rows[0]["CaptureItems"].ToString();
            this.TerminalwithReceiptno = dataSet1.Tables[0].Rows[0]["ADDTermIDwithBill"].ToString();
            this.winauth = dataSet1.Tables[0].Rows[0]["IsWinAuth"].ToString();
            Encryption encryption = new Encryption();
            Random random = new Random();

            this.CreateDirectoryInAppPath("DataFiles\\FileTransfer\\SyncFiles");
            this.CreateDirectoryInAppPath("DataFiles\\FileTransfer\\Backup");
            this.CreateDirectoryInAppPath("DataFiles\\SettingsBaukup");
            this.CreateDirectoryInAppPath("DataFiles\\API\\Transferred");
            this.CreateDirectoryInAppPath("DataFiles\\API\\Transit");
            this.CreateDirectoryInAppPath("DataFiles\\API\\Transit\\Header");
            this.CreateDirectoryInAppPath("DataFiles\\Common\\Log");
            this.CreateDirectoryInAppPath("DataFiles\\Common\\SyncError");
            try
            {
                if (dataSet1.Tables[1].Rows.Count > 0)
                {
                    for (int index1 = 0; index1 < dataSet1.Tables[1].Rows.Count; ++index1)
                    {
                        if (!(dataSet1.Tables[1].Rows[index1]["Selected"].ToString() == string.Empty) && !(dataSet1.Tables[1].Rows[index1]["Selected"].ToString() == "False"))
                        {
                            this.BoardingPath = dataSet1.Tables[1].Rows[index1]["BoardingPath"].ToString();
                            this._portfolioCode = dataSet1.Tables[1].Rows[index1]["PortfolioCode"].ToString();
                            this._SiteID = dataSet1.Tables[1].Rows[index1]["SiteID"].ToString();
                            this.dbs = encryption.decrypt(dataSet1.Tables[1].Rows[index1]["Database"].ToString()).Split(',');
                            this.sdb = this.dbs[0];
                            suser = encryption.decrypt(dataSet1.Tables[1].Rows[index1]["User"].ToString());
                            spwd = encryption.decrypt(dataSet1.Tables[1].Rows[index1]["Password"].ToString());
                            server = encryption.decrypt(dataSet1.Tables[1].Rows[index1]["Server"].ToString());
                            Server2 = encryption.decrypt(dataSet1.Tables[1].Rows[index1]["txtServer2Name"].ToString());
                            string sten = dataSet1.Tables[1].Rows[index1]["tenantid"].ToString();
                            string[] strArray = dataSet1.Tables[1].Rows[index1]["LastPooleddate"].ToString().Split(',');
                            str2 = dataSet1.Tables[1].Rows[index1]["LastEODDate"].ToString();
                            this.sposs = dataSet1.Tables[1].Rows[index1]["poscode"].ToString().Split(',');
                            this.scmp = dataSet1.Tables[1].Rows[index1]["CompanyName"].ToString();
                            string Appkey = dataSet1.Tables[1].Rows[index1]["Appkey"].ToString();
                            this.sapp = dataSet1.Tables[1].Rows[index1]["ApplciationName"].ToString();
                            AccountName = dataSet1.Tables[1].Rows[0]["AccountName"].ToString();
                            server = encryption.decrypt(dataSet1.Tables[1].Rows[index1]["Server"].ToString());
                            center = dataSet1.Tables[1].Rows[index1]["CenterCode"].ToString().Split(','); ;
                            storeid = dataSet1.Tables[1].Rows[index1]["storeId"].ToString();
                            this.RegStatus = dataSet1.Tables[1].Rows[index1]["Registered"].ToString();
                            this.TokenVal = encryption.decrypt(dataSet1.Tables[1].Rows[index1]["AgentToken"].ToString());
                            if (this._DataFlowType.ToString() == "FILE" || this.RegStatus.ToUpper() == "YES" && !string.IsNullOrEmpty(this.TokenVal))
                            {
                                if (Convert.ToBoolean(dataSet1.Tables[1].Rows[index1]["SFTP"]))
                                {
                                    this.ssftp = dataSet1.Tables[1].Rows[index1]["SFTPServer"].ToString();
                                    this.ssftpuser = dataSet1.Tables[1].Rows[index1]["SFTPUser"].ToString();
                                    this.ssftpwd = encryption.decrypt(dataSet1.Tables[1].Rows[index1]["SFTPPwd"].ToString());
                                    Writelog(this.ssftp, "SFTP password");
                                    this.ssftport = dataSet1.Tables[1].Rows[index1]["SFTPPort"].ToString();
                                    this.ssftpfldr = dataSet1.Tables[1].Rows[index1]["SFTPFolder"].ToString();
                                }
                                else
                                {
                                    str7 = dataSet1.Tables[1].Rows[index1]["ftpServer"].ToString();
                                    str8 = dataSet1.Tables[1].Rows[index1]["ftpUser"].ToString();
                                    str9 = encryption.decrypt(dataSet1.Tables[1].Rows[index1]["ftpPwd"].ToString());
                                }
                                str1 = "";
                                int index2 = 0;
                                DateTime dLastDt;
                                if (!this.GenerateOldData)
                                {
                                    dLastDt = Convert.ToDateTime(strArray[index2]);
                                    this.StartDate = Convert.ToDateTime(dLastDt.ToString("dd/MMM/yyyy"));
                                    this.EndDate = Convert.ToDateTime(DateTime.Today.ToString("dd/MMM/yyyy"));
                                }
                                else
                                {
                                    dLastDt = this.StartDate;
                                    dLastDt = Convert.ToDateTime(this.StartDate.ToString("dd/MMM/yyyy"));
                                }
                                this.seqdts = "";
                                DateTime dateTime1 = Convert.ToDateTime(this.StartDate.ToString("dd/MMM/yyyy"));
                                for (DateTime dateTime2 = Convert.ToDateTime(this.EndDate.ToString("dd/MMM/yyyy")); dateTime1 <= dateTime2; dateTime1 = dateTime1.AddDays(1.0))
                                {
                                    this.Ls.Add(dateTime1.ToString("yyyyMMdd"));
                                    clsGenerate clsGenerate = this;
                                    clsGenerate.seqdts = clsGenerate.seqdts + "," + dateTime1.ToString("yyyyMMdd");
                                }
                                this.seqdts = this.seqdts.Substring(1, this.seqdts.Length - 1);
                                spos = this.sposs[index2].ToString();
                                DateTime dateTime3 = Convert.ToDateTime("01/Jan/2000");
                                this.GetDatesBetween(this.StartDate, this.EndDate);
                                DataSet dataSet4 = new DataSet();
                                //  this.GetDatesBetween(this.StartDate, this.EndDate);
                                DataSet dataSet3 = new DataSet();
                                DataSet dtsMast = new DataSet();
                                DataTable dts = new DataTable();
                                DataTable dataTable1 = new DataTable();
                                DataTable dataTable2 = new DataTable();
                                DataSet dtsTrans = new DataSet();
                                DataSet dtsMaster = new DataSet();
                                string dfrom = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd");
                                dfrom = dfrom + "T00:00:01.812Z";
                                string dto = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd");
                                dto = dto + "T11:59:59.812Z";
                                string line = "";
                                string lineBill = "";
                                string transBill = "";
                                string Token = "";
                                try
                                {
                                    //GETTING TOKEN
                                    String s1 = "https://odette.unicommerce.com/oauth/token" + "?grant_type=" + "password" + "&client_id=" + "my-trusted-client" + "&username=" + suser + "&password="+spwd;
                                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(s1);

                                    httpWebRequest.Method = "GET";
                                    httpWebRequest.ContentType = "application/xml";
                                   // httpWebRequest.Headers.Add("x-api-key", AccountName);
                                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                                    {
                                        line = streamReader.ReadToEnd();
                                    }

                                    Token = JObject.Parse(line)["access_token"].ToString();

                                    string status = "COMPLETE";
                                    string dateType = "FULFILLMENT_TAT";   
                                    //GETTING BILLS
                                    String serverBill = server + "/saleOrder/search";
                                        //+ "?grant_type=" + "password" + "&client_id=" + "my-trusted-client" + "&username=" + suser + "&password=" + spwd;
                                    var httpWebBill = (HttpWebRequest)WebRequest.Create(serverBill);
                                                                 
                                    httpWebBill.Method = "POST";
                                    httpWebBill.ContentType = "application/json";
                                    httpWebBill.Headers.Add("Authorization", "Bearer "+ Token);
                                    using (var streamWriter = new StreamWriter(httpWebBill.GetRequestStream()))
                                    {
                                        string str = "{"
                                                         + "\"status\":\"" + status + "\","
                                                         + "\"fromDate\":\"" + dfrom + "\","
                                                         + "\"toDate\":\"" + dto + "\","
                                                         + "\"dateType\":\"" + dateType + "\","
                                                         + "\"facilityCodes\":[\"" + center[0] + "\"]"
                                                         + "}";
                                        streamWriter.Write(str);
                                        streamWriter.Flush();
                                    }
                                    var httpWebBillresponse = (HttpWebResponse)httpWebBill.GetResponse();
                                    using (var streamReader = new StreamReader(httpWebBillresponse.GetResponseStream()))
                                    {
                                        lineBill = streamReader.ReadToEnd();
                                    }

                                    XmlDocument xmlDoc = (XmlDocument)JsonConvert.DeserializeXmlNode(lineBill, "Bills");
                                    string xmlString = xmlDoc.DocumentElement.InnerXml;
                                    
                                    dtsTrans.ReadXml(new XmlNodeReader(xmlDoc));

                                    //GETTING TRANSACTION BILLS TOTAL

                                    foreach (DataRow _row in dtsTrans.Tables["elements"].Rows)
                                    {

                                        String TransBill = server + "/saleorder/get";
                                        //+ "?grant_type=" + "password" + "&client_id=" + "my-trusted-client" + "&username=" + suser + "&password=" + spwd;
                                        var _TransBill = (HttpWebRequest)WebRequest.Create(TransBill);

                                        _TransBill.Method = "POST";
                                        _TransBill.ContentType = "application/json";
                                        _TransBill.Headers.Add("Authorization", "Bearer " + Token);
                                        using (var streamWriter = new StreamWriter(_TransBill.GetRequestStream()))
                                        {
                                            string str = "{"
                                                              + "\"code\":\"" + _row["code"].ToString() + "\","
                                                              + "\"facilityCodes\":[\"" + center[0] + "\"],"
                                                              + "\"paymentDetailRequired\":" + "true"
                                                              + "}";
                                            streamWriter.Write(str);
                                            streamWriter.Flush();
                                        }
                                        var _TransBillResponse = (HttpWebResponse)_TransBill.GetResponse();
                                        using (var streamReader = new StreamReader(_TransBillResponse.GetResponseStream()))
                                        {
                                            transBill = streamReader.ReadToEnd();
                                        }

                                   

                                // Simulating your input JSON line (from file/stream/etc.)
                              //  string line = System.IO.File.ReadAllText("DATA.txt");

                             //   DataSet dtsMaster = new DataSet();

                                // Parse the JSON
                                JObject jObj = JObject.Parse(transBill);

                                // Example: Convert saleOrderItems into a DataTable
                                var saleOrderItems = jObj["saleOrderDTO"]["saleOrderItems"];
                                if (saleOrderItems != null)
                                {
                                    DataTable dtItems = JsonConvert.DeserializeObject<DataTable>(saleOrderItems.ToString());
                                    dtItems.TableName = "SaleOrderItems";
                                    dtsMaster.Tables.Add(dtItems);
                                }

                                // Example: Convert shippingPackages into a DataTable
                                var shippingPackages = jObj["saleOrderDTO"]["shippingPackages"];
                                if (shippingPackages != null)
                                {
                                    DataTable dtPackages = JsonConvert.DeserializeObject<DataTable>(shippingPackages.ToString());
                                    dtPackages.TableName = "ShippingPackages";
                                    dtsMaster.Tables.Add(dtPackages);
                                }

                                
                             






                                    }















                                }
                                catch (Exception https)
                                {
                                    Writelog(https.ToString(), "LoginError");
                                }

                                if (dtsMaster.Tables.Count <= 0 || dtsMaster == null)
                                    return;
                                dtsMaster.Tables[0].TableName = "data";
                                this.Writelog(dtsMaster.Tables["data"].Rows.Count.ToString(), "Data");
                                if (dtsMaster.Tables["data"].Rows.Count > 0)
                                {
                                    dtsMaster.Tables["data"].Columns.Add("Cdate");
                                    dtsMaster.Tables["data"].Columns.Add("Cdate1");
                                    foreach (DataRow row in (InternalDataCollectionBase)dtsMaster.Tables["data"].Rows)
                                    {
                                        if (!string.IsNullOrEmpty(row["DocumentDate"].ToString()))
                                        {
                                            string str22 = row["DocumentDate"].ToString();
                                            row["Cdate"] = DateTime.ParseExact(str22, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MMM/yyyy");
                                            row["Cdate1"] = (object)Convert.ToDateTime(row["Cdate"]).ToString("yyyyMMdd");
                                        }
                                    }
                                    DataRow[] dataRowArray;
                                    //if (this._DataFlowType.ToString() == "API")
                                    //{
                                    //  if (!this.GenerateOldData)
                                    //  {
                                    //    string filterExpression = string.Format("cdate > '{0}'", (object) Convert.ToDateTime(strArray[0]).AddSeconds(1.0));
                                    //    dataRowArray = dtsMaster.Tables["data"].Select(filterExpression);
                                    //  }
                                    //  else
                                    //    dataRowArray = dtsMaster.Tables["data"].Select("Cdate1<>''", "Cdate Asc");
                                    //}
                                    //else
                                    dataRowArray = dtsMaster.Tables["data"].Select("Cdate1<>''", "Cdate Asc");
                                    this.Writelog("Data", "Data");
                                    if (dataRowArray.Length > 0)
                                    {
                                        str3 = string.Format("{0:yyyyMMdd}", (object)DateTime.Today);
                                        DateTime today = DateTime.Today;
                                        DateTime now = DateTime.Now;
                                        TextWriter tw = (TextWriter)null;
                                        if (this._DataFlowType.ToString() == "FILE")
                                        {
                                            str4 = "PPI_" + sten + "_" + string.Format("{0:yyyyMMdd}", (object)today) + "_" + string.Format("{0:HHmmss}", (object)now) + "_" + (object)random.Next(1, 9);
                                            tw = (TextWriter)new StreamWriter(this.AppPath(Application.StartupPath) + "\\DataFiles\\FileTransfer\\SyncFiles\\" + str4 + ".txt");
                                        }

                                        long num4 = 0;
                                        long num5 = 1;
                                        foreach (DataRow dr in dataRowArray)
                                        {
                                            num4 = 0L;
                                            num5 = 1L;
                                            if (dateTime3.ToString("dd/MMM/yyyy") != dLastDt.ToString("dd/MMM/yyyy"))
                                                this.GenerateNoSalesJson(dtsMaster.Tables["data"].Compute("min(Cdate)", "Cdate>'" + dLastDt.ToString("dd/MMM/yyyy") + "'").ToString(), dLastDt, tw);
                                            dateTime3 = Convert.ToDateTime(dLastDt.ToString("dd/MMM/yyyy"));
                                            if (dLastDt < Convert.ToDateTime(dr["Cdate"]))
                                                dLastDt = Convert.ToDateTime(dr["Cdate"]);
                                            this.WriteData(tw, dr, dtsMaster);
                                        }
                                        this.datas = new DataSet();
                                        DataTable dataTable3 = CommonClass.dtTrans.Copy();
                                        this._AgentName = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName;
                                        try
                                        {
                                            if (CommonClass.dtTrans.Rows.Count > 0)
                                            {
                                                if (this.datas.Tables.Count > 0)
                                                {
                                                    this.datas.Tables.Remove("Transaction");
                                                    this.datas.Tables.Remove("Item");
                                                    this.datas.Tables.Remove("Payment");
                                                    this.datas.Clear();
                                                }
                                                this.datas.Tables.Add(CommonClass.dtItem.Copy());
                                                this.datas.Tables.Add(CommonClass.dtPay.Copy());
                                                if (this._DataFlowType.ToString() == "FILE")
                                                {
                                                    this.datas.Tables.Add(CommonClass.dtTrans.Copy());
                                                }
                                                else
                                                {
                                                    this.datas.Tables.Add(CommonClass.dtTrans.Copy());
                                                    //  this.datas.Tables.Add(CommonClass.dtTrans.Clone());
                                                    //  foreach (DataRow row in (InternalDataCollectionBase) CommonClass.dtTrans.Rows)
                                                    //  {
                                                    try
                                                    {
                                                        //      if (this.datas.Tables["Transaction"].Rows.Count > 0)
                                                        //        this.datas.Tables["Transaction"].Rows.Clear();
                                                        //      this.datas.Tables["Transaction"].ImportRow(row);
                                                        //      string str23 = row["RCPT_NUM"].ToString().Trim().Replace("/", "-");
                                                        //      str4 = "PPI_" + sten + "_" + string.Format("{0:yyyyMMdd}", (object) today) + "_" + string.Format("{0:HHmmss}", (object) now) + "_" + Strings.Right(str23.ToString().Replace("-", ""), 5).Trim();
                                                        str4 = "PPI_" + sten + "_" + string.Format("{0:yyyyMMdd}", (object)today) + "_" + string.Format("{0:HHmmss}", (object)now) + "_" + (object)random.Next(1, 9);
                                                        tw = (TextWriter)new StreamWriter(this.AppPath(Application.StartupPath) + "\\DataFiles\\API\\Transit\\" + str4 + ".txt");
                                                        string str24 = this.createjson(sten, str4, this.seqdts, this._portfolioCode, this._SiteID, this._AgentName, this._ReleaseNo, this.frequency(input));
                                                        tw.WriteLine(str24);
                                                        tw.Close();
                                                        MyMethod(str4, "API");

                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        this.Writelog(ex.ToString(), "APIFileErr");
                                                    }

                                                    CommonClass.dtTrans.Rows.Clear();
                                                    CommonClass.dtItem.Rows.Clear();
                                                    CommonClass.dtPay.Rows.Clear();
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            this.Writelog(ex.ToString(), "xmlerr");
                                        }
                                        if (this._DataFlowType.ToString() == "FILE")
                                        {
                                            try
                                            {
                                                string str25 = this.createjson(sten, str4, this.seqdts, this._portfolioCode, this._SiteID, this._AgentName, this._ReleaseNo, this.frequency(input));
                                                if (this.count > 0)
                                                {
                                                    tw.WriteLine(str25);
                                                    tw.Close();
                                                    CommonClass.dtTrans.Rows.Clear();
                                                    CommonClass.dtItem.Rows.Clear();
                                                    CommonClass.dtPay.Rows.Clear();
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                this.Writelog(ex.ToString(), "ExJson");
                                            }
                                        }
                                        this.datas.Clear();
                                        dtsMaster.Tables.Clear();
                                        this.Ls.Clear();

                                        if (this._DataFlowType.ToString() == "FILE")
                                        {
                                            if (dataTable3.Rows.Count > 0 && this.MyMethod(str4, "FILE"))
                                            {
                                                if (Convert.ToBoolean(dataSet1.Tables[1].Rows[index1]["SFTP"]))
                                                {
                                                    try
                                                    {
                                                        if (this.UploadSFTPFile(this.ssftp, this.ssftpuser, this.ssftpwd, this.AppPath(Application.StartupPath) + "\\DataFiles\\FileTransfer\\SyncFiles\\" + str4 + ".zip", this.ssftpfldr, Convert.ToInt32(this.ssftport)))
                                                        {
                                                            System.IO.File.Move(this.AppPath(Application.StartupPath) + "\\DataFiles\\FileTransfer\\SyncFiles\\" + str4 + ".zip", this.AppPath(Application.StartupPath) + "\\DataFiles\\FileTransfer\\BackUp\\" + str4 + ".zip");
                                                            dataSet1.Tables[1].Rows[index1]["LastPooledDate"] = (object)dLastDt.ToString("dd/MMM/yyyy");
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        TextWriter textWriter = (TextWriter)new StreamWriter(this.AppPath(Application.StartupPath) + "\\DataFiles\\Common\\SyncError\\SyncError" + DateTime.Now.ToString("HHmmss") + ".txt");
                                                        textWriter.WriteLine(this.sline);
                                                        textWriter.WriteLine(ex.Message.ToString());
                                                        textWriter.Close();
                                                    }
                                                }
                                                else
                                                {
                                                    this.Ftpconn.Host = str7;
                                                    this.Ftpconn.UserName = str8;
                                                    this.Ftpconn.Password = str9;
                                                    try
                                                    {
                                                        this.Ftpconn.Upload(this.AppPath(Application.StartupPath) + "\\DataFiles\\FileTransfer\\SyncFiles\\" + str4 + ".zip");
                                                        System.IO.File.Move(this.AppPath(Application.StartupPath) + "\\DataFiles\\FileTransfer\\SyncFiles\\" + str4 + ".zip", this.AppPath(Application.StartupPath) + "\\DataFiles\\FileTransfer\\BackUp\\" + str4 + ".zip");
                                                        dataSet1.Tables[1].Rows[index1]["LastPooledDate"] = (object)dLastDt.ToString("dd/MMM/yyyy");
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        TextWriter textWriter = (TextWriter)new StreamWriter(this.AppPath(Application.StartupPath) + "\\DataFiles\\Common\\SyncError\\SyncError" + DateTime.Now.ToString("HHmmss") + ".txt");
                                                        textWriter.WriteLine(this.sline);
                                                        textWriter.WriteLine(ex.Message.ToString());
                                                        textWriter.Close();
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            dataSet1.Tables[1].Rows[index1]["LastPooledDate"] = (object)Convert.ToDateTime(dLastDt).ToString("dd/MMM/yyyy HH:mm:ss");
                                            this.Writelog(dLastDt.ToString("dd/MMM/yyyy HH:mm:ss"), "last");
                                        }
                                    }
                                }
                                int num6 = index2 + 1;
                            } //Registered
                        }
                    }
                    dataSet1.WriteXml(Application.StartupPath + "\\Settings.xml");//"DataFiles\\SettingsBaukup"
                    dataSet1.WriteXml(this.AppPath(Application.StartupPath) + "\\DataFiles\\SettingsBaukup\\Settings.xml");
                }
            }
            catch (Exception ex)
            {
                TextWriter textWriter = (TextWriter)new StreamWriter(this.AppPath(Application.StartupPath) + "\\DataFiles\\Common\\SyncError\\SyncError" + DateTime.Now.ToString("HHmmss") + ".txt");
                textWriter.WriteLine(this.sline);
                textWriter.WriteLine(ex.Message.ToString());
                textWriter.Close();
            }
        }

        private DataSet XmlToDataset(string sxml)
        {
            XmlTextReader reader = (XmlTextReader)null;
            try
            {
                StringReader input = new StringReader(sxml);
                reader = new XmlTextReader((TextReader)input);
                DataSet dataset = new DataSet();
                int num = (int)dataset.ReadXml((XmlReader)reader);
                input.Close();
                return dataset;
            }
            catch (Exception ex)
            {
                this.Writelog(DateTime.Now.ToString() + ":" + ex.Message.ToString(), "XmlError");
                return (DataSet)null;
            }
            finally
            {
                reader.Close();
            }
        }
        private void AppendXmlToDataset(ref DataSet dtsTrans, string sxml)
        {
            DataSet newDataSet = XmlToDataset(sxml);
            if (newDataSet != null && dtsTrans != null)
            {
                // Merge the new data into the existing DataSet
                dtsTrans.Merge(newDataSet, false, MissingSchemaAction.Add);
            }
        }

        public DateTime[] GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates.ToArray();
        }

        public void Generate_EODFile()
        {
        }
        public string pasptroleapi()
        {
            string value = psptrlapi;
            return value;
        }
        private void WriteData(TextWriter tw, DataRow dr, DataSet dtsMaster)
        {
            try
            {
                string custBP = "", dod = "0", paxtype = "";
                string sret = "0.00", snet = "0.00", sinv = "0.00", Transaction = "";
                decimal Netvalue = 0, discountvalue = 0, Retvalue = 0, itemprice = 0, itemtax = 0, itemntamt = 0, itemdis = 0;
                string billtime = "";
                decimal taxvalue = 0;
                string Tenderamt = "0";
                decimal taxvalue1 = 0;
                string ReceiptNo = dr["InvoiceNumber"].ToString();

                DataRow[] drTrans = dtsMaster.Tables[0].Select("InvoiceNumber='" + ReceiptNo.ToString() + "' ", "");
                foreach (DataRow drnet in drTrans)
                {
                    if (drnet["InvoiceAmount"].ToString() != string.Empty || drnet["InvoiceAmount"].ToString() != "")
                    {
                        Netvalue += Math.Abs(Convert.ToDecimal(drnet["InvoiceAmount"]));
                    }
                    if (drnet["TotalTax2"].ToString() != string.Empty || drnet["TotalTax2"].ToString() != "")
                    {
                        taxvalue += Math.Abs(Convert.ToDecimal(drnet["TotalTax2"]));
                    }
                    //if (drnet["TotalTax3"].ToString() != string.Empty || drnet["TotalTax3"].ToString() != "")
                    //{
                    //    taxvalue1 += Math.Abs(Convert.ToDecimal(drnet["TotalTax3"]));
                    //}
                    if (!string.IsNullOrEmpty(drnet["DocumentTime"].ToString()))
                    {
                        billtime = drnet["DocumentTime"].ToString().Replace(":", "");
                    }
                }

                string stax = Convert.ToDecimal(taxvalue + taxvalue1).ToString("0.00");
                string trans = dr["DocumentNature"].ToString();

                if (trans.ToString().Contains("SL"))
                {
                    snet = Convert.ToDecimal(Netvalue).ToString("0.00");
                    Tenderamt = Convert.ToDecimal(Netvalue).ToString("0.00");
                    Transaction = "SALES";
                    sret = "0.00";
                    sinv = Convert.ToDecimal(Netvalue).ToString("0.00");
                    dtot += Convert.ToDecimal(Netvalue);
                }
                else
                {
                    sret = Convert.ToDecimal(Netvalue).ToString("0.00");
                    Tenderamt = Convert.ToDecimal(Netvalue).ToString("0.00");
                    sinv = "0.00";
                    snet = "0.00";
                    Transaction = "RETURN";
                    dtot -= Convert.ToDecimal(Netvalue);
                }

                int len = billtime.Length;
                string billno = dr["InvoiceNumber"].ToString();
                string billdate = Convert.ToDateTime(dr["cdate"]).ToString("yyyyMMdd");

                if (len.ToString() == "5")
                {
                    billtime = billtime + "0";
                }
                else if (len.ToString() == "4")
                {
                    billtime = billtime + "00";
                }
                else if (len.ToString() == "3")
                {
                    billtime = billtime + "000";
                }
                else if (len.ToString() == "2")
                {
                    billtime = billtime + "0000";
                }
                else if (len.ToString() == "1")
                {
                    billtime = billtime + "00000";
                }
                else
                {
                    billtime = "000000";
                }
                string bussdate = Convert.ToDateTime(dr["cdate"]).ToString("yyyyMMdd");

                if (CommonClass.dtTrans.Select("RCPT_NUM='" + billno.ToString() + "'  and RCPT_DT='" + billdate.ToString() + "'  ", "").Length == 0)
                {
                    CommonClass.dtTrans.Rows.Add("G100", " ", "1", "1", billno, billdate, bussdate, billtime, snet.ToString(), stax.ToString(), sret.ToString(), "", "", "", " ", "", paxtype, "", custBP.ToString(), "", "", "", "", "", "", "", "", Transaction.ToString(), "INR", "1.000", discountvalue.ToString(), "", "", " ", dod);
                    CommonClass.dtPay.Rows.Add("G115", billno, billdate, "CASH", "INR", "1.000", Tenderamt.ToString(), "INR", "1.000", Transaction.ToString());
                }
            }
            catch (Exception xe)
            {
                Writelog(xe.StackTrace.ToString(), "WritedataError");
            }
        }
        private bool MyMethod(string TittleHeader, string DataFlowType)
        {
            bool flag = false;
            string[] strArray = new string[2] { "7z32", "7z64" };
            for (int index = 0; index < 2; ++index)
            {
                try
                {
                    SevenZipBase.SetLibraryPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory, strArray[index] + ".dll"));
                    SevenZipCompressor sevenZipCompressor = new SevenZipCompressor();
                    string str1 = "adsr";
                    string path = !(DataFlowType == "FILE") ? this.AppPath(Application.StartupPath) + "\\DataFiles\\API\\Transit\\" : this.AppPath(Application.StartupPath) + "\\DataFiles\\FileTransfer\\SyncFiles\\";
                    foreach (string file1 in Directory.GetFiles(path, TittleHeader + ".txt"))
                    {
                        string str2 = path + TittleHeader + ".zip";
                        sevenZipCompressor.EncryptHeaders = true;
                        sevenZipCompressor.CompressFilesEncrypted(str2, str1, new string[1]
            {
              file1
            });
                        flag = true;
                        if (DataFlowType == "API")
                        {
                            this.CreateDirectoryInAppPath("DataFiles\\API\\Transit\\Header\\" + TittleHeader.Substring(TittleHeader.IndexOf('_') + 1, 8));
                            foreach (string file2 in Directory.GetFiles(this.AppPath(Application.StartupPath) + "\\DataFiles\\API\\Transit\\Header\\" + TittleHeader.Substring(TittleHeader.IndexOf('_') + 1, 8)))
                                System.IO.File.Delete(file2);
                            System.IO.File.Copy(path + TittleHeader + ".zip", this.AppPath(Application.StartupPath) + "\\DataFiles\\API\\Transit\\Header\\" + TittleHeader.Substring(TittleHeader.IndexOf('_') + 1, 8) + "\\" + TittleHeader + ".zip");
                        }
                        System.IO.File.Delete(file1);
                    }
                }
                catch
                {
                }
            }
            return flag;
        }
        private string createjson(string sten, string sTitleheadr, string sSeqDates, string portfolioCode, string StieID, string AgentName, string ReleaseNo, string Frequency)
        {
            string[] strArray = sTitleheadr.Split('_');
            DataTable dataTable = new DataTable();
            this.count = 0;
            this.dtotinv = 0.0;
            return new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            }.Serialize((object)new Root()
            {
                Header = new Header()
                {
                    REC_TYPE = "G010",
                    FILE_TYPE = strArray[0],
                    TENANT_ID = strArray[1],
                    PORTFOLIO_CODE = portfolioCode,
                    SITE_ID = StieID,
                    FREQUENCY = Frequency,
                    FILE_CR_DT = strArray[2],
                    FILE_CR_TM = strArray[3],
                    TRANS_DATE = sSeqDates.Split(','),
                    FILE_SEQ_NUM = Convert.ToInt32(strArray[4]),
                    FILE_NAME = sTitleheadr,
                    AGENT_NAME = AgentName,
                    RELEASE_NO = ReleaseNo,
                    ACTIVE_STATUS = "0"
                },
                Transactions = this.gettran(),
                Footer = new Footer()
                {
                    REC_TYPE = "G020",
                    NO_OF_RECORDS = this.count.ToString(),
                    HASH_TOTAL = this.dtotinv
                }
            });
        }
        private Transactions[] gettran()
        {
            List<Transactions> transactionsList = new List<Transactions>();
            foreach (DataRow row in (InternalDataCollectionBase)this.datas.Tables["Transaction"].Rows)
            {
                string str1 = row["RCPT_NUM"].ToString();
                string str2 = row["RCPT_DT"].ToString();
                string str3 = row["TRAN_STATUS"].ToString();
                string str4 = row["INV_AMT"].ToString();
                string str5 = row["RET_AMT"].ToString();
                this.dtotinv += Convert.ToDouble(str4) - Convert.ToDouble(str5);
                ++this.count;
                Transactions transactions = new Transactions()
                {
                    REC_TYPE = row["REC_TYPE"].ToString(),
                    LOCATION_CODE = row["LOCATION_CODE"].ToString(),
                    TERMINAL_ID = row["TERMINAL_ID"].ToString(),
                    SHIFT_NO = row["SHIFT_NO"].ToString(),
                    RCPT_NUM = row["RCPT_NUM"].ToString(),
                    RCPT_DT = row["RCPT_DT"].ToString(),
                    BUSINESS_DT = row["BUSINESS_DT"].ToString(),
                    RCPT_TM = row["RCPT_TM"].ToString(),
                    INV_AMT = row["INV_AMT"].ToString(),
                    TAX_AMT = row["TAX_AMT"].ToString(),
                    RET_AMT = row["RET_AMT"].ToString(),
                    CUST_NAME = row["CUST_NAME"].ToString(),
                    CUST_GEN = row["CUST_GEN"].ToString(),
                    CUST_NATION = row["CUST_NATION"].ToString(),
                    CUST_EMAIL = row["CUST_EMAIL"].ToString(),
                    CUST_CONT = row["CUST_CONT"].ToString(),
                    CUST_PAX_TYPE = row["CUST_PAX_TYPE"].ToString(),
                    CUST_PP_NO = row["CUST_PP_NO"].ToString(),
                    CUST_BP = row["CUST_BP"].ToString(),
                    CUST_FL_NO = row["CUST_FL_NO"].ToString(),
                    CUST_FL_CLASS = row["CUST_FL_CLASS"].ToString(),
                    CUST_BR_GATE_NO = row["CUST_BR_GATE_NO"].ToString(),
                    CUST_PNR_NO = row["CUST_PNR_NO"].ToString(),
                    TRAN_STATUS = row["TRAN_STATUS"].ToString(),
                    OP_CUR = row["OP_CUR"].ToString(),
                    BC_EXCH = row["BC_EXCH"].ToString(),
                    DISCOUNT = row["DISCOUNT"].ToString(),
                    CUST_ORIGIN = row["CUST_ORIGIN"].ToString(),
                    CUST_DESTINATION = row["CUST_DESTINATION"].ToString(),
                    VOUCH_CODE = row["VOUCH_CODE"].ToString(),
                    DATE_OF_DEPARTURE = Convert.ToInt32(row["DATE_OF_DEPARTURE"].ToString()),
                    ItemDetail = this.getItem(str1, str2, str3),
                    PaymentDetail = this.getPay(str1, str2, str3)
                };
                transactionsList.Add(transactions);
            }
            return transactionsList.ToArray();
        }
        private Itemdetails[] getItem(string irecpt, string ireceptdt, string irecptstatus)
        {
            List<Itemdetails> itemdetailsList = new List<Itemdetails>();
            DataTable table = this.datas.Tables["Item"];
            string filterExpression = "RCPT_NUM='" + irecpt + "' and ITEM_STATUS='" + irecptstatus + "' and RCPT_DT= '" + ireceptdt + "' ";
            foreach (DataRow dataRow in table.Select(filterExpression, ""))
            {
                Itemdetails itemdetails = new Itemdetails()
                {
                    REC_TYPE = dataRow["REC_TYPE"].ToString(),
                    RCPT_NUM = dataRow["RCPT_NUM"].ToString().Trim(),
                    RCPT_DT = dataRow["RCPT_DT"].ToString(),
                    ITEM_CODE = dataRow["ITEM_CODE"].ToString(),
                    ITEM_NAME = dataRow["ITEM_NAME"].ToString(),
                    ITEM_QTY = dataRow["ITEM_QTY"].ToString(),
                    ITEM_PRICE = dataRow["ITEM_PRICE"].ToString(),
                    ITEM_CAT = dataRow["ITEM_CAT"].ToString(),
                    ITEM_TAX = dataRow["ITEM_TAX"].ToString(),
                    ITEM_TAX_TYPE = dataRow["ITEM_TAX_TYPE"].ToString(),
                    ITEM_NET_AMT = dataRow["ITEM_NET_AMT"].ToString(),
                    OP_CUR = dataRow["OP_CUR"].ToString(),
                    BC_EXCH = dataRow["BC_EXCH"].ToString(),
                    ITEM_STATUS = dataRow["ITEM_STATUS"].ToString(),
                    ITEM_DISCOUNT = dataRow["ITEM_DISCOUNT"].ToString()
                };
                itemdetailsList.Add(itemdetails);
            }
            return itemdetailsList.ToArray();
        }
        private PaymentDeatils[] getPay(string precpt, string precptdt, string precptstatus)
        {
            List<PaymentDeatils> paymentDeatilsList = new List<PaymentDeatils>();
            DataTable table = this.datas.Tables["Payment"];
            string filterExpression = "RCPT_NUM='" + precpt + "' and PAYMENT_STATUS='" + precptstatus + "' and RCPT_DT= '" + precptdt + "' ";
            foreach (DataRow dataRow in table.Select(filterExpression, ""))
            {
                PaymentDeatils paymentDeatils = new PaymentDeatils()
                {
                    REC_TYPE = dataRow["REC_TYPE"].ToString(),
                    RCPT_NUM = dataRow["RCPT_NUM"].ToString().Trim(),
                    RCPT_DT = dataRow["RCPT_DT"].ToString(),
                    PAYMENT_NAME = dataRow["PAYMENT_NAME"].ToString(),
                    CURRENCY_CODE = dataRow["CURRENCY_CODE"].ToString(),
                    EXCHANGE_RATE = dataRow["EXCHANGE_RATE"].ToString(),
                    TENDER_AMOUNT = dataRow["TENDER_AMOUNT"].ToString(),
                    OP_CUR = dataRow["OP_CUR"].ToString(),
                    BC_EXCH = dataRow["BC_EXCH"].ToString(),
                    PAYMENT_STATUS = dataRow["PAYMENT_STATUS"].ToString()
                };
                paymentDeatilsList.Add(paymentDeatils);
            }
            return paymentDeatilsList.ToArray();
        }
        //   private string AppPath(string AppPath) => Directory.GetParent(AppPath + "\\").Parent.FullName;
        private string AppPath(string AppPath)
        {
            string path = AppPath + "\\";
            DirectoryInfo parentDir = Directory.GetParent(path);
            var myParentDir = parentDir.Parent.FullName;
            return myParentDir;
        }
        private void CreateDirectoryInAppPath(string sDirecName)
        {
            string fullName = Directory.GetParent(Application.StartupPath + "\\").Parent.FullName;
            if (Directory.Exists(fullName + "\\" + sDirecName))
                return;
            Directory.CreateDirectory(fullName + "\\" + sDirecName);
        }
        //public static long DateTimeToUnixTimestamp(DateTime dateTime) => (long) (int) dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        public bool UploadSFTPFile(string host, string username, string password, string sourcefile, string destinationpath, int port)
        {
            bool flag = false;
            using (SftpClient sftpClient = new SftpClient(host, port, username, password))
            {
                ((BaseClient)sftpClient).Connect();
                try
                {
                    sftpClient.ChangeDirectory(destinationpath);
                }
                catch
                {
                }
                using (FileStream fileStream = new FileStream(sourcefile, FileMode.Open))
                {
                    sftpClient.UploadFile((Stream)fileStream, Path.GetFileName(sourcefile), true, (Action<ulong>)null);
                    flag = true;
                }
            }
            return flag;
        }
        private void GenerateNoSalesJson(string sBusDate, DateTime dLastDt, TextWriter tw)
        {
            int num = 1;
            if (!(sBusDate != ""))
                return;
            for (long index = DateAndTime.DateDiff(DateInterval.Day, Convert.ToDateTime(dLastDt.ToString("dd/MMM/yyyy")), Convert.ToDateTime(sBusDate), FirstDayOfWeek.System, FirstWeekOfYear.System) - 1L; (long)num <= index; ++num)
            {
                CommonClass.dtTrans.Rows.Add((object)"G100", (object)"0", (object)"0", (object)"1", (object)"NOSALES", (object)string.Format("{0:yyyyMMdd}", (object)dLastDt.AddDays((double)num)), (object)string.Format("{0:yyyyMMdd}", (object)dLastDt.AddDays((double)num)), (object)"000000", (object)"0", (object)"0", (object)"0", (object)"", (object)"", (object)"", (object)"", (object)"", (object)"", (object)"", (object)"", (object)"", (object)"", (object)"", (object)"", (object)"", (object)"", (object)"", (object)"", (object)"SALES", (object)"SAR", (object)"1.000", (object)"0", (object)"", (object)"", (object)"", (object)"0");
                CommonClass.dtItem.Rows.Add((object)"G111", (object)"NOSALES", (object)string.Format("{0:yyyyMMdd}", (object)dLastDt.AddDays((double)num)), (object)"", (object)"", (object)"0", (object)"0", (object)"", (object)"0", (object)"I", (object)"0", (object)"SAR", (object)"1.000", (object)"SALES", (object)"0");
                CommonClass.dtPay.Rows.Add((object)"G115", (object)"NOSALES", (object)string.Format("{0:yyyyMMdd}", (object)dLastDt.AddDays((double)num)), (object)"CASH", (object)"SAR", (object)"1.000", (object)"0", (object)"SAR", (object)"1.000", (object)"SALES");
            }
        }
        private string frequency(string input)
        {
            string str = "";
            if (input == "Every 5 Minutes")
                str = "5";
            else if (input == "Every 15 Minutes")
                str = "15";
            else if (input == "Every 30 Minutes")
                str = "30";
            else if (input == "Every 1 Hour")
                str = "60";
            else if (input == "Every 2 Hour")
                str = "120";
            else if (input == "Every 4 Hour")
                str = "240";
            else if (input == "Every 8 Hour")
                str = "480";
            else if (input == "Every 12 Hour")
                str = "720";
            else if (input == "End of Day")
                str = "1440";
            return str;
        }

        private string payment_name(string val)
        {
            string pay = "";
            if (val == "CreditCard" || val == "CARD" || val == "MasterCard" || val == "Visa")
            {
                pay = "CC";
            }
            else if (val == "Cash" || val == "CASH")
            {
                pay = "CASH";
            }
            else
            {
                pay = "OTHERS";
            }
            return pay;
        }

        public DataTable JsonStringToDataTable(string jsonString)
        {
            //  TransactionDetails
            List<string> list = new List<string>();
            DataTable dt = new DataTable();
            string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            if (jsonStringArray.Length <= 1)
            {
                string[] stringSeparators = new string[] { "}," };
                jsonStringArray = jsonStringArray[0].Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            }
            List<string> ColumnsName = new List<string>();

            foreach (string jSA in jsonStringArray)
            {
                string jsonDat = "";
                string[] jsonStringData = Regex.Split(jSA, "}");

                List<string> tempList = new List<string>();
                foreach (string slne in jsonStringData)
                {
                    if (!slne.Contains("GST_Taxes") && !slne.Contains("nonGST_Taxes"))
                    {
                        tempList.Add(slne);
                    }
                }
                jsonStringData = tempList.ToArray();
                foreach (string value in jsonStringData)
                {
                    string[] tokens = value.Split(new[] { "Taxes" }, StringSplitOptions.None);

                    string data = tokens[0];
                    jsonDat += data;
                }
                list.Add(jsonDat);
            }
            foreach (string ColumnsNameData in list)
            {
                string[] ColumnsNames = Regex.Split(ColumnsNameData, ",");
                foreach (string value in ColumnsNames)
                {
                    try
                    {
                        if (value.Contains(":"))
                        {
                            int idx = value.IndexOf(":");
                            string ColumnsNameString = value.Substring(0, idx - 1).Replace("\"", "");
                            if (!ColumnsName.Contains(ColumnsNameString))
                            {
                                ColumnsName.Add(ColumnsNameString);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
            }
            foreach (string AddColumnName in ColumnsName)
            {
                if (!dt.Columns.Contains(AddColumnName))
                {
                    dt.Columns.Add(AddColumnName);
                }
            }
            if (!dt.Columns.Contains("TAX_AMT"))
            {
                dt.Columns.Add("TAX_AMT");
            }
            if (!dt.Columns.Contains("INV_AMT"))
            {
                dt.Columns.Add("INV_AMT");
            }
            if (!dt.Columns.Contains("RET_AMT"))
            {
                dt.Columns.Add("RET_AMT");
            }
            if (!dt.Columns.Contains("DISCOUNT"))
            {
                dt.Columns.Add("DISCOUNT");
            }
            if (!dt.Columns.Contains("RCPT_TM"))
            {
                dt.Columns.Add("RCPT_TM");
            }
            if (!dt.Columns.Contains("TransactionDetails"))
            {
                dt.Columns.Add("TransactionDetails");
            }
            foreach (string jSA in list)
            {
                string JS = jSA.Replace("{", "").Replace("}", "").Replace("\"", "");
                string[] words = Regex.Split(JS, "TransactionDetails:");
                //DataRow nr = dt.NewRow();
                foreach (string rowDataS in words)
                {
                    string[] RowData = Regex.Split(rowDataS, ",");
                    DataRow nr = dt.NewRow();
                    foreach (string rowData in RowData)
                    {
                        try
                        {
                            string[] s = Regex.Split(rowData, ":");
                            //rowData.Split(":")
                            if (s[0].ToString() != "")
                            {
                                string RowColumns = s[0];// rowData.Substring(0, idx - 1).Replace("\"", "");
                                string RowDataString = s[1];// rowData.Substring(idx + 1).Replace("\"", "");
                                nr[RowColumns] = RowDataString;
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                    dt.Rows.Add(nr);
                }
            }
            return dt;
        }
        //  public static DateTime UnixTimeStampToDateTime(long unixTimeStamp) => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double) unixTimeStamp).ToLocalTime();
        private void Writelog(string txt, string fname)
        {
            //this.CreateDirectoryInAppPath("DataFiles\\Common\\Log");
            //StreamWriter streamWriter = new StreamWriter((Stream)new FileStream(this.AppPath(Application.StartupPath) + "\\DataFiles\\Common\\Log\\" + fname + DateTime.Today.ToString("_ddMMMyy") + ".txt", FileMode.OpenOrCreate, FileAccess.Write));
            //streamWriter.BaseStream.Seek(0L, SeekOrigin.End);
            //streamWriter.WriteLine(txt);
            //streamWriter.Flush();
            //streamWriter.Close();
        }
        public void InserDatatoAPI(string Type)
        {
            DataSet dss = new DataSet();
            int num1 = (int)dss.ReadXml(Application.StartupPath + "\\Settings.xml");
            try
            {
                string str1 = this.AppPath(Application.StartupPath) + "\\DataFiles\\API\\Transit\\";
                string[] files = Directory.GetFiles(str1, "*.zip");
                string innerXml;
                foreach (string str2 in files)
                {
                    string fileName = Path.GetFileName(str2);
                    string str3 = this.UnZipData(fileName, str1);
                    DataSet DtMaster = new DataSet();
                    XmlDocument node = JsonConvert.DeserializeXmlNode("{ \"Trans\": " + str3.Trim() + " }", "Trans");
                    innerXml = node.DocumentElement.InnerXml;
                    int num2 = (int)DtMaster.ReadXml((XmlReader)new XmlNodeReader((XmlNode)node));
                    if (DtMaster.Tables["Transactions"].Rows.Count > 0)
                    {
                        APIInsertion.APIHeaderInsertion(DtMaster, dss);
                        APIInsertion.APIWriteData(DtMaster, dss, fileName, str2);
                    }
                }
                if (files.Length != 0 || !(Type == "HEADER"))
                    return;
                foreach (string directory in Directory.GetDirectories(this.AppPath(Application.StartupPath) + "\\DataFiles\\API\\Transit\\Header"))
                {
                    foreach (string file in Directory.GetFiles(directory, "*.zip"))
                    {
                        string str4 = this.UnZipData(Path.GetFileName(file), directory + "\\");
                        DataSet DtMaster = new DataSet();
                        XmlDocument node = JsonConvert.DeserializeXmlNode("{ \"Trans\": " + str4.Trim() + " }", "Trans");
                        innerXml = node.DocumentElement.InnerXml;
                        int num3 = (int)DtMaster.ReadXml((XmlReader)new XmlNodeReader((XmlNode)node));
                        APIInsertion.APIHeaderInsertion(DtMaster, dss);
                        System.IO.File.Delete(file.Replace(".zip", ".txt"));
                    }
                }
            }
            catch (Exception ex)
            {
                this.Writelog(ex.ToString() + ":" + DateTime.Now.ToString(), "InserDatatoAPI_Error");
            }
        }
        private string UnZipData(string FileName, string FilePath)
        {
            string str1 = "";
            string[] strArray = new string[2] { "7z32", "7z64" };
            for (int index = 0; index < 2; ++index)
            {
                try
                {
                    SevenZipBase.SetLibraryPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory, strArray[index] + ".dll"));
                    string str2 = FilePath + FileName;
                    string str3 = "adsr";
                    string path = FilePath + FileName.Replace(".zip", "").Replace(".7z", "").Replace(".rar", "") + ".txt";
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                        new SevenZipExtractor(str2, str3).ExtractArchive(FilePath);
                    }
                    else
                        new SevenZipExtractor(str2, str3).ExtractArchive(FilePath);
                    str1 = System.IO.File.ReadAllText(path);
                }
                catch (Exception ex)
                {
                }
            }
            return str1;
        }
        public void regenerate_Token()
        {
            Encryption encryption = new Encryption();
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            string empty = string.Empty;
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (empty == string.Empty)
                {
                    networkInterface.GetIPProperties();
                    empty = networkInterface.GetPhysicalAddress().ToString();
                }
            }
            DataSet dataSet = new DataSet();
            int num = (int)dataSet.ReadXml(Application.StartupPath + "\\Settings.xml");
            string str1 = encryption.SSH_Locator(dataSet.Tables[0].Rows[0]["AgentSSH"].ToString().ToLower()) + "/api/v4/regenerate_token";
            this.Writelog(str1, "token");
            for (int index = 0; index < dataSet.Tables[1].Rows.Count; ++index)
            {
                WebClient webClient = new WebClient();
                string str2 = dataSet.Tables[1].Rows[index]["AgentUser"].ToString();
                string str3 = dataSet.Tables[1].Rows[index]["Agentpwd"].ToString();
                string s = str2.Trim() + ":" + str3.Trim();
                webClient.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(s));
                System.Collections.Specialized.NameValueCollection formData = new System.Collections.Specialized.NameValueCollection();
                //string txt = Encoding.UTF8.GetString(webClient.UploadValues(str1, "POST", new NameValueCollection()
                //{
                formData["MacId"] = empty;
                formData["TenantId"] = dataSet.Tables[1].Rows[index]["TenantId"].ToString();
                formData["PortfolioCode"] = dataSet.Tables[1].Rows[index]["PortfolioCode"].ToString();
                formData["SiteId"] = dataSet.Tables[1].Rows[index]["SiteID"].ToString();
                byte[] responseBytes = webClient.UploadValues(str1, "POST", formData);
                string result = Encoding.UTF8.GetString(responseBytes);
                int len = result.LastIndexOf(':') + 1;
                dataSet.Tables[1].Rows[index]["AgentToken"] = encryption.Encrytion(result.Substring(len, result.Length - len).Replace("}", "").Replace("\"", "").Replace("Message:", ""));
            }
            dataSet.WriteXml(Application.StartupPath + "\\Settings.xml");
            dataSet.WriteXml(this.AppPath(Application.StartupPath) + "\\DataFiles\\SettingsBaukup\\Settings.xml");
        }
    }
}