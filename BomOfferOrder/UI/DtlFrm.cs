using System;
using System.Drawing;
using System.Windows.Forms;

namespace BomOfferOrder.UI
{
    public partial class DtlFrm : Form
    {


        public DtlFrm()
        {
            InitializeComponent();
            OnRegisterEvents();
            tmsave.Click += Tmsave_Click;
        }

        private void OnRegisterEvents()
        {
            for (var i = 0; i < 11; i++)
            {
                var newpagename = $"测试{i}";
                Adddetail(i, newpagename);
            }
        }

        void Adddetail(int id,string name)
        {
            var newpage=new TabPage(Text=$"{name}");
            tctotalpage.Controls.Add(newpage); //将新创建的Tab Page添加至TabControl控件内

            var showDetail = new ShowDetailFrm
            {
                TopLevel = false,              //重点(注:没有这个设置,会出现异常)
                BackColor = Color.White,       
                Dock = DockStyle.Fill,         //将子窗体完全停靠new tab page
                FormBorderStyle = FormBorderStyle.None
            };
            //对ShowDetailFrm赋值
            showDetail.AddDbToFrm(id);
            showDetail.Show();                 //只能使用Show()
            newpage.Controls.Add(showDetail);  //将窗体控件加入至新创建的Tab Page内
            tctotalpage.SelectedIndex = 0;     //必须要指定当前页是首页或某一页
            // tctotalpage.SelectedTab = newpage;
        }

        /// <summary>
        /// 初始化记录
        /// </summary>
        public void OnInitialize()
        {
            
        }

        private void Tmsave_Click(object sender, System.EventArgs e)
        {
            ShowDetailFrm showDetail=new ShowDetailFrm();

            
            MessageBox.Show(Convert.ToString(a), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }
}
