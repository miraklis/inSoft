using System;
using System.Windows;

namespace inCRM
{
    public partial class CustomersListView : SingletonWindow<CustomersListView>, IView
    {
		#region Constructors

        public CustomersListView() : base()
        {
            InitializeComponent();
			customers = new CustomersListVM();
			customers.View = this;
			this.DataContext = customers;
		}

		public Window GetInstance()
		{
			return this;
		}

		#endregion

		#region Private Members

		private CustomersListVM customers;

		#endregion

		private void CustomersListWindow_Loaded(object sender, RoutedEventArgs e)
		{
			customers?.ReadCustomersCommand.Execute(null);
		}
	}
}
