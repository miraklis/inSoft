using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace inCRM
{
	/// <summary>
	/// ViewModel Class of a Generic List of Data Items
	/// It is meant to be used alongside with a data container
	/// (eg. a datagrid) in a MVVM application
	/// Capabilities for
	///		* Paging
	///		* Filtering
	/// </summary>
	public abstract class DataListVM<T>: BaseViewModel where T:class, new()
	{
		/// <summary>
		/// Default Constructor
		/// We create the background worker which is responsible
		/// for updating the list asynchronous from another thread
		/// </summary>
		public DataListVM()
		{
			worker = new BackgroundWorker
			{
				WorkerSupportsCancellation = true,
				WorkerReportsProgress = true
			};
			worker.DoWork += Worker_DoWork;
			worker.ProgressChanged += Worker_ProgressChanged;
			worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
			restartWorker = false;
		}

		/// <summary>
		/// The data collection of the list
		/// </summary>
		protected ObservableCollection<T> data;

		/// <summary>
		/// The Filtered data collection of the list
		/// </summary>
		private ObservableCollection<T> filteredData;

		/// <summary>
		/// The visible items eg. the items of the
		/// current page whih also satisfies the filter
		/// </summary>
		private ObservableCollection<T> items;
		public ObservableCollection<T> Items
		{
			get => items;
			set
			{
				if (items == value)
					return;
				items = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		///The Selected item (if any) in the list
		/// </summary>
		private T selectedItem;
		public T SelectedItem
		{
			get => selectedItem;
			set
			{
				if (selectedItem == value)
					return;
				selectedItem = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsItemSelected));
			}
		}

		/// <summary>
		/// The string with which the data are filtered
		/// </summary>
		private string filter;
		public string Filter
		{
			get => filter;
			set
			{
				if (filter == value)
					return;
				filter = value;
				FilterData(null);
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// The current page for the data
		/// </summary>
		private int currentPage;
		public int CurrentPage
		{
			get => currentPage;
			set
			{
				if (currentPage == value)
					return;
				if (value < 1)
					currentPage = 1;
				else if (value > TotalPages)
					currentPage = TotalPages;
				else
					currentPage = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// The current pagesize for the datagrid
		/// </summary>
		private int currentPageSize = 100;
		public int CurrentPageSize
		{
			get => currentPageSize;
			set
			{
				if (currentPageSize == value)
					return;
				currentPageSize = value;
				LoadPageItemsAsync();
				OnPropertyChanged();
				OnPropertyChanged(nameof(TotalPages));
			}
		}

		/// <summary>
		/// All the possible page sizes for the combo box to show
		/// </summary>
		private int[] pageSizes;
		public int[] PageSizes
		{
			get => pageSizes;
			set
			{
				pageSizes = value;
				OnPropertyChanged();
			}
		}


		/// <summary>
		/// The Percentage done when we update the list from
		/// the background worker (0 - 100%)
		/// </summary>
		private int updatingListPercentageDone;
		public int UpdatingListPercentageDone
		{
			get => updatingListPercentageDone;
			set
			{
				if (updatingListPercentageDone == value)
					return;
				updatingListPercentageDone = value > 100 ? 100 : value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Read only property to indicate if any
		/// item is selected in the list
		/// </summary>
		public bool IsItemSelected
		{
			get => SelectedItem != null;
		}

		/// <summary>
		/// Read only property counting all the items
		/// </summary>
		public int TotalItemsCount
		{
			get => data == null ? 0 : data.Count;
		}

		/// <summary>
		/// Read only property counting the filtered items
		/// </summary>
		public int FilteredItemsCount
		{
			get => filteredData == null ? 0 : filteredData.Count;
		}

		/// <summary>
		/// Read only property returning the total pages
		/// for the filtered data
		/// </summary>
		public int TotalPages
		{
			get
			{
				int total = (int)Math.Ceiling((double)FilteredItemsCount / CurrentPageSize);
				if (total == 0)
					return 1;
				return total;
			}
		}

		/// <summary>
		/// Read Data Command (suitable for MVVM)
		/// (implementing ICommand through RelayCommand)
		/// </summary>
		private RelayCommand readDataCommand;
		public RelayCommand ReadDataCommand
		{
			get => readDataCommand ?? (readDataCommand = new RelayCommand(ReadData));
		}
		private void ReadData(object parameter)
		{
			data = ReadDataFromDataSource();
			data.CollectionChanged += DataList_CollectionChanged;
			CurrentPage = 1;
			Filter = String.Empty;
			OnPropertyChanged(nameof(TotalItemsCount));
		}
		// Must implement this function to aquire the proper data
		protected abstract ObservableCollection<T> ReadDataFromDataSource();

		/// <summary>
		/// Filter Data Command (suitable for MVVM)
		/// (implementing ICommand through RelayCommand)
		/// </summary>
		private RelayCommand filterDataCommand;
		public RelayCommand FilterDataCommand
		{
			get => filterDataCommand ?? (filterDataCommand = new RelayCommand(FilterData));
		}
		private void FilterData(object parameter)
		{
			filteredData = FilterDataWithCriteria();
			LoadPageItemsAsync();
			OnPropertyChanged(nameof(FilteredItemsCount));
			OnPropertyChanged(nameof(TotalPages));
		}
		// Must implement this function to filter with the proper criteria
		protected abstract ObservableCollection<T> FilterDataWithCriteria();

		/// <summary>
		/// Add Data Item Command (suitable for MVVM)
		/// (implementing ICommand through RelayCommand)
		/// </summary>
		private RelayCommand addDataItemCommand;
		public RelayCommand AddDataItemCommand
		{
			get => addDataItemCommand ?? (addDataItemCommand = new RelayCommand(AddDataItem));
		}
		protected virtual void AddDataItem(object parameter) { }

		/// <summary>
		/// Edit Data Item Command (suitable for MVVM)
		/// (implementing ICommand through RelayCommand)
		/// </summary>
		private RelayCommand editDataItemCommand;
		public RelayCommand EditDataItemCommand
		{
			get => editDataItemCommand ?? (editDataItemCommand = new RelayCommand(EditDataItem, CanEditDataItem));
		}
		private bool CanEditDataItem(object parameter)
		{
			return IsItemSelected;
		}
		protected virtual void EditDataItem(object parameter) { }

		/// <summary>
		/// Remove Data Item Command (suitable for MVVM)
		/// (implementing ICommand through RelayCommand)
		/// </summary>
		private RelayCommand removeDataItemCommand;
		public RelayCommand RemoveDataItemCommand
		{
			get => removeDataItemCommand ?? (removeDataItemCommand = new RelayCommand(RemoveDataItem, CanRemoveDataItem));
		}
		private bool CanRemoveDataItem(object parameter)
		{
			return IsItemSelected;
		}
		protected virtual void RemoveDataItem(object parameter) { }

		/// <summary>
		/// Event to fire whenever the data list is modified
		/// Fires on Add,Remove,Replace(edit) and Move of an item
		/// Also fires on clear of the list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DataList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			FilterData(null);
			OnPropertyChanged(nameof(TotalItemsCount));
		}

		/// <summary>
		/// Next Page Command (suitable for MVVM)
		/// (implementing ICommand through RelayCommand)
		/// </summary>
		private RelayCommand nextPageCommand;
		public RelayCommand NextPageCommand
		{
			get => nextPageCommand ?? (nextPageCommand = new RelayCommand(GoToNextPage, CanGoToNextPage));
		}
		private void GoToNextPage(object parameter)
		{
			CurrentPage++;
			LoadPageItemsAsync();
		}
		private bool CanGoToNextPage(object parameter)
		{
			return CurrentPage < TotalPages;
		}

		/// <summary>
		/// Previous Page Command (suitable for MVVM)
		/// (implementing ICommand through RelayCommand)
		/// </summary>
		private RelayCommand previousPageCommand;
		public RelayCommand PreviousPageCommand
		{
			get => previousPageCommand ?? (previousPageCommand = new RelayCommand(GoToPreviousPage, CanGoToPreviousPage));
		}
		private void GoToPreviousPage(object parameter)
		{
			CurrentPage--;
			LoadPageItemsAsync();
		}
		private bool CanGoToPreviousPage(object parameter)
		{
			return CurrentPage > 1;
		}

		/// <summary>
		/// Async method to load the items per page
		/// </summary>
		private void LoadPageItemsAsync()
		{
			if (worker?.IsBusy == true) {
				worker.CancelAsync();
				restartWorker = true;
			}
			if(!restartWorker)
				worker.RunWorkerAsync();
		}

		/// <summary>
		/// Background worker action which
		/// updates the datagrid by small segments
		/// from another thread so that the UI
		/// doesn't hangs
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			int segmentSize = 50;
			int segment = 0;
			if (CurrentPage > TotalPages)
				CurrentPage = TotalPages;
			int skip = CurrentPage <= 1 ? 0 : (CurrentPage - 1) * CurrentPageSize;
			Items = new ObservableCollection<T>();
			List<T> listToShow = filteredData.Skip(skip).Take(CurrentPageSize).ToList();
			List<T> dataRead = listToShow.Skip(segmentSize * segment).Take(segmentSize).ToList();
			double perc = 0;
			while (dataRead.Any()) {
				if (worker.CancellationPending) {
					e.Cancel = true;
					return;
				}
				dataRead = listToShow.Skip(segmentSize * segment).Take(segmentSize).ToList();
				perc += Math.Ceiling(((double)dataRead.Count / listToShow.Count) * 100);
				worker.ReportProgress((int)perc, dataRead);
				segment++;
				Thread.Sleep(20);
			}
		}

		/// <summary>
		/// Updating background worker progress
		/// This method runs from the same thread as the UI
		/// so we can update the items list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			List<T> dataRead = e.UserState as List<T>;
			dataRead?.ForEach(x => Items.Add(x));
			UpdatingListPercentageDone = e.ProgressPercentage;
		}

		/// <summary>
		/// Executes when the background worker finishes
		/// We just see if we cancelled the worker by requesting
		/// a new list (eg. change of page) and then restarts
		/// the worker
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (restartWorker) {
				restartWorker = false;
				worker.RunWorkerAsync();
			}
		}

		/// <summary>
		/// The background worker to update the list from another thread
		/// </summary>
		private BackgroundWorker worker;

		/// <summary>
		/// The bool to track if we want to restart the worker
		/// (eg. when changing pages and a running worker isn't finished)
		/// </summary>
		private bool restartWorker;

	} // class
} // namespace
