using MediaCatalog2.Model.DTO;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MediaCatalog2.Controls
{
    public partial class TV_Card : UserControl
    {
        internal delegate void OnItemChangedHandler();
        internal event OnItemChangedHandler OnItemChanged;
        private TV_ProgramDTO _tvProgram = new TV_ProgramDTO();
        public TV_ProgramDTO TVProgram 
        {
            get
            {
                return _tvProgram;
            }
            set
            {
                _tvProgram = value;
                if (OnItemChanged != null)
                {
                    OnItemChanged.Invoke();
                }
            }
        }

        internal delegate void ClickEventHandler(object sender);
        internal event ClickEventHandler OnClick;

        internal delegate void EditEventHandler(object sender);
        internal event EditEventHandler OnEdit;

        internal delegate void DeleteEventHandler(object sender);
        internal event DeleteEventHandler OnDelete;

        public TV_Card(TV_ProgramDTO program)
        {
            InitializeComponent();
            OnItemChanged += DrawCard;
            TVProgram = program;
        }

        private void DrawCard()
        {
            NameBlock.Text = TVProgram.Name;
            if (string.IsNullOrWhiteSpace(TVProgram.AvatarSourcePath))
            {
                TV_Image.Source = new BitmapImage(new Uri("/Resources/nopicture.jpg", UriKind.RelativeOrAbsolute));
            }
            else
            {
                TV_Image.Source = new BitmapImage(new Uri(TVProgram.AvatarSourcePath, UriKind.RelativeOrAbsolute));
            }
        }

        public void Select()
        {
            CardBorder.BorderBrush = Brushes.Red;
        }

        public void Unselect()
        {
            CardBorder.BorderBrush = Brushes.Black;
        }

        private void CardBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OnClick != null)
            {
                OnClick.Invoke(this);
            }
        }

        private void ContextEdit_Click(object sender, RoutedEventArgs e)
        {
            if (OnEdit != null)
            {
                OnEdit.Invoke(this);
            }
        }

        private void ContextDelete_Click(object sender, RoutedEventArgs e)
        {
            if (OnDelete != null)
            {
                OnDelete.Invoke(this);
            }
        }

        internal void Refresh()
        {
            DrawCard();
        }
    }
}
