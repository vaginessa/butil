using System;
using System.Linq;
using BUtil.Configurator.Localization;
using BUtil.Core.Options;

namespace BUtil.Configurator.Controls
{
	/// <summary>
	/// Setups scheduler
	/// </summary>
	internal sealed partial class WhenUserControl : BUtil.Core.PL.BackUserControl
	{
		private ScheduleInfo _scheduleInfo;

		public WhenUserControl()
		{
			InitializeComponent();
		}
		
		#region Overrides

		public override void ApplyLocalization() 
		{
			chooseDaysOfWeekLabel.Text = Resources.ChooseDaysOfWeek;
            scheduledDaysCheckedListBox.Items[2] = Resources.Wednesday;
            scheduledDaysCheckedListBox.Items[3] = Resources.Thursday;
            scheduledDaysCheckedListBox.Items[4] = Resources.Friday;
            scheduledDaysCheckedListBox.Items[5] = Resources.Saturday;
            scheduledDaysCheckedListBox.Items[1] = Resources.Tuesday;
            scheduledDaysCheckedListBox.Items[0] = Resources.Monday;
            scheduledDaysCheckedListBox.Items[6] = Resources.Sunday;
            minuteLabel.Text = Resources.Minute;
            hourLabel.Text = Resources.Hour;
		}
	
		public override void SetOptionsToUi(object settings)
		{
			_scheduleInfo = (ScheduleInfo)settings;

            foreach (DayOfWeek enumItem in DayOfWeek.GetValues(typeof(DayOfWeek)))
			{
				scheduledDaysCheckedListBox.SetItemChecked((int) enumItem, _scheduleInfo.Days.Contains(enumItem));
			}
            
            hourComboBox.SelectedIndex = _scheduleInfo.Time.Hours;
            minuteComboBox.SelectedIndex = _scheduleInfo.Time.Minutes;
            
		}
		
		public override void GetOptionsFromUi()
		{
            _scheduleInfo.Days = Enum
				.GetValues(typeof(DayOfWeek))
                .Cast<DayOfWeek>()
                .Where(x => scheduledDaysCheckedListBox.GetItemChecked((int)x))
				.ToList();

            _scheduleInfo.Time = new TimeSpan(
				hourComboBox.SelectedIndex != -1 ? hourComboBox.SelectedIndex : 0,
                minuteComboBox.SelectedIndex != -1 ? minuteComboBox.SelectedIndex : 0,
				0);
		}
		
		#endregion
	}
}
