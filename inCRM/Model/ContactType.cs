using DBHandler;

namespace inCRM
{ 
    public class ContactType
    {
		#region Constructors

		public ContactType() { }

        #endregion

        #region Properties

        [DataProperty]
        public int Id { get; set; }
        [DataProperty]
        public string Type { get; set; }

        #endregion

        #region Override Methods

        public override bool Equals(object obj)
        {
            if (obj is ContactType cdetails)
            {
                return (cdetails.Id == Id);
            }
            return false;
        }

        public static bool operator ==(ContactType ctype1, ContactType ctype2)
        {
            if (ctype1 is null)
                return (ctype2 is null);
            return ctype1.Equals(ctype2);
        }

        public static bool operator !=(ContactType ctype1, ContactType ctype2)
        {
            return !(ctype1 == ctype2);
        }

        #endregion
    }
}
