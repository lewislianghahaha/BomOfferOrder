using System;
using System.Data;
using BomOfferOrder.DB;

namespace BomOfferOrder.Task
{
    //权限相关
    public class ImportPerDt
    {
        DbList dbList=new DbList();
        GenerateDt generateDt=new GenerateDt();
        ImportDt importDt=new ImportDt();

        /// <summary>
        /// 基础资料-用户组别提交
        /// 中心:dt dtl当碰到主键包含-，即为插入,反之为更新;
        /// </summary>
        /// <param name="dt">表头信息</param>
        /// <param name="dtl">表体信息</param>
        /// <param name="deldt">删除表头信息</param>
        /// <param name="deldtldt">删除表体信息</param>
        /// <returns></returns>
        public bool ImportBdUserGroup(DataTable dt,DataTable dtl,DataTable deldt,DataTable deldtldt)
        {
            var result = true;

            try
            {
                GetDtToDb(dt,dtl);
                //最后若deldt deldtldt有值的话都执行删除方法 todo
                //if(deldt.Rows.Count>0)

               // if(deldtldt.Rows.Count>0)

            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 根据指定的表名等相关信息,按情况执行插入或更新操作(中转)
        /// </summary>
        /// <param name="sourcedt"></param>
        /// <param name="sourcedtldt"></param>
        private void GetDtToDb(DataTable sourcedt,DataTable sourcedtldt)
        {
            //记录原groupid值(用于插入时表体查找使用)
            var _oldgroupid = 0;
            //记录新Groupid值(用于插入使用)
            var _newgroupid = 0;

            //先构建插入及更新临时表
            var insertgroupdt = sourcedt.Clone();
            var insertgroupdtldt = sourcedtldt.Clone();

            var upgroupdt = sourcedt.Clone();
            var upgroupdtldt = sourcedtldt.Clone();

            //整理出需‘插入’或‘更新’的记录
            //表头
            foreach (DataRow rows in sourcedt.Rows)
            {
                //若包含“-”就执行插入
                if (Convert.ToString(rows[0]).Contains("-"))
                {
                    insertgroupdt.Merge(ModifyDt(insertgroupdt,rows));
                }
                else
                {
                    upgroupdt.Merge(ModifyDt(upgroupdt, rows));
                }
            }

            //表体
            foreach (DataRow rows in sourcedtldt.Rows)
            {
                //若包含“-”就执行插入
                if (Convert.ToString(rows[0]).Contains("-"))
                {
                    insertgroupdtldt.Merge(ModifyDt(insertgroupdtldt,rows));
                }
                else
                {
                    upgroupdtldt.Merge(ModifyDt(upgroupdtldt,rows));
                }
            }

            //循环insertgroupdt 及 insertgroupdtldt对‘插入’的记录新增groupid 及 dtlid
            for (var i = 0; i < insertgroupdt.Rows.Count; i++)
            {
                //获取原Groupid
                _oldgroupid = Convert.ToInt32(insertgroupdt.Rows[i][0]);
                //获取新增的Groupid值
                _newgroupid = GetGroupidKey();
                //根据_oldgroupid作为条件查询对应的insertgroupdtldt记录,并进行相应的更新
                for (var j = 0; j < insertgroupdtldt.Rows.Count; j++)
                {
                    if(Convert.ToInt32(insertgroupdtldt.Rows[i][0]) != _oldgroupid) continue;
                    insertgroupdtldt.Rows[j].BeginEdit();
                    insertgroupdtldt.Rows[j][0] = _newgroupid;    //groupid
                    insertgroupdtldt.Rows[j][1] = GetDtlidKey();  //dtlid
                    insertgroupdtldt.Rows[j].EndEdit();
                }

                //最后对insertgroupdt进行groupid的更新
                insertgroupdt.Rows[i].BeginEdit();
                insertgroupdt.Rows[i][0] = _newgroupid;
                insertgroupdt.Rows[i].EndEdit();
            }

            //执行插入 及 更新操作
            if (insertgroupdt.Rows.Count > 0)
                importDt.ImportDtToDb("T_BD_UserGroup", insertgroupdt);
            if (insertgroupdtldt.Rows.Count > 0)
                importDt.ImportDtToDb("T_BD_UserGroupDtl", insertgroupdtldt);

            if(upgroupdt.Rows.Count>0)
                importDt.UpdateDbFromDt("T_AD_User", upgroupdt);
            if (upgroupdtldt.Rows.Count>0)
                importDt.UpdateDbFromDt("T_AD_User", upgroupdtldt);
        }

        /// <summary>
        /// 将数据进行分类;分为插入及更新
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        private DataTable ModifyDt(DataTable temp,DataRow rows)
        {
            var newrow = temp.NewRow();
            for (var i = 0; i < temp.Columns.Count; i++)
            {
                newrow[i] = rows[i];
            }
            temp.Rows.Add(newrow);
            return temp;
        }

        /// <summary>
        /// 用户权限提交
        /// </summary>
        /// <returns></returns>
        public bool ImportUserPermissionDt(DataTable sourcedt)
        {
            var result = true;
            try
            {
                //获取用户权限临时表
                var tempdt = dbList.CreateUserPermissionTemp();

                //判断若sourcedt内的Userid为0,即需要插入操作,反之,为更新操作
                var dtlrows = sourcedt.Select("Userid=0");

                //对临时表进行添加操作
                var newrow = tempdt.NewRow();

                newrow[0] = dtlrows.Length > 0 ? GetUseridKey() : sourcedt.Rows[0][0]; //UserId       
                newrow[1] = sourcedt.Rows[0][1];      //用户名称
                newrow[2] = sourcedt.Rows[0][2];      //用户密码
                newrow[3] = sourcedt.Rows[0][3];      //创建人
                newrow[4] = sourcedt.Rows[0][4];      //创建日期
                newrow[5] = sourcedt.Rows[0][5];      //是否启用
                newrow[6] = sourcedt.Rows[0][6];      //能否反审核
                newrow[7] = sourcedt.Rows[0][7];      //能否查阅明细金额
                newrow[8] = sourcedt.Rows[0][8];      //能否对明细物料操作
                newrow[9] = sourcedt.Rows[0][9];      //是否占用
                tempdt.Rows.Add(newrow);

                //执行插入操作
                if (dtlrows.Length > 0)
                {
                    importDt.ImportDtToDb("T_AD_User", tempdt);
                }
                //执行更新操作
                else
                {
                    importDt.UpdateDbFromDt("T_AD_User", tempdt);
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 获取最新的Userid值
        /// </summary>
        /// <returns></returns>
        private int GetUseridKey()
        {
            return generateDt.GetNewUseridValue();
        }

        /// <summary>
        /// 获取GroupID主键值
        /// </summary>
        /// <returns></returns>
        private int GetGroupidKey()
        {
            return generateDt.GetGroupidKey();
        }

        /// <summary>
        /// 获取Dtlid主键值
        /// </summary>
        /// <returns></returns>
        private int GetDtlidKey()
        {
            return generateDt.GetDtlidKey();
        }



    }
}
