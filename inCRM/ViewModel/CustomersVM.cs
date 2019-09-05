using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace inCRM
{
	public class CustomersVM : DataListVM<CustomerItemVM>
	{
		public CustomersVM() : base()
		{
			PageSizes = new int[] { 10, 25, 50, 100, 500, 1000 };
		}

		protected override ObservableCollection<CustomerItemVM> FilterDataWithCriteria()
		{
			if (Filter == String.Empty)
				return data;
			string filterText = Filter.ToUpper();
			return new ObservableList<CustomerItemVM>(data.Where(x => x.FullName.ToUpper().Contains(filterText)).ToList());
		}

		protected override ObservableCollection<CustomerItemVM> ReadDataFromDataSource()
		{
			return new ObservableCollection<CustomerItemVM>(DB.GetCustomerItemsVM());
		}

		protected override void AddDataItem(object parameter)
		{
			CustomerDetailsVM customerVM = new CustomerDetailsVM(0);
			customerVM.CustomerModified += ModifyList;
			CustomerDetailsView customerDetailsView = new CustomerDetailsView(customerVM);
			customerDetailsView.Show();
		}

		protected override void EditDataItem(object parameter)
		{
			CustomerDetailsVM customerVM = new CustomerDetailsVM(SelectedItem.Id);
			customerVM.CustomerModified += ModifyList;
			CustomerDetailsView customerDetailsView = new CustomerDetailsView(customerVM);
			customerDetailsView.Show();
		}

		protected override void RemoveDataItem(object parameter)
		{
			if (MessageBox.Show("Are you sure you want to delete " + SelectedItem.FirstName + " " + SelectedItem.LastName + "?",
								"Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
				int result = DB.DeleteCustomer(SelectedItem.Id);
				if (result > 0) {
					data.Remove(SelectedItem);
				}
			}
		}

		// After a creation or modification of a customer from the details view, modify the data list
		// so we don't need to re-read all the customers from the database
		private void ModifyList(object sender, EventArgs e)
		{
			CustomerDetailsVM newCustomer = (CustomerDetailsVM)sender;
			CustomerItemVM newItem = new CustomerItemVM
			{
				Id = newCustomer.Customer.Id,
				FirstName = newCustomer.Customer.FirstName,
				LastName = newCustomer.Customer.LastName,
				DateOfBirth = newCustomer.Customer.DateOfBirth,
				CompanyName = DB.GetCompany(newCustomer.Customer.CompanyId)?.Name
			};
			int index = data.IndexOf(newItem);
			if (index >= 0)
				data[index] = newItem;
			else
				data.Add(newItem);
		}

	}
}
