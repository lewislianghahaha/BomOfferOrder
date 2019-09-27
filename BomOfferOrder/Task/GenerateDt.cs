using System;
using System.Data;
using System.Data.SqlClient;
using BomOfferOrder.DB;

namespace BomOfferOrder.Task
{
    public class GenerateDt
    {
        DbList dbList=new DbList();
        SqlList sqlList=new SqlList();
        SearchDt searchDt=new SearchDt();

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

            try
            {
                //获取结果临时表
                result = dbList.MakeTemp();
                //循环sourcedt
                foreach (DataRow row in sourcedt.Rows)
                {
                    id++;
                    //获取FMATERIALID
                    var fmaterialid = Convert.ToInt32(row[0]);
                    //获取产品名称
                    var productname = Convert.ToString(row[2]);
                    //获取包装规格
                    var bao = Convert.ToString(row[3]);
                    //获取产品密度
                    var productmi = Convert.ToString(row[4]);

                    //获取BOM编号
                    var dtlrows = bomdt.Select("表头物料ID='" + Convert.ToInt32(row[0]) + "'");
                    var bomnum = Convert.ToString(dtlrows[0][1]);

                    //执行插入相关信息至临时表(使用递归)
                    result.Merge(GetdtltoDt(id,fmaterialid,productname,bao,productmi,bomnum,bomdt,result,0));
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
        /// <param name="fmaterialid">表头物料ID(循环条件)</param>
        /// <param name="productname">产品名称</param>
        /// <param name="bao">包装规格</param>
        /// <param name="productmi">产品密度</param>
        /// <param name="bomnum">BOM编号</param>
        /// <param name="bomdt">初始化BOM明细DT(全部Bom内容)</param>
        /// <param name="resultdt">结果临时表</param>
        /// <param name="qty">用量使用</param>
        /// <returns></returns>
        private DataTable GetdtltoDt(int id,int fmaterialid,string productname,string bao,string productmi, string bomnum,DataTable bomdt,
                                     DataTable resultdt,decimal qty)
        {
            //‘用量’中转值
            decimal qtytemp = 0;

            //根据fmaterialid为‘表头物料ID’为条件,查询bomdt内的明细记录
            var dtlrows = bomdt.Select("表头物料ID='" + fmaterialid + "'");

            for (var i = 0; i < dtlrows.Length; i++)
            {
                //循环判断物料对应的“物料属性”是不是“外购”,若是就插入至resultdt内,反之,进行递归
                if (Convert.ToString(dtlrows[i][5]) == "外购")
                {
                    //判断进入的物料ID是否需要更新或是插入记录
                    //检查获取过来的‘物料编码ID’是否在第一层级的BOM明细行内存在,若存在,即不用创建新行插入,只需累加‘用量’即可
                    var resultrows = resultdt.Select("产品名称='" + productname + "' and 明细行BOM编号='" + bomnum + "' and 物料编码ID='"+ dtlrows[i][2] + "'");  
 
                    if (resultrows.Length > 0)
                    {
                        foreach (DataRow row in resultdt.Rows)
                        {
                            //使用‘产品名称’ ‘明细行BOM编号’ ‘表体物料ID’放到resultdt内判断是否存在;若存在,就更新,不用插入新行至resultdt
                            if (row[1].ToString() == productname && row[9].ToString() == bomnum  && Convert.ToInt32(row[5]) == Convert.ToInt32(dtlrows[i][2]))
                            {
                                //当检查到物料在resultdt存在的话,就进行更新
                                resultdt.BeginInit();
                                //若是第一层级的‘外购’物料，其‘用量’就是取SQL内的‘用量’;反之用量的公式为:‘总用量’*分子/分母*(1+变动损耗率/100) 保留6位小数
                                qtytemp = qty == 0 ? Convert.ToDecimal(dtlrows[i][6]) : 
                                    decimal.Round(qty * Convert.ToDecimal(dtlrows[i][7]) / Convert.ToDecimal(dtlrows[i][8]) * (1 + Convert.ToDecimal(dtlrows[i][9]) / 100), 6);
                                //累加‘用量’
                                row[8] = Convert.ToDecimal(row[8]) + qtytemp;
                                resultdt.EndInit();
                                //当修改完成后,跳出该循环
                                break;
                            }
                        }
                    }
                    else
                    {
                        //若是第一层级的‘外购’物料，其‘用量’就是取SQL内的‘用量’;反之用量的公式为:‘总用量’*分子/分母*(1+变动损耗率/100) 保留6位小数
                        qtytemp = qty == 0 ? Convert.ToDecimal(dtlrows[i][6]) : 
                            decimal.Round(qty * Convert.ToDecimal(dtlrows[i][7]) / Convert.ToDecimal(dtlrows[i][8])*(1+Convert.ToDecimal(dtlrows[i][9])/100),6);

                        var newrow = resultdt.NewRow();
                        newrow[0] = id;                   //产品ID
                        newrow[1] = productname;          //产品名称
                        newrow[2] = bomnum;               //BOM编号
                        newrow[3] = bao;                  //包装规格
                        newrow[4] = productmi;            //产品密度
                        newrow[5] = dtlrows[i][2];        //物料编码ID
                        newrow[6] = dtlrows[i][3];        //物料编码
                        newrow[7] = dtlrows[i][4];        //物料名称
                        newrow[8] = qtytemp;              //配方用量
                        newrow[9] = dtlrows[i][1];        //明细行BOM编号
                        newrow[10] = dtlrows[i][10];      //物料单价
                        newrow[11] = dtlrows[i][11];      //表头物料单价
                        resultdt.Rows.Add(newrow);
                    }
                }
                //递归调用
                else
                {
                    GetdtltoDt(id, Convert.ToInt32(dtlrows[i][2]),productname,bao,productmi,bomnum,bomdt,resultdt,Convert.ToDecimal(dtlrows[i][6]));
                }
            }
            return resultdt;
        }


        /// <summary>
        /// 获取最新的FID值
        /// </summary>
        /// <returns></returns>
        public int GetNewFidValue()
        {
            var sqlscript = sqlList.GetNewFidValue();
            return  Convert.ToInt32(searchDt.UseSqlSearchIntoDt(1, sqlscript).Rows[0][0]);
        }

        ///// <summary>
        ///// 获取最新的Headid值
        ///// </summary>
        ///// <returns></returns>
        public int GetNewHeadidValue()
        {
            var sqlscript = sqlList.GetNewHeadidValue();
            return Convert.ToInt32(searchDt.UseSqlSearchIntoDt(1,sqlscript).Rows[0][0]);
        }

        /// <summary>
        /// 获取最新的Entryid值
        /// </summary>
        /// <returns></returns>
        public int GetNewEntryidValue()
        {
            var sqlscript = sqlList.GetNewEntryidValue();
            return Convert.ToInt32(searchDt.UseSqlSearchIntoDt(1, sqlscript).Rows[0][0]);
        }

        /// <summary>
        /// 根据FID更新此单据占用情况
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="type">type:0(更新当前用户信息) 1(清空占用记录)</param>
        /// <returns></returns>
        public void UpDateUpUseDetail(int fid, int type)
        {
            var sqlscript = type == 0 ? sqlList.UpUsedtl(fid) : sqlList.RemoveUsedtl(fid);
            searchDt.Generdt(sqlscript);
        }

        /// <summary>
        /// 根据FID反审核单据状态(更新为反审核状态,并将审核日期清空)
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public bool UpdateOrderStatus(int fid)
        {
            var sqlscript = sqlList.UpOrderStatus(fid);
            return searchDt.Generdt(sqlscript);
        }

        /// <summary>
        /// 获取最新的Userid值
        /// </summary>
        /// <returns></returns>
        public int GetNewUseridValue()
        {
            var sqlscript = sqlList.GetNewUseridValue();
            return Convert.ToInt32(searchDt.UseSqlSearchIntoDt(1, sqlscript).Rows[0][0]);
        }

        /// <summary>
        /// 根据useid更新用户占用记录
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="typeid">0:更新占用 1:清空占用</param>
        public void UpAccountUseridValue(int userid, int typeid)
        {
            var sqlscript = typeid == 0 ? sqlList.UpdateAccountUseid(userid) : sqlList.RemoveAccountUseid(userid);
            searchDt.Generdt(sqlscript);
        }

        /// <summary>
        /// 更新用户新密码
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="newpwd"></param>
        /// <returns></returns>
        public bool UpdateUserNewpwd(int userid, string newpwd)
        {
            var sqlscript = sqlList.UpdateUserNewpwd(userid, newpwd);
            return searchDt.Generdt(sqlscript);
        }

    }
}
