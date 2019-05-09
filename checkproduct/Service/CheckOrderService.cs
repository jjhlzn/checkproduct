using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Dapper;
using checkproduct.DomainModel;
using log4net;

namespace checkproduct.Service
{
    public class GetCheckOrdersResult
    {
        public List<CheckOrder> checkOrders;
        public int totalCount;
    }
    public class CheckOrderService
    {
        private static string IMAGETYPE_ZLK = "ziliaoku";
        private static string IMAGETYPE_CHECK = "check";
        private static ILog logger = LogManager.GetLogger(typeof(CheckOrderService));
        private UserService userService = new UserService();
        private int Special_SPID = 99999999;

        /**
         * 获取验货单的列表
         * */
        public GetCheckOrdersResult GetCheckOrders(DateTime startDate, DateTime endDate, string status, string username, string checker,
            PageInfo pageInfo, string keyword = "")
        {
            GetCheckOrdersResult result = new GetCheckOrdersResult();

            string role = userService.GetRole(username);
            if (string.IsNullOrEmpty(role))
            {
                logger.Warn("找不到username = " + username + "的角色");
                return result;
            }

            int skipCount = pageInfo.pageSize * pageInfo.pageNo;
            string whereClause = @"  ( yw_mxd.mxdbh = yw_mxd_yhsqd.mxdbh ) and  
                                     ( yw_mxd.bbh = yw_mxd_yhsqd.bbh ) and
                                     ( yw_mxd.bb_flag = 'Y' ) and (yw_mxd.zdrq between '" + startDate + "' and '" + endDate + "') ";

            whereClause += " and yw_mxd_yhsqd.yhfs = '我司验货' ";

            if (status == CheckOrder.Status_Not_Assign)
            {
                whereClause += " and (yw_mxd_yhsqd.yhy is null or yw_mxd_yhsqd.yhy = '')";
            }
            else if (status == CheckOrder.Status_Has_Checked)
            {
                whereClause += " and (yw_mxd_yhsqd.yhy is not null and yw_mxd_yhsqd.yhy != '') and yw_mxd.yhjg = '完成'";
            }
            else if (status == CheckOrder.Status_Not_Complete)
            {
                //whereClause += " and (yw_mxd_yhsqd.yhy is not null and yw_mxd_yhsqd.yhy != '') and yw_mxd.yhjg = '未完成'";
            }
            else if (status == CheckOrder.Status_Not_Check)
            {
                whereClause += " and (yw_mxd_yhsqd.yhy is not null and yw_mxd_yhsqd.yhy != '') and (yw_mxd.yhjg is null or yw_mxd.yhjg = '未完成')";
            }
            else if (status == CheckOrder.Status_All)
            {
                whereClause += @" and (yw_mxd_yhsqd.yhy is null or yw_mxd_yhsqd.yhy = '') and (select COUNT(*) from (
                                select distinct sghth, mxd_spid  from yw_mxd_cmd_yh aa where sghth in (
                            select distinct sghth as contractNo 
                              from yw_mxd_cmd_yh where mxdbh = yw_mxd_yhsqd.mxdbh) and (aa.mj_flag != 'Y' or aa.mj_flag is null) and aa.mxdbh = yw_mxd_yhsqd.mxdbh and aa.bbh = yw_mxd.bbh) as a)
                              
                              > (select COUNT(distinct mxd_spid) from yw_mxd_yhmx_picture where  
             yw_mxd_yhmx_picture.mxdbh = yw_mxd_yhsqd.mxdbh and imagetype = '" + IMAGETYPE_ZLK+"') ";

            }
            else
            {
                logger.Fatal("not known status: " + status);
                return null;
            }


            if (username == "9900")
            {
                                                    
            } else if (role == User.Role_Checker_Manager) {

            } else {
                whereClause += " and yhy = '" +username + "' ";
            }

            if (checker != null && !string.IsNullOrEmpty(checker) && !"-1".Equals(checker) )
            {
                whereClause += " and yhy = '" + checker + "' ";
            }

            if ( !string.IsNullOrEmpty(keyword))
            {
                whereClause
                    += " and (  yw_mxd.mxdbh  like '%" + keyword + @"%'  
                            or  yw_mxd_yhsqd.jcbh  like '%" + keyword + @"%'
                         or 
                        (select COUNT(*) from rs_employee where e_no = yw_mxd_yhsqd.yhy and name like '%" + keyword + @"%')  > 0  
                         or 
                        ( (select COUNT(*) from yw_mxd_cmd_yh aa where aa.mxdbh = yw_mxd.mxdbh and aa.sphh_kh like '%" + keyword + @"%') > 0 )
                      )";
            }

            string sql = "";
            
            sql = @"select top "+ pageInfo.pageSize + @" yw_mxd.mxdbh as ticketNo, yw_mxd_yhsqd.jcbh as jinCangNo, 
                            (select name from rs_employee where e_no in (select top 1 yhy from yw_mxd_yhsqd, yw_mxd as aa where yw_mxd_yhsqd.mxdbh = aa.mxdbh and aa.bb_flag = 'Y' and aa.mxdbh = yw_mxd.mxdbh order by yw_mxd_yhsqd.bbh desc)) as checker,
                            (select name from rs_employee where e_no in (select top 1 yw_bcontract.gdy from yw_bcontract where yw_bcontract.bb_flag='Y' 
                                and yw_bcontract.sghth in (select aa.sghth from yw_mxd_cmd_yh aa where aa.mxdbh = yw_mxd.mxdbh and aa.bbh = yw_mxd.bbh  ))) as tracker,
                            
                            (select COUNT(*) from (
                                select distinct sghth, mxd_spid  from yw_mxd_cmd_yh aa where sghth in (
                            select distinct sghth as contractNo 
                              from yw_mxd_cmd_yh where mxdbh = yw_mxd_yhsqd.mxdbh) and (aa.mj_flag != 'Y' or aa.mj_flag is null) and aa.mxdbh = yw_mxd_yhsqd.mxdbh and aa.bbh = yw_mxd.bbh) as a)  as productCount,
                                
                            (select COUNT(*) from (
                                select distinct sghth, mxd_spid  from yw_mxd_cmd_yh aa where sghth in (
                            select distinct sghth as contractNo 
                              from yw_mxd_cmd_yh where mxdbh = yw_mxd_yhsqd.mxdbh ) and (aa.mj_flag != 'Y' or aa.mj_flag is null) and aa.mxdbh = yw_mxd_yhsqd.mxdbh and aa.bbh = yw_mxd.bbh and aa.yhjg in ('合格','不合格', '待定')) as a) as checkedProductCount,  
                            
(select COUNT(distinct mxd_spid) from yw_mxd_yhmx_picture where  
             yw_mxd_yhmx_picture.mxdbh = yw_mxd_yhsqd.mxdbh and imagetype = '" + IMAGETYPE_ZLK + @"') as hasZlkImageProductCount,

                            yw_mxd_yhsqd.zysx as announcements,
                            yw_mxd_yhsqd.yjckrq as outDate, yw_mxd_yhsqd.yhrq as checkDate,
                            yw_mxd.yhjg as checkResult, yw_mxd.yhms as checkMemo
                                FROM yw_mxd with (nolock) ,yw_mxd_yhsqd      
                                WHERE " + whereClause + @" and yw_mxd.mxdbh not in (select top "+ skipCount + @" yw_mxd.mxdbh from yw_mxd with (nolock), yw_mxd_yhsqd

                              where " + whereClause + @" order by yw_mxd.mxdbh ) order by yw_mxd.mxdbh";


            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                var orders = conn.Query<CheckOrder>(sql);
                foreach(CheckOrder order in orders)
                {
                    order.status = status;
                }
               
                result.checkOrders = orders.AsList<CheckOrder>();

                if (status == CheckOrder.Status_Has_Checked)
                {
                    SetCheckResultStatus(result.checkOrders);
                }

                sql = "select count(*) from  yw_mxd with (nolock) ,yw_mxd_yhsqd where " + whereClause;
                result.totalCount = conn.QuerySingleOrDefault<int>(sql);
                return result;
            }
        }



       /**
        * 分配验货员
        */ 
        public bool AssignChecker(string ticketNo, string checker)
        {
            string sql = "update yw_mxd_yhsqd set yhy = '{0}', fprq = '{1}'  where mxdbh = '{2}'";
            sql = string.Format(sql, checker, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ticketNo);
            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                conn.Execute(sql);
                return true;
            }
        }

        /**
        * 获取验货单的合同号列表
        * */
        public List<CheckOrderContract> GetCheckOrderContracts(string ticketNo)
        {
            string sql = @"select distinct sghth as contractNo, 
(select name from rs_employee where e_no in (select top 1 yhy from yw_mxd_yhsqd, 
            yw_mxd as aa where yw_mxd_yhsqd.mxdbh = aa.mxdbh and aa.bb_flag = 'Y' and aa.mxdbh = yw_mxd.mxdbh order by yw_mxd_yhsqd.bbh desc)) as checker,
                            (select name from rs_employee where e_no in (select top 1 yw_bcontract.gdy from yw_bcontract where yw_bcontract.bb_flag='Y' and yw_bcontract.sghth = yw_mxd_cmd_yh.sghth)) as tracker,
   (select COUNT(*) from (select distinct sghth, mxd_spid  from yw_mxd_cmd_yh aa where sghth = yw_mxd_cmd_yh.sghth and (aa.mj_flag != 'Y' or aa.mj_flag is null) and aa.bbh = yw_mxd.bbh and  aa.mxdbh = yw_mxd.mxdbh) as a) as productCount,
   (select COUNT(*) from (select distinct sghth, mxd_spid  from yw_mxd_cmd_yh aa where sghth = yw_mxd_cmd_yh.sghth and (aa.mj_flag != 'Y' or aa.mj_flag is null) and yhjg in ('合格', '不合格', '待定') and aa.bbh = yw_mxd.bbh and aa.mxdbh = yw_mxd.mxdbh ) as a) as checkedProductCount
                          from yw_mxd_cmd_yh, yw_mxd where yw_mxd_cmd_yh.mxdbh = yw_mxd.mxdbh and yw_mxd_cmd_yh.bbh = yw_mxd.bbh and yw_mxd.bb_flag = 'Y' and yw_mxd.mxdbh = '{0}'";
            sql = string.Format(sql, ticketNo);
            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                var contracts = conn.Query<CheckOrderContract>(sql);
                return contracts.AsList<CheckOrderContract>();
            }
        }

        /**
         * 获取合同号的货物列表
         * */
        public List<Product> GetContractProducts(string ticketNo, string contractNo)
        {
            string sql = @"select spid, 
                           (select top 1 spzwmc from yw_mxd_cmd_yh where mxd_spid = spid and sghth = '{0}' and mxdbh = '{1}') as name,
                           (select top 1 sphh_kh from yw_mxd_cmd_yh where mxd_spid = spid and sghth = '{0}' and mxdbh = '{1}') as productNo,
                            (select top 1 yhjg from yw_mxd_cmd_yh where mxd_spid = spid and sghth = '{0}' and mxdbh = '{1}') as checkResult
                           from (                         
                          select distinct mxd_spid as spid 
                             from yw_mxd_cmd_yh, yw_mxd where yw_mxd_cmd_yh.mxdbh = yw_mxd.mxdbh and yw_mxd.bb_flag = 'Y' and 
                              yw_mxd_cmd_yh.bbh = yw_mxd.bbh and  (yw_mxd_cmd_yh.mj_flag != 'Y' or yw_mxd_cmd_yh.mj_flag is null) and 
              yw_mxd_cmd_yh.sghth = '{0}' and yw_mxd.mxdbh = '{1}') as a";
            sql = string.Format(sql, contractNo, ticketNo);
            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                var products = conn.Query<Product>(sql);
                return products.AsList<Product>();
            }
        }


        /**
         * 获取验货单的某种验货结果的所有货物列表（在已验货 和 资料库 中使用）
         * 
         * */
        public List<Product> GetProducts(string ticketNo, string checkResult)
        {
            string sql = @"  select  distinct
            yw_mxd_cmd_yh.mxdbh as ticketNo, 
            (select top 1 a.spzwmc from yw_mxd_cmd_yh as a where a.mxd_spid = yw_mxd_cmd_yh.mxd_spid and a.sghth = yw_mxd_cmd_yh.sghth and a.mxdbh = yw_mxd_cmd_yh.mxdbh order by a.bbh desc) as name,
            (select top 1 a.sphh_kh from yw_mxd_cmd_yh as a where a.mxd_spid = yw_mxd_cmd_yh.mxd_spid and a.sghth = yw_mxd_cmd_yh.sghth and a.mxdbh = yw_mxd_cmd_yh.mxdbh order by a.bbh desc) as productNo,
            (select top 1 a.yhjg from yw_mxd_cmd_yh as a where a.mxd_spid = yw_mxd_cmd_yh.mxd_spid and a.sghth = yw_mxd_cmd_yh.sghth and a.mxdbh = yw_mxd_cmd_yh.mxdbh order by a.bbh desc) as checkResult,
            yw_mxd_cmd_yh.sghth as contractNo,
            (select COUNT(*) from yw_mxd_yhmx_picture where yw_mxd_yhmx_picture.mxd_spid = yw_mxd_cmd_yh.mxd_spid 
            and yw_mxd_yhmx_picture.mxdbh = yw_mxd_cmd_yh.mxdbh and imagetype = '"+ IMAGETYPE_ZLK + @"') as zlkImagesCount,
            mxd_spid as spid,
            (select name from rs_employee where e_no in (select top 1 yhy from yw_mxd_yhsqd, yw_mxd as aa where yw_mxd_yhsqd.mxdbh = aa.mxdbh and aa.bb_flag = 'Y' and aa.mxdbh = yw_mxd.mxdbh order by yw_mxd_yhsqd.bbh desc)) as checker,
            (select top 1 name from rs_employee where e_no = yw_mxd.zdr) as tracker
           from yw_mxd_cmd_yh, yw_mxd
           where yw_mxd_cmd_yh.mxdbh = yw_mxd.mxdbh and yw_mxd.mxdbh = '{0}' and yw_mxd_cmd_yh.bbh = yw_mxd.bbh and  yw_mxd.bb_flag = 'Y' and  (yw_mxd_cmd_yh.mj_flag != 'Y' or yw_mxd_cmd_yh.mj_flag is null) 
            and mxd_spid in (                         
  select distinct mxd_spid as spid 
                            from yw_mxd_cmd_yh where mxdbh = '{0}') ";

            if (checkResult != "全部")
            {
                sql += " and yw_mxd_cmd_yh.yhjg = '" + checkResult + "' ";
            }
            sql = string.Format(sql, ticketNo);
            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                var products = conn.Query<Product>(sql);
                return products.AsList<Product>();
            }

        }

        /**
         * 获取验货单的信息
         * */
        public CheckOrder GetCheckOrderInfo(string ticketNo)
        {
            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                string sql = @"select Top 1 
                                      yw_mxd.mxdbh as ticketNo, 
                                      yw_mxd_yhsqd.jcbh, 
                                      (select name from rs_employee where e_no in (select top 1 yhy from yw_mxd_yhsqd, yw_mxd as aa where yw_mxd_yhsqd.mxdbh = aa.mxdbh and aa.bb_flag = 'Y' and aa.mxdbh = yw_mxd.mxdbh order by yw_mxd_yhsqd.bbh desc)) as checker,
                                      (select top 1 name from rs_employee where e_no = yw_mxd.zdr) as tracker,

                                       (select COUNT(*) from (
                                            select distinct sghth, mxd_spid  from yw_mxd_cmd_yh aa where sghth in (
                                        select distinct sghth as contractNo 
                                          from yw_mxd_cmd_yh where mxdbh = yw_mxd_yhsqd.mxdbh) and  (aa.mj_flag != 'Y' or aa.mj_flag is null)  and aa.mxdbh = yw_mxd_yhsqd.mxdbh and aa.bbh = yw_mxd.bbh) as a) as productCount,
                                
                                        (select COUNT(*) from (
                                            select distinct sghth, mxd_spid  from yw_mxd_cmd_yh aa where sghth in (
                                        select distinct sghth as contractNo 
                                          from yw_mxd_cmd_yh where mxdbh = yw_mxd_yhsqd.mxdbh )  and  (aa.mj_flag != 'Y' or aa.mj_flag is null) and aa.mxdbh = yw_mxd_yhsqd.mxdbh and aa.bbh = yw_mxd.bbh and aa.yhjg in ('合格','不合格', '待定')) as a) as checkedProductCount,  

                                       yw_mxd_yhsqd.yjckrq as outDate, 
                                       yw_mxd_yhsqd.yhrq as checkDate,
                                       yw_mxd.yhjg as checkResult, 
                                       yw_mxd.yhms as checkMemo
                                FROM yw_mxd with (nolock) ,yw_mxd_yhsqd 
                                where yw_mxd.mxdbh = yw_mxd_yhsqd.mxdbh and yw_mxd.mxdbh = '{0}' and yw_mxd.bbh = yw_mxd_yhsqd.bbh  and yw_mxd.bb_flag = 'Y'
                                order by yw_mxd_yhsqd.bbh desc";
                sql = string.Format(sql, ticketNo);
                logger.Debug("sql: " + sql);
         
                CheckOrder checkOrder = conn.QueryFirstOrDefault<CheckOrder>(sql);

                if (checkOrder != null)
                {
                    //获取验货的图片
                    sql = @"select picture_sourcefile from yw_mxd_yhmx_picture where mxdbh = '{0}'  and mxd_spid = '{1}'   order by sqrq,  CAST(picture_xz AS INT)";
                    sql = string.Format(sql,  ticketNo, Special_SPID);
                    logger.Debug("sql: " + sql);
                    var pictureUrls = conn.Query<string>(sql);
                    checkOrder.pictureUrls = pictureUrls.AsList<string>();


                    List<CheckOrder> orders = new List<CheckOrder>();
                    orders.Add(checkOrder);
                    SetCheckResultStatus(orders);
                }

             

                return checkOrder;
            }

           
        }

        /**
         * 获取合同号信息
         * 
         * */
        public CheckOrderContract GetContractInfo(string ticketNo, string contractNo)
        {
            CheckOrderContract contract = new CheckOrderContract();

            string sql = @"select yw_mxd.mxdbh as ticketNo, 
                                  (select name from rs_employee where e_no in (select top 1 yw_bcontract.gdy from yw_bcontract where yw_bcontract.bb_flag='Y' and yw_bcontract.sghth = '"+ contractNo + @"')) as tracker, 
                                 (select name from rs_employee where e_no = yw_mxd_yhsqd.yhy) as checker, 
                                  yw_mxd_yhsqd.jcbh as jinCangNo, 
                                  yjckrq as deadlineDate 
                           from yw_mxd, yw_mxd_yhsqd 
                           where yw_mxd.mxdbh = yw_mxd_yhsqd.mxdbh and  yw_mxd.bb_flag = 'Y'  and yw_mxd.mxdbh = '{0}' 
                         order by yw_mxd_yhsqd.bbh desc";

            sql = string.Format(sql, ticketNo);
            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                contract = conn.QueryFirstOrDefault<CheckOrderContract>(sql);
                if (contract == null)
                {
                    return null;
                }
            }

            List<Product> products = GetContractProducts(ticketNo, contractNo);
            contract.products = products;
            contract.ticketNo = ticketNo;
            contract.contractNo = contractNo;
            
            return contract;
        }

        /**
         * 获取产品信息
         * 
         */
        public Product GetProductInfo(string ticketNo, string contractNo, string spid)
        {
            string sql = @"select yw_mxd_cmd_yh.mxdbh as ticketNo, 
                                  yw_mxd_cmd_yh.bzjs as totalCount, 
                                  yw_mxd_cmd_yh.mxd_spid as spid, 
                                  yw_mxd_cmd_yh.bzjs_cy as pickCount, 
                                  yw_mxd_cmd_yh.yhjg as checkResult, 
                                  yw_mxd_cmd_yh.djtjms_yh as boxSize, 
                                  yw_mxd_cmd_yh.mjmz as grossWeight, 
                                  yw_mxd_cmd_yh.mjjz as netWeight, 
                                  yw_mxd_cmd_yh.yhms as checkMemo,
                                  yw_mxd_cmd_yh.sphh_kh as sphh,
                                    yw_mxd_cmd_yh.spbm as spbm,
                                  CONVERT(nvarchar(100), yw_mxd_cmd_yh.yhrq, 120)  as checkTime,
                                 (select top 1 lable from yw_commodity_kh where sphh_kh = yw_mxd_cmd_yh.sphh_kh) as package,
                                  (select top 1 spggms from yw_commodity_kh where sphh_kh = yw_mxd_cmd_yh.sphh_kh and sphh = yw_mxd_cmd_yh.sphh) as description
                            from yw_mxd_cmd_yh, yw_mxd 
                                  where yw_mxd_cmd_yh.mxdbh = yw_mxd.mxdbh and yw_mxd.bb_flag = 'Y' and yw_mxd.bbh = yw_mxd_cmd_yh.bbh 
                                            and mxd_spid = '{0}' and yw_mxd.mxdbh = '{1}' and sghth = '{2}'";
                                       // and yw_mxd_cmd_yh.wxhth in (select wxhth from yw_mxd_cmd where  mxdbh = '{1}' and mxd_spid = '{0}' and sghth = '{2}')";
            sql = string.Format(sql, spid, ticketNo, contractNo);
            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                Product product = conn.QueryFirstOrDefault<Product>(sql);

                if (product != null) {
                    //获取验货的图片
                    sql = @"select picture_sourcefile from yw_mxd_yhmx_picture where mxdbh = '{0}'  and mxd_spid = '{1}'  and imagetype = '{2}' order by sqrq,  CAST(picture_xz AS INT)";
                    sql = string.Format(sql, ticketNo, spid, IMAGETYPE_CHECK);
                    logger.Debug("sql: " + sql);
                    var pictureUrls = conn.Query<string>(sql);
                    product.pictureUrls = pictureUrls.AsList<string>();

                    //获取资料库的图片
                    sql = @"select picture_sourcefile from yw_mxd_yhmx_picture where mxdbh = '{0}'  and mxd_spid = '{1}'  and imagetype = '{2}' order by sqrq,  CAST(picture_xz AS INT)";
                    sql = string.Format(sql, ticketNo, spid, IMAGETYPE_ZLK);
                    logger.Debug("sql: " + sql);
                    var zlkUrls = conn.Query<string>(sql);
                    product.zlkUrls = zlkUrls.AsList<string>();


                    //sql = @"select count(*) from nbxhw_add.dbo.yw_commodity_kh_picture where sphh_kh = '{0}' ";

                    sql = @" select count(*) from nbxhw_add.dbo.yw_commodity_kh_picture where kh_spbm in (select kh_spbm from yw_commodity_kh where yw_spbm = '{0}' and sphh_kh = '{1}')";
                    sql = string.Format(sql, product.spbm, product.sphh);
                    
                    logger.Debug("sql: " + sql);
                    var fileCount = conn.QuerySingle<int>(sql);
                    if (fileCount > 0)
                    {
                        List<string> productUrls = new List<string>();
                        productUrls.Add("productpicture.aspx?id=" + product.sphh + "&spbm=" + product.spbm + "&ticketNo=" + ticketNo);
                        product.productUrls = productUrls;
                    }
                }

                
                return product;
            }
        }


        /**
         * 验产品 
         */
        public bool CheckProduct(string ticketNo, string contractNo, string productNo, string spid, string username, CheckProductResult checkResult)
        {
            logger.Debug("check product is called");
            if (!userService.CheckIfHasCheckPermission(username, ticketNo))
            {
                return false;
            }

            //设置值
            string sql = @"update yw_mxd_cmd_yh set bzjs_cy = '{0}', yhjg = '{1}', djtjms_yh = '{2}', mjmz = '{3}', mjjz = '{4}', yhms = '{5}',  yhrq = @checkTime
                            where sghth = '{6}' and sphh_kh = '{7}' and mxd_spid = '{8}' ";

            sql = string.Format(sql, checkResult.pickCount, checkResult.checkResult, checkResult.boxSize, checkResult.grossWeight,
                checkResult.netWeight, checkResult.checkMemo, contractNo, productNo, spid);
            logger.Debug("sql: " + sql);

            //string spid = "";
            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                conn.Execute(sql, new { checkTime = DateTime.Now});

                //设置图片
                int seq = 0;
                foreach (string url in checkResult.addImages)
                {
                    sql = @"insert into yw_mxd_yhmx_picture (mxdbh, bbh, mxd_spid, sqrq, picture_filepath, picture_sourcefile, picture_lx, picture_xz, picture_describe, imagetype)
                        values ('{0}', '{1}', '{2}', '{3}', '{4}',  '{5}', '{6}', '{7}', '{8}', '{9}')";
                    sql = string.Format(sql, ticketNo, 1, spid, DateTime.Now, url, url, "辅图", seq++, contractNo, IMAGETYPE_CHECK);
                    logger.Debug("sql: " + sql);

                    conn.Execute(sql);
                }

                foreach (string url in checkResult.deleteImages)
                {
                    sql = @"delete from yw_mxd_yhmx_picture where picture_filepath = '{0}' and mxdbh = '{1}' and mxd_spid = '{2}' and imagetype = '{3}'";
                    int index = url.IndexOf("uploads/");
                    string fileName = url;
                    if (index != -1)
                    {
                        fileName = url.Substring(index + "uploads/".Length);
                    }
                    sql = string.Format(sql, fileName, ticketNo, spid, IMAGETYPE_CHECK);
                    logger.Debug("sql: " + sql);

                    conn.Execute(sql);
                }

                return true;
            }
           
        }

        public bool ClearProductCheckResult(string ticketNo, string contractNo, string productNo, string spid)
        {

            //设置值
            string sql = @"update yw_mxd_cmd_yh set bzjs_cy = '', yhjg = '', djtjms_yh = '', yhms = '',  yhrq = ''
                            where sghth = '{0}' and sphh_kh = '{1}' and mxd_spid = '{2}' ";

            sql = string.Format(sql,contractNo, productNo, spid);
            logger.Debug("sql: " + sql);

            //string spid = "";
            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                conn.Execute(sql, new { checkTime = DateTime.Now });

                sql = @"delete from yw_mxd_yhmx_picture where  mxdbh = '{0}' and mxd_spid = '{1}' and imagetype = '{2}'";
  
                sql = string.Format(sql, ticketNo, spid, IMAGETYPE_CHECK);
                logger.Debug("sql: " + sql);

                conn.Execute(sql);

            }
           return true;
        }


        /**
         * 整体验货 
         */
        public bool Check(string ticketNo, string username, CheckProductResult checkResult)
        {
            if (!userService.CheckIfHasCheckPermission(username, ticketNo))
            {
                return false;
            }
            
            string sql = @"update yw_mxd set yhjg = '{0}', yhms = '{1}' where mxdbh = '{2}'";
            sql = string.Format(sql, checkResult.checkResult, checkResult.checkMemo, ticketNo);

  
            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                conn.Execute(sql);

                //设置图片
                int seq = 0;
                foreach (string url in checkResult.addImages)
                {
                    sql = @"insert into yw_mxd_yhmx_picture (mxdbh, bbh, mxd_spid, sqrq, picture_filepath, picture_sourcefile, picture_lx, picture_xz, imagetype)
                        values ('{0}', '{1}', '{2}', '{3}', '{4}',  '{5}', '{6}', '{7}', '{8}')";
                    sql = string.Format(sql, ticketNo, 1, Special_SPID, DateTime.Now, url, url, "辅图", seq++, IMAGETYPE_CHECK);
                    logger.Debug("sql: " + sql);

                    conn.Execute(sql);
                }

                foreach (string url in checkResult.deleteImages)
                {
                    sql = @"delete from yw_mxd_yhmx_picture where picture_filepath = '{0}' and mxdbh = '{1}' and mxd_spid = '{2}' and imagetype = '{3}' ";
                    int index = url.IndexOf("uploads/");
                    string fileName = url;
                    if (index != -1)
                    {
                        fileName = url.Substring(index + "uploads/".Length);
                    }
                    sql = string.Format(sql, fileName, ticketNo, Special_SPID, IMAGETYPE_CHECK);
                    logger.Debug("sql: " + sql);

                    conn.Execute(sql);
                }


                return true;
            }
        }

        /**
         * 保存资料库的图片
         * */
        public bool SaveZiliaoKu(string ticketNo, string spid, string username,  List<string> addImages, List<string> deleteImages)
        {
            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                string sql = "";
                //设置图片
                int seq = 0;
                foreach (string url in addImages)
                {
                    sql = @"insert into yw_mxd_yhmx_picture (mxdbh, bbh, mxd_spid, sqrq, picture_filepath, picture_sourcefile, picture_lx, picture_xz, imagetype)
                        values ('{0}', '{1}', '{2}', '{3}', '{4}',  '{5}', '{6}', '{7}', '{8}')";
                    sql = string.Format(sql, ticketNo, 1, spid, DateTime.Now, url, url, "辅图", seq++, IMAGETYPE_ZLK);
                    logger.Debug("sql: " + sql);

                    conn.Execute(sql);
                }

                foreach (string url in deleteImages)
                {
                    sql = @"delete from yw_mxd_yhmx_picture where picture_filepath = '{0}' and mxdbh = '{1}' and mxd_spid = '{2}' and  imagetype = '{3}'";
                    int index = url.IndexOf("uploads/");
                    string fileName = url;
                    if (index != -1)
                    {
                        fileName = url.Substring(index + "uploads/".Length);
                    }
                    sql = string.Format(sql, fileName, ticketNo, spid, IMAGETYPE_ZLK);
                    logger.Debug("sql: " + sql);

                    conn.Execute(sql);
                }


                return true;
            }
        }

        public byte[] GetProductImage(string ticketNo, string sphh_kh, string spbm)
        {
            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
               // string sql = @"select top 1 picture_file from nbxhw_add.dbo.yw_commodity_kh_picture where sphh_kh = '{0}'  
                //                and kh_spbm like (select top 1 sphh from yw_mxd_cmd_yh where mxdbh = '{1}' and sphh_kh = '{2}' ) + '%'"
                 //               + @" and yw_spbm in (select top 1 spbm from yw_mxd_cmd_yh where mxdbh = '{1}' and sphh_kh = '{2}' )";

               string sql = @"select top 1 picture_file from nbxhw_add.dbo.yw_commodity_kh_picture 
where kh_spbm in (select kh_spbm from yw_commodity_kh where yw_spbm = '{0}' and sphh_kh = '{1}')";

                sql = string.Format(sql, spbm, sphh_kh);
                logger.Debug(sql);

                byte[] image = conn.Query<byte[]>(sql).FirstOrDefault();
                if (image == null || image.Length == 0)
                {
                    
                     return new byte[0];
                    

                }

                return image;
            }

            /*
            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                string sql = @"select top 1 picture_file from nbxhw_add.dbo.yw_commodity_kh_picture where sphh_kh = '{0}'  
                                and kh_spbm like (select top 1 sphh from yw_mxd_cmd_yh where mxdbh = '{1}' and sphh_kh = '{2}' ) + '%'"
                                + @" and yw_spbm in (select top 1 spbm from yw_mxd_cmd_yh where mxdbh = '{1}' and sphh_kh = '{2}' )";

                sql = string.Format(sql, sphh_kh, ticketNo, sphh_kh);
                logger.Debug(sql);

                byte[] image = conn.Query<byte[]>(sql).FirstOrDefault();
                if (image == null || image.Length == 0)
                {
                    sql = @"select top 1 picture_file from nbxhw_add.dbo.yw_commodity_kh_picture where sphh_kh = '{0}'  
                                and kh_spbm like (select top 1 sphh from yw_mxd_cmd_yh where mxdbh = '{1}' and sphh_kh = '{2}' ) + '%'";

                    sql = string.Format(sql, sphh_kh, ticketNo, sphh_kh);
                    logger.Debug(sql);

                    image = conn.Query<byte[]>(sql).FirstOrDefault();
                    if  (image == null || image.Length == 0)
                    {
                        return new byte[0];
                    }
                           
                }

                return image;
            } */
        }

        /*********************** private method *******************/
        private class C
        {
            public string ticketNo;
            public string checkResult;
            public int checkCount = 0;
        }
        private void SetCheckResultStatus(List<CheckOrder> orders)
        {
            if (orders.Count == 0)
                return;

            string ordersStr = "(";
            foreach (CheckOrder order in orders)
            {
                ordersStr += string.Format("'{0}',", order.ticketNo);
            }
            ordersStr = ordersStr.Substring(0, ordersStr.Length - 1);
            ordersStr += ")";
            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                string sql = @" select ticketNo, checkResult, SUM(checkCount) as checkCount from (                        
  select 
                                    mxdbh as ticketNo, 
                                    sghth,
                                    yhjg as checkResult, 
                                    COUNT(*) as checkCount 
                               from (select distinct yw_mxd_cmd_yh.mxdbh, yw_mxd_cmd_yh.sghth, mxd_spid, yw_mxd_cmd_yh.yhjg from yw_mxd_cmd_yh, yw_mxd
                                      where yw_mxd_cmd_yh.mxdbh = yw_mxd.mxdbh and yw_mxd_cmd_yh.bbh = yw_mxd.bbh
											and yw_mxd.mxdbh in " + ordersStr + @"
											and  bb_flag = 'Y') as a group by yhjg, mxdbh, sghth) as b group by ticketNo, checkResult";

                logger.Debug("sql: " + sql);
                var resultSet = conn.Query<C>(sql);
                foreach (C result in resultSet)
                {
                    foreach (CheckOrder order in orders)
                    {
                        if (order.ticketNo == result.ticketNo)
                        {
                            if (string.IsNullOrEmpty(result.checkResult))
                            {
                                order.notCheckCount += result.checkCount;
                            }
                            else
                            {
                                //未验货 = 未完成 + 未验货
                                switch (result.checkResult)
                                {
                                    case "合格":
                                        order.qualifiedCount = result.checkCount;
                                        break;
                                    case "不合格":
                                        order.notQualifiedCount = result.checkCount;
                                        break;
                                    case "未验":
                                        order.notCheckCount += result.checkCount;
                                        break;
                                    case "待定":
                                        order.tbdCount += result.checkCount;
                                        break;
                                    case "未完成":
                                        order.notCheckCount += result.checkCount;
                                        break;
                                }
                            }
                            break;
                        }
                    }
                }

            }
        }

    }
}