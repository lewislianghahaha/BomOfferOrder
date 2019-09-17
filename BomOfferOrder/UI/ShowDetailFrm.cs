﻿using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BomOfferOrder.DB;

namespace BomOfferOrder.UI
{
    public partial class ShowDetailFrm : Form
    {
        DbList dbList=new DbList();
        ShowMaterialDetailFrm showMaterial=new ShowMaterialDetailFrm();

        #region 变量参数
            //获取‘原材料’DT
            private DataTable _materialdt;

            //保存查询出来的GridView记录
            private DataTable _dtl;
            //保存查询出来的角色权限记录
            private DataTable _userdt;

            //记录当前页数(GridView页面跳转使用)
            private int _pageCurrent = 1;
            //记录计算出来的总页数(GridView页面跳转使用)
            private int _totalpagecount;
            //记录初始化标记(GridView页面跳转 初始化时使用)
            private bool _pageChange;
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

            bnMoveFirstItem.Click += BnMoveFirstItem_Click;
            bnMovePreviousItem.Click += BnMovePreviousItem_Click;
            bnMoveNextItem.Click += BnMoveNextItem_Click;
            bnMoveLastItem.Click += BnMoveLastItem_Click;
            bnPositionItem.TextChanged += BnPositionItem_TextChanged;
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
                _dtl = gridViewdt;
                panel2.Visible = true;
                //初始化下拉框所选择的默认值
                tmshowrows.SelectedItem = "10";
                //定义初始化标记
                _pageChange = true;
                //GridView分页
                GridViewPageChange();
            }
            //注:当为空记录时,不显示跳转页;只需将临时表赋值至GridView内
            else
            {
                gvdtl.DataSource = gridViewdt;
                panel2.Visible = false;
            }
            //控制GridView单元格显示方式
            ControlGridViewisShow();
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
                showMaterial.Id = 0;
                //初始化GridView
                showMaterial.OnInitializeGridView(_materialdt);
                showMaterial.StartPosition = FormStartPosition.CenterScreen;
                showMaterial.ShowDialog();

                //以下为返回相关记录回本窗体相关处理
                //判断若返回的DT为空的话,就不需要任何效果
                if (showMaterial.ResultTable == null || showMaterial.ResultTable.Rows.Count == 0) return;
                //将返回的结果赋值至GridView内(注:判断若返回的DT不为空或行数大于0才执行插入效果)
                if (showMaterial.ResultTable != null || showMaterial.ResultTable.Rows.Count > 0)
                    InsertDtToGridView(showMaterial.ResultTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                //获取GridView内的主键ID
                var id = Convert.ToInt32(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[0].Value);

                showMaterial.Id = id;
                //初始化GridView
                showMaterial.OnInitializeGridView(_materialdt);
                showMaterial.StartPosition=FormStartPosition.CenterScreen;
                showMaterial.ShowDialog();

                //以下为返回相关记录回本窗体相关处理
                //判断若返回的DT为空的话,就不需要任何效果
                if (showMaterial.ResultTable == null || showMaterial.ResultTable.Rows.Count == 0) return;
                //将返回的结果赋值至GridView内(注:判断若返回的DT不为空或行数大于0才执行更新效果)
                if (showMaterial.ResultTable != null || showMaterial.ResultTable.Rows.Count > 0)
                    UpdateDtToGridView(id,showMaterial.ResultTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 将相关值根据获取过来的DT填充至对应的项内
        /// </summary>
        /// <param name="funState"></param>
        /// <param name="dt"></param>
        /// <param name="materialdt">原材料DT</param>
        public void AddDbToFrm(string funState,DataTable dt,DataTable materialdt)
        {
            try
            {
                //单据状态:创建 C
                if (funState == "C")
                {
                    //将‘原材料’DT赋值至变量内
                    _materialdt = materialdt;
                    FunStateCUse(funState,dt);
                }
                //单据状态:读取 R
                else
                {
                    FunStateRUse(funState,dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            //将相关值赋值给对应的文本框及GridView控件内
            txtname.Text = Convert.ToString(sourcedt.Rows[0][1]);   //产品名称
            txtbom.Text = Convert.ToString(sourcedt.Rows[0][2]);    //BOM编号
            txtbao.Text = Convert.ToString(sourcedt.Rows[0][3]);    //包装规格
            txtmi.Text = Convert.ToString(sourcedt.Rows[0][4]);     //产品密度
            //刷新GridView
            OnInitialize(GetGridViewdt(funState, sourcedt, resultdt));
        }

        /// <summary>
        /// 单据状态为R时使用
        /// </summary>
        /// <param name="funState"></param>
        /// <param name="sourcedt"></param>
        private void FunStateRUse(string funState,DataTable sourcedt)
        {
            //获取临时表(GridView控件时使用) 注:‘创建’及‘读取’也会使用到
            var resultdt = dbList.MakeGridViewTemp();
            //将相关值赋值给对应的文本框及GridView控件内

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
            var entryid = 1;

            try
            {
                //‘创建’状态
                if (funState == "C")
                {
                    //循环获取值赋给对应的控件内
                    foreach (DataRow rows in sourcedt.Rows)
                    {
                        var newrow = resultdt.NewRow();
                        newrow[0] = entryid;   //EntryId
                        newrow[1] = rows[5];   //物料编码ID
                        newrow[2] = rows[6];   //物料编码
                        newrow[3] = rows[7];   //物料名称
                        newrow[4] = rows[8];   //配方用量
                        newrow[5] = rows[10];  //物料单价(含税)
                        resultdt.Rows.Add(newrow);
                        entryid++;
                    }
                }
                //‘读取’状态
                else
                {
                    
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
            gvdtl.Columns[0].Visible = false;  //EntryID
            gvdtl.Columns[1].Visible = false;  //物料ID
            gvdtl.Columns[2].ReadOnly = true; //物料编码
            gvdtl.Columns[3].ReadOnly = true; //物料名称
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
        private void BnPositionItem_TextChanged(object sender, EventArgs e)
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
                var dtltotalrows = _dtl.Rows.Count;
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
                        bnMoveNextItem.Enabled = false;
                        bnMoveLastItem.Enabled = false;
                        bnMoveNextItem.Enabled = false;
                        bnMoveLastItem.Enabled = false;
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
                tstotalrow.Text = $"共 {_dtl.Rows.Count} 行";

                //根据“当前页” 及 “固定行数” 计算出新的行数记录并进行赋值
                //计算进行循环的起始行
                var startrow = (_pageCurrent - 1) * pageCount;
                //计算进行循环的结束行
                var endrow = _pageCurrent == _totalpagecount ? dtltotalrows : _pageCurrent * pageCount;
                //复制 查询的DT的列信息（不包括行）至临时表内
                var tempdt = _dtl.Clone();
                //循环将所需的_dtl的行记录复制至临时表内
                for (var i = startrow; i < endrow; i++)
                {
                    tempdt.ImportRow(_dtl.Rows[i]);
                }

                //最后将刷新的DT重新赋值给GridView
                gvdtl.DataSource = tempdt;
                //将“当前页”赋值给"跳转页"文本框内
                bnPositionItem.Text = Convert.ToString(_pageCurrent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                var gridViewdt = (DataTable)gvdtl.DataSource;
                //获取及计算当前GridView的最大行数(后面累加entryid使用)
                var entryid = gridViewdt.Rows.Count+1;

                //循环将获取过来的值插入至GridView内
                foreach (DataRow rows in sourcedt.Rows)
                {
                    //判断若获取过来的物料ID已在GridView内存在,即跳过不作添加
                    if(gridViewdt.Select("物料编码ID='" + rows[1] + "'").Length>0) continue;

                    var newrow = gridViewdt.NewRow();
                    newrow[0] = entryid;        //EntryId
                    newrow[1] = rows[1];        //物料编码ID
                    newrow[2] = rows[2];        //物料编码
                    newrow[3] = rows[3];        //物料名称
                    newrow[5] = rows[5];        //物料单价
                    gridViewdt.Rows.Add(newrow);
                    entryid++;
                }
                //操作完成后进行刷新
                OnInitialize(gridViewdt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 将获取的值更新至指定的GridView行内(注:只能一行使用)
        /// 判断若sourcedt内的物料ID已在GridView内存在,即跳出异常不能继续替换操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sourcedt"></param>
        private void UpdateDtToGridView(int id,DataTable sourcedt)
        {
            try
            {
                //循环GridView内的值,当发现ID与条件ID相同,即进入行更新
                //将GridView内的内容赋值到DT
                var gridViewdt = (DataTable)gvdtl.DataSource;
                //判断若sourcedt内的物料ID已在GridView内存在,即跳出异常不能继续替换操作
                if(gridViewdt.Select("物料编码ID='"+ sourcedt.Rows[0][1] +"'").Length>0)
                    throw new Exception($"获取物料'{sourcedt.Rows[0][2]}'已存在,故不能进行替换,请重新选择其它物料");

                foreach (DataRow rows in gridViewdt.Rows)
                {
                    //判断若ID相同,就执行更新操作
                    if (Convert.ToInt32(rows[0]) != id) continue;
                    gridViewdt.BeginInit();
                    rows[1] = sourcedt.Rows[0][1];  //物料编码ID
                    rows[2] = sourcedt.Rows[0][2];  //物料编码
                    rows[3] = sourcedt.Rows[0][3];  //物料名称
                    rows[4] = DBNull.Value;         //用量(清空)
                    rows[5] = sourcedt.Rows[0][5];  //物料单价
                    gridViewdt.EndInit();
                }
                //操作完成后进行刷新
                OnInitialize(gridViewdt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
