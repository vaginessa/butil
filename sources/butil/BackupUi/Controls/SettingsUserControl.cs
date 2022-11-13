using System;
using System.Windows.Forms;
using System.Threading;
using BUtil.Core.Options;
using BUtil.Core.Misc;
using BUtil.Core.Logs;

using BackUserControl = BUtil.BackupUiMaster.Controls.BackUserControl;
using BUtil.Configurator.Localization;

namespace BUtil.Configurator.BackupUiMaster.Controls
{
    /// <summary>
    /// Settings that can be adjusted just before backup
    /// </summary>
    internal sealed partial class SettingsUserControl : BackUserControl
	{
		ProgramOptions _options;
		
		public SettingsUserControl()
		{
			InitializeComponent();

			encryptionUserControl.BorderStyle = BorderStyle.None;
			encryptionUserControl.DrawAtractiveBorders = false;
		}
		
		#region Locals
		
		public override void ApplyLocalization()
		{
			encryptionUserControl.ApplyLocalization();
			int previousIndex = backupPriorityComboBox.SelectedIndex;
			backupPriorityComboBox.Items.Clear();
            backupPriorityComboBox.Items.AddRange(new [] { Resources.Low, Resources.BelowNormal, Resources.Normal, Resources.AboveNormal });
            backupPriorityComboBox.SelectedIndex = previousIndex;
            
			chooseBackUpPriorityLabel.Text = Resources.ProcessPriority;
			hearBeepsCheckBox.Text = Resources.BeepSeveralTimes;
			afterEndOfBackupGroupBox.Text = Resources.AfterCompletionOfBackup;
			
			previousIndex = jobAfterOkBackupComboBox.SelectedIndex;
			jobAfterOkBackupComboBox.Items.Clear();
			jobAfterOkBackupComboBox.Items.AddRange(new [] { Resources.ShutdownPc, Resources.LogOff, Resources.Reboot, Resources.DoNothing});
			jobAfterOkBackupComboBox.SelectedIndex = previousIndex;
			
			Title = Resources.Settings;
		}
		
		#endregion
		
		public void SetSettingsToUi(ProgramOptions options, PowerTask task, BackupTask backupTask, bool beepWhenFinished, ThreadPriority priority)
		{
			_options = options;
			_options.Priority = priority;
			
			encryptionUserControl.UpdateModel(backupTask);
			
			backupPriorityComboBox.SelectedIndex = (int)_options.Priority;
			jobAfterOkBackupComboBox.SelectedIndex = (int)task;
            hearBeepsCheckBox.Checked = beepWhenFinished;
            backupPriorityComboBox.SelectedIndex = (int)_options.Priority;
		}
		
		public void GetSettingsFromUi(out PowerTask task, out bool beepWhenFinished)
		{
			encryptionUserControl.GetOptionsFromUi();
			task = (PowerTask)jobAfterOkBackupComboBox.SelectedIndex;
            _options.Priority = (ThreadPriority)backupPriorityComboBox.SelectedIndex;
            beepWhenFinished = hearBeepsCheckBox.Checked;
		}
		
		void HelpActionAfterBackupButtonClick(object sender, EventArgs e)
		{
			Messages.ShowInformationBox(Resources.IfYouChooseSomethingOtherThanDoNothingNprogramWillConfigureOsToShowYouReportNonNextYourLogonToTheSystemIfAnyErrorsNorWarningsWillBeRegisteredDuringTheBackup);
		}
	}
}
