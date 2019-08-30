namespace BomOfferOrder.UI
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.gvdtl = new System.Windows.Forms.DataGridView();
            this.DelMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AddMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.tmclose = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btngenerate = new System.Windows.Forms.Button();
            this.btnsearch = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gvsearchdtl = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.bindingNavigator1 = new System.Windows.Forms.BindingNavigator(this.components);
            this.bnCountItem = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.bnPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bnMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bnMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bnMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bnMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.tmshowrows = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.tstotalrow = new System.Windows.Forms.ToolStripLabel();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvdtl)).BeginInit();
            this.MainMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvsearchdtl)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).BeginInit();
            this.bindingNavigator1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.gvdtl);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 305);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(929, 280);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "添加明细记录(注:添加明细记录不能超过10行)";
            // 
            // gvdtl
            // 
            this.gvdtl.AllowUserToAddRows = false;
            this.gvdtl.AllowUserToDeleteRows = false;
            this.gvdtl.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvdtl.ContextMenuStrip = this.DelMenu;
            this.gvdtl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvdtl.Location = new System.Drawing.Point(3, 17);
            this.gvdtl.Name = "gvdtl";
            this.gvdtl.ReadOnly = true;
            this.gvdtl.RowTemplate.Height = 23;
            this.gvdtl.Size = new System.Drawing.Size(923, 260);
            this.gvdtl.TabIndex = 0;
            // 
            // DelMenu
            // 
            this.DelMenu.Name = "DelMenu";
            this.DelMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // AddMenu
            // 
            this.AddMenu.Name = "AddMenu";
            this.AddMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // MainMenu
            // 
            this.MainMenu.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmclose});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(929, 25);
            this.MainMenu.TabIndex = 6;
            // 
            // tmclose
            // 
            this.tmclose.Name = "tmclose";
            this.tmclose.Size = new System.Drawing.Size(44, 21);
            this.tmclose.Text = "关闭";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btngenerate);
            this.panel1.Controls.Add(this.btnsearch);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(929, 28);
            this.panel1.TabIndex = 7;
            // 
            // btngenerate
            // 
            this.btngenerate.Location = new System.Drawing.Point(600, 2);
            this.btngenerate.Name = "btngenerate";
            this.btngenerate.Size = new System.Drawing.Size(87, 23);
            this.btngenerate.TabIndex = 3;
            this.btngenerate.Text = "生成明细记录";
            this.btngenerate.UseVisualStyleBackColor = true;
            // 
            // btnsearch
            // 
            this.btnsearch.Location = new System.Drawing.Point(472, 2);
            this.btnsearch.Name = "btnsearch";
            this.btnsearch.Size = new System.Drawing.Size(75, 23);
            this.btnsearch.TabIndex = 2;
            this.btnsearch.Text = "查询";
            this.btnsearch.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(139, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(291, 21);
            this.textBox1.TabIndex = 1;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(11, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gvsearchdtl);
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(929, 252);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "查询明细记录";
            // 
            // gvsearchdtl
            // 
            this.gvsearchdtl.AllowUserToAddRows = false;
            this.gvsearchdtl.AllowUserToDeleteRows = false;
            this.gvsearchdtl.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvsearchdtl.ContextMenuStrip = this.AddMenu;
            this.gvsearchdtl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvsearchdtl.Location = new System.Drawing.Point(3, 17);
            this.gvsearchdtl.Name = "gvsearchdtl";
            this.gvsearchdtl.ReadOnly = true;
            this.gvsearchdtl.RowTemplate.Height = 23;
            this.gvsearchdtl.Size = new System.Drawing.Size(923, 206);
            this.gvsearchdtl.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.bindingNavigator1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(3, 223);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(923, 26);
            this.panel2.TabIndex = 0;
            // 
            // bindingNavigator1
            // 
            this.bindingNavigator1.AddNewItem = null;
            this.bindingNavigator1.CountItem = this.bnCountItem;
            this.bindingNavigator1.CountItemFormat = "/ {0} 页";
            this.bindingNavigator1.DeleteItem = null;
            this.bindingNavigator1.Dock = System.Windows.Forms.DockStyle.Right;
            this.bindingNavigator1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.bnPositionItem,
            this.toolStripLabel2,
            this.bnCountItem,
            this.bindingNavigatorSeparator,
            this.bnMoveFirstItem,
            this.bnMovePreviousItem,
            this.bindingNavigatorSeparator1,
            this.bnMoveNextItem,
            this.bnMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.toolStripLabel3,
            this.tmshowrows,
            this.toolStripLabel4,
            this.toolStripLabel5,
            this.tstotalrow});
            this.bindingNavigator1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.bindingNavigator1.Location = new System.Drawing.Point(404, 0);
            this.bindingNavigator1.MoveFirstItem = this.bnMoveFirstItem;
            this.bindingNavigator1.MoveLastItem = this.bnMoveLastItem;
            this.bindingNavigator1.MoveNextItem = this.bnMoveNextItem;
            this.bindingNavigator1.MovePreviousItem = this.bnMovePreviousItem;
            this.bindingNavigator1.Name = "bindingNavigator1";
            this.bindingNavigator1.PositionItem = this.bnPositionItem;
            this.bindingNavigator1.Size = new System.Drawing.Size(517, 24);
            this.bindingNavigator1.TabIndex = 0;
            this.bindingNavigator1.Text = "bindingNavigator1";
            // 
            // bnCountItem
            // 
            this.bnCountItem.Name = "bnCountItem";
            this.bnCountItem.Size = new System.Drawing.Size(48, 21);
            this.bnCountItem.Text = "/ {0} 页";
            this.bnCountItem.ToolTipText = "总项数";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(20, 21);
            this.toolStripLabel1.Text = "第";
            // 
            // bnPositionItem
            // 
            this.bnPositionItem.AccessibleName = "位置";
            this.bnPositionItem.AutoSize = false;
            this.bnPositionItem.Name = "bnPositionItem";
            this.bnPositionItem.Size = new System.Drawing.Size(50, 23);
            this.bnPositionItem.Text = "0";
            this.bnPositionItem.ToolTipText = "当前位置";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(20, 21);
            this.toolStripLabel2.Text = "页";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 24);
            // 
            // bnMoveFirstItem
            // 
            this.bnMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bnMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bnMoveFirstItem.Image")));
            this.bnMoveFirstItem.Name = "bnMoveFirstItem";
            this.bnMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bnMoveFirstItem.Size = new System.Drawing.Size(23, 21);
            this.bnMoveFirstItem.Text = "移到第一条记录";
            // 
            // bnMovePreviousItem
            // 
            this.bnMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bnMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bnMovePreviousItem.Image")));
            this.bnMovePreviousItem.Name = "bnMovePreviousItem";
            this.bnMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bnMovePreviousItem.Size = new System.Drawing.Size(23, 21);
            this.bnMovePreviousItem.Text = "移到上一条记录";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 24);
            // 
            // bnMoveNextItem
            // 
            this.bnMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bnMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bnMoveNextItem.Image")));
            this.bnMoveNextItem.Name = "bnMoveNextItem";
            this.bnMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bnMoveNextItem.Size = new System.Drawing.Size(23, 21);
            this.bnMoveNextItem.Text = "移到下一条记录";
            // 
            // bnMoveLastItem
            // 
            this.bnMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bnMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bnMoveLastItem.Image")));
            this.bnMoveLastItem.Name = "bnMoveLastItem";
            this.bnMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bnMoveLastItem.Size = new System.Drawing.Size(23, 21);
            this.bnMoveLastItem.Text = "移到最后一条记录";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 24);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(59, 21);
            this.toolStripLabel3.Text = "每页显示:";
            // 
            // tmshowrows
            // 
            this.tmshowrows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tmshowrows.Items.AddRange(new object[] {
            "10",
            "50",
            "100",
            "1000"});
            this.tmshowrows.Name = "tmshowrows";
            this.tmshowrows.Size = new System.Drawing.Size(75, 24);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(20, 21);
            this.toolStripLabel4.Text = "行";
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(13, 21);
            this.toolStripLabel5.Text = "/";
            // 
            // tstotalrow
            // 
            this.tstotalrow.Name = "tstotalrow";
            this.tstotalrow.Size = new System.Drawing.Size(55, 21);
            this.tstotalrow.Text = "共 {0} 行";
            // 
            // Main
            // 
            this.AcceptButton = this.btnsearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 585);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.MainMenu);
            this.Controls.Add(this.groupBox2);
            this.MainMenuStrip = this.MainMenu;
            this.Name = "Main";
            this.Text = "成本BOM-报价单相关修改";
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvdtl)).EndInit();
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvsearchdtl)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).EndInit();
            this.bindingNavigator1.ResumeLayout(false);
            this.bindingNavigator1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ContextMenuStrip AddMenu;
        private System.Windows.Forms.ContextMenuStrip DelMenu;
        private System.Windows.Forms.DataGridView gvdtl;
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem tmclose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btngenerate;
        private System.Windows.Forms.Button btnsearch;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.BindingNavigator bindingNavigator1;
        private System.Windows.Forms.ToolStripLabel bnCountItem;
        private System.Windows.Forms.ToolStripButton bnMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bnMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bnPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bnMoveNextItem;
        private System.Windows.Forms.ToolStripButton bnMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.DataGridView gvsearchdtl;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox tmshowrows;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripLabel tstotalrow;
    }
}

