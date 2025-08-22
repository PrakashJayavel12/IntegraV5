using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace DCIA
{
    public partial class Encryption
    {
        
        [DllImport("aesmaster.dll")]
        public static extern void encrypt(string msg, StringBuilder output);
        public string Encrytion(string value)
        {
            string EncryptValue = "";
            StringBuilder SB = new StringBuilder(1024);
            try
            {
                encrypt(value, SB);
                EncryptValue = SB.ToString().Trim();
            }
            catch (System.AccessViolationException ex)
            {

            }
            return EncryptValue;
        }
        [DllImport("aesmaster.dll")]
        public static extern void decrypt(string b, StringBuilder doutput);
        public string decrypt(string val)
        {
            DataSet dataSet1 = new DataSet();
            //dataSet1.ReadXml(Application.StartupPath + "\\Settings.xml");
            //string psptrlapi = dataSet1.Tables[0].Rows[0]["PasptrlAPI"].ToString();
            string decryptvalue = "";
            string output = val.ToString().Trim();
            StringBuilder SB2 = new StringBuilder(8192);
            try
            {
                decrypt(output, SB2);
                decryptvalue = SB2.ToString().Trim();//.Replace("api.pospatrol.com", psptrlapi);
            }
            catch (System.AccessViolationException ex)
            {

            }
            return decryptvalue;
        }
        [DllImport("aesmaster.dll")]
        public static extern void SSH_Locator(string SSH, StringBuilder doutput);
        public string SSH_Locator(string SSH)
        {
            string decryptvalue = "";
            DataSet dataSet1 = new DataSet();
            dataSet1.ReadXml(Application.StartupPath + "\\Settings.xml");
            string psptrlapi = decrypt(dataSet1.Tables[0].Rows[0]["PasptrlAPI"].ToString());
            StringBuilder SB3 = new StringBuilder(2048);
            try
            {
                SSH_Locator(SSH, SB3);
                decryptvalue = SB3.ToString().Replace("api.pospatrol.com", psptrlapi);
            }
            catch (System.AccessViolationException ex)
            {

            }
            return decryptvalue;
        }
    }
}
