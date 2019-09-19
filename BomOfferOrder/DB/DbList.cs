using System;
using System.Collections;
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
            for (var i = 0; i < 28; i++)
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
                        dc.ColumnName= "CreateDt";
                        dc.DataType = Type.GetType("System.DateTime"); 
                        break;
                    //记录当前单据使用标记
                    case 4:
                        dc.ColumnName = "Useid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //记录当前单据使用者信息
                    case 5:
                        dc.ColumnName = "UserName";
                        dc.DataType = Type.GetType("System.String");
                        break;

                    //Headid
                    case 6:
                        dc.ColumnName = "Headid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //产品名称(物料名称)
                    case 7:
                        dc.ColumnName = "ProductName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //包装规格
                    case 8:
                        dc.ColumnName = "Bao";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //产品密度(KG/L)
                    case 9:
                        dc.ColumnName = "ProductMi";
                        dc.DataType = Type.GetType("System.Decimal"); 
                        break;
                    //材料成本(不含税)
                    case 10:
                        dc.ColumnName = "MaterialQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //包装成本
                    case 11:
                        dc.ColumnName = "BaoQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //人工及制造费用
                    case 12:
                        dc.ColumnName = "RenQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //成本(元/KG)
                    case 13:
                        dc.ColumnName = "KGQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //成本(元/L)
                    case 14:
                        dc.ColumnName = "LQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //50%报价
                    case 15:
                        dc.ColumnName = "FiveQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //45%报价
                    case 16:
                        dc.ColumnName = "FourFiveQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //40%报价
                    case 17:
                        dc.ColumnName = "FourQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //备注
                    case 18:
                        dc.ColumnName = "Fremark";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    //对应BOM版本编号
                    case 19:
                        dc.ColumnName = "FBomOrder";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料单价
                    case 20:
                        dc.ColumnName = "FPrice";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;

                    //Entryid
                    case 21:
                        dc.ColumnName = "Entryid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //物料编码ID
                    case 22:
                        dc.ColumnName = "MaterialID";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //物料编码
                    case 23:
                        dc.ColumnName = "MaterialCode";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料名称
                    case 24:
                        dc.ColumnName = "MaterialName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //配方用量
                    case 25:
                        dc.ColumnName = "PeiQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //物料单价(含税)
                    case 26:
                        dc.ColumnName = "MaterialPrice";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //物料成本(含税)
                    case 27:
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
                    //记录当前单据使用标记
                    case 4:
                        dc.ColumnName = "Useid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //记录当前单据使用者信息
                    case 5:
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
            for (var i = 0; i < 15; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //Headid
                    case 0:
                        dc.ColumnName = "Headid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //产品名称(物料名称)
                    case 1:
                        dc.ColumnName = "MaterialName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //包装规格
                    case 2:
                        dc.ColumnName = "Bao";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //产品密度(KG/L)
                    case 3:
                        dc.ColumnName = "ProductMi";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //材料成本(不含税)
                    case 4:
                        dc.ColumnName = "MaterialQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //包装成本
                    case 5:
                        dc.ColumnName = "BaoQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //人工及制造费用
                    case 6:
                        dc.ColumnName = "RenQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //成本(元/KG)
                    case 7:
                        dc.ColumnName = "KGQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //成本(元/L)
                    case 8:
                        dc.ColumnName = "LQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //50%报价
                    case 9:
                        dc.ColumnName = "FiveQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //45%报价
                    case 10:
                        dc.ColumnName = "FourFiveQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //40%报价
                    case 11:
                        dc.ColumnName = "FourQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //备注
                    case 12:
                        dc.ColumnName = "Fremark";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //对应BOM版本编号
                    case 13:
                        dc.ColumnName = "FBomOrder";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料单价
                    case 14:
                        dc.ColumnName = "FPrice";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataTable GetOfferOrderEntryTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 7; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //Entryid
                    case 0:
                        dc.ColumnName = "Entryid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //物料编码ID
                    case 1:
                        dc.ColumnName = "MaterialID";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //物料编码
                    case 2:
                        dc.ColumnName = "MaterialCode";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料名称
                    case 3:
                        dc.ColumnName = "MaterialName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //配方用量
                    case 4:
                        dc.ColumnName = "PeiQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //物料单价(含税)
                    case 5:
                        dc.ColumnName = "MaterialPrice";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //物料成本(含税)
                    case 6:
                        dc.ColumnName = "MaterialAmount";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

    }
}
