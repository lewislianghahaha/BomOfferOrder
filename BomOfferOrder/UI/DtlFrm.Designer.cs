﻿namespace BomOfferOrder.UI
{
    partial class DtlFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DtlFrm));
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.tmConfirm = new System.Windows.Forms.ToolStripMenuItem();
            this.tmsave = new System.Windows.Forms.ToolStripMenuItem();
            this.tmaddpage = new System.Windows.Forms.ToolStripMenuItem();
            this.tmCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tmfresh = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pbimg = new System.Windows.Forms.PictureBox();
            this.txtbom = new System.Windows.Forms.TextBox();
            this.lblmessage = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tctotalpage = new System.Windows.Forms.TabControl();
            this.label2 = new System.Windows.Forms.Label();
            this.comdevgroup = new System.Windows.Forms.ComboBox();
            this.MainMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbimg)).BeginInit();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmConfirm,
            this.tmsave,
            this.tmaddpage,
            this.tmCopy,
            this.tmfresh});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(854, 25);
            this.MainMenu.TabIndex = 0;
            // 
            // tmConfirm
            // 
            this.tmConfirm.Name = "tmConfirm";
            this.tmConfirm.Size = new System.Drawing.Size(44, 21);
            this.tmConfirm.Text = "审核";
            // 
            // tmsave
            // 
            this.tmsave.Name = "tmsave";
            this.tmsave.Size = new System.Drawing.Size(44, 21);
            this.tmsave.Text = "提交";
            // 
            // tmaddpage
            // 
            this.tmaddpage.Name = "tmaddpage";
            this.tmaddpage.Size = new System.Drawing.Size(68, 21);
            this.tmaddpage.Text = "添加新页";
            this.tmaddpage.Visible = false;
            // 
            // tmCopy
            // 
            this.tmCopy.Name = "tmCopy";
            this.tmCopy.Size = new System.Drawing.Size(44, 21);
            this.tmCopy.Text = "复制";
            this.tmCopy.Visible = false;
            // 
            // tmfresh
            // 
            this.tmfresh.Name = "tmfresh";
            this.tmfresh.Size = new System.Drawing.Size(44, 21);
            this.tmfresh.Text = "暂存";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.comdevgroup);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.pbimg);
            this.panel1.Controls.Add(this.txtbom);
            this.panel1.Controls.Add(this.lblmessage);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(854, 29);
            this.panel1.TabIndex = 1;
            // 
            // pbimg
            // 
            this.pbimg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbimg.Location = new System.Drawing.Point(770, -26);
            this.pbimg.Name = "pbimg";
            this.pbimg.Size = new System.Drawing.Size(74, 60);
            this.pbimg.TabIndex = 13;
            this.pbimg.TabStop = false;
            // 
            // txtbom
            // 
            this.txtbom.Location = new System.Drawing.Point(76, 3);
            this.txtbom.Name = "txtbom";
            this.txtbom.Size = new System.Drawing.Size(248, 21);
            this.txtbom.TabIndex = 1;
            // 
            // lblmessage
            // 
            this.lblmessage.AutoSize = true;
            this.lblmessage.Location = new System.Drawing.Point(331, 8);
            this.lblmessage.Name = "lblmessage";
            this.lblmessage.Size = new System.Drawing.Size(0, 12);
            this.lblmessage.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "OA流水号:";
            // 
            // tctotalpage
            // 
            this.tctotalpage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tctotalpage.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tctotalpage.Location = new System.Drawing.Point(0, 54);
            this.tctotalpage.Name = "tctotalpage";
            this.tctotalpage.SelectedIndex = 0;
            this.tctotalpage.Size = new System.Drawing.Size(854, 497);
            this.tctotalpage.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(434, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 15;
            this.label2.Text = "研发类别:";
            // 
            // comdevgroup
            // 
            this.comdevgroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comdevgroup.FormattingEnabled = true;
            this.comdevgroup.Location = new System.Drawing.Point(495, 3);
            this.comdevgroup.Name = "comdevgroup";
            this.comdevgroup.Size = new System.Drawing.Size(138, 20);
            this.comdevgroup.TabIndex = 16;
            // 
            // DtlFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 551);
            this.Controls.Add(this.tctotalpage);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.MainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MainMenu;
            this.Name = "DtlFrm";
            this.Text = "明细记录生成";
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbimg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem tmConfirm;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtbom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tctotalpage;
        private System.Windows.Forms.ToolStripMenuItem tmsave;
        private System.Windows.Forms.PictureBox pbimg;
        private System.Windows.Forms.Label lblmessage;
        private System.Windows.Forms.ToolStripMenuItem tmaddpage;
        private System.Windows.Forms.ToolStripMenuItem tmCopy;
        private System.Windows.Forms.ToolStripMenuItem tmfresh;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comdevgroup;
    }
}