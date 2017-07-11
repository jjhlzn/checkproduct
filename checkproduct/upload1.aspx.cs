using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.IO;
using log4net;

namespace checkproduct
{
    public partial class upload1 : System.Web.UI.Page
    {

        private static ILog logger = LogManager.GetLogger(typeof(upload));

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                String[] paths = new String[Request.Files.Count];

                logger.Debug(Request.Files);

                int i = 0;
                foreach(String fileName in Request.Files )
                {
                    paths[i++] = uploadImage(Request.Files[fileName]);
                }

                var resp = new
                {
                    status = 0,
                    errorMessage = "",
                    fileNames = paths
                };
                Response.Write(JsonConvert.SerializeObject(resp));
                Response.End();
            }
        }

        private String uploadImage(HttpPostedFile myFile) 
        {
            if (myFile != null && myFile.ContentLength != 0)
            {
                logger.Debug("myFile.contentLength = " + myFile.ContentLength);

                string pathForSaving = Server.MapPath("~/uploads");
                String newFileName = DateTime.Now.Ticks + myFile.FileName;
                try
                {
                    
                    myFile.SaveAs(Path.Combine(pathForSaving, newFileName));
                    return newFileName;
                }
                catch (Exception ex)
                {
                    logger.Debug(string.Format("File upload failed: {0}", ex.Message));
                }
                return "";

            }
            return "";
        }
    }

    
}