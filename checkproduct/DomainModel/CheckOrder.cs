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

        public int qualifiedCount;
        public int notQualifiedCount;
        public int notCheckCount;
        public int tbdCount;
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
        public string ticketNo;
        public string spid;
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

    public class CheckProductResult
    {
        public string checkResult;

        public string pickCount; //抽箱数
        public string boxSize; //外箱尺寸
        public string grossWeight; //单件毛重
        public string netWeight; //单件净重
        public string checkMemo; //验货备注

        public List<string> addImages = new List<string>(); //验货图片链接
        public List<string> deleteImages = new List<string>();

        public override string ToString()
        {
            string result = "checkResult = {0}, pickCount = {1}, boxSize = {2}, grossWeight = {3}, netWeight = {4}, checkMemo = {5}";

            return string.Format(result, checkResult, pickCount, boxSize, grossWeight, netWeight, checkMemo);
        }
    }

}