namespace BomOfferOrder.UI.Admin.Basic
{
    partial class BdUserGroupAddFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BdUserGroupAddFrm));
            this.Main = new System.Windows.Forms.MenuStrip();
            this.tmSet = new System.Windows.Forms.ToolStripMenuItem();
            this.tmClose = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.txtvalue = new System.Windows.Forms.TextBox();
            this.Main.SuspendLayout();
            this.SuspendLayout();
            // 
            // Main
            // 
            this.Main.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmSet,
            this.tmClose});
            this.Main.Location = new System.Drawing.Point(0, 0);
            this.Main.Name = "Main";
            this.Main.Size = new System.Drawing.Size(284, 25);
            this.Main.TabIndex = 1;
            this.Main.Text = "menuStrip1";
            // 
            // tmSet
            // 
            this.tmSet.Name = "tmSet";
            this.tmSet.Size = new System.Drawing.Size(44, 21);
            this.tmSet.Text = "设置";
            // 
            // tmClose
            // 
            this.tmClose.Name = "tmClose";
            this.tmClose.Size = new System.Drawing.Size(44, 21);
            this.tmClose.Text = "关闭";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "用户组别名称:";
            // 
            // txtvalue
            // 
            this.txtvalue.Location = new System.Drawing.Point(93, 33);
            this.txtvalue.Name = "txtvalue";
            this.txtvalue.Size = new System.Drawing.Size(179, 21);
            this.txtvalue.TabIndex = 3;
            // 
            // BdUserGroupAddFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 61);
            this.ControlBox = false;
            this.Controls.Add(this.txtvalue);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Main);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BdUserGroupAddFrm";
            this.Text = "用户组别添加/修改";
            this.Main.ResumeLayout(false);
            this.Main.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip Main;
        private System.Windows.Forms.ToolStripMenuItem tmSet;
        private System.Windows.Forms.ToolStripMenuItem tmClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtvalue;
    }
}