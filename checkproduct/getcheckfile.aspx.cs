using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;

namespace checkproduct
{
    public partial class getcheckfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var id = Request.Params["id"];

            if (id == "2")
            {
                getPdf();
            } else
            {
                getImage();
            }

            //
            

        }

        private void getImage()
        {
            Response.ContentType = "image/jpg";
            Response.Clear();
            Response.BufferOutput = true;

            string dir = Server.MapPath("~/uploads");
            string path = Path.Combine(dir, "test.jpg");
            MemoryStream m = new MemoryStream();
            Image i = Image.FromFile(path);
            i.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);

            Response.BinaryWrite(m.ToArray());
            Response.End();
        }

        private void getPdf()
        {
            
            Response.Clear();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            //Response.AppendHeader("Content-Disposition", "attachment; filename=my.pdf");

            
            string dir = Server.MapPath("~/uploads");
            string path = Path.Combine(dir, "test.pdf");
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            MemoryStream m = new MemoryStream();

            Response.WriteFile(path);
            Response.End();

        }
    }
}