using BUtil.Configurator.Configurator.Controls;

namespace BUtil.Configurator.Configurator.Forms
{
	partial class MainForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MainmenuStrip = new System.Windows.Forms.MenuStrip();
            this._logsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restorationToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._beforeAboutToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this._aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.helpToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.helpStatusStrip = new System.Windows.Forms.StatusStrip();
            this._backupTasksUserControl = new BUtil.Configurator.Configurator.Controls.BackupTasksUserControl();
            this.MainmenuStrip.SuspendLayout();
            this.helpStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainmenuStrip
            // 
            this.MainmenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._logsToolStripMenuItem,
            this.restorationToolToolStripMenuItem,
            this._helpToolStripMenuItem});
            this.MainmenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MainmenuStrip.Name = "MainmenuStrip";
            this.MainmenuStrip.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.MainmenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.MainmenuStrip.Size = new System.Drawing.Size(835, 24);
            this.MainmenuStrip.TabIndex = 3;
            this.MainmenuStrip.Text = "MainmenuStrip";
            // 
            // _logsToolStripMenuItem
            // 
            this._logsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("_logsToolStripMenuItem.Image")));
            this._logsToolStripMenuItem.Name = "_logsToolStripMenuItem";
            this._logsToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this._logsToolStripMenuItem.Text = "Logs...";
            this._logsToolStripMenuItem.Click += new System.EventHandler(this.OnOpenLogsFolder);
            // 
            // restorationToolToolStripMenuItem
            // 
            this.restorationToolToolStripMenuItem.Image = global::BUtil.Configurator.Icons.Refresh48x48;
            this.restorationToolToolStripMenuItem.Name = "restorationToolToolStripMenuItem";
            this.restorationToolToolStripMenuItem.Size = new System.Drawing.Size(110, 20);
            this.restorationToolToolStripMenuItem.Text = "Restore Data...";
            this.restorationToolToolStripMenuItem.Click += new System.EventHandler(this.RestorationToolToolStripMenuItemClick);
            // 
            // _helpToolStripMenuItem
            // 
            this._helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this._beforeAboutToolStripSeparator,
            this._aboutToolStripMenuItem});
            this._helpToolStripMenuItem.Name = "_helpToolStripMenuItem";
            this._helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this._helpToolStripMenuItem.Text = "Help";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Image = global::BUtil.Configurator.Icons.Lamp48x48;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.helpToolStripMenuItem.Text = "Manual";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.OnHelpToolStripMenuItemClick);
            // 
            // _beforeAboutToolStripSeparator
            // 
            this._beforeAboutToolStripSeparator.Name = "_beforeAboutToolStripSeparator";
            this._beforeAboutToolStripSeparator.Size = new System.Drawing.Size(130, 6);
            // 
            // _aboutToolStripMenuItem
            // 
            this._aboutToolStripMenuItem.Image = global::BUtil.Configurator.Icons.ProgramInfo48x48;
            this._aboutToolStripMenuItem.Name = "_aboutToolStripMenuItem";
            this._aboutToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this._aboutToolStripMenuItem.Text = "About...";
            this._aboutToolStripMenuItem.Click += new System.EventHandler(this.OnAboutToolStripMenuItemClick);
            // 
            // toolTip
            // 
            this.toolTip.IsBalloon = true;
            // 
            // helpToolStripStatusLabel
            // 
            this.helpToolStripStatusLabel.Name = "helpToolStripStatusLabel";
            this.helpToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // helpStatusStrip
            // 
            this.helpStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripStatusLabel});
            this.helpStatusStrip.Location = new System.Drawing.Point(0, 581);
            this.helpStatusStrip.Name = "helpStatusStrip";
            this.helpStatusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.helpStatusStrip.Size = new System.Drawing.Size(835, 22);
            this.helpStatusStrip.TabIndex = 6;
            this.helpStatusStrip.Text = "statusStrip1";
            // 
            // _backupTasksUserControl
            // 
            this._backupTasksUserControl.BackColor = System.Drawing.SystemColors.Window;
            this._backupTasksUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._backupTasksUserControl.HelpLabel = null;
            this._backupTasksUserControl.Location = new System.Drawing.Point(0, 24);
            this._backupTasksUserControl.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this._backupTasksUserControl.Name = "_backupTasksUserControl";
            this._backupTasksUserControl.Size = new System.Drawing.Size(835, 557);
            this._backupTasksUserControl.TabIndex = 7;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(835, 603);
            this.Controls.Add(this._backupTasksUserControl);
            this.Controls.Add(this.helpStatusStrip);
            this.Controls.Add(this.MainmenuStrip);
            this.Icon = global::BUtil.Configurator.Icons.BUtilIcon;
            this.MainMenuStrip = this.MainmenuStrip;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(788, 641);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configurator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
            this.MainmenuStrip.ResumeLayout(false);
            this.MainmenuStrip.PerformLayout();
            this.helpStatusStrip.ResumeLayout(false);
            this.helpStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.ToolStripMenuItem _helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel helpToolStripStatusLabel;
		private System.Windows.Forms.StatusStrip helpStatusStrip;
		private System.Windows.Forms.MenuStrip MainmenuStrip;
        private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.ToolStripMenuItem restorationToolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator _beforeAboutToolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem _aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _logsToolStripMenuItem;
        private BackupTasksUserControl _backupTasksUserControl;
    }
}
