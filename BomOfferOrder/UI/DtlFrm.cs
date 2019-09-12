using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BomOfferOrder.DB;

namespace BomOfferOrder.UI
{
    public partial class DtlFrm : Form
    {
        DbList dbList=new DbList();

        #region 参数定义

            //单据状态标记(作用:记录打开此功能窗体时是 读取记录 还是 创建记录) C:创建 R:读取
            private string _funState;

        #endregion

        #region Set

            /// <summary>
            /// 获取单据状态标记ID C:创建 R:读取
            /// </summary>
            public string FunState { set { _funState = value; } }

        #endregion

        public DtlFrm()
        {
            InitializeComponent();
            OnRegisterEvents();
        }

        private void OnRegisterEvents()
        {
            tmsave.Click += Tmsave_Click;
            this.FormClosing += DtlFrm_FormClosing;
        }

        /// <summary>
        /// 初始化记录
        /// </summary>
        public void OnInitialize(DataTable materialdt)
        {
            //单据状态:创建 C
            if (_funState=="C")
            {
                CreateDetail(materialdt);
            }
            //单据状态:读取 R
            else
            {
                ReadDetail(materialdt);
            }
        }

        /// <summary>
        /// 创建时使用
        /// </summary>
        void CreateDetail(DataTable sourcedt)
        {
            //获取临时表
            var dt = dbList.MakeTemp();
            //产品名称
            var tabname = string.Empty;

            try
            {
                //将sourcedt根据'ID'列进行拆散sourcedt(从1开始)
                for (var i = 1; i < 11; i++)
                {
                    var dtlrows = sourcedt.Select("ID='" + i + "'");
                    if (dtlrows.Length > 0)
                    {
                        //获取‘产品名称’作为Tab Page名称
                        tabname = Convert.ToString(dtlrows[0][1]);

                        foreach (DataRow t in dtlrows)
                        {
                            var newrow = dt.NewRow();
                            newrow[1] = t[1];           //产品名称
                            newrow[2] = t[2];           //BOM编号 
                            newrow[3] = t[3];           //包装规格 
                            newrow[4] = t[4];           //产品密度 
                            newrow[5] = t[5];           //物料编码ID 
                            newrow[6] = t[6];           //物料编码
                            newrow[7] = t[7];           //物料名称
                            newrow[8] = t[8];           //配方用量
                            dt.Rows.Add(newrow);
                        }
                        //当循环完一个DT的时候,将其作为数据源生成Tab Page及ShowDetailFrm
                        CreateDeatilFrm(tabname,dt);
                        //当生成完成后将dt清空内容,待下一次使用
                        dt.Rows.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 读取记录时使用
        /// </summary>
        void ReadDetail(DataTable sourcedt)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 生成Tab page及对应的ShowDetailFrm
        /// </summary>
        void CreateDeatilFrm(string tabname,DataTable dt)
        {
            var newpage = new TabPage {Text = $"{tabname}"};

            var showDetailFrm = new ShowDetailFrm
            {
                TopLevel = false,                           //重点(注:没有这个设置,会出现异常)
                BackColor = Color.White,
                Dock = DockStyle.Fill,                      //将子窗体完全停靠new tab page
                FormBorderStyle = FormBorderStyle.None
            };
            //对ShowDetailFrm赋值
            showDetailFrm.AddDbToFrm(_funState,dt);
            showDetailFrm.Show();                   //只能使用Show()
            newpage.Controls.Add(showDetailFrm);    //将窗体控件加入至新创建的Tab Page内
            tctotalpage.TabPages.Add(newpage);      //将新创建的Tab Page添加至TabControl控件内
            tctotalpage.SelectedIndex = 0;          //必须要指定当前页是首页或某一页
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmsave_Click(object sender, EventArgs e)
        {
            //这里表示获取第一个选项卡中的第一个控件集合(注:要访问Form的内部成员,其内部的控件中的
            //Modifiers需设置为Public才可以访问)
            //var currentpage = tctotalpage.SelectedIndex;
            string b = string.Empty;
            //for (int i = 0; i < 11; i++)
            //{
            //    var a = tctotalpage.TabPages[i].Controls[0] as ShowDetailFrm;
            //    //b += Convert.ToInt32(a.textBox1.Text);
            //    b =a.textBox3.Text;
            //}
            //var a = tctotalpage.TabPages[tctotalpage.SelectedIndex].Controls[0] as ShowDetailFrm;
            //b = a.textBox12.Text;
            //MessageBox.Show(b, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DtlFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var clickMessage = $"提示:没保存的记录退出后将会消失,是否确定退出?";
            e.Cancel = MessageBox.Show(clickMessage, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes;
            //在关闭时将TabControl已存在的Tab Pages删除(注:需倒序循环进行删除)
            for (var i = tctotalpage.TabCount-1; i >=0 ; i--)
            {
                tctotalpage.TabPages.RemoveAt(i);
            }
            
        }

    }
}
