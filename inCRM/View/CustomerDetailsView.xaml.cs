using System.Collections.Generic;
using System.Windows;

namespace inCRM
{
    public partial class CustomerDetailsView : Window, IWindowView
    {
		public CustomerDetailsView(CustomerDetailsVM customerVM)
		{
			InitializeComponent();			
			customerVM.View = this;
			this.DataContext = customerVM;
			cmbContactTypes.ItemsSource = customerVM.ContactTypes;
		}

		public Window GetInstance()
		{
			return this;
		}

		public void CloseWindow()
		{
			this.Close();
		}
	}
}
