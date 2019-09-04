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
            for (var i = 0; i < 8; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    case 0:
                        dc.ColumnName = "CountId";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    case 1:
                        dc.ColumnName = "Id";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 2:
                        dc.ColumnName = "产品名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 3:
                        dc.ColumnName = "BOM编号";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 4:
                        dc.ColumnName = "包装规格";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 5:
                        dc.ColumnName = "产品密度";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    case 6:
                        dc.ColumnName = "物料编码";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 7:
                        dc.ColumnName = "物料名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        ///用于明细记录保存
        /// </summary>
        /// <returns></returns>
        public DataTable MakeDtlTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 19; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    //产品代码(物料代码)
                    case 0:
                        dc.ColumnName = "MaterialCode";
                        dc.DataType = Type.GetType("System.String");
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
                    //产品密度(KG / L)
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
                    //成本(元 / KG)
                    case 7:
                        dc.ColumnName = "KGQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //成本(元 / L)
                    case 8:
                        dc.ColumnName = "LQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //50 % 报价
                    case 9:
                        dc.ColumnName = "50Qty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //45 % 报价
                    case 10:
                        dc.ColumnName = "45Qty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //40 % 报价
                    case 11:
                        dc.ColumnName = "40Qty";
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

                    //物料编码
                    case 14:
                        dc.ColumnName = "MaterialCode";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料名称
                    case 15:
                        dc.ColumnName = "MaterialName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //配方用量
                    case 16:
                        dc.ColumnName = "PeiQty";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //物料单价(含税)
                    case 17:
                        dc.ColumnName = "MaterialPrice";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //物料成本(含税)
                    case 18:
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
