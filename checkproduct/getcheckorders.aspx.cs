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
    public partial class getcheckorders : System.Web.UI.Page
    {
        private CheckOrderService checkOrderService = new CheckOrderService();
        private static ILog logger = LogManager.GetLogger(typeof(getcheckorders));

        protected void Page_Load(object sender, EventArgs e)
        {
            PageInfo pageInfo = new PageInfo();
            string username = "";

            string ticketNo = Request.Params["ticketNo"];
            string startDate = Request.Params["startDate"];
            string endDate = Request.Params["endDate"];
            string status = Request.Params["status"];
            string pageNo = Request.Params["pageNo"];

            string str = string.Format(@"ticketNo = {0}, startDate = {1}, endDate = {2}, status = {3}, pageNo = {4}",
                ticketNo, startDate, endDate, status, pageNo);
            logger.Debug("params: " + str);

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