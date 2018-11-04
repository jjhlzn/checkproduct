using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using checkproduct.DomainModel;
using checkproduct.Service;
using log4net;

namespace checkproduct
{
    public partial class clearproductcheckresult : System.Web.UI.Page
    {
        private static ILog logger = LogManager.GetLogger(typeof(clearproductcheckresult));
        private CheckOrderService service = new CheckOrderService();

        protected void Page_Load(object sender, EventArgs e)
        {
            string username = Request.Params["username"];


            string ticketNo = Request.Params["ticketNo"];
            string contractNo = Request.Params["contractNo"];
            string productNo = Request.Params["productNo"];
            string spid = Request.Params["spid"];

            logger.Debug("ticketNo = " + ticketNo + ", controctNo = " + contractNo + ", productNo = " + productNo + ", spdi = " + spid);

           


            bool isSuccess = service.ClearProductCheckResult(ticketNo, contractNo, productNo, spid);

            var resp = new
            {
                status = isSuccess ? 0 : -1,
                errorMessage = ""
            };
            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
        }
    }
}