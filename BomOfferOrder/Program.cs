using System;
using System.Windows.Forms;
using BomOfferOrder.UI;
using BomOfferOrder.UI.Admin;

namespace BomOfferOrder
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Main());

            var mutex = new System.Threading.Mutex(false, "ThisOnlyRunOnce");

            //主要用这个来监控是否第一次打开登录窗体,若不是的话,就提示"程序已启动"
            var running = !mutex.WaitOne(0, false);

            if (!running)
            {
                var login = new Login();

                if (DialogResult.Cancel == login.ShowDialog())
                {
                    return;
                }

                //判断若输入的帐号为Admin,即进入权限窗体;反之进入Main窗体
                if (GlobalClasscs.User.StrUsrName == "Admin")
                {
                    PermissionFrm permission=new PermissionFrm();
                    permission.ShowDialog();
                }
                else
                {
                    var main=new Main();
                    main.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("程序已启动.");
            }
        }
    }
}
