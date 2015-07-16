
namespace SoapTestWebService
{
    using System;
    using System.Web.Services;
    using System.ComponentModel;

    /// <summary>
    /// Summary description for StockQuoteService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class StockQuoteService : WebService
    {

        [WebMethod]
        public void VoidMethod()
        {
            Console.WriteLine("VoidMethod called...");
        }
        
        [WebMethod]
        public string GetQuote(string symbol)
        {
            switch(symbol)
            {
                case "MSFT":
                    return "29.98";
                case "INTC":
                    return "23.21";
                default:
                    return "Unknown";
            }
        }
    }
}
