using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace checkproduct.DomainModel
{
    public class CheckOrder
    {
        public static string Status_Not_Assign = "未分配";
        public static string Status_Not_Check = "未验货";
        public static string Status_Has_Checked = "已验货";

        public string ticketNo;
        public string tracker;
        public string checker;
        public string outDate;
        public string status;
    }

    public class CheckOrderContract
    {
        public string ticketNo;
        public string contractNo;
        public string jinCangNo;
        public string tracker;
        public string checker;
        public string deadlineDate;
        public List<Product> products;
    }

    public class Product
    {
        public string productNo;
        public string name;
        public string pickCount; //抽箱数
        public string boxSize; //外箱尺寸
        public string grossWeight; //单件毛重
        public string netWeight; //单件净重
        public string checkMemo; //验货备注
        public string checkResult;
        public List<string> pictureUrls; //验货图片链接
    }
}