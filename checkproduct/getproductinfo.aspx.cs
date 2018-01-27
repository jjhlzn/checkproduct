using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using checkproduct.DomainModel;
using checkproduct.Service;
using Newtonsoft.Json;

namespace checkproduct
{
    public partial class getproductinfo : System.Web.UI.Page
    {
        private CheckOrderService checkOrderService = new CheckOrderService();

        protected void Page_Load(object sender, EventArgs e)
        {
            string contractNo = Request.Params["contractNo"];
            string productNo = Request.Params["productNo"];
            Product product = checkOrderService.GetProductInfo(contractNo, productNo);

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