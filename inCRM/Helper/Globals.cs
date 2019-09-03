namespace inCRM
{
	public class Globals: BaseViewModel
	{
		public Globals()
		{

		}

		private static Globals instance = null;
		public static Globals GetInstance()
		{
			if (instance == null)
				instance = new Globals();
			return instance;
		}

		private bool connectionEstablished = false;
		public bool ConnectionEstablished
		{
			get
			{
				return connectionEstablished;
			}
			set
			{
				if (connectionEstablished != value) {
					connectionEstablished = value;
					OnPropertyChanged();
				}
			}
		}

	}
}
