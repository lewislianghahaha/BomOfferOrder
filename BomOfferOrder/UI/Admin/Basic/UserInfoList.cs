using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace BomOfferOrder.UI.Admin.Basic
{
    public partial class UserInfoList : Form
    {
        #region 参数
        
        #endregion

        public UserInfoList()
        {
            InitializeComponent();
            OnRegisterEvents();
        }

        private void OnRegisterEvents()
        {
            tctotalpage.DrawItem += Tctotalpage_DrawItem;
            this.FormClosing += UserInfoList_FormClosing;
        }

        /// <summary>
        /// 将K3用户DT放到GetAccountFrm.cs进行调用显示
        /// </summary>
        /// <param name="dt"></param>
        public void OnInitialize(DataTable dt)
        {
            var newpage = new TabPage { Text = $"K3用户信息列表"};

            var getAccount = new GetAccountFrm
            {
                TopLevel = false,                      //重点(注:没有这个设置,会出现异常)
                BackColor = Color.White,              
                Dock = DockStyle.Fill,                 //将子窗体完全停靠new tab page
                FormBorderStyle = FormBorderStyle.None 
            };
            getAccount.OnInitialize(dt);               
            getAccount.Show();                         //注:只能使用Show()
            newpage.Controls.Add(getAccount);          //将窗体控件加入至新创建的Tab Page内
            tctotalpage.TabPages.Add(newpage);         //将新创建的Tab Page添加至TabControl控件内
        }



        private void Tctotalpage_DrawItem(object sender, DrawItemEventArgs e)
        {
            //执行顺序:先画一个矩形框,再填充矩形框,最后画关闭符号
            var myTab = tctotalpage.GetTabRect(e.Index);

            //先添加TabPage属性
            var sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center, //设置选项卡名称 垂直居中
                Alignment = StringAlignment.Center      //设置选项卡名称 水平居中 
            };
            e.Graphics.DrawString(tctotalpage.TabPages[e.Index].Text, Font, SystemBrushes.ControlText, myTab, sf);

            e.Graphics.Dispose();
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserInfoList_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                //在关闭时将TabControl已存在的Tab Pages删除(注:需倒序循环进行删除)
                for (var i = tctotalpage.TabCount - 1; i >= 0; i--)
                {
                    tctotalpage.TabPages.RemoveAt(i);
                }
            }
            else
            {
                //将Cancel属性设置为 true 可以“阻止”窗体关闭
                e.Cancel = true;
            }
        }

    }
}
