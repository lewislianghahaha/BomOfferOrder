using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BomOfferOrder.Task;

namespace BomOfferOrder.UI
{
    public partial class Login : Form
    {
        TaskLogic task=new TaskLogic();

        #region 变量参数
        /// <summary>
        /// 获取T_AD_USER DT
        /// </summary>
        private DataTable _userdt;
        #endregion

        public Login()
        {
            InitializeComponent();
            OnInitialize();
            OnRegisterEvents();
        }

        /// <summary>
        /// 初始化相关信息
        /// </summary>
        private void OnInitialize()
        {
            //加载图片
            pbimg.BackgroundImage = Image.FromFile(Application.StartupPath + @"\PIC\2.png");
            //加载T_AD_USER数据表内容
            _userdt = GetUserDt();
        }

        private void OnRegisterEvents()
        {
            btnlogin.Click += Btnlogin_Click;
            btnlogout.Click += Btnlogout_Click;
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btnlogin_Click(object sender, EventArgs e)
        {
            try
            {
                if(txtname.Text=="" || txtpwd.Text=="") throw new Exception("请输入用户名称及用户密码进行登录");
                //登入用户权限窗体
                if (txtname.Text == $"Admin" && txtpwd.Text == $"Yatu8888!")
                {
                    GlobalClasscs.User.StrUsrName = "Admin";
                }
                //普通用户进入
                else
                {
                    //判断所填写的用户及密码是否正确
                    if(_userdt.Select("UserName='" + txtname.Text + "' and UserPwd='" +txtpwd.Text + "'").Length==0)
                        throw new Exception($"登入用户'{txtname.Text}' 不能进入, \n 原因:该用户没建立或密码不正确,请确定后继续");

                    //判断此用户是否为‘已启用’状态
                    var applyrows = _userdt.Select("UserName='" + txtname.Text + "' and ApplyId=0");
                    if(applyrows.Length == 0)throw new Exception($"登入用户'{txtname.Text}'不能进入, \n 原因:该用户没有设置启用,请联系管理员");

                    //判断此用户是否已在其它地方登录(若Useid=0 表示已使用此用户打开) 动态查询判断
                    var userInfodt = SearchAdminUserInfo(Convert.ToInt32(applyrows[0][0]));
                    if(Convert.ToInt32(userInfodt.Rows[0][0])==0)throw new Exception($"登入用户'{txtname.Text}'不能进入, \n 原因:已打开或在其它地方登录,故不能再次登入, \n 请先退出该用户,再进入");

                    //若上面两步通过就将相关信息保存至结构类内
                    GlobalClasscs.User.UserId = Convert.ToInt32(applyrows[0][0]);
                    GlobalClasscs.User.StrUsrName = txtname.Text;
                    GlobalClasscs.User.StrUsrpwd = txtpwd.Text;
                    GlobalClasscs.User.Canbackconfirm = Convert.ToInt32(applyrows[0][4]) == 0;
                    GlobalClasscs.User.Readid = Convert.ToInt32(applyrows[0][5]) == 0;
                    GlobalClasscs.User.Addid = Convert.ToInt32(applyrows[0][6]) == 0;
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //txtname.Text = "";
                txtpwd.Text = "";
            } 
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btnlogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 初始化获取T_AD_User表DT
        /// </summary>
        /// <returns></returns>
        private DataTable GetUserDt()
        {
            task.TaskId = "0.9.1";
            task.StartTask();
            return task.ResultTable;
        }

        /// <summary>
        /// 检测该用户是否占用
        /// </summary>
        /// <returns></returns>
        private DataTable SearchAdminUserInfo(int userid)
        {
            task.TaskId = "0.9.2";
            task.Fid = userid;
            task.StartTask();
            return task.ResultTable;
        }

    }
}
