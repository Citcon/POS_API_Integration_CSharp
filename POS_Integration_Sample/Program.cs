using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Net;

namespace POS_Integration_Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            string api_base_url = ConfigurationManager.AppSettings["API_BASE_URL"];

            NameValueCollection nvc = new NameValueCollection();

            // Initialize connection with store code
            nvc.Add("store_code", "TEST");

            string response = PostData(api_base_url + "initialize", nvc);
            response = response.Replace("???", string.Empty);

            JObject o = JObject.Parse(response);

            string token = o["token"].ToString();

            Console.WriteLine("Merchant name: " + o["name_english"]);
            Console.WriteLine("Merchant token: " + token);

            Console.WriteLine("Press any key to continue");
            Console.Read();

            // Make a payment transaction
            Console.WriteLine();
            nvc.Clear();
            nvc.Add("token", token);
            nvc.Add("barcode", "285457939136662142"); // the barcode should come from the input of scanner
            nvc.Add("total", "1");
            nvc.Add("pos_local_time", DateTime.Now.ToString("u"));
            nvc.Add("currency", "USD");
            nvc.Add("tip", "0");
            nvc.Add("reference", "TEST Payment");
            nvc.Add("note", "Test");

            response = PostData(api_base_url + "pay", nvc);
            response = response.Replace("???", string.Empty);

            o = JObject.Parse(response);
            string transaction_id = o["transaction_id"].ToString();

            Console.WriteLine("Transaction ID: " + transaction_id);
            Console.WriteLine("Result: " + o["result"]);
            Console.WriteLine("Error message: " + o["error_message"]);

            Console.WriteLine("Press any key to continue");
            Console.Read();

            // Inquire the transaction status
            Console.WriteLine();
            nvc.Clear();
            nvc.Add("token", token);
            nvc.Add("transaction_id", o["transaction_id"].ToString());
            nvc.Add("pos_local_time", DateTime.Now.ToString("u"));

            response = PostData(api_base_url + "inquire", nvc);
            response = response.Replace("???", string.Empty);

            o = JObject.Parse(response);

            Console.WriteLine("Code: " + o["code"]);
            Console.WriteLine("Result: " + o["result"]);
            Console.WriteLine("Error message: " + o["error_message"]);
            Console.WriteLine("Method: " + o["method"]);

            Console.WriteLine("Press any key to continue");
            Console.Read();

            // Refund the transaction
            Console.WriteLine();
            nvc.Clear();
            nvc.Add("token", token);
            nvc.Add("transaction_id", transaction_id);
            nvc.Add("reference", "Test Reference");
            nvc.Add("refund_amount", "1");
            nvc.Add("pos_local_time", DateTime.Now.ToString("u"));
            nvc.Add("note", "Test refund");

            response = PostData(api_base_url + "refund", nvc);
            response = response.Replace("???", string.Empty);

            o = JObject.Parse(response);

            Console.WriteLine("Refund transaction_id: " + o["transaction_id"]);
            Console.WriteLine("Original transaction_id: " + o["original_transaction_id"]);
            Console.WriteLine("Code: " + o["code"]);
            Console.WriteLine("Result: " + o["result"]);
            Console.WriteLine("Error message: " + o["error_message"]);
            Console.WriteLine("Method: " + o["method"]);
            Console.WriteLine("Reference: " + o["reference"]);
            Console.WriteLine("Original Reference: " + o["original_reference"]);

            Console.WriteLine("Press any key to continue");
            Console.Read();

        }

        static string PostData(string uri, NameValueCollection nvc)
        {
            string result = null;

            using (WebClient wc = new WebClient())
            {
                byte[] responseArray = wc.UploadValues(uri, nvc);
                result = Encoding.ASCII.GetString(responseArray);
            }

            return result;
        }
    }
}
