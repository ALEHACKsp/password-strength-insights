namespace PsiUserInterface
{
    partial class Form1
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
            this.viewTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.environmentTabControl = new System.Windows.Forms.TabControl();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.accountTabControl = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.viewTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.environmentTabControl.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.accountTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewTabControl
            // 
            this.viewTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewTabControl.Controls.Add(this.tabPage1);
            this.viewTabControl.Controls.Add(this.tabPage2);
            this.viewTabControl.Location = new System.Drawing.Point(0, 0);
            this.viewTabControl.Name = "viewTabControl";
            this.viewTabControl.SelectedIndex = 0;
            this.viewTabControl.Size = new System.Drawing.Size(1077, 550);
            this.viewTabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.environmentTabControl);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1069, 524);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Environment View";
            // 
            // environmentTabControl
            // 
            this.environmentTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.environmentTabControl.Controls.Add(this.tabPage7);
            this.environmentTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.environmentTabControl.Location = new System.Drawing.Point(0, 0);
            this.environmentTabControl.Name = "environmentTabControl";
            this.environmentTabControl.SelectedIndex = 0;
            this.environmentTabControl.Size = new System.Drawing.Size(1073, 528);
            this.environmentTabControl.TabIndex = 0;
            this.environmentTabControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TabControl_DrawItem);
            this.environmentTabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            this.environmentTabControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TabControl_MouseClick);
            // 
            // tabPage7
            // 
            this.tabPage7.BackColor = System.Drawing.Color.Transparent;
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(1065, 502);
            this.tabPage7.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Transparent;
            this.tabPage2.Controls.Add(this.accountTabControl);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1069, 524);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Account View";
            // 
            // accountTabControl
            // 
            this.accountTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.accountTabControl.Controls.Add(this.tabPage4);
            this.accountTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.accountTabControl.Location = new System.Drawing.Point(0, 0);
            this.accountTabControl.Name = "accountTabControl";
            this.accountTabControl.SelectedIndex = 0;
            this.accountTabControl.Size = new System.Drawing.Size(1073, 528);
            this.accountTabControl.TabIndex = 1;
            this.accountTabControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TabControl_DrawItem);
            this.accountTabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            this.accountTabControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TabControl_MouseClick);
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.Transparent;
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1065, 502);
            this.tabPage4.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1074, 549);
            this.Controls.Add(this.viewTabControl);
            this.MinimumSize = new System.Drawing.Size(931, 588);
            this.Name = "Form1";
            this.Text = "Password Strength Insights";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeBegin += new System.EventHandler(this.Form1_ResizeBegin);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.viewTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.environmentTabControl.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.accountTabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl viewTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabControl accountTabControl;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabControl environmentTabControl;
        private System.Windows.Forms.TabPage tabPage7;
    }
}

