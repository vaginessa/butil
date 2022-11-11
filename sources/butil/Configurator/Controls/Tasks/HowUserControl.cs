using BUtil.Configurator.Localization;
using BUtil.Core.BackupModels;
using BUtil.Core.Options;

namespace BUtil.Configurator.Controls
{
	internal sealed partial class HowUserControl : BUtil.Core.PL.BackUserControl
	{
		private BackupTask _task;

		public HowUserControl()
		{
			InitializeComponent();
		}
		
		#region Overrides

		public override void ApplyLocalization() 
		{
			_chooseBackupModel.Text = Resources.ChooseBackupModel;
			_incrementalBackupRadioButton.Text = Resources.IncrementalBackup;
			_backupModelLabel.Text = Resources.IncrementalBackupDescription;
            _disableCompressionEncryptionCheckBox.Text = Resources.DisableCompressionAndEncryption;
            _disableCompressionAndEncryptionUsagesLabel.Text = Resources.DisableCompressionAndEncryptionDescription;
        }
	
		public override void SetOptionsToUi(object settings)
		{
            _task = (BackupTask)settings;

			_incrementalBackupRadioButton.Checked = _task.Model is IncrementalBackupModelOptions;

			if (_task.Model is IncrementalBackupModelOptions)
			{
				var options = _task.Model as IncrementalBackupModelOptions;

				_disableCompressionEncryptionCheckBox.Checked = options.DisableCompressionAndEncryption;
			}
            
		}
		
		public override void GetOptionsFromUi()
		{
            if (_incrementalBackupRadioButton.Checked)
			{
                _task.Model = new IncrementalBackupModelOptions { DisableCompressionAndEncryption = _disableCompressionEncryptionCheckBox.Checked };
			}
        }
		
		#endregion
	}
}
