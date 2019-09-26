namespace BomOfferOrder.UI.Admin
{
    partial class AccountDetailFrm
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
            this.Main = new System.Windows.Forms.MenuStrip();
            this.tmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tmclose = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtphone = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtGroup = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtusername = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbaddid = new System.Windows.Forms.CheckBox();
            this.cbreadid = new System.Windows.Forms.CheckBox();
            this.cbbackconfirm = new System.Windows.Forms.CheckBox();
            this.cbapplyid = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.Main.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.Main.Size = new System.Drawing.Size(351, 25);
            this.Main.TabIndex = 0;
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
            this.panel1.Controls.Add(this.txtphone);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txtGroup);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtusername);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(351, 102);
            this.panel1.TabIndex = 1;
            // 
            // txtphone
            // 
            this.txtphone.Location = new System.Drawing.Point(76, 67);
            this.txtphone.Name = "txtphone";
            this.txtphone.ReadOnly = true;
            this.txtphone.Size = new System.Drawing.Size(213, 21);
            this.txtphone.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "用户手机:";
            // 
            // txtGroup
            // 
            this.txtGroup.Location = new System.Drawing.Point(76, 40);
            this.txtGroup.Name = "txtGroup";
            this.txtGroup.ReadOnly = true;
            this.txtGroup.Size = new System.Drawing.Size(213, 21);
            this.txtGroup.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "用户组别:";
            // 
            // txtusername
            // 
            this.txtusername.Location = new System.Drawing.Point(76, 12);
            this.txtusername.Name = "txtusername";
            this.txtusername.ReadOnly = true;
            this.txtusername.Size = new System.Drawing.Size(213, 21);
            this.txtusername.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "用户姓名:";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.cbaddid);
            this.panel2.Controls.Add(this.cbreadid);
            this.panel2.Controls.Add(this.cbbackconfirm);
            this.panel2.Controls.Add(this.cbapplyid);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 127);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(351, 126);
            this.panel2.TabIndex = 2;
            // 
            // cbaddid
            // 
            this.cbaddid.AutoSize = true;
            this.cbaddid.Location = new System.Drawing.Point(47, 94);
            this.cbaddid.Name = "cbaddid";
            this.cbaddid.Size = new System.Drawing.Size(258, 16);
            this.cbaddid.TabIndex = 5;
            this.cbaddid.Text = "是否可操作物料明细(包括:新增 替换 删除)";
            this.cbaddid.UseVisualStyleBackColor = true;
            // 
            // cbreadid
            // 
            this.cbreadid.AutoSize = true;
            this.cbreadid.Location = new System.Drawing.Point(47, 72);
            this.cbreadid.Name = "cbreadid";
            this.cbreadid.Size = new System.Drawing.Size(132, 16);
            this.cbreadid.TabIndex = 4;
            this.cbreadid.Text = "是否可查询明细金额";
            this.cbreadid.UseVisualStyleBackColor = true;
            // 
            // cbbackconfirm
            // 
            this.cbbackconfirm.AutoSize = true;
            this.cbbackconfirm.Location = new System.Drawing.Point(47, 50);
            this.cbbackconfirm.Name = "cbbackconfirm";
            this.cbbackconfirm.Size = new System.Drawing.Size(96, 16);
            this.cbbackconfirm.TabIndex = 3;
            this.cbbackconfirm.Text = "是否可反审核";
            this.cbbackconfirm.UseVisualStyleBackColor = true;
            // 
            // cbapplyid
            // 
            this.cbapplyid.AutoSize = true;
            this.cbapplyid.Location = new System.Drawing.Point(47, 28);
            this.cbapplyid.Name = "cbapplyid";
            this.cbapplyid.Size = new System.Drawing.Size(72, 16);
            this.cbapplyid.TabIndex = 2;
            this.cbapplyid.Text = "是否启用";
            this.cbapplyid.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.label5);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(349, 21);
            this.panel3.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(5, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "请勾选以下权限复选框";
            // 
            // AccountDetailFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 253);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Main);
            this.MainMenuStrip = this.Main;
            this.Name = "AccountDetailFrm";
            this.Text = "用户权限添加窗体";
            this.Main.ResumeLayout(false);
            this.Main.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
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
        private System.Windows.Forms.TextBox txtphone;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtGroup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtusername;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox cbreadid;
        private System.Windows.Forms.CheckBox cbbackconfirm;
        private System.Windows.Forms.CheckBox cbapplyid;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbaddid;
    }
}