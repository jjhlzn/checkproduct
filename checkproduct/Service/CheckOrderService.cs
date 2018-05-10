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
                                     ( yw_mxd.bb_flag = 'Y' ) ";


            if (status == CheckOrder.Status_Not_Assign) {
                whereClause += " and (yw_mxd_yhsqd.yhy is null or yw_mxd_yhsqd.yhy = '')";
            } else if (status == CheckOrder.Status_Has_Checked) {
                whereClause += " and (yw_mxd_yhsqd.yhy is not null and yw_mxd_yhsqd.yhy != '') and yw_mxd.yhjg = '完成'";
            } else if (status == CheckOrder.Status_Not_Complete) {
                whereClause += " and (yw_mxd_yhsqd.yhy is not null and yw_mxd_yhsqd.yhy != '') and yw_mxd.yhjg = '未完成'";
            } else if (status == CheckOrder.Status_Not_Check) {
                whereClause += " and (yw_mxd_yhsqd.yhy is not null and yw_mxd_yhsqd.yhy != '') and yw_mxd.yhjg is null";
            } else {
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
                whereClause += " and (yw_mxd.mxdbh like '%" + keyword + "%' or yw_mxd_yhsqd.jcbh like '%" + keyword + "%') ";
            }

            string sql = "";
            
            sql = @"select top "+ pageInfo.pageSize + @" yw_mxd.mxdbh as ticketNo, yw_mxd_yhsqd.jcbh as jinCangNo, 
                            (select name from rs_employee where e_no in (select top 1 yhy from yw_mxd_yhsqd, yw_mxd as aa where yw_mxd_yhsqd.mxdbh = aa.mxdbh and aa.bb_flag = 'Y' and aa.mxdbh = yw_mxd.mxdbh order by yw_mxd_yhsqd.bbh desc)) as checker,
                            (select top 1 name from rs_employee where e_no = yw_mxd.zdr) as tracker,
                            (   select COUNT(*) from (
                                select distinct sghth, mxd_spid  from yw_mxd_cmd aa where sghth in (
                            select distinct sghth as contractNo 
                              from yw_mxd_cmd where mxdbh = yw_mxd_yhsqd.mxdbh) and aa.mxdbh = yw_mxd_yhsqd.mxdbh) as a) as productCount,
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
            string sql = "update yw_mxd_yhsqd set yhy = '{0}' where mxdbh = '{1}'";
            sql = string.Format(sql, checker, ticketNo);
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
(select name from rs_employee where e_no in (select top 1 yhy from yw_mxd_yhsqd, yw_mxd as aa where yw_mxd_yhsqd.mxdbh = aa.mxdbh and aa.bb_flag = 'Y' and aa.mxdbh = yw_mxd.mxdbh order by yw_mxd_yhsqd.bbh desc)) as checker,
                            (select name from rs_employee where e_no in (select top 1 zdr from yw_mxd where mxdbh = '{0}' order by yw_mxd.bbh desc)) as tracker
                          from yw_mxd_cmd, yw_mxd where yw_mxd_cmd.mxdbh = yw_mxd.mxdbh and yw_mxd_cmd.bbh = yw_mxd.bbh and yw_mxd.bb_flag = 'Y' and yw_mxd.mxdbh = '{0}'";
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
                           (select top 1 spzwmc from yw_mxd_cmd where mxd_spid = spid and sghth = '{0}' and mxdbh = '{1}') as name,
                           (select top 1 spbm from yw_mxd_cmd where mxd_spid = spid and sghth = '{0}' and mxdbh = '{1}') as productNo,
                            (select top 1 yhjg from yw_mxd_cmd where mxd_spid = spid and sghth = '{0}' and mxdbh = '{1}') as checkResult
                           from (                         
                          select distinct mxd_spid as spid 
                                                    from yw_mxd_cmd where sghth = '{0}' and mxdbh = '{1}') as a";
            sql = string.Format(sql, contractNo, ticketNo);
            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                var products = conn.Query<Product>(sql);
                return products.AsList<Product>();
            }
        }


        /**
         * 获取验货单的某种验货结果的所有货物列表（在已验货中使用）
         * 
         * */
        public List<Product> GetProducts(string ticketNo, string checkResult)
        {
            string sql = @"  select  distinct
            yw_mxd_cmd.mxdbh as ticketNo, 
            (select top 1 a.spzwmc from yw_mxd_cmd as a where a.mxd_spid = yw_mxd_cmd.mxd_spid and a.sghth = yw_mxd_cmd.sghth and a.mxdbh = yw_mxd_cmd.mxdbh order by a.bbh desc) as name,
            (select top 1 a.spbm from yw_mxd_cmd as a where a.mxd_spid = yw_mxd_cmd.mxd_spid and a.sghth = yw_mxd_cmd.sghth and a.mxdbh = yw_mxd_cmd.mxdbh order by a.bbh desc) as productNo,
            (select top 1 a.yhjg from yw_mxd_cmd as a where a.mxd_spid = yw_mxd_cmd.mxd_spid and a.sghth = yw_mxd_cmd.sghth and a.mxdbh = yw_mxd_cmd.mxdbh order by a.bbh desc) as checkResult,
            yw_mxd_cmd.sghth as contractNo,
            mxd_spid as spid,
            (select name from rs_employee where e_no in (select top 1 yhy from yw_mxd_yhsqd, yw_mxd as aa where yw_mxd_yhsqd.mxdbh = aa.mxdbh and aa.bb_flag = 'Y' and aa.mxdbh = yw_mxd.mxdbh order by yw_mxd_yhsqd.bbh desc)) as checker,
            (select top 1 name from rs_employee where e_no = yw_mxd.zdr) as tracker
           from yw_mxd_cmd, yw_mxd
           where yw_mxd_cmd.mxdbh = yw_mxd.mxdbh and yw_mxd.mxdbh = '{0}' and yw_mxd_cmd.bbh = yw_mxd.bbh and  yw_mxd.bb_flag = 'Y' 
            and mxd_spid in (                         
  select distinct mxd_spid as spid 
                            from yw_mxd_cmd where yw_mxd.bb_flag = 'Y'  and mxdbh = '{0}') ";

            if (checkResult != "全部")
            {
                sql += " and yw_mxd_cmd.yhjg = '" + checkResult + "' ";
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
                    sql = @"select picture_sourcefile from yw_mxd_yhmx_picture where mxdbh = '{0}'  and mxd_spid = '{1}'   order by sqrq, picture_xz";
                    sql = string.Format(sql,  ticketNo, Special_SPID);
                    logger.Debug("sql: " + sql);
                    var pictureUrls = conn.Query<string>(sql);
                    checkOrder.pictureUrls = pictureUrls.AsList<string>();
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
                                 (select name from rs_employee where e_no = yw_mxd.zdr) as tracker, 
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
        public Product GetProductInfo(string ticketNo, string contractNo, string productNo, string spid)
        {
            string sql = @"select yw_mxd_cmd.mxdbh as ticketNo, 
                                  yw_mxd_cmd.bzjs as totalCount, 
                                  yw_mxd_cmd.mxd_spid as spid, 
                                  yw_mxd_cmd.bzjs_cy as pickCount, 
                                  yw_mxd_cmd.yhjg as checkResult, 
                                  yw_mxd_cmd.djtjms as boxSize, 
                                  yw_mxd_cmd.mjmz as grossWeight, 
                                  yw_mxd_cmd.mjjz as netWeight, 
                                  yw_mxd_cmd.yhms as checkMemo 
                            from yw_mxd_cmd, yw_mxd 
                                  where yw_mxd_cmd.mxdbh = yw_mxd.mxdbh and yw_mxd.bb_flag = 'Y' and yw_mxd.bbh = yw_mxd_cmd.bbh 
                                            and sghth = '{0}' and spbm = '{1}' and mxd_spid = '{2}' and yw_mxd.mxdbh = '{3}' ";
            sql = string.Format(sql, contractNo, productNo, spid, ticketNo);
            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                Product product = conn.QueryFirstOrDefault<Product>(sql);

                if (product != null) {
                    //获取验货的图片
                    sql = @"select picture_sourcefile from yw_mxd_yhmx_picture where mxdbh = '{0}'  and mxd_spid = '{1}' and picture_describe = '{2}'  order by sqrq, picture_xz";
                    sql = string.Format(sql, ticketNo, spid, contractNo);
                    logger.Debug("sql: " + sql);
                    var pictureUrls = conn.Query<string>(sql);
                    product.pictureUrls = pictureUrls.AsList<string>();
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
            string sql = @"update yw_mxd_cmd set bzjs_cy = '{0}', yhjg = '{1}', djtjms = '{2}', mjmz = '{3}', mjjz = '{4}', yhms = '{5}'
                            where sghth = '{6}' and spbm = '{7}'";

            sql = string.Format(sql, checkResult.pickCount, checkResult.checkResult, checkResult.boxSize, checkResult.grossWeight,
                checkResult.netWeight, checkResult.checkMemo, contractNo, productNo);
            logger.Debug("sql: " + sql);

            //string spid = "";
            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                conn.Execute(sql);

                //设置图片
                int seq = 0;
                foreach (string url in checkResult.addImages)
                {
                    sql = @"insert into yw_mxd_yhmx_picture (mxdbh, bbh, mxd_spid, sqrq, picture_filepath, picture_sourcefile, picture_lx, picture_xz, picture_describe)
                        values ('{0}', '{1}', '{2}', '{3}', '{4}',  '{5}', '{6}', '{7}', '{8}')";
                    sql = string.Format(sql, ticketNo, 1, spid, DateTime.Now, url, url, "辅图", seq++, contractNo);
                    logger.Debug("sql: " + sql);

                    conn.Execute(sql);
                }

                foreach (string url in checkResult.deleteImages)
                {
                    sql = @"delete from yw_mxd_yhmx_picture where picture_filepath = '{0}' and mxdbh = '{1}' and mxd_spid = '{2}' ";
                    int index = url.IndexOf("uploads/");
                    string fileName = url;
                    if (index != -1)
                    {
                        fileName = url.Substring(index + "uploads/".Length);
                    }
                    sql = string.Format(sql, fileName, ticketNo, spid);
                    logger.Debug("sql: " + sql);

                    conn.Execute(sql);
                }

                return true;
            }
           
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
                    sql = @"insert into yw_mxd_yhmx_picture (mxdbh, bbh, mxd_spid, sqrq, picture_filepath, picture_sourcefile, picture_lx, picture_xz)
                        values ('{0}', '{1}', '{2}', '{3}', '{4}',  '{5}', '{6}', '{7}')";
                    sql = string.Format(sql, ticketNo, 1, Special_SPID, DateTime.Now, url, url, "辅图", seq++);
                    logger.Debug("sql: " + sql);

                    conn.Execute(sql);
                }

                foreach (string url in checkResult.deleteImages)
                {
                    sql = @"delete from yw_mxd_yhmx_picture where picture_filepath = '{0}' and mxdbh = '{1}' and mxd_spid = '{2}' ";
                    int index = url.IndexOf("uploads/");
                    string fileName = url;
                    if (index != -1)
                    {
                        fileName = url.Substring(index + "uploads/".Length);
                    }
                    sql = string.Format(sql, fileName, ticketNo, Special_SPID);
                    logger.Debug("sql: " + sql);

                    conn.Execute(sql);
                }


                return true;
            }
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
                               from (select distinct yw_mxd_cmd.mxdbh, yw_mxd_cmd.sghth, mxd_spid, yw_mxd_cmd.yhjg from yw_mxd_cmd, yw_mxd
                                      where yw_mxd_cmd.mxdbh = yw_mxd.mxdbh 
											and yw_mxd.mxdbh in " + ordersStr + @"
											and  bb_flag = 'Y') as a group by yhjg, mxdbh, sghth) as b group by ticketNo, checkResult";
                /*
                string sql = @"select 
                                    mxdbh as ticketNo, 
                                    yhjg as checkResult, 
                                    COUNT(*) as checkCount 
                               from (select distinct mxdbh, spbm, yhjg from yw_mxd_cmd 
                                      where mxdbh in " + ordersStr + " ) as a group by yhjg, mxdbh"; */
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