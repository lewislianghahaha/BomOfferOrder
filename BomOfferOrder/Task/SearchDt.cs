using System;
using System.Data;
using System.Data.SqlClient;
using BomOfferOrder.DB;

namespace BomOfferOrder.Task
{
    public class SearchDt
    {
        SqlList sqlList=new SqlList();

        private string _sqlscript=string.Empty;

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

        /// <summary>
        /// 根据SQL语句查询得出对应的DT
        /// </summary>
        /// <param name="type">0:获取K3-CLOUD数据库 1:获取BomOffer数据库</param>
        /// <param name="sqlscript"></param>
        /// <returns></returns>
        public DataTable UseSqlSearchIntoDt(int type,string sqlscript)
        {
            var resultdt=new DataTable();

            try
            {
                var sqlcon = type == 0 ? GetCloudConn() : GetBomOfferConn();
                var sqlDataAdapter = new SqlDataAdapter(sqlscript, sqlcon);
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
        /// 查询T_OfferOrderHead表的Headid值(ImportDt.cs判断是否插入时使用)
        /// </summary>
        /// <returns></returns>
        public DataTable SearchOfferHeadDt()
        {
            _sqlscript = sqlList.SearchOfferHeadDt();
            return UseSqlSearchIntoDt(1, _sqlscript);
        }

        /// <summary>
        /// 查询功能使用
        /// </summary>
        /// <returns></returns>
        public DataTable SearchValue(int searchid, string searchvalue)
        {
            _sqlscript = sqlList.Get_Search(searchid, searchvalue);
            return UseSqlSearchIntoDt(0,_sqlscript);
        }

        /// <summary>
        /// 查询BOM明细记录信息(生成时使用)
        /// </summary>
        /// <returns></returns>
        public DataTable SearchBomDtl()
        {
            _sqlscript = sqlList.Get_Bomdtl();
            return UseSqlSearchIntoDt(0,_sqlscript);
        }

        /// <summary>
        /// 查询物料明细(物料明细添加时使用 (ShowMaterialDeatailFrm.cs使用))
        /// </summary>
        /// <param name="searchid"></param>
        /// <param name="searchvalue"></param>
        /// <returns></returns>
        public DataTable SearchMaterialDtl(int searchid, string searchvalue)
        {
            _sqlscript = sqlList.Get_MaterialDtl(searchid,searchvalue);
            return UseSqlSearchIntoDt(0,_sqlscript);
        }

        /// <summary>
        /// 检测OA流水号是否存在
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public bool SearchOaOrderInclud(string orderno)
        {
            _sqlscript = sqlList.SearchOaOrderInclud(orderno);
            //若不存在为FALSE 存在为TRUE
            return Convert.ToInt32(UseSqlSearchIntoDt(1,_sqlscript).Rows[0][0])!=0;
        }

        /// <summary>
        /// 查询‘新产品报价单历史记录’DT(ShowMaterialDeatailFrm.cs使用)
        /// </summary>
        /// <returns></returns>
        public DataTable SearchOfferBomHistoryDtl(int searchid, string searchvalue)
        {
            _sqlscript = sqlList.SearchBomHistory(searchid, searchvalue);
            return UseSqlSearchIntoDt(1,_sqlscript);
        }

        /// <summary>
        /// 查询K3客户信息
        /// </summary>
        /// <param name="searchid"></param>
        /// <param name="searchvalue"></param>
        /// <returns></returns>
        public DataTable SearchK3CustomerList(int searchid, string searchvalue)
        {
            _sqlscript = sqlList.SearchK3CustomerList(searchid, searchvalue);
            return UseSqlSearchIntoDt(0, _sqlscript);
        }

        /// <summary>
        /// 报表-物料明细查询
        /// </summary>
        /// <param name="searchid"></param>
        /// <param name="searchvalue"></param>
        /// <returns></returns>
        public DataTable SearchMaterial(int searchid,string searchvalue)
        {
            _sqlscript = sqlList.SearchMaterial(searchid,searchvalue);
            return UseSqlSearchIntoDt(0,_sqlscript);
        }

        /// <summary>
        /// 查询入库单相关
        /// </summary>
        /// <returns></returns>
        public DataTable SearchInstockDt()
        {
            _sqlscript = sqlList.SearchInstock();
            return UseSqlSearchIntoDt(0,_sqlscript);
        }

        /// <summary>
        /// 查询价目表相关
        /// </summary>
        /// <returns></returns>
        public DataTable SearchPricelistDt()
        {
            _sqlscript = sqlList.SearchPricelist();
            return UseSqlSearchIntoDt(0, _sqlscript);
        }

        /// <summary>
        /// 查询销售价目表相关
        /// </summary>
        /// <returns></returns>
        public DataTable SearchSalesPriceDt()
        {
            _sqlscript = sqlList.SearchSalesInstock();
            return UseSqlSearchIntoDt(0, _sqlscript);
        }

        /// <summary>
        /// 查询采购入库单相关-(毛利润报表生成使用)
        /// </summary>
        /// <returns></returns>
        public DataTable SearchPurchaseInstockDt()
        {
            _sqlscript = sqlList.SearchPurchase();
            return UseSqlSearchIntoDt(0, _sqlscript);
        }

        /// <summary>
        /// 查询人工制造费用相关
        /// </summary>
        /// <returns></returns>
        public DataTable SearchRenCostDt()
        {
            _sqlscript = sqlList.SearchRenCost();
            return UseSqlSearchIntoDt(1,_sqlscript);
        }

        //////////////////////////////////////////////////主窗体及查询端使用//////////////////////////////////////////////////////////

        /// <summary>
        /// 查询端窗体使用
        /// </summary>
        /// <param name="typeid"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DataTable SearchBomOrder(int typeid, string value)
        {
            _sqlscript = sqlList.SearchBomList(typeid, value,"");
            return UseSqlSearchIntoDt(1,_sqlscript);
        }

        /// <summary>
        /// Main查询使用
        /// </summary>
        /// <param name="typeid"></param>
        /// <param name="value"></param>
        /// <param name="createname"></param>
        /// <returns></returns>
        public DataTable SearchMainBomOrder(int typeid, string value, string createname)
        {
            _sqlscript = sqlList.SearchBomList(typeid, value, createname);
            return UseSqlSearchIntoDt(1, _sqlscript);
        }

        /// <summary>
        /// 根据FID查询BOM报价单明细
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public DataTable SearchBomOrderDetail(int fid)
        {
            _sqlscript = sqlList.SearchBomDtl(fid);
            return UseSqlSearchIntoDt(1,_sqlscript);
        }

        /// <summary>
        /// 根据FID查询此单据占用情况
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public DataTable SearchUseInfo(int fid)
        {
            _sqlscript = sqlList.SearchUseInfo(fid);
            return UseSqlSearchIntoDt(1,_sqlscript);
        }

        /// <summary>
        /// 权限查询-用户权限主窗体使用
        /// </summary>
        /// <param name="typeid"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DataTable SearchAdminDetail(int typeid, string value)
        {
            _sqlscript = sqlList.SearchAdminDetail(typeid, value);
            return UseSqlSearchIntoDt(1, _sqlscript);
        }

        /// <summary>
        /// 查询K3用户信息(初始化及查询页面使用)
        /// </summary>
        /// <returns></returns>
        public DataTable SearchK3User(string value)
        {
            _sqlscript = sqlList.SearchK3User(value);
            return UseSqlSearchIntoDt(0,_sqlscript);
        }

        /// <summary>
        /// 获取用户权限表明细
        /// </summary>
        /// <returns></returns>
        public DataTable SearchUseDetail()
        {
            _sqlscript = sqlList.SearchUseDetail();
            return UseSqlSearchIntoDt(1,_sqlscript);
        }

        /// <summary>
        /// 根据USERID查询该用户是否占用
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public DataTable SearchAdminUserInfo(int userid)
        {
            _sqlscript = sqlList.SearchAdminUserInfo(userid);
            return UseSqlSearchIntoDt(1, _sqlscript);
        }

    }
}
