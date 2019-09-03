using DBHandler;

namespace inCRM
{
	public class AddressVM: BaseViewModel
	{
		public AddressVM()
		{
			addressDetails = new AddressDetails();
		}

		private AddressDetails addressDetails;

		[DataProperty]
		public int Id
		{
			get
			{
				return addressDetails.Id;
			}
			set
			{
				if(addressDetails.Id != value) {
					addressDetails.Id = value;
					OnPropertyChanged();
				}
			}
		}

		[DataProperty]
		public string Road
		{
			get
			{
				return addressDetails.Road;
			}
			set
			{
				if (addressDetails.Road != value) {
					addressDetails.Road = value;
					OnPropertyChanged();
					OnPropertyChanged(nameof(Address));
				}
			}
		}

		[DataProperty]
		public string Number
		{
			get
			{
				return addressDetails.Number;
			}
			set
			{
				if (addressDetails.Number != value) {
					addressDetails.Number = value;
					OnPropertyChanged();
					OnPropertyChanged(nameof(Address));
				}
			}
		}

		[DataProperty]
		public string City
		{
			get
			{
				return addressDetails.City;
			}
			set
			{
				if (addressDetails.City != value) {
					addressDetails.City = value;
					OnPropertyChanged();
				}
			}
		}

		[DataProperty]
		public string Region
		{
			get
			{
				return addressDetails.Region;
			}
			set
			{
				if (addressDetails.Region != value) {
					addressDetails.Region = value;
					OnPropertyChanged();
				}
			}
		}

		[DataProperty]
		public string State
		{
			get
			{
				return addressDetails.State;
			}
			set
			{
				if (addressDetails.State != value) {
					addressDetails.State = value;
					OnPropertyChanged();
				}
			}
		}

		[DataProperty]
		public string PostalCode
		{
			get
			{
				return addressDetails.PostalCode;
			}
			set
			{
				if (addressDetails.PostalCode != value) {
					addressDetails.PostalCode = value;
					OnPropertyChanged();
				}
			}
		}

		[DataProperty]
		public string Country
		{
			get
			{
				return addressDetails.Country;
			}
			set
			{
				if (addressDetails.Country != value) {
					addressDetails.Country = value;
					OnPropertyChanged();
				}
			}
		}

		public string Address
		{
			get
			{
				return addressDetails.Road + ", " + addressDetails.Number;
			}
		}

		public AddressDetails AddressDetails
		{
			get
			{
				return addressDetails;
			}
		}


	}
}
