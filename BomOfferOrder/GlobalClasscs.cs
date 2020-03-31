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

        public static Account User;

        public static FunName Fun;
    }
}
