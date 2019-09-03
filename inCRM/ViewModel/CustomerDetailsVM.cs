using DBHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace inCRM
{
	public class CustomerDetailsVM : ValidatedViewModel
	{
		#region Constructors

		public CustomerDetailsVM(int id)
		{
			if (id == 0) {
				action = DBActionType.Create;
				customer = new Customer();
				contacts = new ObservableList<ContactVM>();
				addresses = new ObservableList<AddressVM>();
			}
			else {
				action = DBActionType.Update;
				customer = DB.GetCustomer(id);
				contacts = new ObservableList<ContactVM>(DB.GetCustomerNumbersList(id));
				addresses = new ObservableList<AddressVM>(DB.GetCustomerAddressList(id));				
			}
		}

		#endregion

		#region Events

		public delegate void CustomerModifiedHandler(object sender, EventArgs e);
		public event CustomerModifiedHandler CustomerModified;

		#endregion

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

		// Save Customer
		private RelayCommand<object> saveCustomerCommand;
		public RelayCommand<object> SaveCustomerCommand
		{
			get => saveCustomerCommand ?? (saveCustomerCommand = new RelayCommand<object>(SaveCustomer, CanSaveCustomer));
		}
		private void SaveCustomer(object obj)
		{
			if (Save()) {
				CustomerModified(this, EventArgs.Empty);
				CloseWindow(null);
			}
		}
		private bool CanSaveCustomer(object obj)
		{
			return Globals.GetInstance().ConnectionEstablished;
		}
		private bool Save()
		{
			if (!ValidateData())
				return false;
			int result = -1;
			try {
				switch (action) {
					case DBActionType.Create:
						result = DB.AddCustomer(Customer);
						if (result > 0) {
							Customer.Id = result;
							foreach (var ci in Contacts) {
								ci.ContatDetails.CustomerId = Customer.Id;
								DB.AddCustomerNumber(ci.ContatDetails);
							}
							foreach (var ai in Addresses) {
								ai.AddressDetails.CustomerId = Customer.Id;
								DB.AddCustomerAddress(ai.AddressDetails);
							}
						}
						break;
					case DBActionType.Update:
						result = DB.UpdateCustomer(Customer);
						if (result > 0) {
							DB.DeleteCustomerNumbers(Customer.Id);
							foreach (var ci in Contacts) {
								ci.ContatDetails.CustomerId = Customer.Id;
								DB.AddCustomerNumber(ci.ContatDetails);
							}
							DB.DeleteCustomerAddresses(Customer.Id);
							foreach (var ai in Addresses) {
								ai.AddressDetails.CustomerId = Customer.Id;
								DB.AddCustomerAddress(ai.AddressDetails);
							}
						}
						break;
					default:
						return false;
				}
				return (result > 0);
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
				return false;
			}
		}

		#endregion

		#region Properties

		public IWindowView View;

		private Customer customer;
		public Customer Customer
		{
			get
			{
				return customer;
			}
			set
			{
				customer = value;
				OnPropertyChanged();
			}
		}

		private ObservableList<ContactVM> contacts;
		public ObservableList<ContactVM> Contacts
		{
			get
			{
				return contacts;
			}
			set
			{
				contacts = value;
				OnPropertyChanged();
			}
		}

		private ObservableList<AddressVM> addresses;
		public ObservableList<AddressVM> Addresses
		{
			get
			{
				return addresses;
			}
			set
			{
				addresses = value;
				OnPropertyChanged();
			}
		}

		public List<Company> Companies
		{
			get
			{
				return DB.GetAllCompanies();
			}
		}

		public List<ContactType> ContactTypes
		{
			get
			{
				return DB.GetContactTypes();
			}
		}

		public string VMTitle
		{
			get
			{
				if (Customer.Id == 0)
					return "Add New Customer";
				else
					return "Edit Customer:" + Customer.FirstName + " " + Customer.LastName;
			}
		}

		#endregion

		#region Private Members

		private DBActionType action;

		private void ValidateFirstName()
		{
			List<string> propertyErrors = new List<string>();
			if (String.IsNullOrWhiteSpace(Customer.FirstName))
				propertyErrors.Add("First Name cannot be empty");
			SetErrors(nameof(Customer.FirstName), propertyErrors);
		}

		private void ValidateLastName()
		{
			List<string> propertyErrors = new List<string>();
			if (String.IsNullOrWhiteSpace(Customer.LastName))
				propertyErrors.Add("Last Name cannot be empty");
			SetErrors(nameof(Customer.LastName), propertyErrors);
		}

		private void ValidateContactsDetailss()
		{
			List<string> propertyErrors = new List<string>();
			foreach (var ci in Contacts) {
				if (ci?.ContactTypeId <= 0)
					propertyErrors.Add("You must select a type for the contact");
				if (String.IsNullOrWhiteSpace(ci?.ContactInfo))
					propertyErrors.Add("You must give info for the contact");
			}
			SetErrors(nameof(Contacts), propertyErrors);
		}

		private bool ValidateData()
		{
			ValidateFirstName();
			ValidateLastName();
			ValidateContactsDetailss();
			if (HasErrors) {
				StringBuilder msg = new StringBuilder();
				foreach (var k in validationErrors) {
					foreach (var v in k.Value) {
						msg.Append(v);
						msg.Append(Environment.NewLine);
					}
				}
				MessageBox.Show(msg.ToString(), "Validation Errors");
			}
			return !HasErrors;
		}

		#endregion
	}
}
