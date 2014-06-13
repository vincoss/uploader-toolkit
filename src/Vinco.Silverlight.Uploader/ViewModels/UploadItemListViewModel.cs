using System;
using System.Windows.Input;
using System.Collections.Generic;
using Vinco.Silverlight.Framework.Models;
using System.Collections.ObjectModel;
using Vinco.Silverlight.Commands;
using Vinco.Silverlight.Framework.Commands;
using Vinco.Silverlight.Views;
using System.Collections.Specialized;
using Vinco.Uploader.Tasks;


namespace Vinco.Silverlight.ViewModels
{
    public class UploadItemListViewModel : PropertyChangedBase
    {
        private const int READ_BUFFER_LENGTH = 65535;

        private readonly ICommand _pauseCommand;
        private readonly ICommand _cancelCommand;
        private readonly ICommand _clearCommand;
        private readonly ICommand _optionsCommand;
        private readonly ICommand _selectedItemsCommand;
        private readonly ICommand _dropCommand;

        private bool _overwriteExistingFiles;
        private readonly ObservableCollection<UploadItemViewModel> _itemsSource;
        private readonly ObservableCollection<UploadItemViewModel> _selectedItems; 
        private readonly IUploadItemListView _view;
       
        public UploadItemListViewModel() : this(new UploadItemListView())
        {
        }

        public UploadItemListViewModel(IUploadItemListView view)
        {
            // Commands
            _pauseCommand = new PauseCommand(this);
            _cancelCommand = new CancelCommand(this);
            _clearCommand = new ClearCommand(this);
            _optionsCommand = new OptionsCommand();
            _selectedItemsCommand = new SelectionChangedCommand(this);
            _dropCommand = new DropCommand(this);

            Multiselect = true;
            OverwriteExistingFiles = true;
            MaxReadSize = READ_BUFFER_LENGTH;
            this._itemsSource = new ObservableCollection<UploadItemViewModel>();
            this._selectedItems = new ObservableCollection<UploadItemViewModel>();

            // Events
            this.ItemsSource.CollectionChanged += ItemsSource_CollectionChanged;

            _view = view;
            _view.DataContext = this;
            this.Start();
        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (UploadItemViewModel item in e.OldItems)
                {
                    item.UploadItem.ProgressChanged -= UploadItem_ProgressChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (UploadItemViewModel item in e.NewItems)
                {
                    item.UploadItem.ProgressChanged += UploadItem_ProgressChanged;
                }
            }
        }

        public void Start()
        {
            ScheduledTasks.Instance.Run();
        }

        private void UploadItem_ProgressChanged(object sender, EventArgs e)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                                         {
                                                                             ((Command) _pauseCommand).RaiseCanExecuteChanged();
                                                                             ((Command) _cancelCommand).RaiseCanExecuteChanged();
                                                                         });
        }

        // TODO:
        public bool Multiselect { get; set; }
        public string Filter { get; set; }
        public byte MaxConcurrentUploads { get; set; }
        public ushort MaxReadSize { get; set; }
        public ulong MaximumUploadSize { get; set; }
        public ulong MaximumTotalUploadSize { get; set; }
        public ulong CurrentTotalUploadSize { get; set; }

        public IEnumerable<string> EnabledUploadExtensions { get; set; }

        #region Public properties

        public ObservableCollection<UploadItemViewModel> SelectedItems
        {
            get { return _selectedItems; }
        }
       
        public ObservableCollection<UploadItemViewModel> ItemsSource
        {
            get { return _itemsSource; }
        }

        public bool OverwriteExistingFiles
        {
            get { return _overwriteExistingFiles; }
            set
            {
                if (_overwriteExistingFiles != value)
                {
                    _overwriteExistingFiles = value;
                    NotifyPropertyChanged(() => OverwriteExistingFiles);
                }
            }
        }

        public IUploadItemListView View
        {
            get { return _view; }
        }

        #endregion

        #region Commands

        public ICommand PauseCommand
        {
            get { return _pauseCommand; }
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        public ICommand ClearCommand
        {
            get { return _clearCommand; }
        }

        public ICommand OptionsCommand
        {
            get { return _optionsCommand; }
        }

        public ICommand SelectedItemsCommand
        {
            get { return _selectedItemsCommand; }
        }

        public ICommand DropCommand
        {
            get { return _dropCommand; }
        }

        #endregion
    }
}
