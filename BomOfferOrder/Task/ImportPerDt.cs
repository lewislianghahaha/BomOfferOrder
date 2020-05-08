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
        SearchDt searchDt=new SearchDt();
        SqlList sqlList=new SqlList();

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
               //最后若deldt deldtldt有值的话都执行删除方法
               if(deldt.Rows.Count>0)
                    DelRecord(0,deldt);
               if(deldtldt.Rows.Count>0)
                    DelRecord(1,deldtldt);
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
                if (Convert.ToString(rows[1]).Contains("-"))
                {
                    insertgroupdtldt.Merge(ModifyDt(insertgroupdtldt,rows));
                }
                else
                {
                    upgroupdtldt.Merge(ModifyDt(upgroupdtldt,rows));
                }
            }

            //循环insertgroupdt 及 insertgroupdtldt对‘插入’的记录新增groupid 及 dtlid
            if (insertgroupdt.Rows.Count > 0)
            {
                for (var i = 0; i < insertgroupdt.Rows.Count; i++)
                {
                    //获取原Groupid
                    _oldgroupid = Convert.ToInt32(insertgroupdt.Rows[i][0]);
                    //获取新增的Groupid值
                    _newgroupid = GetGroupidKey();

                    //根据_oldgroupid作为条件查询对应的insertgroupdtldt记录,并进行相应的更新
                    for (var j = 0; j < insertgroupdtldt.Rows.Count; j++)
                    {
                        if (Convert.ToInt32(insertgroupdtldt.Rows[j][0]) != _oldgroupid) continue;
                        insertgroupdtldt.Rows[j].BeginEdit();
                        insertgroupdtldt.Rows[j][0] = _newgroupid;    //groupid
                        insertgroupdtldt.Rows[j][1] = GetDtlidKey();  //dtlid
                        insertgroupdtldt.Rows[j].EndEdit();
                    }

                    //最后对insertgroupdt进行groupid的更新
                    if (Convert.ToInt32(insertgroupdt.Rows[i][0]) != _oldgroupid) continue;
                    insertgroupdt.Rows[i].BeginEdit();
                    insertgroupdt.Rows[i][0] = _newgroupid;
                    insertgroupdt.Rows[i].EndEdit();
                }
            }           
            //当insertgroupdt表头没有新增记录,而insertgroupdtldt表体有时
            else
            {
                for (var i = 0; i < insertgroupdtldt.Rows.Count; i++)
                {
                    //若Dtlid包含“-”时才更新获取新ID值
                    if (!Convert.ToString(insertgroupdtldt.Rows[i][1]).Contains("-")) continue;
                    insertgroupdtldt.Rows[i].BeginEdit();
                    insertgroupdtldt.Rows[i][1] = GetDtlidKey();
                    insertgroupdtldt.Rows[i].EndEdit();
                }
            }

            //执行插入 及 更新操作
            if (insertgroupdt.Rows.Count > 0)
                importDt.ImportDtToDb("T_BD_UserGroup", insertgroupdt);
            if (insertgroupdtldt.Rows.Count > 0)
                importDt.ImportDtToDb("T_BD_UserGroupDtl", insertgroupdtldt);

            if(upgroupdt.Rows.Count>0)
                importDt.UpdateDbFromDt("T_BD_UserGroup", upgroupdt);
            if (upgroupdtldt.Rows.Count>0)
                importDt.UpdateDbFromDt("T_BD_UserGroupDtl", upgroupdtldt);
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

        /// <summary>
        /// 删除相关记录-(基础资料:用户组别使用)
        /// </summary>
        /// <param name="id">0:表头删除 1:表体删除</param>
        /// <param name="deldt"></param>
        private void DelRecord(int id,DataTable deldt)
        {
            var fidlist = string.Empty;

            foreach (DataRow rows in deldt.Rows)
            {
                if (string.IsNullOrEmpty(fidlist))
                {
                    fidlist = "'"+Convert.ToString(id == 0 ? rows[0] : rows[1])+"'";
                }
                else
                {
                    fidlist += id == 0 ? "," + "'"+Convert.ToString(rows[0])+"'" : "," + "'"+Convert.ToString(rows[1])+"'";
                }
            }

            searchDt.Generdt(sqlList.DelGroupRecord(id, fidlist));
        }

        #region 用户权限提交

        /// <summary>
        /// 用户权限提交
        /// 注:在操作reluserdt 及 reluserdtldt时,顺序为:先删除后插入=>注:创建状态除外
        /// </summary>
        /// <param name="userdt">用户表</param>
        /// <param name="reluserdt">用户关联表头</param>
        /// <param name="reluserdtldt">用户关联表体</param>
        /// <returns></returns>
        public bool ImportUserPermissionDt(DataTable userdt,DataTable reluserdt,DataTable reluserdtldt)
        {
            var result = true;
            //设置userid变量
            var userid = 0;
            //通过userid是否为0,确定reluserdt 及 reluserdtldt是否需要执行删除操作(0:不需要 1:需要)
            var markid = 0;

            try
            {
                //获取用户权限临时表
                var tempdt = dbList.CreateUserPermissionTemp();

                //判断若sourcedt内的Userid为0,即需要插入操作,反之,为更新操作
                var dtlrowsLength = userdt.Select("Userid=0").Length;
                //根据情况获取userid
                userid = dtlrowsLength > 0 ? GetUseridKey() : Convert.ToInt32(userdt.Rows[0][0]);
                //确定reluserdt 及 reluserdtldt是否需要执行删除操作(0:不需要 1:需要)
                markid = dtlrowsLength > 0 ? 0 : 1;

                //对临时表进行添加操作
                var newrow = tempdt.NewRow();
                newrow[0] = userid;                 //UserId       
                newrow[1] = userdt.Rows[0][1];      //用户名称
                newrow[2] = userdt.Rows[0][2];      //用户密码
                newrow[3] = userdt.Rows[0][3];      //创建人
                newrow[4] = userdt.Rows[0][4];      //创建日期
                newrow[5] = userdt.Rows[0][5];      //是否启用
                newrow[6] = userdt.Rows[0][6];      //能否反审核
                newrow[7] = userdt.Rows[0][7];      //能否查阅明细金额
                newrow[8] = userdt.Rows[0][8];      //能否对明细物料操作
                newrow[9] = userdt.Rows[0][9];      //是否占用
                newrow[10] = userdt.Rows[0][10];    //是否不关联用户
                tempdt.Rows.Add(newrow);

                //执行插入操作
                if (dtlrowsLength > 0)
                {
                    importDt.ImportDtToDb("T_AD_User", tempdt);
                }
                //执行更新操作
                else
                {
                    importDt.UpdateDbFromDt("T_AD_User", tempdt);
                }

                //执行reluserdt 及 reluserdtldt(注:当表内有值时才执行)
                if (reluserdt.Rows.Count > 0)
                {
                    CreateRelUserIntoDb(markid,userid,reluserdt);
                }

                if (reluserdtldt.Rows.Count > 0)
                {
                    CreateRelUserDtlIntoDb(markid,userid,reluserdtldt);
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 创建reluser相关记录并插入至对应的数据表内
        /// </summary>
        /// <param name="markid">确定reluserdt 及 reluserdtldt是否需要执行删除操作(0:不需要 1:需要)</param>
        /// <param name="userid"></param>
        /// <param name="reluserdt"></param>
        private void CreateRelUserIntoDb(int markid,int userid,DataTable reluserdt)
        {
            var tb = reluserdt.Clone();

            if (markid == 0)
            {
                foreach (DataRow rows in reluserdt.Rows)
                {
                    var newrow = tb.NewRow();
                    newrow[0] = userid;         //Userid
                    newrow[1] = rows[1];        //GroupID
                    newrow[2] = rows[2];        //CreateDt
                    tb.Rows.Add(newrow);
                }
            }
            //先删除,后插入
            else
            {
                searchDt.Generdt(sqlList.DelRelUser(userid));
            }
            importDt.ImportDtToDb("T_AD_RelUser", reluserdt);
        }

        /// <summary>
        /// 创建reluserdtldt相关记录并插入至对应的数据表内
        /// </summary>
        /// <param name="markid">确定reluserdt 及 reluserdtldt是否需要执行删除操作(0:不需要 1:需要)</param>
        /// <param name="userid"></param>
        /// <param name="reluserdtldt"></param>
        private void CreateRelUserDtlIntoDb(int markid, int userid,DataTable reluserdtldt)
        {
            var tb = reluserdtldt.Clone();

            //需将userid重新添加至对应的项内
            if (markid == 0)
            {
                foreach (DataRow rows in reluserdtldt.Rows)
                {
                    var newrow = tb.NewRow();
                    newrow[0] = userid;     //Userid
                    newrow[1] = rows[1];    //Groupid
                    newrow[2] = rows[2];    //Dtlid
                    newrow[3] = rows[3];    //CreateDt
                }
            }
            //先删除,后插入
            else
            {
                searchDt.Generdt(sqlList.DelRelUserDtl(userid));
            }
            importDt.ImportDtToDb("T_AD_RelUserDtl", reluserdtldt);
        }

        /// <summary>
        /// 获取最新的Userid值
        /// </summary>
        /// <returns></returns>
        private int GetUseridKey()
        {
            return generateDt.GetNewUseridValue();
        }

        #endregion

    }
}
