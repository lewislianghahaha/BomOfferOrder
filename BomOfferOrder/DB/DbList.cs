using System;
using System.CodeDom;
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
            for (var i = 0; i < 12; i++)
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
                    case 11:
                        dc.ColumnName = "表头物料单价";
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
            for (var i = 0; i < 7; i++)
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
                        dc.ColumnName = "物料单价(含税)";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 6:
                        dc.ColumnName = "物料成本(含税)";
                        dc.DataType = Type.GetType("System.String");
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
            for (var i = 0; i < 30; i++)
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
                    //审核日期
                    case 3:
                        dc.ColumnName = "ConfirmDt";
                        dc.DataType = Type.GetType("System.DateTime");
                        break;
                    //创建日期
                    case 4:
                        dc.ColumnName= "CreateDt";
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

                    //Headid
                    case 8:
                        dc.ColumnName = "Headid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //产品名称(物料名称)
                    case 9:
                        dc.ColumnName = "ProductName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //包装规格
                    case 10:
                        dc.ColumnName = "Bao";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //产品密度(KG/L)
                    case 11:
                        dc.ColumnName = "ProductMi";
                        dc.DataType = Type.GetType("System.Decimal"); 
                        break;
                    //材料成本(不含税)
                    case 12:
                        dc.ColumnName = "MaterialQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //包装成本
                    case 13:
                        dc.ColumnName = "BaoQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //人工及制造费用
                    case 14:
                        dc.ColumnName = "RenQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //成本(元/KG)
                    case 15:
                        dc.ColumnName = "KGQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //成本(元/L)
                    case 16:
                        dc.ColumnName = "LQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //50%报价
                    case 17:
                        dc.ColumnName = "FiveQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //45%报价
                    case 18:
                        dc.ColumnName = "FourFiveQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //40%报价
                    case 19:
                        dc.ColumnName = "FourQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //备注
                    case 20:
                        dc.ColumnName = "Fremark";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    //对应BOM版本编号
                    case 21:
                        dc.ColumnName = "FBomOrder";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料单价
                    case 22:
                        dc.ColumnName = "FPrice";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;

                    //Entryid
                    case 23:
                        dc.ColumnName = "Entryid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //物料编码ID
                    case 24:
                        dc.ColumnName = "MaterialID";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //物料编码
                    case 25:
                        dc.ColumnName = "MaterialCode";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料名称
                    case 26:
                        dc.ColumnName = "MaterialName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //配方用量
                    case 27:
                        dc.ColumnName = "PeiQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //物料单价(含税)
                    case 28:
                        dc.ColumnName = "MaterialPrice";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //物料成本(含税)
                    case 29:
                        dc.ColumnName = "MaterialAmount";
                        dc.DataType = Type.GetType("System.Decimal");
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
            for (var i = 0; i < 8; i++)
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
                    //审核日期
                    case 3:
                        dc.ColumnName = "ConfirmDt";
                        dc.DataType = Type.GetType("System.DateTime");
                        break;
                    //创建日期
                    case 4:
                        dc.ColumnName = "CreateDt";
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
            for (var i = 0; i < 16; i++)
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
            for (var i = 0; i < 8; i++)
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
                    //物料单价(含税)
                    case 6:
                        dc.ColumnName = "MaterialPrice";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //物料成本(含税)
                    case 7:
                        dc.ColumnName = "MaterialAmount";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

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
            for (var i = 0; i < 10; i++)
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
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

    }
}
