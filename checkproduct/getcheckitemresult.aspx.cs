using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace checkproduct
{
    public class GetCheckItemResultRequest : BaseRequest
    {
        public string ticetNo
        {
            get;
            set;
        }
    }


    public partial class getcheckitemresult : BasePage<GetCheckItemRequest>
    {
        protected override object handle(GetCheckItemRequest req)
        {
            return new
            {
                status = 0,
                errorMessage = "",
                result = new
                {
                    result = "完成",
                    description = "这是一条验货描述",
                    images = new[]
                                    {
                                        new {
                                            id = "1",
                                            url = "animal.jpg"
                                        },
                                        new
                                        {
                                            id = "2",
                                            url = "test.jpg"
                                        }
                                    }
                }
            };
        }
    }
}