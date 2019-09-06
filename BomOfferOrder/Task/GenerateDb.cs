using System;
using System.Data;
using BomOfferOrder.DB;

namespace BomOfferOrder.Task
{
    public class GenerateDb
    {
        DbList dbList=new DbList();

        /// <summary>
        /// 运算-(将‘已添加’的物料记录生成其对应的BOM明细记录)
        /// </summary>
        /// <param name="valuelist">FMATERIALID列表ID(如:'426464','738199')</param>
        /// <param name="sourcedt">从查询窗体添加过来的DT记录</param>
        /// <param name="bomdt">BOM明细记录DT</param>
        /// <returns></returns>
        public DataTable Generatedt(string valuelist,DataTable sourcedt,DataTable bomdt)
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
                //循环sourcedt
                foreach (DataRow row in sourcedt.Rows)
                {
                    id++;
                    //获取BOM明细记录临时表
                    var bomTemp = dbList.GetBomTemp();

                    //获取FMATERIALID
                   // var fmaterialid = Convert.ToInt32(row[0]);
                    //获取产品名称
                    var productname = Convert.ToString(row[2]);
                    //获取包装规格
                    var bao = Convert.ToString(row[3]);
                    //获取产品密度
                    var productmi = Convert.ToString(row[4]);

                    //判断表头信息
                    var dtlrows = bomdt.Select("表头物料ID='" + Convert.ToInt32(row[0]) + "'");

                    //排除并获取最新‘修改日期’的相关BOM明细记录
                    for (var i = 0; i < dtlrows.Length; i++)
                    {
                        if (modifydt == "" || modifydt == Convert.ToString(dtlrows[i][2]))
                        {
                            var newrow = bomTemp.NewRow();
                            newrow[0] = dtlrows[i][1];   //BOM编号
                            newrow[1] = row[0];          //表头物料ID
                            newrow[2] = dtlrows[i][3];   //表体物料ID
                            newrow[3] = dtlrows[i][4];   //物料编码
                            newrow[4] = dtlrows[i][5];   //物料名称
                            newrow[5] = dtlrows[i][6];   //物料属性
                            bomTemp.Rows.Add(newrow);
                            modifydt = Convert.ToString(dtlrows[i][2]);
                        }
                    }
                    //当结束循环后将modifydt变量清空
                    modifydt = "";

                    //当得出最新的BOM明细记录后,获取BOM编号
                    var bomnum = Convert.ToString(bomTemp.Rows[0][0]);

                    //执行插入相关信息至临时表
                    result.Merge(GetdtltoDt(id,productname,bao,productmi,bomnum,bomdt,result,bomTemp));
                }
            }
            catch (Exception)
            {
                result.Rows.Clear();
                result.Columns.Clear();
            }
            return result;
        }

        /// <summary>
        /// 执行循环插入(递归)
        /// </summary>
        /// <param name="id">产品ID（从1开始)</param>
        /// <param name="productname">产品名称</param>
        /// <param name="bao">包装规格</param>
        /// <param name="productmi">产品密度</param>
        /// <param name="bomnum">BOM编号</param>
        /// <param name="bomdt">初始化BOM明细DT(全部Bom内容)</param>
        /// <param name="resultdt">结果临时表</param>
        /// <param name="bomtemp">BOM临时表(递归时使用)(重)</param>
        /// <returns></returns>
        private DataTable GetdtltoDt(int id,string productname,string bao,string productmi, string bomnum,
                                     DataTable bomdt,DataTable resultdt,DataTable bomtemp)
        {
            //保存‘修改日期’(作用:获取同一个FMATERIALID中最大的‘修改日期’)
            var modifydt = string.Empty;

            //保存'BOM等级'
            var bomlevel = 0;
            //检测bomtemp内的‘表体物料ID’是否已在resultdt内‘物料编码ID’存在时使用
            var markbool = true;

            //核心思路:若该物料的‘物料属性’为‘外购’,即进行添加至resultdt内;反之,继续以此FMATERIALID查找与其对应的BOM物料明细记录,直至全部‘物料明细记录’为‘外购’才结束循环

            //检测若bomtemp内的‘物料属性’全部为‘外购’ 并且已在resultdt内存在,就跳出循环
            // 1)判断若在bomtemp内的物料记录的‘物料属性’都不是‘自制’的话,表示都为‘外购’
            var checkincloudzhi = bomtemp.Select("物料属性='自制'");  
            // 2)判断bomtemp内的‘表体物料ID’是否已在resultdt内‘物料编码ID’存在
            foreach (DataRow row in bomtemp.Rows)
            {
                var checkinclouddt = resultdt.Select("物料编码ID ='" + Convert.ToInt32(row[2]) + "'");
                markbool = checkinclouddt.Length > 0;
            }

            if(checkincloudzhi.Length==0 && markbool)
                return resultdt;

            //循环bomtemp
            foreach (DataRow rows in bomtemp.Rows)
            {
                //判断此物料中的‘物料属性’是不是‘外购’
                var dtlrows = bomdt.Select("表头物料ID='" + Convert.ToInt32(rows[1]) + "' and 表体物料ID= '" + Convert.ToInt32(rows[2]) + "' and 物料属性='外购'");

                //判断没有在resultdt内存在才插入
                var checkinclouddt = resultdt.Select("物料编码ID ='" + Convert.ToInt32(dtlrows[0][3]) + "'");
                markbool = checkinclouddt.Length > 0;

                if (dtlrows.Length > 0 && markbool)
                {
                    bomlevel++;

                    var newrow = resultdt.NewRow();
                    newrow[0] = id;                   //产品ID
                    newrow[1] = productname;          //产品名称
                    newrow[2] = bomnum;               //BOM编号
                    newrow[3] = bao;                  //包装规格
                    newrow[4] = productmi;            //产品密度
                    newrow[5] = bomlevel;             //Bom等级
                    newrow[6] = dtlrows[0][3];        //物料编码ID
                    newrow[7] = dtlrows[0][4];        //物料编码
                    newrow[8] = dtlrows[0][5];        //物料名称
                    resultdt.Rows.Add(newrow);
                }
                //(当发现循环的物料ID对应的‘物料属性’为‘自制’时),弱执行递归调用
                else
                {
                    //将bomtemp清空,再插入新值
                    bomtemp.Rows.Clear();

                    //将‘表体物料ID’放到‘表头物料ID’内进行查询
                    var bomrows = bomdt.Select("表头物料ID='" + Convert.ToInt32(rows[2]) + "'");

                    //排除并获取最新‘修改日期’的相关BOM明细记录
                    for (var i = 0; i < bomrows.Length; i++)
                    {
                        if (modifydt == "" || modifydt == Convert.ToString(bomrows[i][2]))
                        {
                            var newrow = bomtemp.NewRow();
                            newrow[0] = bomnum;          //BOM编号
                            newrow[1] = rows[2];         //表头物料ID
                            newrow[2] = bomrows[i][3];   //表体物料ID
                            newrow[3] = bomrows[i][4];   //物料编码
                            newrow[4] = bomrows[i][5];   //物料名称
                            newrow[5] = bomrows[i][6];   //物料属性
                            bomtemp.Rows.Add(newrow);
                            modifydt = Convert.ToString(bomrows[i][2]);
                        }
                    }
                    //当结束循环后将modifydt变量清空
                    modifydt = "";
                    //进入递归
                    GetdtltoDt(id,productname,bao,productmi,bomnum,bomdt,resultdt,bomtemp);
                }
            }
            return resultdt;
        }

    }
}
