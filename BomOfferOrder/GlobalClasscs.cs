using System.Data;

namespace BomOfferOrder
{
    public class GlobalClasscs
    {
        public struct Account
        {
            public int UserId;           //获取帐号ID
            public string StrUsrName;    //获取帐号名称
            public string StrUsrpwd;     //获取帐号密码
            public bool Canbackconfirm;  //获取权限标记-能否反审核
            public bool Readid;          //获取权限标记-能否读取TabPages内GridView控件下的明细金额
            public bool Addid;           //获取权限标记-能否对明细物料操作(包括:新增 替换 以及删除)
        };

        //单据标记说明:N=>新产品报价单 B=>BOM成本报价单 E=>空白报价单 RF=>暂存单据
        public struct FunName
        {
            public string FunctionName;
            public string EmptyFunctionName;   //空白报价单功能使用
            public string RfFunctionName;      //暂存单据使用
        }

        //记录用户权限相关信息
        public struct Admin
        {
            public DataTable UserGroupDt;      //获取用户组别表头DT
            public DataTable UserGroupDtlDt;   //获取用户组别表体DT
            public DataTable RelUserDt;        //获取关联用户表头DT
            public DataTable RelUserDtlDt;     //获取关联用户表体DT
            public DataTable DevGroupDt;       //获取研发类别DT
        }

        /// <summary>
        /// 记录各初始化的数据表
        /// </summary>
        public struct Bddt
        {
            public DataTable Bomdt;            //初始化BOM明细DT(全部Bom内容)
        }


        public static Account User;

        public static FunName Fun;

        public static Admin Ad;

        public static Bddt Bd;
    }
}
