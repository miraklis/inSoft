using DBHandler;

namespace inCRM
{
    public class ContactDetails
    {

        #region Constructors

        public ContactDetails() {}

        #endregion

        #region Properties

        [DataProperty]
        public int Id { get; set; }
        [DataProperty]
        public int CustomerId { get; set; }
        [DataProperty]
        public int ContactTypeId { get; set; }
        [DataProperty]
        public string ContactInfo { get; set; }

        #endregion

        #region Override Methods

        public override bool Equals(object obj)
        {
            if (obj is ContactDetails cdetails)
            {
                return (cdetails.Id == Id);
            }
            return false;
        }

        public static bool operator ==(ContactDetails cdetails1, ContactDetails cdetails2)
        {
            if (cdetails1 is null)
                return (cdetails2 is null);
            return cdetails1.Equals(cdetails2);
        }

        public static bool operator !=(ContactDetails cdetails1, ContactDetails cdetails2)
        {
            return !(cdetails1 == cdetails2);
        }

        #endregion
    }
}
