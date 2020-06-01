using MediaCatalog2.Model.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MediaCatalog2.Controls
{
    public partial class TV_CardsWrap : UserControl
    {
        #region ItemsSource
        private ObservableCollection<TV_ProgramDTO> _itemsSource = new ObservableCollection<TV_ProgramDTO>();
        public ObservableCollection<TV_ProgramDTO> ItemsSource
        {
            get { return _itemsSource; }
            set { _itemsSource = value; }
        }
        #endregion

        #region SelectedItem
        internal delegate void SelectedItemChangedEventHandler();
        internal event SelectedItemChangedEventHandler OnSelectedItemChanged;
        private TV_ProgramDTO _selectedItem;
        public TV_ProgramDTO SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                if (OnSelectedItemChanged != null)
                {
                    OnSelectedItemChanged.Invoke();
                }
            }
        }
        #endregion

        #region Events
        internal delegate void EditEventHandler(TV_Card sender);
        internal event EditEventHandler OnEdit;

        internal delegate void DeleteEventHandler(TV_Card sender);
        internal event DeleteEventHandler OnDelete;

        internal delegate void SelectEventHandler(TV_Card sender);
        internal event SelectEventHandler OnSelect;
        #endregion

        public TV_CardsWrap()
        {
            InitializeComponent();
            OnSelectedItemChanged += UpdateWrapPanel;
            ItemsSource.CollectionChanged += Refresh;
        }

        public void ReloadCard(TV_ProgramDTO changedProgram)
        {
            foreach(TV_Card card in CardsWrapPanel.Children)
            {
                if(card.TVProgram == changedProgram)
                {
                    card.Refresh();
                    break;
                }
            }
        }

        private void Refresh(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateWrapPanel();
        }

        private void UpdateWrapPanel()
        {
            CardsWrapPanel.Children.Clear();
            foreach (TV_ProgramDTO program in ItemsSource)
            {
                TV_Card tvCard = new TV_Card(program);
                tvCard.OnClick += SelectCard;
                tvCard.OnEdit += EditProgram;
                tvCard.OnDelete += DeleteProgram;
                
                if (!IsSourceCorrect(tvCard.TV_Image))
                {
                    tvCard.TV_Image.Source = new BitmapImage(new Uri("/Resources/nopicture.jpg", UriKind.Relative));
                }

                CardsWrapPanel.Children.Add(tvCard);
            }
            if (ItemsSource.Count() > 0)
            {
                if (SelectedItem == null)
                {
                    SelectedItem = ItemsSource[0];
                }
                SelectItem(SelectedItem);
            }
        }

        private void DeleteProgram(object sender)
        {
            if (OnDelete != null)
            {
                OnDelete.Invoke(sender as TV_Card);
            }
        }

        private void EditProgram(object sender)
        {
            if (OnEdit != null)
            {
                OnEdit.Invoke(sender as TV_Card);
            }
        }

        private bool IsSourceCorrect(Image image)
        {
            try
            {
                double i = image.Source.Height;
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void SelectItem(TV_ProgramDTO program)
        {
            foreach (TV_Card card in CardsWrapPanel.Children)
            {
                if (card.TVProgram == program)
                {
                    SelectCard(card);
                    break;
                }
            }
        }

        private void SelectCard(object card)
        {
            UnselectAllCards();
            Select(card as TV_Card);
            if(OnSelect != null)
            {
                OnSelect.Invoke(card as TV_Card);
            }
        }

        private void UnselectAllCards()
        {
            foreach (TV_Card card in CardsWrapPanel.Children)
            {
                card.Unselect();
            }
        }

        private void Select(TV_Card card)
        {
            card.Select();
        }
    }
}
