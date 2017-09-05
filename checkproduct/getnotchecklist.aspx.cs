using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;


namespace checkproduct
{
    public partial class getnotchecklist : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var resp = new
            {
                status = 0,
                errorMessage = "",
                totalCount = 6,
                items = new Object[] {

                    new {
                        ticketNo = "123456789a",  
                        inHourseNo = "abcd123",
                        tracker = "张三",
                        checker = "小周",
                        outDate = "2017-10-01",
                        status = "未验货"
                    },
                    new {
                        ticketNo = "123456789b",
                        inHourseNo = "abcd124",
                        tracker = "张三",
                        checker = "未知",
                        outDate = "2017-10-20",
                        status = "未分配"
                    },
                    new {
                        ticketNo = "123456789c",
                        inHourseNo = "abcd125",
                        tracker = "张三",
                        checker = "小周",
                        outDate = "2017-10-01",
                        status = "未验货"
                    },
                    new {
                        ticketNo = "123456789d",
                        inHourseNo = "abcd126",
                        tracker = "张三",
                        checker = "小李飞刀",
                        outDate = "2017-10-11",
                        status = "未验货"
                    },
                    new {
                        ticketNo = "123456789e",
                        inHourseNo = "abcd125",
                        tracker = "乔峰",
                        checker = "李寻欢",
                        outDate = "2017-10-01",
                        status = "未验货"
                    },
                    new {
                        ticketNo = "123456789f",
                        inHourseNo = "abcd126",
                        tracker = "段誉",
                        checker = "张三丰",
                        outDate = "2017-10-11",
                        status = "未验货"
                    },
                }
            };
            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
        }
    }
}