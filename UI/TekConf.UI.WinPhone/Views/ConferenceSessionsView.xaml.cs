﻿using System.Windows;
using System.Windows.Controls;
using Cirrious.MvvmCross.WindowsPhone.Views;

namespace TekConf.UI.WinPhone.Views
{
	public partial class ConferenceSessionsView : MvxPhonePage
	{
		public ConferenceSessionsView()
		{
			InitializeComponent();
		}

		private void SessionTitle_OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			var title = (sender as TextBlock);
			title.MaxWidth = this.ActualWidth;
		}
	}
}