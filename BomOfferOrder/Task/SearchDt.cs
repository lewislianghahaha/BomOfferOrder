using System;
using System.Data;
using System.Data.SqlClient;
using BomOfferOrder.DB;

namespace BomOfferOrder.Task
{
    public class SearchDt
    {
        SqlList sqlList=new SqlList();

        /// <summary>
        /// 获取K3-Cloud连接
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetCloudConn()
        {
            var conn = new Conn();
            var sqlcon = new SqlConnection(conn.GetConnectionString(0));
            return sqlcon;
        }

        /// <summary>
        /// 获取BomOffer库连接
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetBomOfferConn()
        {
            var conn = new Conn();
            var sqlcon = new SqlConnection(conn.GetConnectionString(1));
            return sqlcon;
        }

        /// <summary>
        /// 查询功能使用
        /// </summary>
        /// <returns></returns>
        public DataTable SearchValue(int searchid, string searchvalue)
        {
            var resultdt = new DataTable();

            try
            {
                var sqlscript = sqlList.Get_Search(searchid, searchvalue);
                var sqlDataAdapter = new SqlDataAdapter(sqlscript, GetCloudConn());
                sqlDataAdapter.Fill(resultdt);
            }
            catch (Exception)
            {
                resultdt.Rows.Clear();
                resultdt.Columns.Clear();
            }
            return resultdt;
        }

        /// <summary>
        /// 查询BOM明细记录信息(生成时使用)
        /// </summary>
        /// <returns></returns>
        public DataTable SearchBomDtl()
        {
            var resultdt = new DataTable();
            try
            {
                var sqlscript = sqlList.Get_Bomdtl();
                var sqlDataAdapter = new SqlDataAdapter(sqlscript, GetCloudConn());
                sqlDataAdapter.Fill(resultdt);
            }
            catch (Exception)
            {
                resultdt.Rows.Clear();
                resultdt.Columns.Clear();
            }
            return resultdt;
        }

        /// <summary>
        /// 查询物料明细(物料明细添加时使用)
        /// </summary>
        /// <param name="searchid"></param>
        /// <param name="searchvalue"></param>
        /// <returns></returns>
        public DataTable SearchMaterialDtl(int searchid, string searchvalue)
        {
            var resultdt=new DataTable();
            try
            {
                var sqlscript = sqlList.Get_MaterialDtl(searchid,searchvalue);
                var sqlDataAdapter = new SqlDataAdapter(sqlscript, GetCloudConn());
                sqlDataAdapter.Fill(resultdt);
            }
            catch (Exception)
            {
                resultdt.Rows.Clear();
                resultdt.Columns.Clear();
            }
            return resultdt;
        }

        /// <summary>
        /// 检测OA流水号是否存在
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public bool SearchOaOrderInclud(string orderno)
        {
            var resultdt=new DataTable();
            var sqlscript = sqlList.SearchOaOrderInclud(orderno);
            var sqlDataAdapter = new SqlDataAdapter(sqlscript, GetBomOfferConn());
            sqlDataAdapter.Fill(resultdt);
            //若不存在为FALSE 存在为TRUE
            var result = Convert.ToInt32(resultdt.Rows[0][0]) != 0;
            return result;
        }

        /// <summary>
        /// 按照指定的SQL语句执行记录并返回执行结果（true 或 false）
        /// </summary>
        public bool Generdt(string sqlscript)
        {
            var result = true;

            try
            {
                using (var sql = GetBomOfferConn())
                {
                    sql.Open();
                    var sqlCommand = new SqlCommand(sqlscript, sql);
                    sqlCommand.ExecuteNonQuery();
                    sql.Close();
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        //////////////////////////////////////////////////主窗体及查询端使用//////////////////////////////////////////////////////////
        


    }
}
