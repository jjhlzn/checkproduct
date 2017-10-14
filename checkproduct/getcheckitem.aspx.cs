using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace checkproduct
{
    public class GetCheckItemRequest : BaseRequest
    {
        public string ticketNo
        {
            get;
            set;
        }
    }

    public partial class getcheckitem : BasePage<GetCheckItemRequest>
    {
        protected override object handle(GetCheckItemRequest req)
        {
            var resp = new
            {
                status = 0,
                errorMessage = "",
                item = new
                {
                    id = "1",
                    content = "这是一段测试文本",
                    files = new String[2] { "file1", "file2" },
                    properties = new[]
                    {
                        new
                        {
                            title = "发票号",
                            value = "123232313131313"
                        },
                        new
                        {
                            title = "进仓编号",
                            value = "abcffsfsffdsf"
                        },
                        new
                        {
                             title = "跟单员",
                             value = "张三"
                        },
                        new
                        {
                             title = "验货员",
                            value = "李四"
                        },
                        new
                        {
                             title = "预计出货日期",
                            value = "2017-11-12"
                        },
                    },
                    products = new[] {
                        new
                        {
                            id = "1",
                            name = "商品A",
                            productNo = "888888888",
                            checkResult = "合格",
                            properties = new []
                            {
                                new
                                {
                                    code = "khhh",
                                    title = "客户货号",
                                    value = "888888888",
                                    canEdit = false
                                },
                                new
                                {
                                    code = "wshh",
                                     title = "我司货号",
                                    value = "999999999",
                                    canEdit = false
                                },
                                new
                                {
                                    code = "cxs",
                                     title = "抽箱数",
                                    value = "10",
                                    canEdit = true
                                },
                                new
                                {
                                    code = "wxcj",
                                     title = "外箱尺寸",
                                    value = "100",
                                    canEdit = true

                                },
                                new
                                {
                                    code = "djmz",
                                     title = "单件毛重",
                                    value = "10kg",
                                    canEdit = true
                                },
                                new
                                {
                                    code = "djjz",
                                     title = "单件净重",
                                    value = "9kg",
                                    canEdit = true
                                }
                              }
                        },
                        new
                        {
                            id = "2",
                            name = "商品B",
                            productNo = "999999999",
                             checkResult = "不合格",
                             properties = new []
                            {
                                new
                                {
                                    code = "khhh",
                                     title = "客户货号",
                                    value = "888888888",
                                    canEdit = false
                                },
                                new
                                {
                                    code = "wshh",
                                     title = "我司货号",
                                    value = "999999999",
                                    canEdit = false
                                },
                                new
                                {
                                     code = "cxs",
                                     title = "抽箱数",
                                    value = "10",
                                    canEdit = true
                                },
                                new
                                {
                                    code = "wxcj",
                                     title = "外箱尺寸",
                                    value = "100",
                                     canEdit = true
                                },
                                new
                                {
                                    code = "djmz",
                                     title = "单件毛重",
                                    value = "10kg",
                                     canEdit = true
                                },
                                new
                                {
                                     code = "djjz",
                                     title = "单件净重",
                                    value = "9kg",
                                     canEdit = true
                                }
                              }
                        },
                        new
                        {
                            id = "3",
                            name = "商品C",
                            productNo = "777777777",
                             checkResult = "待验货",
                             properties = new []
                            {
                                new
                                {
                                     code = "khhh",
                                     title = "客户货号",
                                    value = "888888888",
                                    canEdit = false
                                },
                                new
                                {
                                    code = "wshh",
                                     title = "我司货号",
                                    value = "999999999",
                                    canEdit = false
                                },
                                new
                                {
                                    code = "cxs",
                                     title = "抽箱数",
                                    value = "10",
                                    canEdit = true
                                },
                                new
                                {
                                     code = "wxcj",
                                     title = "外箱尺寸",
                                    value = "100",
                                    canEdit = true
                                },
                                new
                                {
                                     code = "djmz",
                                     title = "单件毛重",
                                    value = "10kg",
                                    canEdit = true
                                },
                                new
                                {
                                     code = "djjz",
                                     title = "单件净重",
                                    value = "9kg",
                                    canEdit = true
                                }
                              }
                        },
                    },

                    checkResult = new
                    {
                        result = "完成",
                        description = "这是验货说明",
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

            return resp;
        }

    }
}