using DBHandler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inCRM
{
    public class AddressDetails
    {
        public AddressDetails()
        {

        }

		[DataProperty]
        public int Id { get; set; }
		[DataProperty]
		public int CustomerId { get; set; }
		[DataProperty]
		public string City { get; set; }
		[DataProperty]
		public string Road { get; set; }
		[DataProperty]
		public string Number { get; set; }
		[DataProperty]
		public string Region { get; set; }
		[DataProperty]
		public string State { get; set; }
		[DataProperty]
		public string PostalCode { get; set; }
		[DataProperty]
		public string Country { get; set; }

    }
}
