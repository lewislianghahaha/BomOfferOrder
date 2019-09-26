namespace BomOfferOrder
{
    public class GlobalClasscs
    {
        public struct Account
        {
            public string StrUsrName;    //获取帐号名称
            public string StrUsrpwd;     //获取帐号密码
            public bool Canbackconfirm;  //获取权限标记-能否反审核
            public bool Readid;          //获取权限标记-能否读取TabPages内GridView控件下的明细金额
            public bool Addid; //获取权限标记-能否对明细物料操作(包括:新增 替换 以及删除)
        };

        public static Account User;
    }
}
