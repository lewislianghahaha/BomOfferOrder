using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BomOfferOrder.DB;
using BomOfferOrder.Task;

namespace BomOfferOrder.UI
{
    public partial class DtlFrm : Form
    {
        DbList dbList=new DbList();
        TaskLogic task=new TaskLogic();

        #region 参数定义
            //单据状态标记(作用:记录打开此功能窗体时是 读取记录 还是 创建记录) C:创建 R:读取
            private string _funState;
            //记录是否已点击‘审核’按钮(默认:没点击)
            private bool _click=false;
            //记录审核状态(Y:已审核;N:没审核)
            private bool _confirmMarkId=false;
            //收集TabControl内各Tab Pages的内容
            private DataTable _bomdt;
            //记录读取过来的FID值
            private int _fid=0;
            //记录读取过来的Headid值
            private int _headid=0;
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
            tmConfirm.Click += TmConfirm_Click;
            tmsave.Click += Tmsave_Click;
            this.FormClosing += DtlFrm_FormClosing;
        }

        /// <summary>
        /// 初始化记录
        /// </summary>
        public void OnInitialize(DataTable bomdt)
        {
            //初始化获取‘原材料’物料明细信息(注:添加物料明细窗体使用)
            var materialdt = OnInitializeMaterialDt();

            //单据状态:创建 C
            if (_funState=="C")
            {
                CreateDetail(bomdt, materialdt);
            }
            //单据状态:读取 R
            else
            {
                ReadDetail(bomdt, materialdt);
            }
        }

        /// <summary>
        /// 创建时使用
        /// </summary>
        void CreateDetail(DataTable sourcedt,DataTable materialdt)
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
                            newrow[10] = t[10];         //物料单价
                            newrow[11] = t[11];         //表头物料单价
                            dt.Rows.Add(newrow);
                        }
                        //当循环完一个DT的时候,将其作为数据源生成Tab Page及ShowDetailFrm
                        CreateDetailFrm(tabname,dt,materialdt);
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
        void ReadDetail(DataTable sourcedt, DataTable materialdt)
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
        void CreateDetailFrm(string tabname,DataTable dt,DataTable materialdt)
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
            showDetailFrm.AddDbToFrm(_funState,dt,materialdt);
            showDetailFrm.Show();                   //只能使用Show()
            newpage.Controls.Add(showDetailFrm);    //将窗体控件加入至新创建的Tab Page内
            tctotalpage.TabPages.Add(newpage);      //将新创建的Tab Page添加至TabControl控件内
            tctotalpage.SelectedIndex = 0;          //必须要指定当前页是首页或某一页
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                //if(!_confirmMarkId)throw new Exception("此单据已审核,请先反审核再继续");
                //获取BOM报价单临时表
                _bomdt = dbList.GetBomDtlTemp();
                //通过循环获取TagePages各页的值并最后整合到bomdt内
                for (var i = 0; i < tctotalpage.TabCount; i++)
                {
                    //循环获取TabPages内各页的内容
                    var showdetail = tctotalpage.TabPages[i].Controls[0] as ShowDetailFrm;
                    if (showdetail != null)
                    {
                        var bomdtldt = (DataTable)showdetail.gvdtl.DataSource;
                        //根据明细项循环获取其内部值
                        foreach (DataRow rows in bomdtldt.Rows)
                        {
                            var newrow = _bomdt.NewRow();
                            newrow[0] = _fid;                                               //fid
                            newrow[1] = txtbom.Text;                                        //流水号
                            newrow[2] = 0;                                                  //单据状态(0:已审核 1:反审核)
                            newrow[3] = DateTime.Now.Date;                                  //创建日期
                            newrow[4] = 0;                                                  //记录当前单据使用标记(0:正在使用 1:没有使用)
                            newrow[5] = DateTime.Now.Date;                                  //记录当前单据使用者信息

                            newrow[6] = _headid;                                            //Headid
                            newrow[7] = showdetail.txtname.Text;                            //产品名称(物料名称)
                            newrow[8] = showdetail.txtbao.Text;                             //包装规格
                            newrow[9] = Convert.ToDecimal(showdetail.txtmi.Text);           //产品密度(KG/L)
                            newrow[10] = Convert.ToDecimal(showdetail.txtmaterial.Text);    //材料成本(不含税)
                            newrow[11] = Convert.ToDecimal(showdetail.txtbaochenben.Text);  //包装成本
                            newrow[12] = Convert.ToDecimal(showdetail.txtren.Text);         //人工及制造费用
                            newrow[13] = Convert.ToDecimal(showdetail.txtkg.Text);          //成本(元/KG)
                            newrow[14] = Convert.ToDecimal(showdetail.txtl.Text);           //成本(元/L)
                            newrow[15] = Convert.ToDecimal(showdetail.txt50.Text);          //50%报价
                            newrow[16] = Convert.ToDecimal(showdetail.txt45.Text);          //45%报价
                            newrow[17] = Convert.ToDecimal(showdetail.txt40.Text);          //40%报价
                            newrow[18] = showdetail.txtremark.Text;                         //备注
                            newrow[19] = showdetail.txtbom.Text;                            //对应BOM版本编号
                            newrow[20] = Convert.ToDecimal(showdetail.txtprice.Text);       //物料单价

                            newrow[21] = rows[0];                                           //Entryid
                            newrow[22] = rows[1];                                           //物料编码ID
                            newrow[23] = rows[2];                                           //物料编码
                            newrow[24] = rows[3];                                           //物料名称
                            newrow[25] = rows[4];                                           //配方用量
                            newrow[26] = rows[5];                                           //物料单价(含税)
                            newrow[27] = rows[6];                                           //物料成本(含税)
                            _bomdt.Rows.Add(newrow);
                        }
                    }
                }
                var a = _bomdt;
                //获取后进行检测是否填写正确(正常:1)关闭整体窗体不能修改 2)显示审核图标 异常:跳出错误信息)
                if(!CheckDetail(_bomdt)) throw new Exception($"审核不通过,原因:检测到要提交的单据有异常状态出现, \n 请检查单据是否填写正确,再进行审核");
                else
                {
                    //审核成功后操作

                }
            }
            catch (Exception ex)
            {
                var exmessage = ex.Message.Contains("输入字符串的格式不正确") ? $"'人工及制造费用'项必须填写数字,请重新填写再继续" : ex.Message;
                MessageBox.Show(exmessage, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmsave_Click(object sender, EventArgs e)
        {
            try
            {
                //判断若没有完成审核,即不能执行
                if (!_click) throw new Exception("请先点击‘审核’再继续");
                var a = _bomdt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        /// <summary>
        /// 初始化原材料物料DT
        /// </summary>
        /// <returns></returns>
        private DataTable OnInitializeMaterialDt()
        {
            task.TaskId = "0.2";
            task.SearchId = 0;
            task.StartTask();
            return task.ResultTable;
        }

        /// <summary>
        /// 根据dt条件判断其内是否合法
        /// </summary>
        /// <param name="bomdt"></param>
        /// <returns></returns>
        private bool CheckDetail(DataTable bomdt)
        {
            var result = true;
            foreach (DataRow rows in bomdt.Rows)
            {
                //检测:1)'OA流水号'是否填写
                if (rows[1].ToString() == "" /*|| rows[21] == DBNull.Value*/)
                    result = false;
                break;
            }
            return result;
        }

        #region 获取Tabpages方法
        //这里表示获取第一个选项卡中的第一个控件集合(注:要访问Form的内部成员,其内部的控件中的Modifiers需设置为Public才可以访问)
        //var currentpage = tctotalpage.SelectedIndex;
        //string b = string.Empty;
        //for (int i = 0; i < 11; i++)
        //{
        //    var a = tctotalpage.TabPages[i].Controls[0] as ShowDetailFrm;
        //    //b += Convert.ToInt32(a.textBox1.Text);
        //    b =a.textBox3.Text;
        //}
        //var a = tctotalpage.TabPages[tctotalpage.SelectedIndex].Controls[0] as ShowDetailFrm;
        //b = a.textBox12.Text;
        //MessageBox.Show(b, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        #endregion

    }
}
