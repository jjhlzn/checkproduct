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
    public partial class checkproduct : System.Web.UI.Page
    {
        private static ILog logger = LogManager.GetLogger(typeof(checkproduct));
        private CheckOrderService service = new CheckOrderService();

        protected void Page_Load(object sender, EventArgs e)
        {
            string username = Request.Params["username"];


            string ticketNo = Request.Params["ticketNo"];
            string contractNo = Request.Params["contractNo"];
            string productNo = Request.Params["productNo"];
            string spid = Request.Params["spid"];

            logger.Debug("ticketNo = " + ticketNo + ", controctNo = " + contractNo + ", productNo = " + productNo + ", spdi = " + spid);

            CheckProductResult checkResult = new CheckProductResult();
            checkResult.checkResult = Request.Params["checkResult"];
            if (string.IsNullOrEmpty(checkResult.checkResult) || checkResult.checkResult == "null")
            {
                checkResult.checkResult = "未完成";
            }
            checkResult.pickCount = Request.Params["pickCount"];
            checkResult.boxSize = Request.Params["boxSize"];
            checkResult.grossWeight = Request.Params["grossWeight"];
            checkResult.netWeight = Request.Params["netWeight"];
            checkResult.checkMemo = Request.Params["checkMemo"];

            logger.Debug("checker username: " + username);
            logger.Debug("checkResult: " + checkResult.ToString());

            logger.Debug("addImages:");
            string addFiles = Request.Params["addImages"];
            if (!string.IsNullOrEmpty(addFiles))
            {
                string[] files = addFiles.Split('^');
                foreach(string file in files)
                {
                    logger.Debug(file);
                    checkResult.addImages.Add(file);

                }
            }

            logger.Debug("deleteImages:");
            string deleteFiles = Request.Params["deleteImages"];
            if (!string.IsNullOrEmpty(deleteFiles))
            {
                string[] files = deleteFiles.Split('^');
                foreach (string file in files)
                {
                    logger.Debug(file);
                    checkResult.deleteImages.Add(file);
                }
            }

            bool isSuccess = service.CheckProduct(ticketNo, contractNo, productNo, spid, username, checkResult);

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