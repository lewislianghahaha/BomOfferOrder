using System;
using System.Data;

namespace BomOfferOrder.DB
{
    public class DbList
    {
        /// <summary>
        /// 查询页(注:当使用‘添加’时使用)
        /// </summary>
        /// <returns></returns>
        public DataTable Get_Searchdt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 5; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    case 0:
                        dc.ColumnName = "FMATERIALID";
                        dc.DataType = Type.GetType("System.Int32"); 
                        break;
                    case 1:
                        dc.ColumnName = "物料编码";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 2:
                        dc.ColumnName = "物料名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 3:
                        dc.ColumnName = "规格型号";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 4:
                        dc.ColumnName = "密度(KG/L)";
                        dc.DataType = Type.GetType("System.Decimal"); 
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 用于明细窗体生成时使用
        /// </summary>
        /// <returns></returns>
        public DataTable MakeTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 11; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //产品ID
                    case 0:
                        dc.ColumnName = "Id";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 1:
                        dc.ColumnName = "产品名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 2:
                        dc.ColumnName = "BOM编号";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 3:
                        dc.ColumnName = "包装规格";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 4:
                        dc.ColumnName = "产品密度";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    case 5:
                        dc.ColumnName = "物料编码ID";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    case 6:
                        dc.ColumnName = "物料编码";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 7:
                        dc.ColumnName = "物料名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 8:
                        dc.ColumnName = "配方用量";
                        dc.DataType = Type.GetType("System.Decimal"); 
                        break;
                    case 9:
                        dc.ColumnName = "明细行BOM编号";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 10:
                        dc.ColumnName = "物料单价";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// ShowDetailFrm中的Gridview窗体使用
        /// </summary>
        /// <returns></returns>
        public DataTable MakeGridViewTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 12; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //记录每行的行ID
                    case 0:
                        dc.ColumnName = "Entryid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    case 1:
                        dc.ColumnName = "物料编码ID";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    case 2:
                        dc.ColumnName = "物料编码";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 3:
                        dc.ColumnName = "物料名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 4:
                        dc.ColumnName = "配方用量";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 5:
                        dc.ColumnName = "占比";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 6:
                        dc.ColumnName = "物料单价(含税)";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 7:
                        dc.ColumnName = "物料成本(含税)";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 8:
                        dc.ColumnName = "备注";
                        dc.DataType = Type.GetType("System.String");
                        break;
                     //用于记录新增行时的行ID,更新明细行信息时使用(注:临时值,不与数据库关联)
                    case 9:
                        dc.ColumnName = "TempRowid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //最新修改人
                    case 10:
                        dc.ColumnName = "LastChangeUser";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //最新修改日期
                    case 11:
                        dc.ColumnName = "LastChanageDt";
                        dc.DataType = Type.GetType("System.DateTime");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        ///用于明细记录保存-提交时整合使用
        /// </summary>
        /// <returns></returns>
        public DataTable GetBomDtlTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 37; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //FID
                    case 0:
                        dc.ColumnName = "FID";
                        dc.DataType = Type.GetType("System.Int32"); 
                        break;
                    //流水号
                    case 1:
                        dc.ColumnName = "OAorderno";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    //单据状态
                    case 2:
                        dc.ColumnName = "Fstatus";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //创建日期
                    case 3:
                        dc.ColumnName = "CreateDt";
                        dc.DataType = Type.GetType("System.DateTime");                
                        break;
                    //审核日期
                    case 4:
                        dc.ColumnName = "ConfirmDt";
                        dc.DataType = Type.GetType("System.DateTime");
                        break;
                    //创建人
                    case 5:
                        dc.ColumnName = "CreateName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //单据类型ID(0:BOM成本报价单 1:新产品成本报价单及其它单据)
                    case 6:
                        dc.ColumnName = "Typeid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //'研发类别'ID
                    case 7:
                        dc.ColumnName = "DevGroupid";
                        dc.DataType=Type.GetType("System.Int32");
                        break;
                    //记录当前单据使用标记(0:正在使用 1:没有使用)
                    case 8:
                        dc.ColumnName = "Useid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //记录当前单据使用者信息
                    case 9:
                        dc.ColumnName = "UserName";
                        dc.DataType = Type.GetType("System.String");
                        break;

                    //Headid
                    case 10:
                        dc.ColumnName = "Headid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //产品名称(物料名称)
                    case 11:
                        dc.ColumnName = "ProductName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //包装规格
                    case 12:
                        dc.ColumnName = "Bao";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //产品密度(KG/L)
                    case 13:
                        dc.ColumnName = "ProductMi";
                        dc.DataType = Type.GetType("System.Decimal"); 
                        break;
                    //材料成本(不含税)
                    case 14:
                        dc.ColumnName = "MaterialQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //包装成本
                    case 15:
                        dc.ColumnName = "BaoQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //人工及制造费用
                    case 16:
                        dc.ColumnName = "RenQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //成本(元/KG)
                    case 17:
                        dc.ColumnName = "KGQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //成本(元/L)
                    case 18:
                        dc.ColumnName = "LQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //50%报价
                    case 19:
                        dc.ColumnName = "FiveQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //45%报价
                    case 20:
                        dc.ColumnName = "FourFiveQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //40%报价
                    case 21:
                        dc.ColumnName = "FourQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //备注
                    case 22:
                        dc.ColumnName = "Fremark";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    //对应BOM版本编号
                    case 23:
                        dc.ColumnName = "FBomOrder";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料单价
                    case 24:
                        dc.ColumnName = "FPrice";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //客户名称
                    case 25:
                        dc.ColumnName = "CustName";
                        dc.DataType = Type.GetType("System.String");
                        break;

                    //Entryid
                    case 26:
                        dc.ColumnName = "Entryid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //物料编码ID
                    case 27:
                        dc.ColumnName = "MaterialID";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //物料编码
                    case 28:
                        dc.ColumnName = "MaterialCode";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料名称
                    case 29:
                        dc.ColumnName = "MaterialName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //配方用量
                    case 30:
                        dc.ColumnName = "PeiQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //占比
                    case 31:
                        dc.ColumnName = "ratioQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //物料单价(含税)
                    case 32:
                        dc.ColumnName = "MaterialPrice";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //物料成本(含税)
                    case 33:
                        dc.ColumnName = "MaterialAmount";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //备注
                    case 34:
                        dc.ColumnName = "Remark";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //最新修改人
                    case 35:
                        dc.ColumnName = "LastChangeUser";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //最新修改日期
                    case 36:
                        dc.ColumnName = "LastChanageDt";
                        dc.DataType = Type.GetType("System.DateTime");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 用于记录-BOM报价单返回的信息(提交时使用)
        /// </summary>
        /// <returns></returns>
        public DataTable GetOfferOrderTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 10; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //FID
                    case 0:
                        dc.ColumnName = "FID";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //流水号
                    case 1:
                        dc.ColumnName = "OAorderno";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //单据状态
                    case 2:
                        dc.ColumnName = "Fstatus";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //创建日期
                    case 3:
                        dc.ColumnName = "CreateDt";
                        dc.DataType = Type.GetType("System.DateTime");
                        break;
                    //审核日期
                    case 4:
                        dc.ColumnName = "ConfirmDt";
                        dc.DataType = Type.GetType("System.DateTime");
                        break;
                    //创建人
                    case 5:
                        dc.ColumnName = "CreateName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //记录当前单据使用标记
                    case 6:
                        dc.ColumnName = "Useid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //记录当前单据使用者信息
                    case 7:
                        dc.ColumnName = "UserName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //单据类型ID(0:BOM成本报价单 1:新产品成本报价单及其它单据)
                    case 8:
                        dc.ColumnName = "Typeid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //研发类别ID
                    case 9:
                        dc.ColumnName = "DevGroupid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 用于记录-BOM报价单返回的表头信息(提交时使用)
        /// </summary>
        /// <returns></returns>
        public DataTable GetOfferOrderHeadTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 17; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //fid
                    case 0:
                        dc.ColumnName = "FId";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //Headid
                    case 1:
                        dc.ColumnName = "Headid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //产品名称(物料名称)
                    case 2:
                        dc.ColumnName = "ProductName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //包装规格
                    case 3:
                        dc.ColumnName = "Bao";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //产品密度(KG/L)
                    case 4:
                        dc.ColumnName = "ProductMi";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //材料成本(不含税)
                    case 5:
                        dc.ColumnName = "MaterialQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //包装成本
                    case 6:
                        dc.ColumnName = "BaoQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //人工及制造费用
                    case 7:
                        dc.ColumnName = "RenQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //成本(元/KG)
                    case 8:
                        dc.ColumnName = "KGQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //成本(元/L)
                    case 9:
                        dc.ColumnName = "LQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //50%报价
                    case 10:
                        dc.ColumnName = "FiveQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //45%报价
                    case 11:
                        dc.ColumnName = "FourFiveQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //40%报价
                    case 12:
                        dc.ColumnName = "FourQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //备注
                    case 13:
                        dc.ColumnName = "Fremark";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //对应BOM版本编号
                    case 14:
                        dc.ColumnName = "FBomOrder";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料单价
                    case 15:
                        dc.ColumnName = "FPrice";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //客户名称
                    case 16:
                        dc.ColumnName = "CustName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 用于记录-BOM报价单返回的表体信息(提交时使用)
        /// </summary>
        /// <returns></returns>
        public DataTable GetOfferOrderEntryTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 12; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //Headid
                    case 0:
                        dc.ColumnName = "Headid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //Entryid
                    case 1:
                        dc.ColumnName = "Entryid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //物料编码ID
                    case 2:
                        dc.ColumnName = "MaterialID";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //物料编码
                    case 3:
                        dc.ColumnName = "MaterialCode";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料名称
                    case 4:
                        dc.ColumnName = "MaterialName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //配方用量
                    case 5:
                        dc.ColumnName = "PeiQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //占比
                    case 6:
                        dc.ColumnName = "ratioQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //物料单价(含税)
                    case 7:
                        dc.ColumnName = "MaterialPrice";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //物料成本(含税)
                    case 8:
                        dc.ColumnName = "MaterialAmount";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //备注
                    case 9:
                        dc.ColumnName = "Remark";
                        dc.DataType=Type.GetType("System.String");
                        break;
                    //最新修改人
                    case 10:
                        dc.ColumnName = "LastChangeUser";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //最新修改日期
                    case 11:
                        dc.ColumnName = "LastChanageDt";
                        dc.DataType = Type.GetType("System.DateTime");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        #region 暂存功能使用

        public DataTable GetTempOrderTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 6; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //FID
                    case 0:
                        dc.ColumnName = "FID";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //流水号
                    case 1:
                        dc.ColumnName = "OAorderno";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //创建日期
                    case 2:
                        dc.ColumnName = "CreateDt";
                        dc.DataType = Type.GetType("System.DateTime");
                        break;
                    //创建人
                    case 3:
                        dc.ColumnName = "CreateName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //单据类型ID(0:BOM成本报价单 1:新产品成本报价单及其它单据 2:空白报价单)
                    case 4:
                        dc.ColumnName = "Typeid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //研发类别ID
                    case 5:
                        dc.ColumnName = "DevGroupid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 用于记录-BOM报价单返回的表头信息(提交时使用)
        /// </summary>
        /// <returns></returns>
        public DataTable GetTempOrderHeadTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 17; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //fid
                    case 0:
                        dc.ColumnName = "FId";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //Headid
                    case 1:
                        dc.ColumnName = "Headid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //产品名称(物料名称)
                    case 2:
                        dc.ColumnName = "ProductName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //包装规格
                    case 3:
                        dc.ColumnName = "Bao";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //产品密度(KG/L)
                    case 4:
                        dc.ColumnName = "ProductMi";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //材料成本(不含税)
                    case 5:
                        dc.ColumnName = "MaterialQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //包装成本
                    case 6:
                        dc.ColumnName = "BaoQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //人工及制造费用
                    case 7:
                        dc.ColumnName = "RenQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //成本(元/KG)
                    case 8:
                        dc.ColumnName = "KGQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //成本(元/L)
                    case 9:
                        dc.ColumnName = "LQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //50%报价
                    case 10:
                        dc.ColumnName = "FiveQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //45%报价
                    case 11:
                        dc.ColumnName = "FourFiveQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //40%报价
                    case 12:
                        dc.ColumnName = "FourQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //备注
                    case 13:
                        dc.ColumnName = "Fremark";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //对应BOM版本编号
                    case 14:
                        dc.ColumnName = "FBomOrder";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料单价
                    case 15:
                        dc.ColumnName = "FPrice";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //客户名称
                    case 16:
                        dc.ColumnName = "CustName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 用于记录-BOM报价单返回的表体信息(提交时使用)
        /// </summary>
        /// <returns></returns>
        public DataTable GetTempOrderEntryTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 10; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //Headid
                    case 0:
                        dc.ColumnName = "Headid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //Entryid
                    case 1:
                        dc.ColumnName = "Entryid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //物料编码ID
                    case 2:
                        dc.ColumnName = "MaterialID";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //物料编码
                    case 3:
                        dc.ColumnName = "MaterialCode";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料名称
                    case 4:
                        dc.ColumnName = "MaterialName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //配方用量
                    case 5:
                        dc.ColumnName = "PeiQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //占比
                    case 6:
                        dc.ColumnName = "ratioQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //物料单价(含税)
                    case 7:
                        dc.ColumnName = "MaterialPrice";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //物料成本(含税)
                    case 8:
                        dc.ColumnName = "MaterialAmount";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //备注
                    case 9:
                        dc.ColumnName = "Remark";
                        dc.DataType = Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 暂存单据删除使用
        /// </summary>
        /// <returns></returns>
        public DataTable DelTempOrderTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 1; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //FID
                    case 0:
                        dc.ColumnName = "FID";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        #endregion

        /// <summary>
        /// 创建产成品名称临时表
        /// </summary>
        /// <returns></returns>
        public DataTable CreateBomProductTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 1; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //产品名称(物料名称)
                    case 0:
                        dc.ColumnName = "ProductName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 创建用户权限表临时表
        /// </summary>
        /// <returns></returns>
        public DataTable CreateUserPermissionTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 11; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    case 0:
                        dc.ColumnName = "Userid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    case 1:
                        dc.ColumnName = "UserName";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    case 2:
                        dc.ColumnName = "UserPwd";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 3:
                        dc.ColumnName = "CreateName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 4:
                        dc.ColumnName = "CreateDt";
                        dc.DataType = Type.GetType("System.DateTime"); 
                        break;
                    case 5:
                        dc.ColumnName = "ApplyId";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    case 6:
                        dc.ColumnName = "CanBackConfirm";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    case 7:
                        dc.ColumnName = "Readid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    case 8:
                        dc.ColumnName = "Addid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    case 9:
                        dc.ColumnName = "Useid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    case 10:
                        dc.ColumnName = "UserRelid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 创建K3-客户临时表
        /// </summary>
        /// <returns></returns>
        public DataTable MakeCustinfoTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 1; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //记录每行的行ID
                    case 0:
                        dc.ColumnName = "CustName";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 批量成本查询-报表使用
        /// </summary>
        /// <returns></returns>
        public DataTable MarkMaterialReportTemp()
        {
            var dt=new DataTable();
            for (var i = 0; i < 11; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //记录每行的行ID
                    case 0:
                        dc.ColumnName = "FMATERIALID";
                        dc.DataType = Type.GetType("System.Int32"); 
                        break;
                    //物料编码
                    case 1:
                        dc.ColumnName = "物料编码";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    //物料名称(品名)
                    case 2:
                        dc.ColumnName = "物料名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //规格
                    case 3:
                        dc.ColumnName = "规格";
                        dc.DataType=Type.GetType("System.String");
                        break;
                    //换算率(密度(KG/L))
                    case 4:
                        dc.ColumnName = "换算率";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //重量(净重)
                    case 5:
                        dc.ColumnName = "重量";
                        dc.DataType = Type.GetType("System.Decimal"); 
                        break;
                    //罐/箱
                    case 6:
                        dc.ColumnName = "罐箱";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //分类
                    case 7:
                        dc.ColumnName = "分类";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //品类
                    case 8:
                        dc.ColumnName = "品类";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //销售计价单位
                    case 9:
                        dc.ColumnName = "销售计价单位";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //U订货计价规格
                    case 10:
                        dc.ColumnName = "U订货计价规格";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// ‘批量成本查询’报表最终临时表
        /// </summary>
        /// <returns></returns>
        public DataTable ReportPrintTempdt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 12; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //物料编码
                    case 0:
                        dc.ColumnName = "物料编码";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //品名(物料名称)
                    case 1:
                        dc.ColumnName = "品名";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //规格
                    case 2:
                        dc.ColumnName = "规格";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //计量单位
                    case 3:
                        dc.ColumnName = "计量单位";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //数量
                    case 4:
                        dc.ColumnName = "数量";
                        dc.DataType = Type.GetType("System.Decimal"); 
                        break;
                    //旧标准成本单价(来源:成本BOM-物料对应的父项金额)
                    case 5:
                        dc.ColumnName = "旧标准成本单价";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //标准成本单价(来源:新成本BOM-物料对应的父项金额)
                    case 6:
                        dc.ColumnName = "标准成本单价";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //换算率
                    case 7:
                        dc.ColumnName = "换算率";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //重量
                    case 8:
                        dc.ColumnName = "重量";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //重量成本单价
                    case 9:
                        dc.ColumnName = "重量成本单价";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //人工及制造费用
                    case 10:
                        dc.ColumnName = "人工及制造费用";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //Markid(用于标记‘标准成本单价’项是否为红色显示 0:是 1:否)
                    case 11:
                        dc.ColumnName = "Markid";
                        dc.DataType = Type.GetType("System.Int32"); 
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// ‘批量成本查询’报表-中间临时表
        /// </summary>
        /// <returns></returns>
        public DataTable ReportTempdt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 8; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //表头物料名称
                    case 0:
                        dc.ColumnName = "表头物料名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //FMATERIALID
                    case 1:
                        dc.ColumnName = "FMATERIALID";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //物料名称
                    case 2:
                        dc.ColumnName = "物料名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //用量
                    case 3:
                        dc.ColumnName = "用量";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //单价-标准成本单价使用
                    case 4:
                        dc.ColumnName = "单价";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //单价-旧标准成本单价使用
                    case 5:
                        dc.ColumnName = "旧标准单价";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //子项金额-标准成本单价使用
                    case 6:
                        dc.ColumnName = "子项金额";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //子项金额-旧标准成本单价使用
                    case 7:
                        dc.ColumnName = "旧子项金额";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        ///  导入EXCEL临时表-批量成本报表使用
        /// </summary>
        /// <returns></returns>
        public DataTable ImportExcelTempdt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 6; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //物料编码
                    case 0:
                        dc.ColumnName = "物料编码";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料名称
                    case 1:
                        dc.ColumnName = "物料名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //规格型号
                    case 2:
                        dc.ColumnName = "规格型号";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //计价单位
                    case 3:
                        dc.ColumnName = "计价单位";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //密度
                    case 4:
                        dc.ColumnName = "密度";
                        dc.DataType = Type.GetType("System.Double"); 
                        break;
                    //净重
                    case 5:
                        dc.ColumnName = "净重";
                        dc.DataType = Type.GetType("System.Double");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 导入EXCEL临时表-BOM物料明细使用
        /// </summary>
        /// <returns></returns>
        public DataTable ImportBomExcelTempdt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 2; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //物料名称
                    case 0:
                        dc.ColumnName = "物料名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //配方用量
                    case 1:
                        dc.ColumnName = "配方用量";
                        dc.DataType = Type.GetType("System.Double");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 导入EXCEL临时表-毛利润报表使用
        /// </summary>
        /// <returns></returns>
        public DataTable ImportProfitExcelTempdt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 2; i++)
            {
                var dc=new DataColumn();

                switch (i)
                {
                    //物料编码
                    case 0:
                        dc.ColumnName = "物料编码";
                        dc.DataType=Type.GetType("System.String");
                        break;
                    //物料名称
                    case 1:
                        dc.ColumnName = "物料名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// '产品成本毛利润表'最终输出临时表
        /// </summary>
        /// <returns></returns>
        public DataTable PrintProfitReportTempdt()
        {
            var dt=new DataTable();
            for (var i = 0; i < 24; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //物料编码
                    case 0:
                        dc.ColumnName = "物料编码";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料名称
                    case 1:
                        dc.ColumnName = "物料名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //规格型号
                    case 2:
                        dc.ColumnName = "规格型号";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //罐/箱
                    case 3:
                        dc.ColumnName = "罐箱";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //旧标准成本单价
                    case 4:
                        dc.ColumnName = "旧标准成本单价";
                        dc.DataType = Type.GetType("System.Decimal"); 
                        break;
                    //标准成本单价
                    case 5:
                        dc.ColumnName = "标准成本单价";
                        dc.DataType=Type.GetType("System.Decimal");
                        break;
                    //销售计价单位
                    case 6:
                        dc.ColumnName = "销售计价单位";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //销售价目表售价
                    case 7:
                        dc.ColumnName = "销售价目表售价";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //币别
                    case 8:
                        dc.ColumnName = "币别";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //毛利润率
                    case 9:
                        dc.ColumnName = "毛利润率";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //计价成本
                    case 10:
                        dc.ColumnName = "计价成本";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //计价单位单位成本
                    case 11:
                        dc.ColumnName = "计价单位单位成本";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //计价单位单位成本(千克)
                    case 12:
                        dc.ColumnName = "计价单位单位成本KG";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //换算率(密度)
                    case 13:
                        dc.ColumnName = "换算率";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //重量
                    case 14:
                        dc.ColumnName = "重量";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //每公斤含税成本小计
                    case 15:
                        dc.ColumnName = "每公斤含税成本小计";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //每公斤材料成本单价
                    case 16:
                        dc.ColumnName = "每公斤材料成本单价";
                        dc.DataType=Type.GetType("System.Decimal");
                        break;
                    //人工及制造费用
                    case 17:
                        dc.ColumnName = "人工及制造费用";
                        dc.DataType=Type.GetType("System.Decimal");
                        break;
                    //包装罐
                    case 18:
                        dc.ColumnName = "包装罐";
                        dc.DataType=Type.GetType("System.String");
                        break;
                    //包装罐单价
                    case 19:
                        dc.ColumnName = "包装罐单价";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //纸箱
                    case 20:
                        dc.ColumnName = "纸箱";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //纸箱单价
                    case 21:
                        dc.ColumnName = "纸箱单价";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //分类
                    case 22:
                        dc.ColumnName = "分类";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //品类
                    case 23:
                        dc.ColumnName = "品类";
                        dc.DataType = Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }
        
        /// <summary>
        /// 基础资料-‘用户组别’使用
        /// </summary>
        /// <returns></returns>
        public DataTable K3UserDtTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 3; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //K3用户名称
                    case 0:
                        dc.ColumnName = "K3用户名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //K3用户组别
                    case 1:
                        dc.ColumnName = "K3用户组别";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //K3用户手机
                    case 2:
                        dc.ColumnName = "K3用户手机";
                        dc.DataType = Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// '研发类别'DT-用户关联-‘研发类别’整合显示使用
        /// </summary>
        /// <returns></returns>
        public DataTable DevGroupTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 6; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //Userid
                    case 0:
                        dc.ColumnName = "Userid";
                        dc.DataType = Type.GetType("System.Int32"); 
                        break;
                    //Groupid
                    case 1:
                        dc.ColumnName = "Groupid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //Dtlid
                    case 2:
                        dc.ColumnName = "Dtlid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //DevGroupId
                    case 3:
                        dc.ColumnName = "DevGroupId";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //研发类别
                    case 4:
                        dc.ColumnName = "研发类别";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //不启用
                    case 5:
                        dc.ColumnName = "不启用";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// '研发类别'DT-用户关联-‘研发类别’提交使用
        /// </summary>
        /// <returns></returns>
        public DataTable DevGroupDt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 5; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //Userid
                    case 0:
                        dc.ColumnName = "Userid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //Groupid
                    case 1:
                        dc.ColumnName = "Groupid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //Dtlid
                    case 2:
                        dc.ColumnName = "Dtlid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //DevGroupId
                    case 3:
                        dc.ColumnName = "DevGroupId";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //CreateDt
                    case 4:
                        dc.ColumnName = "CreateDt";
                        dc.DataType = Type.GetType("System.DateTime"); 
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 记录需要删除的单据明细中的Headid值
        /// 注:只需记录
        /// </summary>
        /// <returns></returns>
        public DataTable DelOfferOrderHeadDt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 1; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //Headid
                    case 0:
                        dc.ColumnName = "Headid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }
        
        /// <summary>
        /// 主窗体删除单据使用(包含暂存单据)
        /// </summary>
        /// <returns></returns>
        public DataTable DelOrderDt()
        {
            var dt=new DataTable();
            for (int i = 0; i < 2; i++)
            {
                var dc=new DataColumn();
                switch (i)
                {
                    //Fid
                    case 0:
                        dc.ColumnName = "Fid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //orderno
                    case 1:
                        dc.ColumnName = "Orderno";
                        dc.DataType = Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

    }
}
