﻿namespace BomOfferOrder.UI.Admin.Basic
{
    partial class UserInfoList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserInfoList));
            this.Main = new System.Windows.Forms.MenuStrip();
            this.tmGet = new System.Windows.Forms.ToolStripMenuItem();
            this.tmClose = new System.Windows.Forms.ToolStripMenuItem();
            this.tctotalpage = new System.Windows.Forms.TabControl();
            this.Main.SuspendLayout();
            this.SuspendLayout();
            // 
            // Main
            // 
            this.Main.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmGet,
            this.tmClose});
            this.Main.Location = new System.Drawing.Point(0, 0);
            this.Main.Name = "Main";
            this.Main.Size = new System.Drawing.Size(576, 25);
            this.Main.TabIndex = 5;
            this.Main.Text = "menuStrip1";
            // 
            // tmGet
            // 
            this.tmGet.Name = "tmGet";
            this.tmGet.Size = new System.Drawing.Size(44, 21);
            this.tmGet.Text = "获取";
            // 
            // tmClose
            // 
            this.tmClose.Name = "tmClose";
            this.tmClose.Size = new System.Drawing.Size(44, 21);
            this.tmClose.Text = "关闭";
            // 
            // tctotalpage
            // 
            this.tctotalpage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tctotalpage.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tctotalpage.Location = new System.Drawing.Point(0, 25);
            this.tctotalpage.Name = "tctotalpage";
            this.tctotalpage.SelectedIndex = 0;
            this.tctotalpage.Size = new System.Drawing.Size(576, 276);
            this.tctotalpage.TabIndex = 6;
            // 
            // UserInfoList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 301);
            this.ControlBox = false;
            this.Controls.Add(this.tctotalpage);
            this.Controls.Add(this.Main);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UserInfoList";
            this.Text = "用户信息列表";
            this.Main.ResumeLayout(false);
            this.Main.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip Main;
        private System.Windows.Forms.ToolStripMenuItem tmGet;
        private System.Windows.Forms.ToolStripMenuItem tmClose;
        private System.Windows.Forms.TabControl tctotalpage;
    }
}