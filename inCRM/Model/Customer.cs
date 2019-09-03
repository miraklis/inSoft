using DBHandler;
using System;
using System.Text;
using System.Windows.Media.Imaging;

namespace inCRM
{
    public class Customer
    {

        #region Constructors

        public Customer()
        {
            this.Id = 0;
        }

		#endregion

		#region Properties

		[DataProperty]
        public int Id { get; set; }
        [DataProperty]
        public string FirstName { get; set; }
        [DataProperty]
        public string LastName { get; set; }
        [DataProperty]
        public DateTime? DateOfBirth { get; set; }
        [DataProperty]
        public int? CompanyId { get; set; }

		#endregion

		#region Override Methods

		public override bool Equals(object obj)
		{
			if (obj is Customer cust) {
				return (cust.Id == Id);
			}
			return false;
		}

		public static bool operator ==(Customer cust1, Customer cust2)
		{
			if (cust1 is null)
				return (cust2 is null);
			return cust1.Equals(cust2);
		}

		public static bool operator !=(Customer cust1, Customer cust2)
		{
			return !(cust1 == cust2);
		}

		#endregion

	}
}
