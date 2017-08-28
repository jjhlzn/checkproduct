using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace checkproduct
{
    public partial class getcheckitem : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var resp = new
            {
                status = 0,
                errorMessage = "",
                item = new
                {
                    id = "1",
                    content = "这是一段测试文本",
                    files = new String[2]{ "file1", "file2"},
                    checkResult = new {
                       images = new String[] {

                           "animal.jpg",
                           "dog.jpg",
                           "test.jpg",
                            "test.jpg",
                           "test.jpg",
                           "test.jpg",
                            "test.jpg",
                           "test.jpg",
                           "test.jpg",
                            "dog.jpg",
                           "test.jpg",
                           "test.jpg",
                            "test.jpg",
                           "test.jpg",
                           "test.jpg",
                            "test.jpg",
                           "test.jpg",
                           "test.jpg",
                            "test.jpg",
                           "test.jpg",
                           "test.jpg",
                        }
                   }
                }
            };
            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
        }
    }
}