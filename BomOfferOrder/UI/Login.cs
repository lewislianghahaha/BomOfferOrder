using System;
using System.Drawing;
using System.Windows.Forms;
using BomOfferOrder.Task;

namespace BomOfferOrder.UI
{
    public partial class Login : Form
    {
        TaskLogic task=new TaskLogic();

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
                //

                if (txtname.Text == "Admin" && txtpwd.Text == "Yatu8888!")
                {
                    GlobalClasscs.User.StrUsrName = "Admin";
                }


                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtname.Text = "";
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



    }
}
