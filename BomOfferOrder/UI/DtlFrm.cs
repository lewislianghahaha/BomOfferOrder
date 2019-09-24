using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using BomOfferOrder.DB;
using BomOfferOrder.Task;

namespace BomOfferOrder.UI
{
    public partial class DtlFrm : Form
    {
        DbList dbList=new DbList();
        TaskLogic task=new TaskLogic();
        Load load=new Load();

        #region 参数定义
            //单据状态标记(作用:记录打开此功能窗体时是 读取记录 还是 创建记录) C:创建 R:读取
            private string _funState;
            //记录审核状态(True:已审核;False:没审核)
            private bool _confirmMarkId;
            //记录单据是否占用情况 占用:true 末占用:false
            private bool _useid = false;

            //收集TabControl内各Tab Pages的内容
            private DataTable _bomdt;
            //收集TabContol内各Tab Pages内的GridView要删除的内容(注:单据状态为R时使用)
            private DataTable _deldt;

            //记录读取过来的FID值
            private int _fid;
            //记录读取过来的创建人信息
            private string _creatname;
            //记录读取过来的创建日期信息
            private DateTime _createtime;
        #endregion

        #region Set

        /// <summary>
        /// 获取单据状态标记ID C:创建 R:读取
        /// </summary>
        public string FunState { set { _funState = value; } }

        /// <summary>
        /// 记录审核状态(True:已审核;False:没审核)
        /// </summary>
        public bool ConfirmMarkid { set { _confirmMarkId = value; } }

        /// <summary>
        /// 记录单据是否占用情况 占用:true 末占用:false
        /// </summary>
        public bool Useid { set { _useid = value; } }

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
                _confirmMarkId = false;
                CreateDetail(bomdt, materialdt);
            }
            //单据状态:读取 R
            else
            {
                //根据bomdt判断,若rows[2]=0为:已审核 1:反审核
                _confirmMarkId = Convert.ToInt32(bomdt.Rows[0][2])==0;
                ReadDetail(bomdt, materialdt);
            }
            //权限控制
            PrivilegeControl();
        }

        /// <summary>
        /// 创建时使用
        /// </summary>
        void CreateDetail(DataTable sourcedt,DataTable materialdt)
        {
            //获取临时表
            var dt = dbList.MakeTemp();
            //设置TabName
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
        /// <param name="sourcedt">数据源DT</param>
        /// <param name="materialdt">原材料物料DT</param>
        void ReadDetail(DataTable sourcedt, DataTable materialdt)
        {
            //创建产成品名称临时表
            var bomproductorderdt = dbList.CreateBomProductTemp();
            //获取临时表
            var bomdtldt = dbList.GetBomDtlTemp();

            try
            {
                //过滤得出不相同的‘产品名称’临时表
                foreach (DataRow rows in sourcedt.Rows)
                {
                    if (bomproductorderdt.Select("ProductName='" + rows[7] + "'").Length > 0) continue;
                    var newrow = bomproductorderdt.NewRow();
                    newrow[0] = rows[7];
                    bomproductorderdt.Rows.Add(newrow);
                }
                //再根据bomproductorderdt临时表进行循环
                foreach (DataRow rows in bomproductorderdt.Rows)
                {
                    var tabname = Convert.ToString(rows[0]);
                    var dtlrows = sourcedt.Select("ProductName='" + rows[0] + "'");
                    for (var i = 0; i < dtlrows.Length; i++)
                    {
                        //当‘OA流水号’为空时,就将第一行的值赋给以下对应的变量内
                        if (txtbom.Text == "")
                        {
                            txtbom.Text = dtlrows[i][1].ToString();
                            _fid = Convert.ToInt32(dtlrows[i][0]);
                            _creatname = Convert.ToString(dtlrows[i][5]);
                            _createtime = Convert.ToDateTime(dtlrows[i][4]);
                        }
                        //将记录赋值给bomdtldt内
                        var newrow = bomdtldt.NewRow();
                        newrow[8] = dtlrows[i][6];                 //Headid
                        newrow[9] = dtlrows[i][7];                 //产品名称
                        newrow[10] = dtlrows[i][8];                //包装规格
                        newrow[11] = dtlrows[i][9];                //产品密度(KG/L)
                        newrow[12] = dtlrows[i][10];                //材料成本(不含税)
                        newrow[13] = dtlrows[i][11];               //包装成本
                        newrow[14] = dtlrows[i][12];               //人工及制造费用
                        newrow[15] = dtlrows[i][13];               //成本(元/KG)
                        newrow[16] = dtlrows[i][14];               //成本(元/L)
                        newrow[17] = dtlrows[i][15];               //50%报价
                        newrow[18] = dtlrows[i][16];               //45%报价
                        newrow[19] = dtlrows[i][17];               //40%报价
                        newrow[20] = dtlrows[i][18];               //备注
                        newrow[21] = dtlrows[i][19];               //对应BOM版本编号
                        newrow[22] = dtlrows[i][20];               //物料单价

                        newrow[23] = dtlrows[i][21];               //Entryid
                        newrow[24] = dtlrows[i][22];               //物料编码ID
                        newrow[25] = dtlrows[i][23];               //物料编码
                        newrow[26] = dtlrows[i][24];               //物料名称
                        newrow[27] = dtlrows[i][25];               //配方用量
                        newrow[28] = dtlrows[i][26];               //物料单价(含税)
                        newrow[29] = dtlrows[i][27];               //物料成本(含税)
                        bomdtldt.Rows.Add(newrow);
                    }
                    //将其作为数据源生成Tab Page及ShowDetailFrm
                    CreateDetailFrm(tabname, bomdtldt, materialdt);
                    //当生成完成后将bomdtldtdt清空内容,待下一次使用
                    bomdtldt.Rows.Clear();
                }
                //最后将‘OA流水号’设置为只读
                txtbom.ReadOnly = true;
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
                var clickMessage = $"您所选择的信息为:\n 单据名称:{txtbom.Text} \n 是否继续? \n 注:审核后需反审核才能对该单据的记录进行修改, \n 请谨慎处理.";
                if (MessageBox.Show(clickMessage, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    //获取BOM报价单临时表
                    _bomdt = dbList.GetBomDtlTemp();
                    //获取需要删除的DT临时表
                    _deldt = dbList.MakeGridViewTemp();

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
                                newrow[0] = _funState == "C" ? 0: _fid;                                     //Fid
                                newrow[1] = txtbom.Text;                                                    //流水号
                                newrow[2] = 0;                                                              //单据状态(0:已审核 1:反审核)
                                newrow[3] = DateTime.Now;                                                   //审核日期
                                newrow[4] = _funState =="C" ? DateTime.Now : _createtime;                   //创建日期
                                newrow[5] = _funState == "C" ? GlobalClasscs.User.StrUsrName : _creatname;  //创建人
                                newrow[6] = 0;                                                              //记录当前单据使用标记(0:正在使用 1:没有使用)
                                newrow[7] = GlobalClasscs.User.StrUsrName;                                  //记录当前单据使用者名称信息

                                newrow[8] = _funState == "C" ? 0 : showdetail.Headid;                       //Headid
                                newrow[9] = showdetail.txtname.Text;                                        //产品名称(物料名称)
                                newrow[10] = showdetail.txtbao.Text;                                        //包装规格
                                newrow[11] = Convert.ToDecimal(showdetail.txtmi.Text);                      //产品密度(KG/L)
                                newrow[12] = Convert.ToDecimal(showdetail.txtmaterial.Text);                //材料成本(不含税)
                                newrow[13] = Convert.ToDecimal(showdetail.txtbaochenben.Text);              //包装成本
                                newrow[14] = Convert.ToDecimal(showdetail.txtren.Text);                     //人工及制造费用
                                newrow[15] = Convert.ToDecimal(showdetail.txtkg.Text);                      //成本(元/KG)
                                newrow[16] = Convert.ToDecimal(showdetail.txtl.Text);                       //成本(元/L)
                                newrow[17] = Convert.ToDecimal(showdetail.txt50.Text);                      //50%报价
                                newrow[18] = Convert.ToDecimal(showdetail.txt45.Text);                      //45%报价
                                newrow[19] = Convert.ToDecimal(showdetail.txt40.Text);                      //40%报价
                                newrow[20] = showdetail.txtremark.Text;                                     //备注
                                newrow[21] = showdetail.txtbom.Text;                                        //对应BOM版本编号
                                newrow[22] = Convert.ToDecimal(showdetail.txtprice.Text);                   //物料单价
                                 
                                newrow[23] = rows[0];                                                       //Entryid
                                newrow[24] = rows[1];                                                       //物料编码ID
                                newrow[25] = rows[2];                                                       //物料编码
                                newrow[26] = rows[3];                                                       //物料名称
                                newrow[27] = rows[4];                                                       //配方用量
                                newrow[28] = rows[5];                                                       //物料单价(含税)
                                newrow[29] = rows[6];                                                       //物料成本(含税)
                                _bomdt.Rows.Add(newrow);
                            }
                            //将各TabPages内GridView中的需要进行删除的记录合并整理
                            if (_funState == "R" && showdetail.Deldt.Rows.Count>0)
                            {
                                _deldt.Merge(showdetail.Deldt);
                            }
                        }
                    }

                    //获取后进行检测是否填写正确(正常:1)关闭整体窗体不能修改 2)显示审核图标 异常:跳出错误信息) 注:CheckDetail方法只在单据状态为‘创建’时执行
                    if (!CheckDetail()) throw new Exception($"审核不通过,原因:OA流水号为空或已存在, \n 请检查是否填写正确,再进行审核");
                    else
                    {
                        //审核成功后操作 =>1)审核图片显示 2)将控件设为不可修改 3)弹出成功信息窗体 4)将_confirmMarkid标记设为True
                        MessageBox.Show($"审核成功,请进行提交操作", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _confirmMarkId = true;
                        //权限控制
                        PrivilegeControl();
                    }
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
                if (!_confirmMarkId) throw new Exception("请先点击‘审核’再继续");

                task.TaskId = "2";
                task.Importdt = _bomdt;
                task.FunState = _funState;
                task.Deldt = _deldt;

                new Thread(Start).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();

                if(!task.ResultMark)throw new Exception("提交异常,请联系管理员");
                else
                {
                    MessageBox.Show($"单据'{txtbom.Text}'提交成功,可关闭此单据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tmsave.Enabled = false;
                }
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
            var clickMessage = !_confirmMarkId ? $"提示:单据'{txtbom.Text}'没提交, \n 其记录退出后将会消失,是否确定退出?" : $"是否退出?";

            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                var result = MessageBox.Show(clickMessage, "提示", MessageBoxButtons.YesNo,MessageBoxIcon.Information);
                //当点击"OK"按钮时执行以下操作
                if (result == DialogResult.Yes)
                {
                    //TODO 预留:当退出时,清空useid等相关占用信息

                    //在关闭时将TabControl已存在的Tab Pages删除(注:需倒序循环进行删除)
                    for (var i = tctotalpage.TabCount - 1; i >= 0; i--)
                    {
                        tctotalpage.TabPages.RemoveAt(i);
                    }
                    //允许窗体关闭
                    e.Cancel = false;
                    //Application.Exit();
                }
                else
                {
                    //将Cancel属性设置为 true 可以“阻止”窗体关闭
                    e.Cancel = true;
                }
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
        /// 根据判断数据是否合法(只在创建状态使用)
        /// </summary>
        /// <returns></returns>
        private bool CheckDetail()
        {
            var result = true;

            //检测:1)'OA流水号'是否填写 2)此流水号是否存在
            if (_funState == "C")
            {
                task.TaskId = "0.3";
                task.Oaorder = txtbom.Text;
                task.StartTask();
                result = txtbom.Text != "" && !task.ResultMark;
            }
            return result;
        }

        /// <summary>
        /// 权限控制
        /// </summary>
        private void PrivilegeControl()
        {
            //注:查看该单据是否让其它用户占用=>true:占用 false:未占用
            //if (!_useid)
            //{
                //若为“审核”状态的话，就执行以下语句
                if (_confirmMarkId)
                {
                    //加载图片
                    pbimg.Visible = true;
                    pbimg.BackgroundImage = Image.FromFile(Application.StartupPath + @"\PIC\1.png");
                    //对相关控件设为不可改或只读
                    txtbom.ReadOnly = true;
                    //循环TabPages内的控件将其设为只读
                    ControlTabPagesReadOnly();

                    if (_funState == "R")
                    {
                        tmConfirm.Enabled = false;
                        tmsave.Enabled = false;
                        //todo:添加是否显示金额明细权限控制 _readid

                    }
                    else
                    {
                        tmConfirm.Enabled = false;
                    }
                }
                //若为“非审核”状态的,就执行以下语句
                else
                {
                    pbimg.Visible = false;
                    txtbom.ReadOnly = false;
                    tmsave.Enabled = true;
                    tmConfirm.Enabled = true;
                    //todo:添加是否显示金额明细权限控制 _readid

                }
           // }
            //else
            //{
            //    lblmessage.Text = $"该单据已被用户'{GlobalClasscs.User.StrUsrName}'占用,故只能只读";
            //    lblmessage.ForeColor=Color.DarkRed;
            //    txtbom.ReadOnly = true;
            //    tmConfirm.Enabled = false;
            //    tmsave.Enabled = false;
            //    ControlTabPagesReadOnly();
            //}
        }

        /// <summary>
        /// 循环控制TabPages内的控件只读
        /// </summary>
        void ControlTabPagesReadOnly()
        {
            for (var i = 0; i < tctotalpage.TabCount; i++)
            {
                //循环获取TabPages内各页的内容并进行相关设置
                var showdetail = tctotalpage.TabPages[i].Controls[0] as ShowDetailFrm;
                if (showdetail != null)
                {
                    showdetail.txtren.ReadOnly = true;
                    showdetail.gvdtl.ReadOnly = true;
                    showdetail.tmReplace.Visible = false;
                    showdetail.ts1.Visible = false;
                    showdetail.tmAdd.Visible = false;
                    showdetail.ts2.Visible = false;
                    showdetail.tmdel.Visible = false;
                }
            }
        }

        /// <summary>
        ///子线程使用(重:用于监视功能调用情况,当完成时进行关闭LoadForm)
        /// </summary>
        private void Start()
        {
            task.StartTask();

            //当完成后将Form2子窗体关闭
            this.Invoke((ThreadStart)(() =>
            {
                load.Close();
            }));
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
