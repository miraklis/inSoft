using System;
using System.Windows;
using System.Windows.Input;

namespace inCRM
{
    public partial class MainWindow : Window, IWindowView
    {
		public MainWindow()
		{
			InitializeComponent();
			MainWindowVM mainWindowVM = new MainWindowVM();
			mainWindowVM.View = this;
			this.DataContext = mainWindowVM;
		}

		public void CloseWindow()
		{
			this.Close();
		}

		public Window GetInstance()
		{
			return this;
		}
    }
}
