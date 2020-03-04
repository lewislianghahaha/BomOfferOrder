namespace BomOfferOrder.UI
{
    partial class SearchFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchFrm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtvalue = new System.Windows.Forms.TextBox();
            this.dtpdt = new System.Windows.Forms.DateTimePicker();
            this.comstatus = new System.Windows.Forms.ComboBox();
            this.btnsearch = new System.Windows.Forms.Button();
            this.comselectvalue = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.bngat = new System.Windows.Forms.BindingNavigator(this.components);
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
            this.panel3 = new System.Windows.Forms.Panel();
            this.gvdtl = new System.Windows.Forms.DataGridView();
            this.Menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tmshowdetail = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tmbackconfirm = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bngat)).BeginInit();
            this.bngat.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvdtl)).BeginInit();
            this.Menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtvalue);
            this.panel1.Controls.Add(this.dtpdt);
            this.panel1.Controls.Add(this.comstatus);
            this.panel1.Controls.Add(this.btnsearch);
            this.panel1.Controls.Add(this.comselectvalue);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(929, 29);
            this.panel1.TabIndex = 0;
            // 
            // txtvalue
            // 
            this.txtvalue.Location = new System.Drawing.Point(155, 3);
            this.txtvalue.Name = "txtvalue";
            this.txtvalue.Size = new System.Drawing.Size(172, 21);
            this.txtvalue.TabIndex = 12;
            // 
            // dtpdt
            // 
            this.dtpdt.Location = new System.Drawing.Point(155, 3);
            this.dtpdt.Name = "dtpdt";
            this.dtpdt.Size = new System.Drawing.Size(172, 21);
            this.dtpdt.TabIndex = 20;
            // 
            // comstatus
            // 
            this.comstatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comstatus.FormattingEnabled = true;
            this.comstatus.Location = new System.Drawing.Point(154, 3);
            this.comstatus.Name = "comstatus";
            this.comstatus.Size = new System.Drawing.Size(121, 20);
            this.comstatus.TabIndex = 22;
            // 
            // btnsearch
            // 
            this.btnsearch.Location = new System.Drawing.Point(339, 2);
            this.btnsearch.Name = "btnsearch";
            this.btnsearch.Size = new System.Drawing.Size(75, 23);
            this.btnsearch.TabIndex = 21;
            this.btnsearch.Text = "查询";
            this.btnsearch.UseVisualStyleBackColor = true;
            // 
            // comselectvalue
            // 
            this.comselectvalue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comselectvalue.FormattingEnabled = true;
            this.comselectvalue.Location = new System.Drawing.Point(11, 3);
            this.comselectvalue.Name = "comselectvalue";
            this.comselectvalue.Size = new System.Drawing.Size(138, 20);
            this.comselectvalue.TabIndex = 18;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.bngat);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 559);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(929, 26);
            this.panel2.TabIndex = 1;
            // 
            // bngat
            // 
            this.bngat.AddNewItem = null;
            this.bngat.CountItem = this.bnCountItem;
            this.bngat.CountItemFormat = "/ {0} 页";
            this.bngat.DeleteItem = null;
            this.bngat.Dock = System.Windows.Forms.DockStyle.Right;
            this.bngat.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.bngat.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.bngat.Location = new System.Drawing.Point(441, 0);
            this.bngat.MoveFirstItem = this.bnMoveFirstItem;
            this.bngat.MoveLastItem = this.bnMoveLastItem;
            this.bngat.MoveNextItem = this.bnMoveNextItem;
            this.bngat.MovePreviousItem = this.bnMovePreviousItem;
            this.bngat.Name = "bngat";
            this.bngat.PositionItem = this.bnPositionItem;
            this.bngat.Size = new System.Drawing.Size(486, 24);
            this.bngat.TabIndex = 0;
            this.bngat.Text = "bindingNavigator1";
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
            // panel3
            // 
            this.panel3.Controls.Add(this.gvdtl);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 29);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(929, 530);
            this.panel3.TabIndex = 2;
            // 
            // gvdtl
            // 
            this.gvdtl.AllowUserToAddRows = false;
            this.gvdtl.AllowUserToDeleteRows = false;
            this.gvdtl.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvdtl.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvdtl.ContextMenuStrip = this.Menu;
            this.gvdtl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvdtl.Location = new System.Drawing.Point(0, 0);
            this.gvdtl.Name = "gvdtl";
            this.gvdtl.ReadOnly = true;
            this.gvdtl.RowTemplate.Height = 23;
            this.gvdtl.Size = new System.Drawing.Size(929, 530);
            this.gvdtl.TabIndex = 0;
            // 
            // Menu
            // 
            this.Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmshowdetail,
            this.toolStripSeparator1,
            this.tmbackconfirm});
            this.Menu.Name = "Menu";
            this.Menu.Size = new System.Drawing.Size(125, 54);
            // 
            // tmshowdetail
            // 
            this.tmshowdetail.Name = "tmshowdetail";
            this.tmshowdetail.Size = new System.Drawing.Size(124, 22);
            this.tmshowdetail.Text = "查询明细";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(121, 6);
            // 
            // tmbackconfirm
            // 
            this.tmbackconfirm.Name = "tmbackconfirm";
            this.tmbackconfirm.Size = new System.Drawing.Size(124, 22);
            this.tmbackconfirm.Text = "反审核";
            // 
            // SearchFrm
            // 
            this.AcceptButton = this.btnsearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 585);
            this.ControlBox = false;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "SearchFrm";
            this.Text = "成本BOM报价单-查询";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bngat)).EndInit();
            this.bngat.ResumeLayout(false);
            this.bngat.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvdtl)).EndInit();
            this.Menu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnsearch;
        private System.Windows.Forms.DateTimePicker dtpdt;
        private System.Windows.Forms.ComboBox comselectvalue;
        private System.Windows.Forms.TextBox txtvalue;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView gvdtl;
        private System.Windows.Forms.BindingNavigator bngat;
        private System.Windows.Forms.ToolStripLabel bnCountItem;
        private System.Windows.Forms.ToolStripButton bnMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bnMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bnPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bnMoveNextItem;
        private System.Windows.Forms.ToolStripButton bnMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox tmshowrows;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripLabel tstotalrow;
        private System.Windows.Forms.ContextMenuStrip Menu;
        private System.Windows.Forms.ToolStripMenuItem tmshowdetail;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tmbackconfirm;
        private System.Windows.Forms.ComboBox comstatus;
    }
}