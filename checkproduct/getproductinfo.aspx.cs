using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using checkproduct.DomainModel;
using checkproduct.Service;
using Newtonsoft.Json;
using log4net;

namespace checkproduct
{
    public partial class getproductinfo : System.Web.UI.Page
    {
        private CheckOrderService checkOrderService = new CheckOrderService();

        private ILog logger = LogManager.GetLogger(typeof(getproductinfo));

        protected void Page_Load(object sender, EventArgs e)
        {
            string ticketNo = Request.Params["ticketNo"];
            string contractNo = Request.Params["contractNo"];
            string productNo = Request.Params["productNo"];
            string spid = Request.Params["spid"];

            logger.Debug(string.Format("ticketNo = {0}, contractNo = {1}, productNo = {2}, spid = {3}", ticketNo, contractNo, productNo, spid));

            Product product = checkOrderService.GetProductInfo(ticketNo, contractNo, spid);
              
            /*
            List<string> urls = new List<string>();
            foreach(string url in product.pictureUrls)
            {
                string newUrl = string.Format("http://{0}:{1}/{2}", Request.)
            }*/

            var resp = new
            {
                status = product != null ? 0 : -1,
                errorMessage = "",
                product = product
            };

            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
        }
    }
}