using System;
using System.Drawing;
using System.Windows.Forms;

namespace BomOfferOrder.UI
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            OnInitialize();
            OnRegisterEvents();
        }

        private void OnInitialize()
        {
            //加载图片
            pbimg.BackgroundImage = Image.FromFile(Application.StartupPath + @"\PIC\2.png");
            //

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
            GlobalClasscs.User.Canbackconfirm = true;
            GlobalClasscs.User.Readid = false;

            Main main = new Main();
            main.ShowDialog();
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
