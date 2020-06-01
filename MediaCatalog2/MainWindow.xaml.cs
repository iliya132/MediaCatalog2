using MediaCatalog2.Controls;
using MediaCatalog2.Model.DTO;
using MediaCatalog2.Model.Implementations;
using MediaCatalog2.Model.Interfaces;

using Microsoft.Win32;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MediaCatalog2
{
    public partial class MainWindow : Window
    {
        IProgramsProvider _dataProvider;
        IMediaInfoProvider _mediaInfoProvider;
        internal ObservableCollection<TV_ProgramDTO> AllPrograms;

        #region SelectedItem
        private delegate void SelectedProgramChangedHandler();
        private event SelectedProgramChangedHandler OnSelectedProgramChanged;
        private TV_ProgramDTO _selectedProgram = new TV_ProgramDTO();
        public TV_ProgramDTO SelectedProgram
        {
            get
            {
                return _selectedProgram;
            }
            set
            {
                _selectedProgram = value;
                if (OnSelectedProgramChanged != null)
                {
                    OnSelectedProgramChanged.Invoke();
                }
            }
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
            InitializeEvents();
            InitializeForm();
        }

        private void InitializeData()
        {
            _dataProvider = new SQLiteDataProvider();
            _mediaInfoProvider = new MediaInfo();
            AllPrograms = new ObservableCollection<TV_ProgramDTO>(_dataProvider.GetPrograms());
            DisableMainPanelIfNoCardsExist(null, null);
        }

        private void InitializeEvents()
        {
            AllPrograms.CollectionChanged += UpdateForm;
            AllPrograms.CollectionChanged += DisableMainPanelIfNoCardsExist;
            OnSelectedProgramChanged += SelectedProgramChanged;
            CardsWrapPanel.OnSelect += CardSelectionChanged;
            CardsWrapPanel.OnEdit += CardsWrapPanel_OnEdit;
            CardsWrapPanel.OnDelete += CardsWrapPanel_OnDelete;
        }

        private void DisableMainPanelIfNoCardsExist(object sender, NotifyCollectionChangedEventArgs e)
        {
            SwitchInterfaceEnabled(AllPrograms.Count == 0);
        }

        private void SwitchInterfaceEnabled(bool isEnabled)
        {
            EditProgram.IsEnabled = isEnabled;
            DeleteProgram.IsEnabled = isEnabled;
            ExtendedCardInfoGrid.IsEnabled = isEnabled;
        }

        private void CardsWrapPanel_OnEdit(TV_Card sender)
        {
            CardsWrapPanel.SelectedItem = sender.TVProgram;
            EditProgramMethod();
        }

        private void SelectedProgramChanged()
        {
            if (SelectedProgram != null)
            {
                TV_NameBlock.Text = string.Format("Название программы: {0}", SelectedProgram.Name);
                TV_ActorsBlock.Text = string.Format("Ведущие: {0}", SelectedProgram.Actors);
                TV_DescriptionBlock.Text = string.Format("Описание программы: {0}", SelectedProgram.Description);
                TV_EstablishedBlock.Text = string.Format("Год выпуска: {0}", SelectedProgram.YearEstablished.ToString());
                TV_FilesList.ItemsSource = SelectedProgram.MediaFiles;
            }
            else
            {
                TV_NameBlock.Text = "Название программы:";
                TV_ActorsBlock.Text = "Ведущие:";
                TV_DescriptionBlock.Text = "Описание программы:";
                TV_EstablishedBlock.Text = "Год выпуска:";
                TV_FilesList.ItemsSource = null;
            }
        }

        private void CardSelectionChanged(object sender)
        {
            SelectedProgram = (sender as TV_Card).TVProgram;
        }

        private void UpdateForm(object sender, NotifyCollectionChangedEventArgs e)
        {
            InitializeForm();
        }

        private void InitializeForm()
        {
            CardsWrapPanel.ItemsSource.Clear();
            foreach (TV_ProgramDTO program in AllPrograms)
            {
                CardsWrapPanel.ItemsSource.Add(program);
            }
        }

        private void ScrollViewer_DragEnter(object sender, DragEventArgs e)
        {
            ContentBorder.BorderThickness = new Thickness(2);
            ContentBorder.BorderBrush = Brushes.OrangeRed;
        }

        private void ScrollViewer_DragLeave(object sender, DragEventArgs e)
        {
            ContentBorder.BorderThickness = new Thickness(0, 2, 0, 0);
            ContentBorder.BorderBrush = Brushes.DarkGray;
        }

        private void AddNewProgram_Click(object sender, RoutedEventArgs e)
        {
            TV_ProgramDTO EditedProgram = new TV_ProgramDTO();
            EditProgramWindow editWindow = new EditProgramWindow(EditedProgram);
            if (editWindow.ShowDialog() == true)
            {
                _dataProvider.AddProgram(EditedProgram);
                AllPrograms.Add(EditedProgram);
            }

            if (SelectedProgram == null)
            {
                CardsWrapPanel.SelectedItem = AllPrograms[0];
            }
        }

        private void EditProgram_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProgram == null) { return; }
            EditProgramMethod();
        }

        private void EditProgramMethod()
        {
            TV_ProgramDTO EditedProgram = new TV_ProgramDTO
            {
                Actors = SelectedProgram.Actors,
                AvatarSourcePath = SelectedProgram.AvatarSourcePath,
                Description = SelectedProgram.Description,
                Name = SelectedProgram.Name,
                YearEstablished = SelectedProgram.YearEstablished
            };

            EditProgramWindow editWindow = new EditProgramWindow(EditedProgram);
            if (editWindow.ShowDialog() == true)
            {
                SelectedProgram.Actors = EditedProgram.Actors;
                SelectedProgram.AvatarSourcePath = EditedProgram.AvatarSourcePath;
                SelectedProgram.Description = EditedProgram.Description;
                SelectedProgram.Name = EditedProgram.Name;
                SelectedProgram.YearEstablished = EditedProgram.YearEstablished;
                _dataProvider.EditProgram(oldValue: SelectedProgram, newValue: EditedProgram);
                if (OnSelectedProgramChanged != null)
                {
                    OnSelectedProgramChanged.Invoke();
                }
                CardsWrapPanel.ReloadCard(SelectedProgram);
            }
        }

        private void DeleteProgram_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProgram == null)
            {
                return;
            }
            DeleteProgramHandle(SelectedProgram);
        }

        private void CardsWrapPanel_OnDelete(TV_Card sender)
        {
            DeleteProgramHandle(sender.TVProgram);
        }

        private void DeleteProgramHandle(TV_ProgramDTO program)
        {
            AllPrograms.Remove(program);
            _dataProvider.Remove(program);
            if (SelectedProgram == program && AllPrograms.Count > 0)
            {
                CardsWrapPanel.SelectedItem = AllPrograms[0];
            }
            else if (AllPrograms.Count == 0)
            {
                CardsWrapPanel.SelectedItem = null;
                SelectedProgram = null;
                SelectedProgramChanged();
            }
        }

        private void ScrollViewer_Drop(object sender, DragEventArgs DroppedArgs)
        {
            ScrollViewer_DragLeave(sender, DroppedArgs);

            if (!IsDroppedDataAvailable(DroppedArgs))
            {
                return;
            }
            string[] fileNames = GetDroppedFileNames(DroppedArgs);
            AddVideosAsync(fileNames);
        }

        private bool IsDroppedDataAvailable(DragEventArgs args)
        {
            return args.Data.GetDataPresent(DataFormats.FileDrop);
        }

        private string[] GetDroppedFileNames(DragEventArgs args)
        {
            return (string[])args.Data.GetData(DataFormats.FileDrop);
        }

        private async void AddVideosAsync(string[] Files)
        {
            TV_ProgramDTO parentProgram = SelectedProgram;
            foreach (string file in Files)
            {
                if (!_mediaInfoProvider.IsMediaFile(file))
                {
                    continue;
                }

                await Task.Run(() =>
                {
                    MediaFileDTO video = _mediaInfoProvider.GetMediaFileInfo(file, SelectedProgram);
                    video.ParentTvProgramId = parentProgram.Id;
                    App.Current.Dispatcher.Invoke(() => parentProgram.MediaFiles.Add(video));
                    App.Current.Dispatcher.Invoke(() => _dataProvider.AddVideoFileAsync(video));
                });
            }
            SelectedProgramChanged();
        }

        private void SelectVideosBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == true)
            {
                AddVideosAsync(dialog.FileNames);
            }
        }

        private void DeleteVideoBtn_Click(object sender, RoutedEventArgs e)
        {
            Button clickedBtn = sender as Button;
            MediaFileDTO mediaToDelete = clickedBtn.DataContext as MediaFileDTO;
            _dataProvider.RemoveMediaFile(mediaToDelete);
            SelectedProgram.MediaFiles.Remove(mediaToDelete);
            SelectedProgramChanged();
        }
    }
}
