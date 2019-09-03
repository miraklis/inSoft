using System;
using System.Windows;

namespace inCRM
{
	public class MainWindowVM : BaseViewModel
	{
		public MainWindowVM()
		{
		}

		private LoginViewModel loginVM;

		#region Commands

		// Close Window
		private RelayCommand<object> closeWindowCommand;
		public RelayCommand<object> CloseWindowCommand
		{
			get => closeWindowCommand ?? (closeWindowCommand = new RelayCommand<object>(CloseWindow));
		}
		private void CloseWindow(object obj)
		{
			View?.CloseWindow();
		}

		// Login
		private RelayCommand<object> loginCommand;
		public RelayCommand<object> LoginCommand
		{
			get => loginCommand ?? (loginCommand = new RelayCommand<object>(Login));
		}
		private void Login(object obj)
		{
			if (loginVM is null)
				loginVM = new LoginViewModel();
			if (loginVM.ConnectionEstablished) {
				loginVM.LogoutCommand.Execute(null);
			} else {
				LoginView loginDialog = new LoginView(loginVM);
				if(loginDialog.ShowDialog()==true) {
					OnPropertyChanged(nameof(Username));
					OnPropertyChanged(nameof(SQLServer));
				}
			}
			OnPropertyChanged(nameof(ConnectionEstablished));
		}

		// Show Customers
		private RelayCommand<object> showCustomersCommand;
		public RelayCommand<object> ShowCustomersCommand
		{
			get
			{
				if (showCustomersCommand == null)
					showCustomersCommand = new RelayCommand<object>(ShowCustomers, CanShowCustomers);
				return showCustomersCommand;
			}
		}
		private void ShowCustomers(object obj)
		{
			CustomersListView customersListView = SingletonWindow<CustomersListView>.CreateWindow();
			customersListView.Owner = View.GetInstance();
			customersListView.Show();
			customersListView.Activate();
		}
		private bool CanShowCustomers(object obj)
		{
			return ConnectionEstablished;
		}

		#endregion

		#region Properties

		public IWindowView View { get; set; }

		public bool ConnectionEstablished
		{
			get
			{
				return Globals.GetInstance().ConnectionEstablished;
			}
		}

		public string Username
		{
			get
			{
				return loginVM == null ? String.Empty : loginVM.Username;
			}
		}

		public string SQLServer
		{
			get
			{
				return loginVM == null ? String.Empty : loginVM.SQLServer;
			}
		}

		#endregion

	}
}
