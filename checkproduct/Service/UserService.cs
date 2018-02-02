using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using checkproduct.DomainModel;
using System.Data;
using log4net;

namespace checkproduct.Service
{
    public class UserService
    {
        private static String letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static ILog logger = LogManager.GetLogger(typeof(UserService));

        public string GetRole(string username)
        {
            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                string sql = @"select role_no from t_operator_role_base where o_no = '{0}' and qy_flag = 'Y' 
                        and role_no like 'yh%' order by role_no desc";
                sql = string.Format(sql, username);
                logger.Debug("sql: " + sql);
                var roleNames = conn.Query<string>(sql).AsList<string>();

                if (roleNames.Count() == 0)
                {
                    logger.Debug("userName: " + username + " 找不到任何角色");
                    return "";
                }

                foreach (string roleName in roleNames)
                {
                    if (roleName == "yhzz")
                    {
                        return User.Role_Checker_Manager;
                    }

                    if (roleName == "yhy")
                    {
                        return User.Role_Checker;
                    }
                }
                return "";

            }
        }

        public bool CheckIfHasCheckPermission(string username, string ticketNo)
        {
            string role = GetRole(username);
            if (string.IsNullOrEmpty(username))
            {
                logger.Warn("username = " + username + " 没有验货");
                return false;
            }

            if (role == User.Role_Checker_Manager)
            {
                logger.Debug("验货主管有验货权限");
                return true;
            }

            if (role == User.Role_Checker)
            {
                using (IDbConnection conn = ConnectionFactory.GetInstance())
                {
                    string sql = @"select Top 1 yw_mxd.mxdbh as ticketNo, yw_mxd_yhsqd.jcbh, 
                                        yw_mxd_yhsqd.yhy as checker
                                FROM yw_mxd with (nolock) ,yw_mxd_yhsqd 
                                where yw_mxd.mxdbh = yw_mxd_yhsqd.mxdbh and yw_mxd.mxdbh = '{0}' ";
                    sql = string.Format(sql, ticketNo);
                    logger.Debug("sql: " + sql);
                    var order = conn.QueryFirstOrDefault<CheckOrder>(sql);
                    if (order == null)
                    {
                        logger.Fatal("ticketNo = " + ticketNo + "不存在");
                        return false;
                    }
                    logger.Debug("ticketNo = " + ticketNo + "的验货员id为" + order.checker);
                    if (order.checker == username)
                    {
                        return true;
                    } else
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        public User Login(string userName, string password)
        {
            
            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                string sql = "select e_no as username, name, password from rs_employee where e_no = @userName";
                var user = conn.QueryFirstOrDefault<User>(sql, new { userName });

                if (user == null)
                    return null;

                String decrptPassword = Decrpt(userName, user.password);
                logger.DebugFormat("originPassword = {0}, dbPassword = {1} decrptPassword = {2}", password, user.password, decrptPassword);

                /*
                if (decrptPassword != password.ToUpper())
                {
                    //loginResult.errorMessage = "密码错误";
                    logger.Debug("密码错误");
                    return null;
                }*/

                string roleName = GetRole(userName);
                
                if (string.IsNullOrEmpty(roleName))
                {
                    logger.Debug("userName: " + userName + " 找不到任何角色");
                    return null;
                }
                user.role = roleName;
            
                return user;
            }
        }



        private String Decrpt(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
                return "";

            String result = "";
            userName = userName.ToUpper();
            password = password.ToUpper();
            int j = 9999;
            for (int i = 0; i < password.Length; i++)
            {
                j++;
                if (j > userName.Length - 1)
                {
                    j = 0;
                }
                // Logger.Debug("j = " + j);
                String decodeString = LoadDecode(userName[j]);
                // Logger.DebugFormat("decodeString = {0}", decodeString);
                for (int k = 0; k < letters.Length; k++)
                {
                    if (password[i] == decodeString[k])
                    {
                        // Logger.DebugFormat("i = {0}, {1}", i, letters[k]);
                        result += letters[k];
                        break;
                    }
                }
            }
            //79NC8L9

            return Reverse(result);
        }

        private String Reverse(String str)
        {
            if (string.IsNullOrEmpty(str) || str.Length == 1)
                return str;


            return Reverse(str.Substring(1, str.Length - 1)) + str[0];
        }

        private String LoadDecode(char arg)
        {
            String decodeString = "";
            int start = 0;
            for (int i = 0; i < letters.Length; i++)
            {
                if (letters[i] == arg)
                {
                    start = i;
                    break;
                }
            }
            //  Logger.DebugFormat("start = {0}", start);
            // int nbyte = 0;
            for (int i = start; i < letters.Length; i++)
            {

                decodeString += letters[i];
                //nbyte++;
            }

            for (int i = 0; i < start; i++)
            {

                decodeString += letters[i];
                // nbyte++;
            }

            return decodeString;
        }

        public List<User> GetAllCheckers()
        {
            string sql = @"select e_no as username, name from rs_employee where e_no in (select o_no from t_operator_role_base where role_no = 'yhy')";
            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                var checkers = conn.Query<User>(sql);
                return checkers.AsList<User>();
            }
        }
    }
}