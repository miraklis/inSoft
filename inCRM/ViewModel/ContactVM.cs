using DBHandler;

namespace inCRM
{
    public class ContactVM : BaseViewModel
    {
        public ContactVM()
		{
			contactDetails = new ContactDetails();
		}

        private ContactDetails contactDetails;
		public ContactDetails ContatDetails
		{
			get
			{
				return contactDetails;
			}
		}

		[DataProperty]
        public int Id
        {
            get
            {
                return contactDetails.Id;
            }
            set
            {
                if(contactDetails.Id != value) {
					contactDetails.Id = value;
					OnPropertyChanged();
				}
            }
        }

        [DataProperty]
		public string Type { get; set; }

		[DataProperty]
		public int ContactTypeId
		{
			get
			{
				return contactDetails.ContactTypeId;
			}
			set
			{
				if(contactDetails.ContactTypeId != value) {
					contactDetails.ContactTypeId = value;
					OnPropertyChanged();
					OnPropertyChanged(nameof(Type));
				}
			}
		}
		[DataProperty]
		public string ContactInfo
		{
			get
			{
				return contactDetails.ContactInfo;
			}
			set
			{
				if(contactDetails.ContactInfo != value) {
					contactDetails.ContactInfo = value;
					OnPropertyChanged();
				}
			}
		}

	}
}
