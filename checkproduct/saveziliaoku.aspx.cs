using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using checkproduct.Service;
using Newtonsoft.Json;

namespace checkproduct
{
    public partial class saveziliaoku : System.Web.UI.Page
    {
        private ILog logger = LogManager.GetLogger(typeof(saveziliaoku));
        private CheckOrderService service = new CheckOrderService();

        protected void Page_Load(object sender, EventArgs e)
        {
            string username = Request.Params["username"];
            string ticketNo = Request.Params["ticketNo"];
            string spid = Request.Params["spid"];


            logger.Debug("checker username: " + username);
            logger.Debug(string.Format("ticketNo = {0}, spid = {1}", ticketNo, spid));

           
            string addFiles = Request.Params["addImages"];
            logger.Debug("addImages:" + addFiles);
     
            List<string> addImages = new List<string>();
            if (!string.IsNullOrEmpty(addFiles))
            {
                string[] files = addFiles.Split('^');
                foreach (string file in files)
                {
                    logger.Debug(file);
                    addImages.Add(file);

                }
            }

            
            string deleteFiles = Request.Params["deleteImages"];
            logger.Debug("deleteImages:" + deleteFiles);
            List<string> deleteImages = new List<string>();
            if (!string.IsNullOrEmpty(deleteFiles))
            {
                string[] files = deleteFiles.Split('^');
                foreach (string file in files)
                {
                    logger.Debug(file);
                    deleteImages.Add(file);
                }
            }

            bool isSuccess = service.SaveZiliaoKu(ticketNo, spid, username, addImages, deleteImages);

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