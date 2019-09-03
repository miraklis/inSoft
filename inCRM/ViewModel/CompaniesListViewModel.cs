namespace inCRM
{
	public class CompaniesListViewModel : BaseViewModel
	{
		public CompaniesListViewModel()
		{
			companies = new ObservableList<Company>(DB.GetAllCompanies());
		}

		private ObservableList<Company> companies;
		public ObservableList<Company> Companies
		{
			get
			{
				return companies;
			}
			set
			{
				companies = value;
				OnPropertyChanged();
			}
		}
	}
}
