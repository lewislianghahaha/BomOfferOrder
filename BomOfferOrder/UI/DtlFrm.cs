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
        //反审核标记(注:当需要反审核的R状态单据要进行再次审核时使用)
        private bool _backconfirm;

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
        //(单据类型ID=>0:BOM成本报价单 1:新产品成本报价单及其它单据)
        private int _typeid;

        //获取‘原材料’‘原漆半成品’‘原漆’等物料明细信息(注:添加物料明细窗体使用)
        private DataTable _materialdt;
        //获取‘新产品报价单历史记录’
        private DataTable _historydt;
        //获取K3客户信息
        private DataTable _custinfodt;
        //定义关闭符号的宽
        const int CloseSize = 11;
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
            tmaddpage.Click += Tmaddpage_Click;
            tctotalpage.SelectedIndexChanged += Tctotalpage_SelectedIndexChanged;
            tctotalpage.DrawItem += Tctotalpage_DrawItem;
            tctotalpage.MouseDown += Tctotalpage_MouseDown;
        }

        /// <summary>
        /// 初始化记录
        /// </summary>
        public void OnInitialize(DataTable bomdt)
        {
            //初始化获取‘原材料’‘原漆半成品’‘原漆’等物料明细信息(注:添加物料明细窗体使用)
            _materialdt = OnInitializeMaterialDt();
            //初始化获取‘新产品报价单历史记录’
            _historydt = OnInitializeHistoryDt();
            //初始化获取K3客户信息
            _custinfodt = OnInitializeK3CustinfoDt();

            //单据状态:创建 C
            if (_funState=="C")
            {
                _confirmMarkId = false;
                //对_typeid进行赋值(注:(0:BOM成本报价单 1:新产品成本报价单及其它单据))
                _typeid = GlobalClasscs.Fun.FunctionName == "B" ? 0 : 1;

                //空白报价单-创建使用
                if (bomdt == null)
                {
                    //将‘添加新页’按钮显示
                    tmaddpage.Visible = true;
                    CreateNewProductEmptyDetail();
                }
                else
                {
                    //新产品成本报价单-创建使用
                    if (GlobalClasscs.Fun.FunctionName == "N") 
                    {
                        CreateNewProductDeatail(bomdt);
                    }
                    //成本BOM报价单-创建使用
                    else
                    {
                        CreateBomDetail(bomdt);
                    }
                }
            }
            //单据状态:读取 R
            else
            {
                //初始化反审核标记为false
                _backconfirm = false;
                //根据bomdt判断,若rows[2]=0为:已审核 1:反审核
                _confirmMarkId = Convert.ToInt32(bomdt.Rows[0][2])==0;
                //更新_useid及username值
                UpdateUseValue(Convert.ToInt32(bomdt.Rows[0][0]),0,"");
                //执行读取记录
                ReadDetail(bomdt);
            }
            //权限控制
            PrivilegeControl();
        }

        /// <summary>
        /// 新产品成本报价单-创建使用
        /// </summary>
        private void CreateNewProductDeatail(DataTable sourcedt)
        {
            //获取临时表
            var dt = dbList.MakeTemp();
            //设置TabName
            var tabname = string.Empty;

            try
            {
                //循环sourcedt,并将其相关信息插入至临时表,以及生成TabPages
                foreach (DataRow rows in sourcedt.Rows)
                {
                    var newrow = dt.NewRow();
                    newrow[1] = rows[2];           //产品名称
                    newrow[3] = rows[3];           //包装规格 
                    newrow[4] = rows[4];           //产品密度 
                    dt.Rows.Add(newrow);
                    tabname = Convert.ToString(rows[2]);
                    //当循环完一个DT的时候,将其作为数据源生成Tab Page及ShowDetailFrm
                    CreateDetailFrm(tabname, dt, _materialdt,_historydt,_custinfodt);
                    //当生成完成后将dt清空内容,待下一次使用
                    dt.Rows.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 空白报价单-创建使用
        /// </summary>
        private void CreateNewProductEmptyDetail()
        {
            try
            {
                //生成Tab Page及ShowDetailFrm
                CreateDetailFrm("",null,_materialdt,_historydt,_custinfodt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 成本BOM报价单-创建使用
        /// </summary>
        private void CreateBomDetail(DataTable sourcedt)
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
                            //newrow[11] = t[11];         //表头物料单价
                            dt.Rows.Add(newrow);
                        }
                        //当循环完一个DT的时候,将其作为数据源生成Tab Page及ShowDetailFrm
                        CreateDetailFrm(tabname,dt,_materialdt,_historydt,_custinfodt);
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
        void ReadDetail(DataTable sourcedt)
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
                    if (bomproductorderdt.Select("ProductName='" + rows[8] + "'").Length > 0) continue;
                    var newrow = bomproductorderdt.NewRow();
                    newrow[0] = rows[8];
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
                            txtbom.Text = dtlrows[i][1].ToString();             //OA流水号
                            _fid = Convert.ToInt32(dtlrows[i][0]);              //fid主键
                            _createtime = Convert.ToDateTime(dtlrows[i][3]);    //创建日期
                            _creatname = Convert.ToString(dtlrows[i][5]);       //创建人
                            _typeid = Convert.ToInt32(dtlrows[i][6]);           //单据类型ID=>0:BOM成本报价单 1:新产品成本报价单及其它单据
                        }
                        //将记录赋值给bomdtldt内
                        var newrow = bomdtldt.NewRow();
                        newrow[9] = dtlrows[i][7];                 //Headid
                        newrow[10] = dtlrows[i][8];                //产品名称
                        newrow[11] = dtlrows[i][9];                //包装规格
                        newrow[12] = dtlrows[i][10];               //产品密度(KG/L)
                        newrow[13] = dtlrows[i][11];               //材料成本(不含税)
                        newrow[14] = dtlrows[i][12];               //包装成本
                        newrow[15] = dtlrows[i][13];               //人工及制造费用
                        newrow[16] = dtlrows[i][14];               //成本(元/KG)
                        newrow[17] = dtlrows[i][15];               //成本(元/L)
                        newrow[18] = dtlrows[i][16];               //50%报价
                        newrow[19] = dtlrows[i][17];               //45%报价
                        newrow[20] = dtlrows[i][18];               //40%报价
                        newrow[21] = dtlrows[i][19];               //备注
                        newrow[22] = dtlrows[i][20];               //对应BOM版本编号
                        newrow[23] = dtlrows[i][21];               //产品成本含税(物料单价)
                        newrow[24] = dtlrows[i][22];               //客户名称

                        newrow[25] = dtlrows[i][23];               //Entryid
                        newrow[26] = dtlrows[i][24];               //物料编码ID
                        newrow[27] = dtlrows[i][25];               //物料编码
                        newrow[28] = dtlrows[i][26];               //物料名称
                        newrow[29] = dtlrows[i][27];               //配方用量
                        newrow[30] = dtlrows[i][28];               //占比
                        newrow[31] = dtlrows[i][29];               //物料单价(含税)
                        newrow[32] = dtlrows[i][30];               //物料成本(含税)
                        bomdtldt.Rows.Add(newrow);
                    }
                    //将其作为数据源生成Tab Page及ShowDetailFrm
                    CreateDetailFrm(tabname, bomdtldt, _materialdt,_historydt,_custinfodt);
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
        private void CreateDetailFrm(string tabname,DataTable dt,DataTable materialdt,DataTable historydt,DataTable custinfodt)
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
            showDetailFrm.AddDbToFrm(_funState,dt,materialdt, historydt,custinfodt);
            showDetailFrm.Show();                   //注:只能使用Show()
            newpage.Controls.Add(showDetailFrm);    //将窗体控件加入至新创建的Tab Page内
            tctotalpage.TabPages.Add(newpage);      //将新创建的Tab Page添加至TabControl控件内
            //若使用的功能是‘空白报价单’,就以最后一页为最新页,其它就是首页为最新页
            tctotalpage.SelectedIndex = GlobalClasscs.Fun.EmptyFunctionName == "E" ? 
                                        tctotalpage.TabCount - 1 : 0;
        }

        /// <summary>
        /// 新增新页-空白报价单使用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmaddpage_Click(object sender, EventArgs e)
        {
            try
            {
                //循环已添加的Tabpage,并利用该页的产品名称更新至对应的PAGE.TEXT属性内
                for (var i = 0; i < tctotalpage.TabCount; i++)
                {
                    //循环获取TabPages内各页的内容
                    var showdetail = tctotalpage.TabPages[i].Controls[0] as ShowDetailFrm;
                    if (showdetail != null && showdetail.txtname.Text != "")
                    {
                        tctotalpage.TabPages[i].Text = showdetail.txtname.Text;
                    }
                }
                //执行新增
                var tabname = "新页" + Convert.ToString(tctotalpage.TabCount+1);
                //生成Tab page及对应的ShowDetailFrm
                CreateDetailFrm(tabname, null, _materialdt, _historydt, _custinfodt);
                //权限控制
                PrivilegeControl();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 对TABPAGE页画‘关闭’按钮(注:当TabControl需要绘制它的每一个选项卡时发生,直至跳出整个应用程序)
        /// 注:只有在‘空白报价单’功能时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tctotalpage_DrawItem(object sender, DrawItemEventArgs e)
        {

            //执行顺序:先画一个矩形框,再填充矩形框,最后画关闭符号
            var myTab = tctotalpage.GetTabRect(e.Index);

            //设置当读取至Index=0 即首页时,就不需要画‘X’关闭图标,其它就需要(注:除‘空白报价单’外的功能不需画‘X’)
            if (e.Index == 0 && GlobalClasscs.Fun.EmptyFunctionName != "E")
            {
                //先添加TabPage属性
                var sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center, //设置选项卡名称 垂直居中
                    Alignment = StringAlignment.Center      //设置选项卡名称 水平居中 
                };
                e.Graphics.DrawString(tctotalpage.TabPages[e.Index].Text, Font, SystemBrushes.ControlText, myTab, sf);
            }
            else
            {
                //先添加TabPage属性
                e.Graphics.DrawString(tctotalpage.TabPages[e.Index].Text, Font, SystemBrushes.ControlText, myTab.X + 3, myTab.Y + 3);

                    //再画一个矩形框
                    using (Pen p = new Pen(Color.White))
                    {
                        myTab.Offset((myTab.Width - (CloseSize + 3)), 2);
                        myTab.Width = CloseSize;
                        myTab.Height = CloseSize;
                        e.Graphics.DrawRectangle(p, myTab);
                    }
                    //填充
                    Color recColor = e.State == DrawItemState.Selected ? Color.Brown : Color.Beige;
                    using (Brush b = new SolidBrush(recColor))
                    {
                        e.Graphics.FillRectangle(b, myTab);
                    }

                    //画关闭符号
                    using (Pen objpen = new Pen(Color.Black))
                    {
                        //'\'线
                        Point p1 = new Point(myTab.X + 3, myTab.Y + 3);
                        Point p2 = new Point(myTab.X + myTab.Width - 3, myTab.Y + myTab.Height - 3);
                        e.Graphics.DrawLine(objpen, p1, p2);
                        //'/'线
                        Point p3 = new Point(myTab.X + 3, myTab.Y + myTab.Height - 3);
                        Point p4 = new Point(myTab.X + myTab.Width - 3, myTab.Y + 3);
                        e.Graphics.DrawLine(objpen, p3, p4);
                    }
                }
                e.Graphics.Dispose();
        }

        /// <summary>
        /// 控制当点击‘关闭’按钮时将TABPAGE 页面关闭
        /// 注:只有在‘空白报价单’功能时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tctotalpage_MouseDown(object sender, MouseEventArgs e)
        {
            //当使用Mouse左键点击 及只有在‘空白报价单’功能时执行
            if (e.Button == MouseButtons.Left && GlobalClasscs.Fun.EmptyFunctionName == "E")
            {
                int x = e.X, y = e.Y;
                //计算关闭区域
                Rectangle mytab = this.tctotalpage.GetTabRect(this.tctotalpage.SelectedIndex);

                mytab.Offset(mytab.Width - (CloseSize + 3), 2);
                mytab.Width = CloseSize;
                mytab.Height = CloseSize;

                //如果Mouse在区域内就关闭选项卡
                var isClose = x > mytab.X && x < mytab.Right && y > mytab.Y && y < mytab.Bottom;

                if (isClose)
                {
                    var id = tctotalpage.SelectedIndex;
                    this.tctotalpage.TabPages.Remove(this.tctotalpage.SelectedTab);
                    //当完成‘关闭’后,将当前页设置为'关闭'页的‘前一页’(注:若不这样设置,当关闭后会返回当前页为'首页',当id>0时才执行)
                    if (id > 0)
                    {
                        tctotalpage.SelectedTab = tctotalpage.TabPages[id - 1];
                    }
                }
            }
        }

        /// <summary>
        /// Tab page页改变时执行(注:目的是将对应的‘产品名称’信息更新至PAGE.TEXT属性内)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tctotalpage_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //循环已添加的Tabpage,并利用该页的产品名称更新至对应的PAGE.TEXT属性内
                for (var i = 0; i < tctotalpage.TabCount; i++)
                {
                    //循环获取TabPages内各页的内容
                    var showdetail = tctotalpage.TabPages[i].Controls[0] as ShowDetailFrm;
                    if (showdetail != null && showdetail.txtname.Text != "")
                    {
                        tctotalpage.TabPages[i].Text = showdetail.txtname.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                //检测所审核的TabPages内是否有GridView行没有一行也没有填的情况,若发现,跳出异常
                if(!CheckTabPagesGridView()) throw new Exception($"检测到单据'{txtbom.Text}' 内有物料明细行没有填写,请至少填写一行再进行审核");

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
                                newrow[3] = _funState == "C" ? DateTime.Now : _createtime;                  //创建日期
                                newrow[4] = DateTime.Now;                                                   //审核日期
                                newrow[5] = _funState == "C" ? GlobalClasscs.User.StrUsrName : _creatname;  //创建人
                                newrow[6] = _typeid;                                                        //单据类型ID(0:BOM成本报价单 1:新产品成本报价单及其它单据)
                                newrow[7] = 0;                                                              //记录当前单据使用标记(0:正在使用 1:没有使用)
                                newrow[8] = GlobalClasscs.User.StrUsrName;                                  //记录当前单据使用者名称信息

                                newrow[9] = _funState == "C" ? 0 : showdetail.Headid;                       //Headid
                                newrow[10] = showdetail.txtname.Text;                                       //产品名称(物料名称)
                                newrow[11] = showdetail.txtbao.Text;                                        //包装规格
                                newrow[12] = Convert.ToDecimal(showdetail.txtmi.Text);                      //产品密度(KG/L)
                                newrow[13] = Convert.ToDecimal(showdetail.txtmaterial.Text);                //材料成本(不含税)
                                newrow[14] = Convert.ToDecimal(showdetail.txtbaochenben.Text);              //包装成本
                                newrow[15] = Convert.ToDecimal(showdetail.txtren.Text);                     //人工及制造费用
                                newrow[16] = Convert.ToDecimal(showdetail.txtkg.Text);                      //成本(元/KG)
                                newrow[17] = Convert.ToDecimal(showdetail.txtl.Text);                       //成本(元/L)
                                newrow[18] = Convert.ToDecimal(showdetail.txt50.Text);                      //50%报价
                                newrow[19] = Convert.ToDecimal(showdetail.txt45.Text);                      //45%报价
                                newrow[20] = Convert.ToDecimal(showdetail.txt40.Text);                      //40%报价
                                newrow[21] = showdetail.txtremark.Text;                                     //备注
                                newrow[22] = showdetail.txtbom.Text;                                        //对应BOM版本编号
                                newrow[23] = Convert.ToDecimal(showdetail.txtprice.Text);                   //产品成本含税(物料单价)
                                newrow[24] = showdetail.txtcust.Text;                                       //客户

                                newrow[25] = rows[0];                                                       //Entryid
                                newrow[26] = rows[1];                                                       //物料编码ID
                                newrow[27] = rows[2];                                                       //物料编码
                                newrow[28] = rows[3];                                                       //物料名称
                                newrow[29] = rows[4];                                                       //配方用量
                                newrow[30] = rows[5];                                                       //占比
                                newrow[31] = rows[6];                                                       //物料单价(含税)
                                newrow[32] = rows[7];                                                       //物料成本(含税)
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
                        //审核成功后将‘新增页’按钮设置为不可用
                        tmaddpage.Enabled = false;
                        //若单据状态为R时,_backconfirm为TRUE
                        if (_funState == "R")
                            _backconfirm = true;
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
                    MessageBox.Show($"单据'{txtbom.Text}'提交成功,可关闭此单据", $"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            var clickMessage = !_confirmMarkId ? $"提示:单据'{txtbom.Text}'没提交, \n 其记录退出后将会消失,是否确定退出?" : $"是否退出? \n 注:若已审核但没有提交,已填写的记录也消失";

            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                var result = MessageBox.Show(clickMessage, "提示", MessageBoxButtons.YesNo,MessageBoxIcon.Information);
                //当点击"OK"按钮时执行以下操作
                if (result == DialogResult.Yes)
                {
                    //当退出时,清空useid等相关占用信息
                    //当单据状态为C时执行
                    if (_funState == "C")
                    {
                        UpdateUseValue(0,1,txtbom.Text);
                    }
                    //当单据状态为R时执行
                    else
                    {
                       UpdateUseValue(_fid,1,""); 
                    }

                    //在关闭时将TabControl已存在的Tab Pages删除(注:需倒序循环进行删除)
                    for (var i = tctotalpage.TabCount - 1; i >= 0; i--)
                    {
                        tctotalpage.TabPages.RemoveAt(i);
                    }
                    //将OA流水号文本框清空
                    txtbom.Text = "";
                    //将GlobalClasscs.Fun.EmptyFunctionName变量清空(待下一次使用‘空白报价单’功能时再对其赋值)
                    GlobalClasscs.Fun.EmptyFunctionName = "";
                    //允许窗体关闭
                    e.Cancel = false;
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
        /// 初始化新产品报价单历史记录DT
        /// </summary>
        /// <returns></returns>
        private DataTable OnInitializeHistoryDt()
        {
            task.TaskId = "0.9.3";
            task.SearchId = 0;
            task.StartTask();
            return task.ResultTable;
        }

        /// <summary>
        /// 初始化K3客户信息列表DT
        /// </summary>
        /// <returns></returns>
        private DataTable OnInitializeK3CustinfoDt()
        {
            task.TaskId = "0.9.4";
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
                if (txtbom.Text == "")
                {
                    result = false;
                }
                else
                {
                    task.TaskId = "0.3";
                    task.Oaorder = txtbom.Text;
                    task.StartTask();
                    result = txtbom.Text != "" && !task.ResultMark;
                } 
            }
            return result;
        }

        /// <summary>
        /// 检测各TabPages内的GridView是否有内容(注:若没有内容的话,就跳出异常)
        /// </summary>
        /// <returns></returns>
        private bool CheckTabPagesGridView()
        {
            var result = true;

            for (var i = 0; i < tctotalpage.TabCount; i++)
            {
                var showdetail = tctotalpage.TabPages[i].Controls[0] as ShowDetailFrm;
                //检测各TabPages内的GridView是否有内容
                if (showdetail != null)
                {
                    var bomdtldt = (DataTable)showdetail.gvdtl.DataSource;

                    if (bomdtldt.Rows.Count != 0) continue;
                    result = false;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 权限控制
        /// </summary>
        private void PrivilegeControl()
        {
            //若用户没有可读权限,即不会显示明细金额 true:有 flase:没有
            if (!GlobalClasscs.User.Readid)
            {
                //设置TabPages内的GridView的某些字段设为不可见
                ControlTabPages(0);
            }
            //若用户没有修改明细物料权限,即不会显示右键菜单功能
            if (!GlobalClasscs.User.Addid)
            {
                //设置用户的右键菜单不可见
                ControlTabPages(0);
            }

            //若为“审核”状态的话，就执行以下语句
            if (_confirmMarkId)
            {
                //加载图片
                pbimg.Visible = true;
                pbimg.BackgroundImage = Image.FromFile(Application.StartupPath + @"\PIC\1.png");
                //对相关控件设为不可改或只读
                txtbom.ReadOnly = true;
                //将‘添加新页’按钮设置为不显示
                tmaddpage.Visible = false;
                //循环TabPages内的控件
                ControlTabPages(0);

                //若单据状态为R并且不为‘反审核’时执行
                if (_funState == "R" && !_backconfirm)
                {
                    tmConfirm.Enabled = false;
                    tmsave.Enabled = false;
                }
                //单据状态为C“创建” 及R “反审核”时使用,当审核完成单据,但还没提交时执行
                else
                {
                    tmConfirm.Enabled = false;
                }
            }
            //若为“非审核”状态的,就执行以下语句
            else
            {
                //当选择的窗体是‘新产品成本报价单-创建’时执行,令指定的文本框可修改
                if (GlobalClasscs.Fun.FunctionName == "N" || GlobalClasscs.Fun.EmptyFunctionName == "E")
                {
                    ControlTabPages(1);
                }

                pbimg.Visible = false;
                txtbom.ReadOnly = false;
                tmsave.Enabled = true;
                tmConfirm.Enabled = true;
            }
        }

        /// <summary>
        /// 循环控制TabPages内的控件
        /// </summary>
        /// <param name="typeid">0:控制TabPages各子窗体不可见 1:设置子窗体中两个文本框可修改</param>
        void ControlTabPages(int typeid)
        {
            for (var i = 0; i < tctotalpage.TabCount; i++)
            {
                //循环获取TabPages内各页的内容并进行相关设置
                var showdetail = tctotalpage.TabPages[i].Controls[0] as ShowDetailFrm;
                if (showdetail != null)
                {
                    if (typeid == 0)
                    {
                        //若该用户没有设置‘查阅金额’权限,即将以下两列设置为不可见
                        if (!GlobalClasscs.User.Readid)
                        {
                            showdetail.gvdtl.Columns[5].Visible = false; //物料单价(含税)
                            showdetail.gvdtl.Columns[6].Visible = false; //物料成本(含税)
                        }
                        //若该用户没有设置‘可对明细物料操作’权限,即右键菜单不可见 以及 '配方用量'只读 ‘物料名称’ 只读
                        if (!GlobalClasscs.User.Addid)
                        {
                            //设置‘右键菜单’下的各项不显示
                            showdetail.tmReplace.Visible = false;
                            showdetail.ts1.Visible = false;
                            showdetail.tmAdd.Visible = false;
                            showdetail.ts2.Visible = false;
                            showdetail.tmdel.Visible = false;
                            showdetail.ts3.Visible = false;
                            showdetail.tmshowhistory.Visible = false;
                            showdetail.ts4.Visible = false;
                            showdetail.tmimportexcel.Visible = false;

                            //‘物料名称’只读
                            showdetail.gvdtl.Columns[3].ReadOnly = true;
                            //‘配方用量’只读
                            showdetail.gvdtl.Columns[4].ReadOnly = true;
                        }
                        //若为‘审核’状态时,将以下控件设为不可见或只读
                        if (_confirmMarkId)
                        {
                            showdetail.txtren.ReadOnly = true;         //人工制造费用
                            showdetail.txtbaochenben.ReadOnly = true;  //包装成本
                            showdetail.txtremark.ReadOnly = true;      //备注

                            showdetail.llcust.Enabled = false;         //将客户超连接设置为不可用
                            showdetail.gvdtl.ReadOnly = true;

                            //设置‘右键菜单’下的各项不显示
                            showdetail.tmReplace.Visible = false;
                            showdetail.ts1.Visible = false;
                            showdetail.tmAdd.Visible = false;
                            showdetail.ts2.Visible = false;
                            showdetail.tmdel.Visible = false;
                            showdetail.ts3.Visible = false;
                            showdetail.tmshowhistory.Visible = false;
                            showdetail.ts4.Visible = false;
                            showdetail.tmimportexcel.Visible = false;
                        }
                    }
                    //控制‘产品名称’及‘对应BOM版本编号’可修改
                    else if (typeid == 1)
                    {
                        //控制‘生成空白报价单’指定项的操作方式
                        if (GlobalClasscs.Fun.EmptyFunctionName != "")
                        {
                            showdetail.txtname.ReadOnly = false;           //产品名称
                            showdetail.txtbom.ReadOnly = false;            //对应BOM版本编号
                            showdetail.txtbao.ReadOnly = false;            //包装规格
                            showdetail.txtmi.ReadOnly = false;             //产品密度(KG/L)
                        }
                        //控制‘新产品报价单’指定项的操作方式
                        else if (GlobalClasscs.Fun.FunctionName == "N")
                        {
                            showdetail.txtname.ReadOnly = false;           //产品名称
                            showdetail.txtbom.ReadOnly = false;            //对应BOM版本编号
                        }
                    }
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

        /// <summary>
        /// 根据相关情况将占用情况进行更新 type:0(更新当前用户信息) 1(清空占用记录)
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="type"></param>
        /// <param name="oaorder"></param>
        private void UpdateUseValue(int fid,int type,string oaorder)
        {
            task.TaskId = "4";
            task.Fid = fid;
            task.Type = type;
            task.Oaorder = oaorder;
            task.StartTask();
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
