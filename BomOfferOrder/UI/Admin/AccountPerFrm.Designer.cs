namespace BomOfferOrder.UI.Admin
{
    partial class AccountPerFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountPerFrm));
            this.Main = new System.Windows.Forms.MenuStrip();
            this.tmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tmclose = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbaddid = new System.Windows.Forms.CheckBox();
            this.cbreadid = new System.Windows.Forms.CheckBox();
            this.cbbackconfirm = new System.Windows.Forms.CheckBox();
            this.cbapplyid = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtphone = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtGroup = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtusername = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.panel5 = new System.Windows.Forms.Panel();
            this.gvdtl = new System.Windows.Forms.DataGridView();
            this.panel4 = new System.Windows.Forms.Panel();
            this.bngat = new System.Windows.Forms.BindingNavigator(this.components);
            this.bnCountItemtemp = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.bnPositionItemtemp = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel7 = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.bnMoveFirstItemtemp = new System.Windows.Forms.ToolStripButton();
            this.bnMovePreviousItemtemp = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.bnMoveNextItemtemp = new System.Windows.Forms.ToolStripButton();
            this.bnMoveLastItemtemp = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel8 = new System.Windows.Forms.ToolStripLabel();
            this.tmshowrowstemp = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel9 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel10 = new System.Windows.Forms.ToolStripLabel();
            this.tstotalrowtemp = new System.Windows.Forms.ToolStripLabel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.Main.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvdtl)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bngat)).BeginInit();
            this.bngat.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // Main
            // 
            this.Main.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmSave,
            this.tmclose});
            this.Main.Location = new System.Drawing.Point(0, 0);
            this.Main.Name = "Main";
            this.Main.Size = new System.Drawing.Size(1042, 25);
            this.Main.TabIndex = 1;
            this.Main.Text = "menuStrip1";
            // 
            // tmSave
            // 
            this.tmSave.Name = "tmSave";
            this.tmSave.Size = new System.Drawing.Size(44, 21);
            this.tmSave.Text = "保存";
            // 
            // tmclose
            // 
            this.tmclose.Name = "tmclose";
            this.tmclose.Size = new System.Drawing.Size(44, 21);
            this.tmclose.Text = "关闭";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1042, 81);
            this.panel1.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.groupBox2.Controls.Add(this.cbaddid);
            this.groupBox2.Controls.Add(this.cbreadid);
            this.groupBox2.Controls.Add(this.cbbackconfirm);
            this.groupBox2.Controls.Add(this.cbapplyid);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 43);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1040, 37);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "功能权限";
            // 
            // cbaddid
            // 
            this.cbaddid.AutoSize = true;
            this.cbaddid.Location = new System.Drawing.Point(516, 16);
            this.cbaddid.Name = "cbaddid";
            this.cbaddid.Size = new System.Drawing.Size(264, 16);
            this.cbaddid.TabIndex = 8;
            this.cbaddid.Text = "是否可操作物料明细及设置配方用量为可修改";
            this.cbaddid.UseVisualStyleBackColor = true;
            // 
            // cbreadid
            // 
            this.cbreadid.AutoSize = true;
            this.cbreadid.Location = new System.Drawing.Point(338, 16);
            this.cbreadid.Name = "cbreadid";
            this.cbreadid.Size = new System.Drawing.Size(132, 16);
            this.cbreadid.TabIndex = 7;
            this.cbreadid.Text = "是否可查询明细金额";
            this.cbreadid.UseVisualStyleBackColor = true;
            // 
            // cbbackconfirm
            // 
            this.cbbackconfirm.AutoSize = true;
            this.cbbackconfirm.Location = new System.Drawing.Point(183, 16);
            this.cbbackconfirm.Name = "cbbackconfirm";
            this.cbbackconfirm.Size = new System.Drawing.Size(96, 16);
            this.cbbackconfirm.TabIndex = 6;
            this.cbbackconfirm.Text = "是否可反审核";
            this.cbbackconfirm.UseVisualStyleBackColor = true;
            // 
            // cbapplyid
            // 
            this.cbapplyid.AutoSize = true;
            this.cbapplyid.Location = new System.Drawing.Point(59, 16);
            this.cbapplyid.Name = "cbapplyid";
            this.cbapplyid.Size = new System.Drawing.Size(72, 16);
            this.cbapplyid.TabIndex = 3;
            this.cbapplyid.Text = "是否启用";
            this.cbapplyid.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtphone);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtGroup);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtusername);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1040, 43);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "用户信息";
            // 
            // txtphone
            // 
            this.txtphone.Location = new System.Drawing.Point(731, 15);
            this.txtphone.Name = "txtphone";
            this.txtphone.ReadOnly = true;
            this.txtphone.Size = new System.Drawing.Size(213, 21);
            this.txtphone.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(670, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "用户手机:";
            // 
            // txtGroup
            // 
            this.txtGroup.Location = new System.Drawing.Point(438, 15);
            this.txtGroup.Name = "txtGroup";
            this.txtGroup.ReadOnly = true;
            this.txtGroup.Size = new System.Drawing.Size(213, 21);
            this.txtGroup.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(377, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "用户组别:";
            // 
            // txtusername
            // 
            this.txtusername.Location = new System.Drawing.Point(133, 15);
            this.txtusername.Name = "txtusername";
            this.txtusername.ReadOnly = true;
            this.txtusername.Size = new System.Drawing.Size(213, 21);
            this.txtusername.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(72, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "用户姓名:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 106);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1042, 480);
            this.panel2.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.splitContainer1);
            this.groupBox3.Controls.Add(this.panel3);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1042, 480);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "关联用户";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 39);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel5);
            this.splitContainer1.Panel2.Controls.Add(this.panel4);
            this.splitContainer1.Size = new System.Drawing.Size(1036, 438);
            this.splitContainer1.SplitterDistance = 184;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(184, 438);
            this.treeView1.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.gvdtl);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(848, 412);
            this.panel5.TabIndex = 1;
            // 
            // gvdtl
            // 
            this.gvdtl.AllowUserToAddRows = false;
            this.gvdtl.AllowUserToDeleteRows = false;
            this.gvdtl.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvdtl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvdtl.Location = new System.Drawing.Point(0, 0);
            this.gvdtl.Name = "gvdtl";
            this.gvdtl.ReadOnly = true;
            this.gvdtl.RowTemplate.Height = 23;
            this.gvdtl.Size = new System.Drawing.Size(846, 410);
            this.gvdtl.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.bngat);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 412);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(848, 26);
            this.panel4.TabIndex = 0;
            // 
            // bngat
            // 
            this.bngat.AddNewItem = null;
            this.bngat.CountItem = this.bnCountItemtemp;
            this.bngat.CountItemFormat = "/ {0} 页";
            this.bngat.DeleteItem = null;
            this.bngat.Dock = System.Windows.Forms.DockStyle.Right;
            this.bngat.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel6,
            this.bnPositionItemtemp,
            this.toolStripLabel7,
            this.bnCountItemtemp,
            this.bindingNavigatorSeparator3,
            this.bnMoveFirstItemtemp,
            this.bnMovePreviousItemtemp,
            this.bindingNavigatorSeparator4,
            this.bnMoveNextItemtemp,
            this.bnMoveLastItemtemp,
            this.bindingNavigatorSeparator5,
            this.toolStripLabel8,
            this.tmshowrowstemp,
            this.toolStripLabel9,
            this.toolStripLabel10,
            this.tstotalrowtemp});
            this.bngat.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.bngat.Location = new System.Drawing.Point(329, 0);
            this.bngat.MoveFirstItem = this.bnMoveFirstItemtemp;
            this.bngat.MoveLastItem = this.bnMoveLastItemtemp;
            this.bngat.MoveNextItem = this.bnMoveNextItemtemp;
            this.bngat.MovePreviousItem = this.bnMovePreviousItemtemp;
            this.bngat.Name = "bngat";
            this.bngat.PositionItem = this.bnPositionItemtemp;
            this.bngat.Size = new System.Drawing.Size(517, 24);
            this.bngat.TabIndex = 4;
            this.bngat.Text = "bindingNavigator1";
            // 
            // bnCountItemtemp
            // 
            this.bnCountItemtemp.Name = "bnCountItemtemp";
            this.bnCountItemtemp.Size = new System.Drawing.Size(48, 21);
            this.bnCountItemtemp.Text = "/ {0} 页";
            this.bnCountItemtemp.ToolTipText = "总项数";
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(20, 21);
            this.toolStripLabel6.Text = "第";
            // 
            // bnPositionItemtemp
            // 
            this.bnPositionItemtemp.AccessibleName = "位置";
            this.bnPositionItemtemp.AutoSize = false;
            this.bnPositionItemtemp.Name = "bnPositionItemtemp";
            this.bnPositionItemtemp.Size = new System.Drawing.Size(50, 23);
            this.bnPositionItemtemp.Text = "0";
            this.bnPositionItemtemp.ToolTipText = "当前位置";
            // 
            // toolStripLabel7
            // 
            this.toolStripLabel7.Name = "toolStripLabel7";
            this.toolStripLabel7.Size = new System.Drawing.Size(20, 21);
            this.toolStripLabel7.Text = "页";
            // 
            // bindingNavigatorSeparator3
            // 
            this.bindingNavigatorSeparator3.Name = "bindingNavigatorSeparator3";
            this.bindingNavigatorSeparator3.Size = new System.Drawing.Size(6, 24);
            // 
            // bnMoveFirstItemtemp
            // 
            this.bnMoveFirstItemtemp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bnMoveFirstItemtemp.Image = ((System.Drawing.Image)(resources.GetObject("bnMoveFirstItemtemp.Image")));
            this.bnMoveFirstItemtemp.Name = "bnMoveFirstItemtemp";
            this.bnMoveFirstItemtemp.RightToLeftAutoMirrorImage = true;
            this.bnMoveFirstItemtemp.Size = new System.Drawing.Size(23, 21);
            this.bnMoveFirstItemtemp.Text = "移到第一条记录";
            // 
            // bnMovePreviousItemtemp
            // 
            this.bnMovePreviousItemtemp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bnMovePreviousItemtemp.Image = ((System.Drawing.Image)(resources.GetObject("bnMovePreviousItemtemp.Image")));
            this.bnMovePreviousItemtemp.Name = "bnMovePreviousItemtemp";
            this.bnMovePreviousItemtemp.RightToLeftAutoMirrorImage = true;
            this.bnMovePreviousItemtemp.Size = new System.Drawing.Size(23, 21);
            this.bnMovePreviousItemtemp.Text = "移到上一条记录";
            // 
            // bindingNavigatorSeparator4
            // 
            this.bindingNavigatorSeparator4.Name = "bindingNavigatorSeparator4";
            this.bindingNavigatorSeparator4.Size = new System.Drawing.Size(6, 24);
            // 
            // bnMoveNextItemtemp
            // 
            this.bnMoveNextItemtemp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bnMoveNextItemtemp.Image = ((System.Drawing.Image)(resources.GetObject("bnMoveNextItemtemp.Image")));
            this.bnMoveNextItemtemp.Name = "bnMoveNextItemtemp";
            this.bnMoveNextItemtemp.RightToLeftAutoMirrorImage = true;
            this.bnMoveNextItemtemp.Size = new System.Drawing.Size(23, 21);
            this.bnMoveNextItemtemp.Text = "移到下一条记录";
            // 
            // bnMoveLastItemtemp
            // 
            this.bnMoveLastItemtemp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bnMoveLastItemtemp.Image = ((System.Drawing.Image)(resources.GetObject("bnMoveLastItemtemp.Image")));
            this.bnMoveLastItemtemp.Name = "bnMoveLastItemtemp";
            this.bnMoveLastItemtemp.RightToLeftAutoMirrorImage = true;
            this.bnMoveLastItemtemp.Size = new System.Drawing.Size(23, 21);
            this.bnMoveLastItemtemp.Text = "移到最后一条记录";
            // 
            // bindingNavigatorSeparator5
            // 
            this.bindingNavigatorSeparator5.Name = "bindingNavigatorSeparator5";
            this.bindingNavigatorSeparator5.Size = new System.Drawing.Size(6, 24);
            // 
            // toolStripLabel8
            // 
            this.toolStripLabel8.Name = "toolStripLabel8";
            this.toolStripLabel8.Size = new System.Drawing.Size(59, 21);
            this.toolStripLabel8.Text = "每页显示:";
            // 
            // tmshowrowstemp
            // 
            this.tmshowrowstemp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tmshowrowstemp.Items.AddRange(new object[] {
            "10",
            "50",
            "100",
            "1000"});
            this.tmshowrowstemp.Name = "tmshowrowstemp";
            this.tmshowrowstemp.Size = new System.Drawing.Size(75, 24);
            // 
            // toolStripLabel9
            // 
            this.toolStripLabel9.Name = "toolStripLabel9";
            this.toolStripLabel9.Size = new System.Drawing.Size(20, 21);
            this.toolStripLabel9.Text = "行";
            // 
            // toolStripLabel10
            // 
            this.toolStripLabel10.Name = "toolStripLabel10";
            this.toolStripLabel10.Size = new System.Drawing.Size(13, 21);
            this.toolStripLabel10.Text = "/";
            // 
            // tstotalrowtemp
            // 
            this.tstotalrowtemp.Name = "tstotalrowtemp";
            this.tstotalrowtemp.Size = new System.Drawing.Size(55, 21);
            this.tstotalrowtemp.Text = "共 {0} 行";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.checkBox1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(3, 17);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1036, 22);
            this.panel3.TabIndex = 0;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(56, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(84, 16);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "不需要关联";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // AccountPerFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1042, 586);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Main);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AccountPerFrm";
            this.Text = "用户权限设置";
            this.Main.ResumeLayout(false);
            this.Main.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvdtl)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bngat)).EndInit();
            this.bngat.ResumeLayout(false);
            this.bngat.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip Main;
        private System.Windows.Forms.ToolStripMenuItem tmSave;
        private System.Windows.Forms.ToolStripMenuItem tmclose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtusername;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtGroup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtphone;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbapplyid;
        private System.Windows.Forms.CheckBox cbaddid;
        private System.Windows.Forms.CheckBox cbreadid;
        private System.Windows.Forms.CheckBox cbbackconfirm;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.DataGridView gvdtl;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.BindingNavigator bngat;
        private System.Windows.Forms.ToolStripLabel bnCountItemtemp;
        private System.Windows.Forms.ToolStripLabel toolStripLabel6;
        private System.Windows.Forms.ToolStripTextBox bnPositionItemtemp;
        private System.Windows.Forms.ToolStripLabel toolStripLabel7;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator3;
        private System.Windows.Forms.ToolStripButton bnMoveFirstItemtemp;
        private System.Windows.Forms.ToolStripButton bnMovePreviousItemtemp;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator4;
        private System.Windows.Forms.ToolStripButton bnMoveNextItemtemp;
        private System.Windows.Forms.ToolStripButton bnMoveLastItemtemp;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator5;
        private System.Windows.Forms.ToolStripLabel toolStripLabel8;
        private System.Windows.Forms.ToolStripComboBox tmshowrowstemp;
        private System.Windows.Forms.ToolStripLabel toolStripLabel9;
        private System.Windows.Forms.ToolStripLabel toolStripLabel10;
        private System.Windows.Forms.ToolStripLabel tstotalrowtemp;
    }
}