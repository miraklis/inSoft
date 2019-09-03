using DBHandler;

namespace inCRM
{
    public class Company
    {
        #region Constructors

        public Company()
        {

        }

        #endregion

        #region Properties

        [DataProperty]
        public int Id { get; set; }
        [DataProperty]
        public string Name { get; set; }
        [DataProperty]
        public string Title { get; set; }
        [DataProperty]
        public string Country { get; set; }

        #endregion

        #region Override Methods

        public override bool Equals(object obj)
        {
            if (obj is Company comp)
            {
                return (comp.Id == Id);
            }
            return false;
        }

        public static bool operator ==(Company comp1, Company comp2)
        {
            if (comp1 is null)
                return (comp2 is null);
            return comp1.Equals(comp2);
        }

        public static bool operator !=(Company comp1, Company comp2)
        {
            return !(comp1 == comp2);
        }

        #endregion
    }
}
