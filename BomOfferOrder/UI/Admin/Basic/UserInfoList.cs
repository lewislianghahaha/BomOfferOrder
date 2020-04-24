using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BomOfferOrder.DB;

namespace BomOfferOrder.UI.Admin.Basic
{
    public partial class UserInfoList : Form
    {
        DbList dbList=new DbList();

        #region 参数
        //收集从GridView获取的DT
        private DataTable _resultTable;
        #endregion

        #region Get
        /// <summary>
        /// 返回DT
        /// </summary>
        public DataTable ResultTable => _resultTable;
        #endregion

        public UserInfoList()
        {
            InitializeComponent();
            OnRegisterEvents();
        }

        private void OnRegisterEvents()
        {
            tmGet.Click += TmGet_Click;
            tmClose.Click += TmClose_Click;
            tctotalpage.DrawItem += Tctotalpage_DrawItem;
        }

        /// <summary>
        /// 将K3用户DT放到GetAccountFrm.cs进行调用显示
        /// </summary>
        /// <param name="dt"></param>
        public void OnInitialize(DataTable dt)
        {
            //将_resultTable初始化(若_resultTable有值时)
            if (_resultTable?.Rows.Count > 0) 
            {
                _resultTable.Rows.Clear();
                _resultTable.Columns.Clear();
            }

            var newpage = new TabPage { Text = $"K3用户信息列表"};

            var getAccount = new GetAccountFrm
            {
                TopLevel = false,                      //重点(注:没有这个设置,会出现异常)
                BackColor = Color.White,              
                Dock = DockStyle.Fill,                 //将子窗体完全停靠new tab page
                FormBorderStyle = FormBorderStyle.None 
            };
            getAccount.OnInitialize(1,dt);               
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
        /// 获取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmGet_Click(object sender, EventArgs e)
        {
            try
            {
                //设置临时表
                _resultTable = dbList.K3UserDtTemp();
                //循环获取TabPages内各页的内容
                var showdetail = tctotalpage.TabPages[0].Controls[0] as GetAccountFrm;
                if (showdetail != null)
                {
                    var dtldt = (DataGridView)showdetail.gvdtl;
                    //循环将所选择的行添加至_restultTable内
                    foreach (DataGridViewRow rows in dtldt.SelectedRows)
                    {
                        var newrow = _resultTable.NewRow();
                        newrow[0] = rows.Cells[0].Value;  //K3用户名称
                        newrow[1] = rows.Cells[1].Value;  //K3用户组别
                        newrow[2] = rows.Cells[2].Value;  //K3用户手机
                        _resultTable.Rows.Add(newrow);
                    }
                }
                //在关闭时将TabControl已存在的Tab Pages删除(注:需倒序循环进行删除)
                for (var i = tctotalpage.TabCount - 1; i >= 0; i--)
                {
                    tctotalpage.TabPages.RemoveAt(i);
                }
                //完成后关闭窗体
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmClose_Click(object sender, EventArgs e)
        {
            //在关闭时将TabControl已存在的Tab Pages删除(注:需倒序循环进行删除)
            for (var i = tctotalpage.TabCount - 1; i >= 0; i--)
            {
                tctotalpage.TabPages.RemoveAt(i);
            }
            this.Close();
        }
    }
}
