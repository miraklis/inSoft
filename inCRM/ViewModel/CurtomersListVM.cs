using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;

namespace inCRM
{
	public class CustomersListVM : BaseViewModel
	{
		#region Constructors

		public CustomersListVM()
		{
		}

		#endregion

		#region Private Members

		private ObservableList<CustomerItemVM> data;
		private ObservableList<CustomerItemVM> filteredData;

		private void InitializeCustomersPage()
		{
			if (CurrentPage > TotalPages)
				CurrentPage = TotalPages;
			if (PageSize < 1000) {
				int skip = CurrentPage <= 1 ? 0 : (CurrentPage - 1) * PageSize;
				CustomersList = new ObservableList<CustomerItemVM>(filteredData.Skip(skip).Take(PageSize).ToList());
			}
			else {
				LoadCustomersSegmentential();
			}
		}

		private void LoadCustomersSegmentential()
		{
			int segmentSize = 50;
			int segment = 0;

			CustomersList = new ObservableList<CustomerItemVM>();
			using (System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker()) {
				worker.WorkerReportsProgress = true;
				worker.DoWork += (s, arg) =>
				{
					List<CustomerItemVM> dataRead = null;
					int skip = CurrentPage <= 1 ? 0 : (CurrentPage - 1) * PageSize;
					var listToShow = new List<CustomerItemVM>(filteredData.Skip(skip).Take(PageSize).ToList());
					while (dataRead == null || dataRead.Any()) {
						dataRead = listToShow.Skip(segmentSize * segment).Take(segmentSize).ToList();
						worker.ReportProgress(0, dataRead);
						segment++;
						Thread.Sleep(20);
					}
				};
				worker.ProgressChanged += (s, e) =>
				{
					List<CustomerItemVM> dataRead = e.UserState as List<CustomerItemVM>;
					dataRead.ForEach(x => CustomersList.Add(x));
				};
				worker.RunWorkerAsync();
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
			int index = data.IndexOf(data.First(x => x.Id == newItem.Id));
			if (index >= 0)
				data[index] = newItem;
			else
				data.Add(newItem);
		}

		private void CustomersList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(DataCnt));
			FilterCustomers(null);
		}

		#endregion

		#region Commands

		// ReadCustomers
		private RelayCommand<object> readCustomersCommand;
		public RelayCommand<object> ReadCustomersCommand
		{
			get => readCustomersCommand ?? (readCustomersCommand = new RelayCommand<object>(ReadCustomers));
		}
		private void ReadCustomers(object obj)
		{
			data = new ObservableList<CustomerItemVM>(DB.GetCustomerItemsVM());
			data.CollectionChanged += CustomersList_CollectionChanged;
			OnPropertyChanged(nameof(DataCnt));
			CurrentPage = 1;
			Filter = String.Empty;
			FilterCustomers(null);
		}

		// Filter Customers
		private RelayCommand<object> filterCustomersCommand;
		public RelayCommand<object> FilterCustomersCommand
		{
			get => filterCustomersCommand ?? (filterCustomersCommand = new RelayCommand<object>(FilterCustomers));
		}
		private void FilterCustomers(object obj)
		{
			filteredData = new ObservableList<CustomerItemVM>(data.Where(x => Filter == String.Empty || x.FullName.ToUpper().Contains(Filter.ToUpper())).ToList());
			InitializeCustomersPage();
			OnPropertyChanged(nameof(FilteredCnt));
			OnPropertyChanged(nameof(TotalPages));
		}

		// Add New Customer
		private RelayCommand<object> addNewCustomerCommand;
		public RelayCommand<object> AddNewCustomerCommand
		{
			get => addNewCustomerCommand ?? (addNewCustomerCommand = new RelayCommand<object>(AddNewCustomer));
		}
		private void AddNewCustomer(object obj)
		{
			CustomerDetailsVM customerVM = new CustomerDetailsVM(0);
			customerVM.CustomerModified += ModifyList;
			CustomerDetailsView customerDetailsView = new CustomerDetailsView(customerVM);
			customerDetailsView.Show();
		}

		// Edit Customer
		private RelayCommand<object> editCustomerCommand;
		public RelayCommand<object> EditCustomerCommand
		{
			get => editCustomerCommand ?? (editCustomerCommand = new RelayCommand<object>(EditCustomer, CanEditCustomer));
		}
		private void EditCustomer(object obj)
		{
			CustomerDetailsVM customerVM = new CustomerDetailsVM(SelectedCustomer.Id);
			customerVM.CustomerModified += ModifyList;
			CustomerDetailsView customerDetailsView = new CustomerDetailsView(customerVM);
			customerDetailsView.Show();
		}
		private bool CanEditCustomer(object obj)
		{
			return IsCustomerSelected;
		}

		// Remove Customer
		private RelayCommand<object> removeCustomerCommand;
		public RelayCommand<object> RemoveCustomerCommand
		{
			get => removeCustomerCommand ?? (removeCustomerCommand = new RelayCommand<object>(RemoveCustomer, CanRemoveCustomer));
		}
		private void RemoveCustomer(object obj)
		{
			if (MessageBox.Show("Are you sure you want to delete " + SelectedCustomer.FirstName + " " + SelectedCustomer.LastName + "?",
								"Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
				int result = DB.DeleteCustomer(SelectedCustomer.Id);
				if (result > 0) {
					data.Remove(SelectedCustomer);
				}
			}
		}
		private bool CanRemoveCustomer(object obj)
		{
			return IsCustomerSelected;
		}

		// Next Page Command
		private RelayCommand<object> nextPageCommand;
		public RelayCommand<object> NextPageCommand
		{
			get => nextPageCommand ?? (nextPageCommand = new RelayCommand<object>(GoToNextPage, CanGoToNextPage));
		}
		private void GoToNextPage(object obj)
		{
			CurrentPage++;
			InitializeCustomersPage();
		}
		private bool CanGoToNextPage(object obj)
		{
			return CurrentPage < TotalPages;
		}

		// Previous Page Command
		private RelayCommand<object> previousPageCommand;
		public RelayCommand<object> PreviousPageCommand
		{
			get => previousPageCommand ?? (previousPageCommand = new RelayCommand<object>(GoToPreviousPage, CanGoToPreviousPage));
		}
		private void GoToPreviousPage(object obj)
		{
			CurrentPage--;
			InitializeCustomersPage();
		}
		private bool CanGoToPreviousPage(object obj)
		{
			return CurrentPage > 1;
		}
		#endregion

		#region Properties

		public IView View { get; set; }

		private ObservableList<CustomerItemVM> customersList;
		public ObservableList<CustomerItemVM> CustomersList
		{
			get
			{
				return customersList;
			}
			set
			{
				customersList = value;
				OnPropertyChanged();
			}
		}

		private CustomerItemVM selectedCustomer;
		public CustomerItemVM SelectedCustomer
		{
			get
			{
				return selectedCustomer;
			}
			set
			{
				selectedCustomer = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsCustomerSelected));
			}
		}

		public int DataCnt { get => data == null ? 0 : data.Count; }
		public int FilteredCnt { get => filteredData == null ? 0 : filteredData.Count; }
		public bool IsCustomerSelected { get => SelectedCustomer != null; }

		private int pageSize = 100;
		public int PageSize
		{
			get => pageSize;
			set
			{
				if (value == pageSize)
					return;
				pageSize = value;
				InitializeCustomersPage();
				OnPropertyChanged(nameof(TotalPages));
				OnPropertyChanged();
			}
		}

		private string filter;
		public string Filter
		{
			get
			{
				return filter;
			}
			set
			{
				if (filter == value)
					return;
				filter = value;
				OnPropertyChanged();
				FilterCustomersCommand.Execute(null);
			}
		}

		private int currentPage;
		public int CurrentPage
		{
			get
			{
				return currentPage;
			}
			set
			{
				if (currentPage == value)
					return;
				if (value < 1) {
					currentPage = 1;
					OnPropertyChanged();
					return;
				}
				if (value > TotalPages) {
					currentPage = TotalPages;
					OnPropertyChanged();
					return;
				}
				currentPage = value;
				OnPropertyChanged();
			}
		}

		public int TotalPages
		{
			get
			{
				int total = (FilteredCnt / PageSize);
				if (total == 0 || FilteredCnt % PageSize != 0)
					total++;
				return total;
			}
		}

        #endregion

        #region Segmentantial Customer List Loading


        #endregion
    }

}
