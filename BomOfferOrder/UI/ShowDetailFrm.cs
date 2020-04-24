using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using BomOfferOrder.DB;
using BomOfferOrder.Task;

namespace BomOfferOrder.UI
{
    public partial class ShowDetailFrm : Form
    {
        DbList dbList=new DbList();
        ShowMaterialDetailFrm showMaterial=new ShowMaterialDetailFrm();
        CustInfoFrm custInfo=new CustInfoFrm();
        TaskLogic task=new TaskLogic();
        Load load=new Load();

        #region 变量参数
        //定义单据状态(C:创建 R:读取)
        private string _funState;
        //获取‘原材料’‘原漆半成品’‘产成品’DT
        private DataTable _materialdt;
        //获取‘新产品报价单历史记录’DT
        private DataTable _historydt;
        //获取‘客户列表’DT
        private DataTable _custinfo;
        //保存需要进行删除的行记录(提交时使用) 注:状态为R读取时才适用
        private DataTable _deldt;
        //获取采购价目表DT
        private DataTable _pricelistdt;
        //获取采购入库表DT
        private DataTable _purchaseinstockdt;

        //保存查询出来的GridView记录
        public DataTable Dtl;
        //返回所记录的Headid
        private int _headid;

        //记录当前页数(GridView页面跳转使用)
        private int _pageCurrent = 1;
        //记录计算出来的总页数(GridView页面跳转使用)
        private int _totalpagecount;
        //记录初始化标记(GridView页面跳转 初始化时使用)
        private bool _pageChange;

        //记录从EXCEL导入的DT
        private DataTable _exceldt;
        //记录整理后的导入DT
        private DataTable _importdt;
        #endregion

        #region Get
        /// <summary>
        /// 返回GridView中需要删除的DT记录
        /// </summary>
        public DataTable Deldt => _deldt;
        /// <summary>
        /// 返回所记录的Headid
        /// </summary>
        public int Headid=> _headid;
        #endregion

        public ShowDetailFrm()
        {
            InitializeComponent();
            OnRegisterEvents();
        }

        private void OnRegisterEvents()
        {
            tmAdd.Click += TmAdd_Click;
            tmReplace.Click += TmReplace_Click;
            gvdtl.CellValueChanged += Gvdtl_CellValueChanged;
            txtren.Leave += Txtren_Leave;
            txtbaochenben.Leave += Txtbaochenben_Leave;
            tmdel.Click += Tmdel_Click;
            tmHAdd.Click += TmHAdd_Click;
            tmHReplace.Click += TmHReplace_Click;
            llcust.Click += Llcust_Click;
            tmimportexcel.Click += Tmimportexcel_Click;
            txtmi.Leave += Txtmi_Leave;

            bnMoveFirstItem.Click += BnMoveFirstItem_Click;
            bnMovePreviousItem.Click += BnMovePreviousItem_Click;
            bnMoveNextItem.Click += BnMoveNextItem_Click;
            bnMoveLastItem.Click += BnMoveLastItem_Click;
            bnPositionItem.Leave += BnPositionItem_Leave;
            tmshowrows.DropDownClosed += Tmshowrows_DropDownClosed;
            panel2.Visible = false;
        }

        /// <summary>
        ///  当GridView控件值有变化时使用
        /// </summary>
        void OnInitialize(DataTable gridViewdt)
        {
            if (gridViewdt.Rows.Count > 0)
            {
                Dtl = gridViewdt;
                panel2.Visible = true;
                //初始化下拉框所选择的默认值
                //tmshowrows.SelectedItem = "10"; 
                tmshowrows.SelectedItem = Convert.ToInt32(tmshowrows.SelectedItem) == 0
                    ? (object) "10"
                    : Convert.ToInt32(tmshowrows.SelectedItem);
                //定义初始化标记
                _pageChange = _pageCurrent <= 1;
                //GridView分页
                GridViewPageChange();
            }
            //注:当为空记录时,不显示跳转页;只需将临时表赋值至GridView内
            else
            {
                Dtl = gridViewdt;
                gvdtl.DataSource = gridViewdt;
                panel2.Visible = false;
            }
            //控制GridView单元格显示方式
            ControlGridViewisShow();
        }

        /// <summary>
        /// ‘人工及制造费用’文本框修改时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Txtren_Leave(object sender, EventArgs e)
        {
            try
            {
                //检测所输入的数字必须为数字(包括小数)
                if (!Regex.IsMatch(txtren.Text, @"^-?\d+$|^(-?\d+)(\.\d+)?$"))
                {
                    txtren.Text = "";
                    throw new Exception("不能输入非数字外的值,请输入数字后再继续");
                }
                //根据指定值将相关项进行改变指定文本框内的值
                GenerateValue();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ‘包装成本’文本框修改时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Txtbaochenben_Leave(object sender, EventArgs e)
        {
            try
            {
                //检测所输入的数字必须为数字(包括小数)
                if (!Regex.IsMatch(txtren.Text, @"^-?\d+$|^(-?\d+)(\.\d+)?$"))
                {
                    txtbaochenben.Text = "";
                    throw new Exception("不能输入非数字外的值,请输入数字后再继续");
                }
                //根据指定值将相关项进行改变指定文本框内的值
                GenerateValue();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// '产品密度(KG/L)文本框修改时执行'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Txtmi_Leave(object sender, EventArgs e)
        {
            try
            {
                //检测所输入的数字必须为数字(包括小数)
                if (!Regex.IsMatch(txtmi.Text, @"^-?\d+$|^(-?\d+)(\.\d+)?$"))
                {
                    txtmi.Text = "";
                    throw new Exception("不能输入非数字外的值,请输入数字后再继续");
                }
                //根据指定值将相关项进行改变指定文本框内的值
                GenerateValue();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 新增记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmAdd_Click(object sender, EventArgs e)
        {
            try
            {
                InsertRecordToGridView("A",null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 替换此行数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmReplace_Click(object sender, EventArgs e)
        {
            try
            {
                if(gvdtl.SelectedRows.Count==0) throw new Exception("请选择任意一行,再继续");
                if(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[1].Value == DBNull.Value) throw new Exception("空行不能进行替换,请再次选择");
                //获取GridView内的物料编码ID
                var materialId = Convert.ToInt32(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[1].Value);
                UpdateRecordToGridView(materialId,"U",null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 删除明细行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmdel_Click(object sender, EventArgs e)
        {
            try
            {
                if(gvdtl.SelectedRows.Count==0) throw new Exception("没有选择行,不能继续");
                if(Dtl.Rows.Count==0)throw new Exception("没有明细记录,不能进行删除");
                
                var clickMessage = $"您所选择需删除的行数为:{gvdtl.SelectedRows.Count}行 \n 是否继续?";

                if (MessageBox.Show(clickMessage, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    //注:执行方式:当判断到_funState变量为R时,将要进行删除的行保存至_deldt内,(供保存时使用),完成后再删除GridView指定行;反之,只需将GridView进行指定行删除即可
                    //注:在R状态下,需判断Entryid是否为空,若不为空,才进行插入至_deldt内

                    if (_funState == "R")
                    {
                        foreach (DataGridViewRow rows in gvdtl.SelectedRows)
                        {
                            //判断若Entryid不为空,才执行插入
                            if(rows.Cells[0].Value == DBNull.Value)continue;

                            var newrow = _deldt.NewRow();
                            for (var i = 0; i < _deldt.Columns.Count; i++)
                            {
                                newrow[i] = rows.Cells[i].Value;
                            }
                            _deldt.Rows.Add(newrow);
                        }
                    }

                    //先根据GridView所选择的行将_dtl对应的行删除
                    //注:使用‘物料名称’进行循环对比
                    for (var i = Dtl.Rows.Count; i > 0; i--)
                    {
                        for (var j = 0; j < gvdtl.SelectedRows.Count; j++)
                        {
                            if (Convert.ToString(Dtl.Rows[i - 1][2]) == Convert.ToString(gvdtl.SelectedRows[j].Cells[2].Value))
                            {
                                Dtl.Rows.RemoveAt(i - 1);
                                break;  //注:此处必加!!!
                            }
                        }
                    }

                    #region 完成后将GridView内的指定行进行删除(目前已不需要,只需将_dtl相关的记录删除即可)
                    //for (var i = gvdtl.SelectedRows.Count; i > 0; i--)
                    //{
                    //    gvdtl.Rows.RemoveAt(gvdtl.SelectedRows[i - 1].Index);
                    //}
                    #endregion

                    //根据指定值将相关项进行改变指定文本框内的值
                    GenerateValue();
                    //操作完成后进行刷新
                    OnInitialize(Dtl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 获取新产品BOM明细记录-新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmHAdd_Click(object sender, EventArgs e)
        {
            try
            {
                InsertRecordToGridView("HA",null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 获取新产品BOM明细记录-替换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmHReplace_Click(object sender, EventArgs e)
        {
            try
            {
                if (gvdtl.SelectedRows.Count == 0) throw new Exception("请选择任意一行,再继续");
                if (gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[1].Value == DBNull.Value) throw new Exception("空行不能进行替换,请再次选择");
                //获取GridView内的物料编码ID
                var materialId = Convert.ToInt32(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[1].Value);
                UpdateRecordToGridView(materialId,null,"HU");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 客户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Llcust_Click(object sender, EventArgs e)
        {
            try
            {
                //初始化GridView
                custInfo.OnInitialize(_custinfo);
                custInfo.StartPosition = FormStartPosition.CenterScreen;
                custInfo.ShowDialog();

                //以下为返回相关记录回本窗体相关处理
                //判断若返回的DT为空的话,就不需要任何效果
                if (custInfo.ResultTable == null || custInfo.ResultTable.Rows.Count == 0) return;
                //将返回的结果赋值至GridView内(注:判断若返回的DT不为空或行数大于0才执行更新效果)
                if (custInfo.ResultTable != null || custInfo.ResultTable.Rows.Count > 0)
                    InsertCustRecordToTxt(custInfo.ResultTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 导入物料明细Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmimportexcel_Click(object sender, EventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog { Filter = $"Xlsx文件|*.xlsx" };
                if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                task.TaskId = "6";
                task.FileAddress = openFileDialog.FileName;
                task.Reporttype = "1";  //导入EXCEL时的类型(0:报表功能使用  1:BOM物料明细使用)
                task.StartTask();

                //将整理过后的DT存放至_exceldt内
                _exceldt = task.ImportExceldtTable;
                //将从EXCEL获取的记录传送至ShowDetailFrm.ImportExcelRecordToBom内
                new Thread(ImportExcelRecordToBom).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();

                //若整理的DT行数大于0,就执行
                if (_importdt.Rows.Count > 0)
                {
                    OnInitialize(_importdt);
                    //累加并获取‘配方用量’合计
                    txtpeitotal.Text = GenerateSumpeitotal();
                    //计算物料成本(含税)之和
                    var materialsumqty = GernerateSumQty();
                    //产品成本含税(物料单价)
                    txtprice.Text = Convert.ToString(Math.Round(materialsumqty, 4));
                    //材料成本(不含税)
                    txtmaterial.Text = Convert.ToString(Math.Round(materialsumqty / Convert.ToDecimal(1.13), 4));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// 将相关值根据获取过来的DT填充至对应的项内
        /// </summary>
        /// <param name="funState"></param>
        /// <param name="typeid">单据类型 0:BOM成本报价单 1:新产品成本报价单 2:空白报价单 </param>
        /// <param name="dt">数据源</param>
        /// <param name="materialdt">原材料 ‘原漆半成品’‘产成品’DT</param>
        /// <param name="historydt">新产品报价单历史记录DT</param>
        /// <param name="custinfodt">记录K3客户列表DT</param>
        /// <param name="pricelistdt">采购价目表DT-BOM物料-物料单价使用</param>
        /// <param name="purchaseInstockdt">采购入库单DT-BOM物料-物料单价使用</param>
        public void AddDbToFrm(string funState,int typeid,DataTable dt,DataTable materialdt,DataTable historydt,DataTable custinfodt,
                               DataTable pricelistdt,DataTable purchaseInstockdt)
        {
            //将‘原材料’‘原漆半成品’‘产成品’DT赋值至变量内
            _materialdt = materialdt;
            //将‘新产品报价单历史记录’DT赋值至变量内
            _historydt = historydt;
            //将‘K3-客户信息’DT赋值至变量内
            _custinfo = custinfodt;
            //初始化获取采购价目表DT
            _pricelistdt = pricelistdt;
            //初始化获取采购入库表DT
            _purchaseinstockdt = purchaseInstockdt;

            try
            {
                //将单据状态获取至_funState变量内
                _funState = funState;

                //单据状态:创建 C
                if (_funState == "C")
                {
                    //此为‘空白报价单’复制功能使用
                    if (typeid == 2 && dt != null)
                    {
                        FunStateEmptyOrder(dt);
                    }
                    else
                    {
                        FunStateCUse(funState,dt);
                    }
                }
                //单据状态:读取 R
                else
                {
                    //若funstate为‘读取’状态 并且 单据类型为‘空白报价单’ 及 DT为NULL时,就执行,
                    //注:执行这个目的环境在=>R状态 并且 在‘空白报价单’功能上需要‘新增新页’时使用
                    if (funState == "R" && typeid == 2 && dt == null)
                    {
                        txtmi.Text = Convert.ToString(0);          //产品密度
                        txtren.Text = "0";                         //人工制造费用(自填)
                        txtbaochenben.Text = "0";                  //包装成本(自填)
                        txtmi.Text = "0";                          //产品密度(KG/L)
                        OnInitialize(dbList.MakeGridViewTemp());   //将临时表(空行记录)插入到GridView内
                    }
                    //此为‘空白报价单’复制功能使用
                    else if (funState == "R" && typeid == 2 && dt != null)
                    {
                        //若dt.rows[25](EntryId)为空的话,就执行
                        if (dt.Rows[0][25] == DBNull.Value)
                        {
                            FunStateEmptyOrder(dt);
                        }
                        else
                        {
                            //若dt.rows[25](EntryId)不为空的话,就表示为读取数据
                            FunStateRUse(funState,dt);
                        }
                    }
                    //将从数据库获取的数据读取
                    else
                    {
                        FunStateRUse(funState, dt);
                    }

                    //初始化定义删除临时表(单据状态为R,并且执行‘删除明细行’时才使用)
                    _deldt = dbList.MakeGridViewTemp();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 计算BOM物料明细内的‘物料单价’
        /// </summary>
        /// <param name="fmaterialid"></param>
        /// <returns></returns>
        private decimal GenerateMaterialPrice(int fmaterialid)
        {
            //定义结果变量
            decimal result = 0;
            //检测若fmaterialid在_pricelistdt内不存在,就继续在_purchaseinstockdt内查找是否存在
            var dtlrows = _pricelistdt.Select("子项物料内码='" + fmaterialid + "'");

            if (dtlrows.Length > 0)
            {
                result = decimal.Round(Convert.ToDecimal(dtlrows[0][1]), 4);
            }
            //若没有就在_purchaseinstockdt查询,若还是没有就返回0
            else
            {
                var dtlrow = _purchaseinstockdt.Select("FMATERIALID='" + fmaterialid + "'");
                result = dtlrow.Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dtlrow[0][1]), 4);
            }
            return result;
        }

        /// <summary>
        /// 导入物料明细记录
        /// </summary>
        private void ImportExcelRecordToBom()
        {
            try
            {
                //获取临时表(GridView控件时使用)
                var resultdt = dbList.MakeGridViewTemp();
                //将相关记录集放至方法内并进行整理,完成后放至GridView内进行显示
                //若_dtl原来是有记录的,就判断其内的‘物料名称’是否在dt内存在,若有就更新此行的‘配方用量’;最后将dt内的此行物料记录删除
                var dt = GetExceldtToBomDetail(_materialdt, _exceldt, resultdt);
                if (Dtl.Rows.Count > 0)
                {
                    //循环_dtl,并判断其‘物料名称’(3)是否在dt内存在,若存在,即更新‘配方用量’
                    foreach (DataRow rows in Dtl.Rows)
                    {
                        for (var i = dt.Rows.Count; i >0; i--)
                        {
                            //若'物料名称'相同,即更新‘配方用量’‘物料单价()’‘物料成本()’,在更新完成后,将dt此行记录删除
                            if (Convert.ToString(dt.Rows[i - 1][3]) == Convert.ToString(rows[3]))
                            {
                                //更新‘配方用量’及'物料'
                                Dtl.BeginInit();
                                rows[4] = dt.Rows[i - 1][4];                           //配方用量
                                rows[5] = Convert.ToDecimal(dt.Rows[i - 1][5]);        //占比=配方用量*100
                                rows[6] = Convert.ToDecimal(dt.Rows[i - 1][6]);        //物料单价(含税)
                                rows[7] = Convert.ToDecimal(dt.Rows[i - 1][7]);        //物料成本(含税)
                                rows[8] = DBNull.Value;                                //备注
                                Dtl.EndInit();
                                //删除dt行
                                dt.Rows.RemoveAt(i-1);
                            }
                        }
                    }
                    //最后将记录整合
                    Dtl.Merge(dt);
                    _importdt = Dtl;
                }
                else
                {
                    _importdt = dt;
                }

                //当完成后将Load子窗体关闭
                this.Invoke((ThreadStart)(() =>
                {
                    load.Close();
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }   
        }

        /// <summary>
        /// 对导入的EXCEL记录进行整理并返回插入的记录
        /// 注:若导入的‘物料名称’不在materialdt存在,即FMATERIALID从‘0’开始,并且‘物料编码’为空
        /// </summary>
        /// <param name="materialdt">K3物料明细DT</param>
        /// <param name="sourcedt">EXCEL导入过来DT</param>
        /// <param name="resultdt">返回结果DT</param>
        /// <returns></returns>
        private DataTable GetExceldtToBomDetail(DataTable materialdt,DataTable sourcedt, DataTable resultdt)
        {
            try
            {
                //定义各变量
                var fmaterialid = 0;               //物料编码ID
                string fmaterialcode;              //物料编码
                string fmaterialname;              //物料名称
                decimal peiqty = 0;                //配方用量
                decimal price = 0;                 //物料单价
                //定义‘物料编码’为空对应的FMATERIALID最大值
                var maxFmaterialid = 0;

                foreach (DataRow rows in sourcedt.Rows)
                {
                    //使用‘物料名称’为条件,在materialdt内查询对应的
                    var dtlrow = materialdt.Select("物料名称='"+rows[0]+"'");
                    //若为空,就需要将FMATERIALID (从0开始) 物料编码为空
                    if (dtlrow.Length == 0)
                    {
                        //利用maxFmaterialid为累加FMATERIALID值,没有记录时从0开始,其它情况为累加
                        var dtrows = resultdt.Select("物料编码 is null").Length;
                        if (dtrows == 0)
                        {
                            maxFmaterialid = 0;
                        }
                        else
                        {
                            maxFmaterialid += 1;
                        }

                        fmaterialid = maxFmaterialid;
                        fmaterialcode = null;
                        fmaterialname = Convert.ToString(rows[0]);
                        peiqty = Convert.ToDecimal(rows[1]);
                        price = 0;
                    }
                    //若存在就执行以下语句
                    else
                    {
                        fmaterialid = Convert.ToInt32(dtlrow[0][0]);
                        fmaterialcode = Convert.ToString(dtlrow[0][1]);
                        fmaterialname = Convert.ToString(rows[0]);
                        peiqty = Convert.ToDecimal(rows[1]);
                        //根据FMATERIALID计算其对应的‘物料单价’
                        price = GenerateMaterialPrice(fmaterialid);
                    }

                    var newrow = resultdt.NewRow();
                    newrow[0] = DBNull.Value;                           //EntryId
                    newrow[1] = fmaterialid;                            //物料编码ID
                    newrow[2] = fmaterialcode;                          //物料编码
                    newrow[3] = fmaterialname;                          //物料名称
                    newrow[4] = peiqty;                                 //配方用量
                    newrow[5] = Convert.ToDecimal(peiqty) * 100;        //占比=配方用量*100
                    newrow[6] = price;                                  //物料单价(含税)
                    newrow[7] = decimal.Round(peiqty / 100 * price,4);  //物料成本(含税) 公式:配方用量/100*物料单价
                    resultdt.Rows.Add(newrow);
                }
            }
            catch (Exception)
            {
                resultdt.Rows.Clear();
                resultdt.Columns.Clear();
            }
            return resultdt;
        }

        /// <summary>
        /// '空白报价单'=>‘复制’功能使用
        /// </summary>
        /// <param name="sourcedt"></param>
        private void FunStateEmptyOrder(DataTable sourcedt)
        {
            //将相关值插入至对应的文本框内
            _headid = Convert.ToInt32(sourcedt.Rows[0][9]);                                                  //Headid
            txtname.Text = Convert.ToString(sourcedt.Rows[0][10]);                                           //产品名称
            txtbao.Text = Convert.ToString(sourcedt.Rows[0][11]);                                            //包装规格

            txtmi.Text = Convert.ToString(Math.Round(Convert.ToDecimal(sourcedt.Rows[0][12]), 4));            //产品密度(KG/L)
            txtbaochenben.Text = Convert.ToString(Math.Round(Convert.ToDecimal(sourcedt.Rows[0][14]), 4));    //包装成本
            txtren.Text = Convert.ToString(Math.Round(Convert.ToDecimal(sourcedt.Rows[0][15]), 4));           //人工及制造费用
            txtkg.Text = Convert.ToString(Math.Round(Convert.ToDecimal(sourcedt.Rows[0][16]), 4));            //成本(元/KG)
            txtl.Text = Convert.ToString(Math.Round(Convert.ToDecimal(sourcedt.Rows[0][17]), 4));             //成本(元/L)
            txt50.Text = Convert.ToString(Math.Round(Convert.ToDecimal(sourcedt.Rows[0][18]), 4));            //50%报价
            txt45.Text = Convert.ToString(Math.Round(Convert.ToDecimal(sourcedt.Rows[0][19]), 4));            //45%报价
            txt40.Text = Convert.ToString(Math.Round(Convert.ToDecimal(sourcedt.Rows[0][20]), 4));            //40%报价

            txtremark.Text = Convert.ToString(sourcedt.Rows[0][21]);                                         //备注
            txtbom.Text = Convert.ToString(sourcedt.Rows[0][22]);                                            //对应BOM版本编号
            txtcust.Text = Convert.ToString(sourcedt.Rows[0][24]);                                           //客户名称
            //将临时表(空行记录)插入到GridView内
            OnInitialize(dbList.MakeGridViewTemp());   
        }

        /// <summary>
        /// 单据状态为C时使用
        /// </summary>
        /// <param name="funState"></param>
        /// <param name="sourcedt"></param>
        private void FunStateCUse(string funState, DataTable sourcedt)
        {
            //获取临时表(GridView控件时使用) 注:‘创建’及‘读取’也会使用到
            var resultdt = dbList.MakeGridViewTemp();

            //包装成本(自填)
            txtbaochenben.Text = "0";
            //人工制造费用(自填)               
            txtren.Text = "0";

            //判断若是NewProduct的话,就只将‘产品名称’,‘包装规格’ 以及 ‘产品密度’赋值上就可以;明细内容不用理会

            //空白报价单-创建使用
            if (sourcedt == null)
            {
                txtmi.Text = Convert.ToString(0);
                //将临时表(空行记录)插入到GridView内
                OnInitialize(resultdt);
            }
            else
            {
                //新产品成本报价单-创建使用
                if (GlobalClasscs.Fun.FunctionName == "N")
                {
                    txtname.Text = Convert.ToString(sourcedt.Rows[0][1]);   //产品名称
                    txtbao.Text = Convert.ToString(sourcedt.Rows[0][3]);    //包装规格
                    txtmi.Text = Convert.ToString(sourcedt.Rows[0][4]);     //产品密度
                    //将临时表(空行记录)插入到GridView内
                    OnInitialize(resultdt);
                }
                //“生成成本BOM报价单”代码
                else
                {
                    //将相关值赋值给对应的文本框及GridView控件内
                    txtname.Text = Convert.ToString(sourcedt.Rows[0][1]);     //产品名称
                    txtbom.Text = Convert.ToString(sourcedt.Rows[0][2]);      //BOM编号
                    txtbao.Text = Convert.ToString(sourcedt.Rows[0][3]);      //包装规格
                    txtmi.Text = Convert.ToString(sourcedt.Rows[0][4]);       //产品密度
                    //设置及刷新GridView
                    OnInitialize(GetGridViewdt(funState, sourcedt, resultdt));
                    //根据指定值将相关项进行改变指定文本框内的值
                    GenerateValue();
                }
            }

            #region Hide 原包装成本公式
            //包装成本 公式:表头物料单价/包装规格/1.13
            //txtbaochenben.Text = Convert.ToString(Math.Round(Convert.ToDecimal(txtprice.Text) / 
            //                                        GetNumberInt(Convert.ToString(sourcedt.Rows[0][3])) / Convert.ToDecimal(1.13),4));
            #endregion
        }

        /// <summary>
        /// 单据状态为R时使用
        /// </summary>
        /// <param name="funState">单据状态</param>
        /// <param name="sourcedt">数据源</param>
        private void FunStateRUse(string funState,DataTable sourcedt)
        {
            //获取临时表(GridView控件时使用) 注:‘创建’及‘读取’也会使用到
            var resultdt = dbList.MakeGridViewTemp();
            //将相关值赋值给对应的文本框及GridView控件内
            _headid = Convert.ToInt32(sourcedt.Rows[0][9]);                                                  //Headid
            txtname.Text = Convert.ToString(sourcedt.Rows[0][10]);                                           //产品名称
            txtbao.Text = Convert.ToString(sourcedt.Rows[0][11]);                                            //包装规格

            txtmi.Text = Convert.ToString(Math.Round(Convert.ToDecimal(sourcedt.Rows[0][12]),4));            //产品密度(KG/L)
            txtbaochenben.Text = Convert.ToString(Math.Round(Convert.ToDecimal(sourcedt.Rows[0][14]),4));    //包装成本
            txtren.Text = Convert.ToString(Math.Round(Convert.ToDecimal(sourcedt.Rows[0][15]),4));           //人工及制造费用
            txtkg.Text = Convert.ToString(Math.Round(Convert.ToDecimal(sourcedt.Rows[0][16]),4));            //成本(元/KG)
            txtl.Text = Convert.ToString(Math.Round(Convert.ToDecimal(sourcedt.Rows[0][17]),4));             //成本(元/L)
            txt50.Text = Convert.ToString(Math.Round(Convert.ToDecimal(sourcedt.Rows[0][18]),4));            //50%报价
            txt45.Text = Convert.ToString(Math.Round(Convert.ToDecimal(sourcedt.Rows[0][19]),4));            //45%报价
            txt40.Text = Convert.ToString(Math.Round(Convert.ToDecimal(sourcedt.Rows[0][20]),4));            //40%报价

            txtremark.Text = Convert.ToString(sourcedt.Rows[0][21]);                                         //备注
            txtbom.Text = Convert.ToString(sourcedt.Rows[0][22]);                                            //对应BOM版本编号
            txtcust.Text = Convert.ToString(sourcedt.Rows[0][24]);                                           //客户名称

            //设置及刷新GridView
            OnInitialize(GetGridViewdt(funState, sourcedt, resultdt));
            //累加并获取‘配方用量’合计
            txtpeitotal.Text = GenerateSumpeitotal();
            //计算物料成本(含税)之和
            var materialsumqty = GernerateSumQty();
            //产品成本含税(物料单价)
            txtprice.Text = Convert.ToString(Math.Round(materialsumqty, 4));
            //材料成本(不含税)
            txtmaterial.Text= Convert.ToString(Math.Round(materialsumqty / Convert.ToDecimal(1.13), 4));
        }

        /// <summary>
        /// 生成GridView需要的DT
        /// </summary>
        /// <param name="funState">状态:C:创建 R:读取</param>
        /// <param name="sourcedt">数据源DT</param>
        /// <param name="resultdt">根据‘创建’或‘读取’生成的临时表</param>
        /// <returns></returns>
        private DataTable GetGridViewdt(string funState,DataTable sourcedt,DataTable resultdt)
        {
            try
            {
                //‘创建’状态
                if (funState == "C")
                {
                    //循环获取值赋给对应的控件内
                    foreach (DataRow rows in sourcedt.Rows)
                    {
                        var newrow = resultdt.NewRow();
                        newrow[0] = DBNull.Value;                   //EntryId
                        newrow[1] = rows[5];                        //物料编码ID
                        newrow[2] = rows[6];                        //物料编码
                        newrow[3] = rows[7];                        //物料名称
                        newrow[4] = rows[8];                        //配方用量
                        newrow[5] = Convert.ToDecimal(rows[8])*100; //占比
                        newrow[6] = rows[10];                       //物料单价(含税)
                        newrow[7] = decimal.Round(Convert.ToDecimal(rows[8]) /100  * Convert.ToDecimal(rows[10]), 4);  //物料成本(含税) 公式:配方用量/100*物料单价
                        newrow[8] = DBNull.Value;                   //备注
                        resultdt.Rows.Add(newrow);
                    }
                }
                //‘读取’状态
                else
                {
                    foreach (DataRow rows in sourcedt.Rows)
                    {
                        var newrow = resultdt.NewRow();
                        newrow[0] = rows[25];       //EntryId
                        newrow[1] = rows[26];       //物料编码ID
                        newrow[2] = rows[27];       //物料编码
                        newrow[3] = rows[28];       //物料名称
                        newrow[4] = rows[29];       //配方用量
                        newrow[5] = rows[30];       //占比
                        newrow[6] = rows[31];       //物料单价(含税)
                        newrow[7] = rows[32];       //物料成本(含税)
                        newrow[8] = rows[33];       //备注
                        resultdt.Rows.Add(newrow);
                    }
                }
            }
            catch (Exception)
            {
                resultdt.Rows.Clear();
                resultdt.Columns.Clear();
            }
            return resultdt;
        }

        /// <summary>
        /// 控制GridView单元格显示方式
        /// </summary>
        private void ControlGridViewisShow()
        {
            gvdtl.Columns[0].Visible = false;      //EntryID
            gvdtl.Columns[1].Visible = false;     //物料ID
            gvdtl.Columns[2].ReadOnly = true;    //物料编码
            gvdtl.Columns[5].Visible = false;   //占比(不显示)
            gvdtl.Columns[7].ReadOnly = true;  //物料成本(含税)
        }

        /// <summary>
        /// 首页按钮(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveFirstItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将当前页变量PageCurrent=1; 2)并将“首页” 及 “上一页”按钮设置为不可用 将“下一页” “末页”按设置为可用
                _pageCurrent = 1;
                bnMoveFirstItem.Enabled = false;
                bnMovePreviousItem.Enabled = false;

                bnMoveNextItem.Enabled = true;
                bnMoveLastItem.Enabled = true;
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 上一页(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMovePreviousItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将PageCurrent自减 2)将“下一页” “末页”按钮设置为可用
                _pageCurrent--;
                bnMoveNextItem.Enabled = true;
                bnMoveLastItem.Enabled = true;
                //判断若PageCurrent=1的话,就将“首页” “上一页”按钮设置为不可用
                if (_pageCurrent == 1)
                {
                    bnMoveFirstItem.Enabled = false;
                    bnMovePreviousItem.Enabled = false;
                }
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 下一页按钮(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveNextItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将PageCurrent自增 2)将“首页” “上一页”按钮设置为可用
                _pageCurrent++;
                bnMoveFirstItem.Enabled = true;
                bnMovePreviousItem.Enabled = true;
                //判断若PageCurrent与“总页数”一致的话,就将“下一页” “末页”按钮设置为不可用
                if (_pageCurrent == _totalpagecount)
                {
                    bnMoveNextItem.Enabled = false;
                    bnMoveLastItem.Enabled = false;
                }
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 末页按钮(GridView页面跳转使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveLastItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将“总页数”赋值给PageCurrent 2)将“下一页” “末页”按钮设置为不可用 并将 “上一页” “首页”按钮设置为可用
                _pageCurrent = _totalpagecount;
                bnMoveNextItem.Enabled = false;
                bnMoveLastItem.Enabled = false;

                bnMovePreviousItem.Enabled = true;
                bnMoveFirstItem.Enabled = true;

                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 跳转页文本框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnPositionItem_Leave(object sender, EventArgs e)
        {
            try
            {
                //判断所输入的跳转数必须为整数
                if (!Regex.IsMatch(bnPositionItem.Text, @"^-?[1-9]\d*$|^0$")) throw new Exception("请输入整数再继续");
                //判断所输入的跳转数不能大于总页数
                if (Convert.ToInt32(bnPositionItem.Text) > _totalpagecount) throw new Exception("所输入的页数不能超出总页数,请修改后继续");
                //判断若所填跳转数为0时跳出异常
                if (Convert.ToInt32(bnPositionItem.Text) == 0) throw new Exception("请输入大于0的整数再继续");

                //将所填的跳转页赋值至“当前页”变量内
                _pageCurrent = Convert.ToInt32(bnPositionItem.Text);
                //根据所输入的页数动态控制四个方向键是否可用
                //若为第1页，就将“首页” “上一页”按钮设置为不可用 将“下一页” “末页”设置为可用
                if (_pageCurrent == 1)
                {
                    bnMoveFirstItem.Enabled = false;
                    bnMovePreviousItem.Enabled = false;

                    bnMoveNextItem.Enabled = true;
                    bnMoveLastItem.Enabled = true;
                }
                //若为末页,就将"下一页" “末页”按钮设置为不可用 将“上一页” “首页”设置为可用
                else if (_pageCurrent == _totalpagecount)
                {
                    bnMoveNextItem.Enabled = false;
                    bnMoveLastItem.Enabled = false;

                    bnMovePreviousItem.Enabled = true;
                    bnMoveFirstItem.Enabled = true;
                }
                //否则四个按钮都可用
                else
                {
                    bnMoveFirstItem.Enabled = true;
                    bnMovePreviousItem.Enabled = true;
                    bnMoveNextItem.Enabled = true;
                    bnMoveLastItem.Enabled = true;
                }
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bnPositionItem.Text = Convert.ToString(_pageCurrent);
            }
        }

        /// <summary>
        /// 每页显示行数 下拉框关闭时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmshowrows_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                //每次选择新的“每页显示行数”，都要 1)将_pageChange标记设为true(即执行初始化方法) 2)将“当前页”初始化为1
                _pageChange = true;
                _pageCurrent = 1;
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// GridView分页功能
        /// </summary>
        private void GridViewPageChange()
        {
            try
            {
                //获取查询的总行数
                var dtltotalrows = Dtl.Rows.Count;
                //获取“每页显示行数”所选择的行数
                var pageCount = Convert.ToInt32(tmshowrows.SelectedItem);
                //计算出总页数
                _totalpagecount = dtltotalrows % pageCount == 0 ? dtltotalrows / pageCount : dtltotalrows / pageCount + 1;
                //赋值"总页数"项
                bnCountItem.Text = $"/ {_totalpagecount} 页";

                //初始化BindingNavigator控件内的各子控件 及 对应初始化信息
                if (_pageChange)
                {
                    bnPositionItem.Text = Convert.ToString(1);                       //初始化填充跳转页为1
                    tmshowrows.Enabled = true;                                      //每页显示行数（下拉框）  

                    //初始化时判断;若“总页数”=1，四个按钮不可用；若>1,“下一页” “末页”按钮可用
                    if (_totalpagecount == 1)
                    {
                        bnMoveFirstItem.Enabled = false;                            //'首页'按钮
                        bnMovePreviousItem.Enabled = false;                         //'上一页'按钮
                        bnMoveNextItem.Enabled = false;                             //'下一页'按钮
                        bnMoveLastItem.Enabled = false;                             //'末页'按钮
                        bnPositionItem.Enabled = false;                             //跳转页文本框
                    }
                    else
                    {
                        bnMoveNextItem.Enabled = true;
                        bnMoveLastItem.Enabled = true;
                        bnPositionItem.Enabled = true;                             //跳转页文本框
                    }
                    _pageChange = false;
                }

                //显示_dtl的查询总行数
                tstotalrow.Text = $"共 {Dtl.Rows.Count} 行";

                //根据“当前页” 及 “固定行数” 计算出新的行数记录并进行赋值
                //计算进行循环的起始行
                var startrow = (_pageCurrent - 1) * pageCount;
                //计算进行循环的结束行
                var endrow = _pageCurrent == _totalpagecount ? dtltotalrows : _pageCurrent * pageCount;
                //复制 查询的DT的列信息（不包括行）至临时表内
                var tempdt = Dtl.Clone();
                //循环将所需的_dtl的行记录复制至临时表内
                for (var i = startrow; i < endrow; i++)
                {
                    tempdt.ImportRow(Dtl.Rows[i]);
                }

                //最后将刷新的DT重新赋值给GridView
                gvdtl.DataSource = tempdt;
                //将“当前页”赋值给"跳转页"文本框内
                bnPositionItem.Text = Convert.ToString(_pageCurrent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 将获取的数据插入至GridView内(注:可多行使用)
        /// 判断若sourcedt内的物料ID已经在GridView内已存在,即跳过不作添加
        /// </summary>
        /// <param name="sourcedt"></param>
        private void InsertDtToGridView(DataTable sourcedt)
        {
            try
            {
                //将GridView内的内容赋值到DT
                var gridViewdt = Dtl;   //(DataTable)gvdtl.DataSource;

                //循环将获取过来的值插入至GridView内
                foreach (DataRow rows in sourcedt.Rows)
                {
                    //判断若获取过来的物料ID已在GridView内存在,即跳过不作添加
                    if(gridViewdt.Select("物料编码ID='" + rows[1] + "'").Length>0) continue;

                    var newrow = gridViewdt.NewRow();
                    newrow[1] = rows[1];                                          //物料编码ID
                    newrow[2] = rows[2];                                          //物料编码
                    newrow[3] = rows[3];                                          //物料名称
                    newrow[6] = GenerateMaterialPrice(Convert.ToInt32(rows[1]));  //物料单价 rows[5]
                    newrow[7] = 0;                                                //物料成本(设置为0)
                    newrow[8] = DBNull.Value;                                     //备注
                    gridViewdt.Rows.Add(newrow);
                }
                //操作完成后进行刷新
                OnInitialize(gridViewdt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 将获取的值更新至指定的GridView行内(注:只能一行使用)
        /// 判断若sourcedt内的物料ID已在GridView内存在,即跳出异常不能继续替换操作
        /// </summary>
        /// <param name="materialId">物料编码ID</param>
        /// <param name="sourcedt"></param>
        private void UpdateDtToGridView(int materialId, DataTable sourcedt)
        {
            try
            {
                //注:在同一个'产品名称'下,MaterialID都是唯一的!!!

                //循环GridView内的值,当发现ID与条件ID相同,即进入行更新
                //将GridView内的内容赋值到DT
                var gridViewdt = Dtl;  //(DataTable)gvdtl.DataSource;
                //判断若sourcedt内的物料ID已在GridView内存在,即跳出异常不能继续替换操作
                if (gridViewdt.Select("物料编码ID='"+ sourcedt.Rows[0][1] +"'").Length>0)
                        throw new Exception($"获取物料'{sourcedt.Rows[0][2]}'已存在,故不能进行替换,请重新选择其它物料");

                foreach (DataRow rows in gridViewdt.Rows)
                {
                    //判断若materialid相同,就执行更新操作
                    if (Convert.ToInt32(rows[1]) != materialId) continue;

                    gridViewdt.BeginInit();
                    rows[1] = sourcedt.Rows[0][1];  //物料编码ID
                    rows[2] = sourcedt.Rows[0][2];  //物料编码
                    rows[3] = sourcedt.Rows[0][3];  //物料名称
                    rows[4] = DBNull.Value;         //配方用量(清空)
                    rows[5] = DBNull.Value;         //占比(清空)
                    rows[6] = GenerateMaterialPrice(Convert.ToInt32(sourcedt.Rows[0][1])); //物料单价 sourcedt.Rows[0][5];
                    rows[7] = 0;                    //物料成本(设置为0)
                    rows[8] = DBNull.Value;         //备注
                    gridViewdt.EndInit();
                }
                //操作完成后进行刷新
                OnInitialize(gridViewdt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 当GridView单元格值发生变化时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gvdtl_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var colindex = e.ColumnIndex;
                //判断若所选中的行中的Materialid没有值(注:并且所填的项不为‘物料名称’),即不能执行运算
                if (gvdtl.Rows[e.RowIndex].Cells[1].Value == DBNull.Value && colindex != 3)
                {
                    //需将不合法的行删除
                    gvdtl.Rows.RemoveAt(gvdtl.RowCount-2);
                    throw new Exception($"不能在没有物料编码的前提下填写用量或单价, \n 请删除并通过右键菜单进行添加新物料");
                }
                //当修改的列是‘物料名称’时,执行以下语句
                if (colindex == 3)
                {
                    //获取该行的‘物料ID’,更新使用(当为-1时表示新行插入,其它为更新)
                    var materialid = gvdtl.Rows[e.RowIndex].Cells[1].Value == DBNull.Value ? Convert.ToInt32(-1) : Convert.ToInt32(gvdtl.Rows[e.RowIndex].Cells[1].Value);
                    //获取所填写的‘物料名称’记录
                    var materialname = Convert.ToString(gvdtl.Rows[e.RowIndex].Cells[3].Value);
                    //根据‘物料名称’放到_materialdt进行查询
                    var dtlrows = _materialdt.Select("物料名称 like '%" + materialname + "%'");

                    //若没有记录的话,就执行如下
                    //->change date:20191214:当发现所输入的物料名称没有在_materialdt存在时,不作异常提示,而是可正常输入,但FMATERIALID从0开始,并且‘物料名称’为空
                    if (dtlrows.Length == 0)
                    {
                        GetMaterialDeatail(materialid , materialname , dtlrows);
                        #region Hide
                        //if (_dtl.Rows.Count == 0)
                        //{
                        //    gvdtl.Rows.RemoveAt(gvdtl.RowCount - 2);
                        //}
                        //else
                        //{
                        //    //刷新记录,将原来填写的值还原
                        //    OnInitialize(_dtl);
                        //}
                        //throw new Exception($"找不到关于'{materialname}'物料名称的相关记录, \n 请重新进行填写");
                        #endregion
                    }
                    //若只有一行的话,就执行以下语句
                    if (dtlrows.Length == 1)
                    {
                        GetMaterialDeatail(materialid, materialname, dtlrows);
                    }
                    //当发现有多行的话,就作出提示并执行以下语句
                    else if (dtlrows.Length > 1)
                    {
                        var clickMessage = $"检测到所输入的物料名称'{materialname}'有多条可选择的记录, \n 是否继续?";
                        if (MessageBox.Show(clickMessage, $"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            GetMaterialDeatail(materialid,materialname,dtlrows);
                        }
                        else
                        {
                            //若不继续就刷新GridView
                            OnInitialize(Dtl);
                        }
                    }
                }
                //当修改的列是‘配方用量’或‘物料单价(含税)’时,将以下关联的值作出改变
                else if (colindex == 4 || colindex == 6)
                {
                    //获取当前行的配方用量(注:若为空就为0)
                    var peiqty = Convert.ToDecimal(gvdtl.Rows[e.RowIndex].Cells[4].Value == DBNull.Value ? 0 : gvdtl.Rows[e.RowIndex].Cells[4].Value);
                    //计算‘占比’=配方用量*100
                    var ratio = peiqty*100;
                    //获取当前行的物料单价
                    var materialprice = Convert.ToDecimal(gvdtl.Rows[e.RowIndex].Cells[6].Value == DBNull.Value ? 0 : gvdtl.Rows[e.RowIndex].Cells[6].Value);
                    //计算‘物料成本(含税)’项 公式:配方用量/100*物料单价
                    var qtytemp = decimal.Round( peiqty / 100 * materialprice,4);
                    //根据‘物料编码ID’更新_dtl内的对应的'物料成本(含税)' 目的:更新_dtl
                    UpdateGridViewValue(materialprice,peiqty,ratio,Convert.ToInt32(gvdtl.Rows[e.RowIndex].Cells[1].Value), qtytemp);
                    //根据指定值将相关项进行改变指定文本框内的值
                    GenerateValue();
                    //操作完成后进行刷新
                    OnInitialize(Dtl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 查询及获取对应的物料信息
        /// </summary>
        /// 若发现一对一的关系,并且fmaterialid为空,就执行InsertDtToGridView(),反之执行UpdateDtToGridView()
        /// 若发现一对多关系,并且fmaterialid为空,就执行InsertRecordToGridView(),反之执行UpdateRecordToGridView() 
        /// <param name="materialid"></param>
        /// <param name="materialname"></param>
        /// <param name="dtlrows"></param>
        private void GetMaterialDeatail(int materialid,string materialname,DataRow[] dtlrows)
        {
            var resultTable = dbList.MakeGridViewTemp();

            //当dtlrows为0时,表示所输入的‘物料名称’不在K3里存在
            if (dtlrows.Length == 0)
            {
                //根据_dtl查询出‘物料编码’为空的Length,然后作为其最新的fmaterialid值
                var fmaterialid = Dtl.Select("物料编码 is null").Length;

                var newrow = resultTable.NewRow();
                newrow[1] = fmaterialid;     //物料编码ID
                newrow[2] = DBNull.Value;    //物料编码
                newrow[3] = materialname;    //物料名称
                newrow[6] = DBNull.Value;    //物料单价
                resultTable.Rows.Add(newrow);

                //执行插入
                if (materialid == Convert.ToInt32(-1))
                {
                    //先将在GridView内填写的行删除
                    gvdtl.Rows.RemoveAt(gvdtl.RowCount - 2);
                    //再进行插入操作
                    InsertDtToGridView(resultTable);
                }
                //执行更新
                else
                { 
                    UpdateDtToGridView(materialid,resultTable);
                }
            }
            //当dtlrows为1时,表示将K3记录插入
            else if (dtlrows.Length == 1)
            {
                //将相关结果插入至resultTable内
                var newrow = resultTable.NewRow();
                newrow[1] = dtlrows[0][0];   //物料编码ID
                newrow[2] = dtlrows[0][1];   //物料编码
                newrow[3] = dtlrows[0][2];   //物料名称
                newrow[6] = dtlrows[0][6];   //物料单价 
                resultTable.Rows.Add(newrow);

                //执行插入(FMATERIALID为空时表示新行,需插入)
                if (materialid == Convert.ToInt32(-1))
                {
                    //先将在GridView内填写的行删除
                    gvdtl.Rows.RemoveAt(gvdtl.RowCount - 2);
                    //再进行插入操作
                    InsertDtToGridView(resultTable);
                }
                //执行更新
                else
                {
                    UpdateDtToGridView(materialid,resultTable);
                }
            }
            //多行结果时执行
            else
            {
                //执行插入(FMATERIALID为空时表示新行,需插入)
                if (materialid == Convert.ToInt32(-1))
                {
                    ////先将在GridView内填写的行删除
                    gvdtl.Rows.RemoveAt(gvdtl.RowCount - 2);
                    //再进行插入操作
                    InsertRecordToGridView("A",materialname);
                }
                //执行更新
                else
                {
                    UpdateRecordToGridView(materialid, "U",materialname);
                }
            }
        }

        /// <summary>
        /// 累加‘物料成本(含税)’
        /// </summary>
        /// <returns></returns>
        private decimal GernerateSumQty()
        {
            decimal result = 0;

            foreach (DataRow rows in Dtl.Rows)
            {
                if (rows[7] == DBNull.Value) continue;
                //累加‘物料成本(含税)’
                result += Convert.ToDecimal(rows[7]);
            }
            return result;
        }

        /// <summary>
        /// 累加‘配方用量’
        /// </summary>
        /// <returns></returns>
        private string GenerateSumpeitotal()
        {
            var result = string.Empty;
            decimal tempqty = 0;

            foreach (DataRow rows in Dtl.Rows)
            {
                if(rows[4]==DBNull.Value) continue;
                //累加‘配方用量’
                tempqty += Convert.ToDecimal(rows[4]);
            }
            result = Convert.ToString(tempqty);
            return result;
        }

        /// <summary>
        /// 获取字符串中的数字('包装规格'使用)
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>数字</returns>
        public decimal GetNumberInt(string str)
        {
            var result = 0;
            if (!string.IsNullOrEmpty(str))
            {
                // 正则表达式剔除非数字字符（不包含小数点.）
                str = Regex.Replace(str, @"[^\d.\d]", "");
                //当判断到str值是空的,就返回1,返回对应值
                if (str == "")
                {
                    result = 1;
                }
                else
                {
                    // 如果是数字，则转换为decimal类型
                    if (Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$"))
                    {
                        result = int.Parse(str);
                    }
                }
                
            }
            return result;
        }

        /// <summary>
        /// 根据指定值将相关文本框进行改变
        /// </summary>
        private void GenerateValue()
        {
            //获取累加的‘物料成本(含税)’之和
            var materialsumqty = GernerateSumQty();

            //累加并获取‘配方用量’合计
            txtpeitotal.Text = GenerateSumpeitotal();

            //产品成本含税
            txtprice.Text = Convert.ToString(Math.Round(materialsumqty,4));

            //材料成本(不含税) 公式:物料成本之和(产品成本含税)/1.13
            txtmaterial.Text = Convert.ToString(Math.Round(materialsumqty / Convert.ToDecimal(1.13), 4));

            //成本(元/KG) 公式:材料成本+包装成本(自填)+人工制造费用(自填)=>已取消
            //change date:20200225:成本(元/KG)=产品成本含税+包装成本(自填)+人工制造费用(自填)
            //txtkg.Text = Convert.ToString(Math.Round(Convert.ToDecimal(txtmaterial.Text) + Convert.ToDecimal(txtbaochenben.Text) + Convert.ToDecimal(txtren.Text),4));
            txtkg.Text= Convert.ToString(Math.Round(Convert.ToDecimal(txtprice.Text) + Convert.ToDecimal(txtbaochenben.Text) + Convert.ToDecimal(txtren.Text), 4));
            //成本(元/L) 公式:成本(元/KG)*产品密度(KG/L)
            txtl.Text = Convert.ToString(Math.Round(Convert.ToDecimal(txtkg.Text) * Convert.ToDecimal(txtmi.Text),4));
            //50%报价   公式:成本(元/KG)/(1-50/100)
            txt50.Text = Convert.ToString(Math.Round(Convert.ToDecimal(txtkg.Text) / Convert.ToDecimal(0.5), 4));
            //45%报价   公式:成本(元/KG)/(1-45/100)     
            txt45.Text = Convert.ToString(Math.Round(Convert.ToDecimal(txtkg.Text) / Convert.ToDecimal(0.55), 4));
            //40%报价   公式:成本(元/KG)/(1-40/100)       
            txt40.Text = Convert.ToString(Math.Round(Convert.ToDecimal(txtkg.Text) / Convert.ToDecimal(0.6), 4));
        }

        /// <summary>
        /// 利用指定值更新_dtl内的'物料成本(含税)'
        /// </summary>
        /// <param name="materialprice">物料单价</param>
        /// <param name="peiqty">配方用量</param>
        /// <param name="ratio">占比</param>
        /// <param name="fmaterialid">物料ID</param>
        /// <param name="value">物料成本(含税) 中间值</param>
        private void UpdateGridViewValue(decimal materialprice, decimal peiqty, decimal ratio,int fmaterialid,decimal value)
        {
            //针对_dtl循环其内容
            foreach (DataRow rows in Dtl.Rows)
            {
                if (Convert.ToInt32(rows[1]) == fmaterialid)
                {
                    Dtl.BeginInit();
                    rows[4] = peiqty;         //配方用量
                    rows[5] = ratio;          //占比
                    rows[6] = materialprice;  //物料单价
                    rows[7] = value;          //物料成本(含税)
                    Dtl.EndInit();
                }
            }
        }

        /// <summary>
        /// 新增记录至GridView(注:支持materialname不填的情况)
        /// </summary>
        /// <param name="remark">标记 A:新增 HI:历史记录新增</param>
        /// <param name="materialname">物料名称</param>
        private void InsertRecordToGridView(string remark,string materialname)
        {
            var sourcedt=new DataTable();

            //若remark="A",数据源就采用_materialdt,反之采用_historydt
            if (remark == "A")
            {
                //若materialname不为空,即以此为条件查询出相关值并放到sourcedt内
                sourcedt = materialname != "" ? GetMaterialDetailDt(materialname) : _materialdt;
            }
            else
            {
                sourcedt = _historydt;
            }

            showMaterial.Remark = remark;
            //初始化GridView
            showMaterial.OnInitializeGridView(sourcedt);
            showMaterial.StartPosition = FormStartPosition.CenterScreen;
            showMaterial.ShowDialog();

            //以下为返回相关记录回本窗体相关处理
            //判断若返回的DT为空的话,就不需要任何效果
            if (showMaterial.ResultTable == null || showMaterial.ResultTable.Rows.Count == 0) return;
            //将返回的结果赋值至GridView内(注:判断若返回的DT不为空或行数大于0才执行插入效果)
            if (showMaterial.ResultTable != null || showMaterial.ResultTable.Rows.Count > 0)
                InsertDtToGridView(showMaterial.ResultTable);
        }

        /// <summary>
        /// 更新记录至GridView
        /// </summary>
        /// <param name="materialId">物料ID 替换时使用</param>
        /// <param name="remark">标记 U:替换 HU:历史记录替换</param>
        /// <param name="materialname">物料名称</param>
        private void UpdateRecordToGridView(int materialId, string remark,string materialname)
        {
            var sourcedt = new DataTable();

            //若remark="U",数据源就采用_materialdt,反之采用_historydt
            if (remark == "U")
            {
                //若materialname不为空,即以此为条件查询出相关值并放到sourcedt内
                sourcedt = materialname != "" ? GetMaterialDetailDt(materialname) : _materialdt;
            }
            else
            {
                sourcedt = _historydt;
            }

            showMaterial.Remark = remark;
            //初始化GridView
            showMaterial.OnInitializeGridView(sourcedt);
            showMaterial.StartPosition = FormStartPosition.CenterScreen;
            showMaterial.ShowDialog();

            //以下为返回相关记录回本窗体相关处理
            //判断若返回的DT为空的话,就不需要任何效果
            if (showMaterial.ResultTable == null || showMaterial.ResultTable.Rows.Count == 0) return;
            //将返回的结果赋值至GridView内(注:判断若返回的DT不为空或行数大于0才执行更新效果)
            if (showMaterial.ResultTable != null || showMaterial.ResultTable.Rows.Count > 0)
                UpdateDtToGridView(materialId, showMaterial.ResultTable);
        }

        /// <summary>
        /// 根据materialname获取其明细内容
        /// </summary>
        /// <returns></returns>
        private DataTable GetMaterialDetailDt(string materialname)
        {
            var dt = _materialdt.Clone();
            //根据materialname在_materialdt内查询并将结果插入至dt内
            var dtlrows = _materialdt.Select("物料名称 like '%"+ materialname +"%'");

            for (var i = 0; i < dtlrows.Length; i++)
            {
                var newrow = dt.NewRow();
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    newrow[j] = dtlrows[i][j];
                }
                dt.Rows.Add(newrow);
            }
            return dt;
        }

        /// <summary>
        /// 插入客户信息
        /// </summary>
        private void InsertCustRecordToTxt(DataTable sourcedt)
        {
            if (sourcedt.Rows.Count > 0)
            {
                txtcust.Text = Convert.ToString(sourcedt.Rows[0][0]);
            }
        }

    }
}
