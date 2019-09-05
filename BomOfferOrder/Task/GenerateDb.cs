using System;
using System.Data;
using BomOfferOrder.DB;

namespace BomOfferOrder.Task
{
    public class GenerateDb
    {
        DbList dbList=new DbList();
        SearchDb searchDb=new SearchDb();

        /// <summary>
        /// 运算-(将‘已添加’的物料记录生成其对应的BOM明细记录)
        /// </summary>
        /// <param name="valuelist">FMATERIALID列表ID(如:'426464','738199')</param>
        /// <param name="sourcedt">从查询窗体添加过来的DT记录</param>
        /// <returns></returns>
        public DataTable Generatedt(string valuelist,DataTable sourcedt)
        {
            var result=new DataTable();
            //保存产品ID（从1开始）
            var id = 0;
            //保存‘修改日期’(作用:获取同一个FMATERIALID中最大的‘修改日期’)
            var modifydt = string.Empty;

            try
            {
                //获取结果临时表
                result = dbList.MakeTemp();
                //获取数据源(成本BOM明细)
                var bomdt = searchDb.SearchMaterialDtl(valuelist);

                //循环Sourcedt
                foreach (DataRow row in sourcedt.Rows)
                {
                    //产品ID记录
                    id++;
                    //循环使用FMATERIALID及markid作条件判断
                    for (var i = 0; i < 2; i++)
                    {
                        var dtlrows = bomdt.Select("FMATERIALID='" + Convert.ToInt32(row[0]) + "' and markid= '" + i + "'");
   
                        if (dtlrows.Length > 0)
                        {
                            //循环判断
                            for (var r = 0; r < dtlrows.Length; r++)
                            {
                                //获取修改日期
                                var dt = Convert.ToString(dtlrows[r][2]);
                                //判断若‘修改日期’变量为空或 变量与数据表内获得的‘修改日期’一样才执行插入
                                if (modifydt=="" || modifydt==dt)
                                {
                                    //新相关记录插入至结果临时表
                                    var newrow = result.NewRow();
                                    //newrow[0] = sourcedt.Rows.Count;  //累加值ID
                                    newrow[0] = id;                   //产品ID
                                    newrow[1] = row[2];               //产品名称
                                    newrow[2] = dtlrows[r][1];        //BOM编号
                                    newrow[3] = row[3];               //包装规格
                                    newrow[4] = row[4];               //产品密度
                                    newrow[5] = dtlrows[r][3];        //物料编码ID
                                    newrow[6] = dtlrows[r][4];        //物料编码
                                    newrow[7] = dtlrows[r][5];        //物料名称
                                    result.Rows.Add(newrow);
                                    modifydt = dt;
                                }
                            }
                        }
                        //当结束循环后将modifydt变量清空
                        modifydt = "";
                    }
                }
            }
            catch (Exception)
            {
                result.Rows.Clear();
                result.Columns.Clear();
            }
            return result;
        }
        
    }
}
