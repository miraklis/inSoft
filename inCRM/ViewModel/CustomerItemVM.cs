using DBHandler;
using System;

namespace inCRM
{
	public class CustomerItemVM: BaseViewModel, IComparable
	{
		[DataProperty]
		public int Id { get; set; }
		[DataProperty]
		public string FirstName { get; set; }
		[DataProperty]
		public string LastName { get; set; }
		[DataProperty]
		public DateTime? DateOfBirth { get; set; }
		[DataProperty]
		public string CompanyName { get; set; }

		public string FullName
		{
			get
			{
				return FirstName + " " + LastName;
			}
		}

		public ObservableList<ContactVM> Contacts
		{
			get
			{
				return new ObservableList<ContactVM>(DB.GetCustomerNumbersList(Id));
			}
		}

		public ObservableList<AddressVM> Addresses
		{
			get
			{
				return new ObservableList<AddressVM>(DB.GetCustomerAddressList(Id));
			}
		}

		public void Clone(CustomerItemVM source)
		{
			Id = source.Id;
			FirstName = source.FirstName;
			LastName = source.LastName;
			DateOfBirth = source.DateOfBirth;
			CompanyName = source.CompanyName;
			OnPropertyChanged("");
		}

		public int CompareTo(object obj)
		{
			return this.FullName.CompareTo(((CustomerItemVM)obj).FullName);
		}

		#region Override Methods

		public override bool Equals(object obj)
		{
			if (obj is CustomerItemVM cust) {
				return (cust.Id == Id);
			}
			return false;
		}

		public static bool operator ==(CustomerItemVM cust1, CustomerItemVM cust2)
		{
			if (cust1 is null)
				return (cust2 is null);
			return cust1.Equals(cust2);
		}

		public static bool operator !=(CustomerItemVM cust1, CustomerItemVM cust2)
		{
			return !(cust1 == cust2);
		}

		#endregion
	}
}
