using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using checkproduct.DomainModel;
using checkproduct.Service;


namespace checkproduct
{
    public partial class getcheckorders : System.Web.UI.Page
    {
        private CheckOrderService checkOrderService = new CheckOrderService();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageInfo pageInfo = new PageInfo();
            string username = "";

            string status = Request.Params["status"];
            string pageNo = Request.Params["pageNo"];
            pageInfo.pageNo = int.Parse(pageNo);

            GetCheckOrdersResult checkOrdersResult = checkOrderService.GetCheckOrders(DateTime.Now, DateTime.Now, status, username, pageInfo);

            var resp = new
            {
                status = 0,
                errorMessage = "",
                totalCount = checkOrdersResult.totalCount,
                items = checkOrdersResult.checkOrders
            };

            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
        }
    }
}