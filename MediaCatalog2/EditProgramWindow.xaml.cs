using MediaCatalog2.Model.DTO;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media;

namespace MediaCatalog2
{
    public partial class EditProgramWindow : Window
    {
        TV_ProgramDTO EditedProgram;
        public EditProgramWindow(TV_ProgramDTO program)
        {
            InitializeComponent();
            EditedProgram = program;
            InitializeForm();
        }

        private void InitializeForm()
        {
            if (EditedProgram == null) 
            { 
                return;
            }
            ProgramName.Text = EditedProgram.Name;
            ProgramDescription.Text = EditedProgram.Description;
            Actors.Text = EditedProgram.Actors;
            YearEstablished.Text = EditedProgram.YearEstablished.ToString();
            AvatarFilePath.Text = EditedProgram.AvatarSourcePath;
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                EditedProgram.Name = ProgramName.Text;
                EditedProgram.Description = ProgramDescription.Text;
                EditedProgram.Actors = Actors.Text;
                EditedProgram.YearEstablished = int.Parse(YearEstablished.Text);
                EditedProgram.AvatarSourcePath = AvatarFilePath.Text;
                DialogResult = true;
                Close();
            }
        }

        private bool ValidateFields()
        {
            bool success = true;

            if (string.IsNullOrWhiteSpace(ProgramName.Text))
            {
                success = false;
                ProgramNameLabel.Text = "Поле не может быть пустым";
                ProgramNameLabel.Foreground = Brushes.Red;
            }

            if (string.IsNullOrWhiteSpace(ProgramDescription.Text))
            {
                success = false;
                ProgramDescriptionLabel.Text = "Поле не может быть пустым";
                ProgramDescriptionLabel.Foreground = Brushes.Red;
            }

            if (string.IsNullOrWhiteSpace(Actors.Text))
            {
                success = false;
                ActorsLabel.Text = "Поле не может быть пустым";
                ActorsLabel.Foreground = Brushes.Red;
            }

            if (string.IsNullOrWhiteSpace(YearEstablished.Text))
            {
                success = false;
                YearEstablishedLabel.Text = "Поле не может быть пустым";
                YearEstablishedLabel.Foreground = Brushes.Red;
            }

            int Year;
            if(!int.TryParse(YearEstablished.Text, out Year))
            {
                success = false;
                YearEstablishedLabel.Text = "Поле принимает только целые числа";
                YearEstablishedLabel.Foreground = Brushes.Red;
            }

            if(Year > DateTime.Now.Year || Year < 1910)
            {
                success = false;
                YearEstablishedLabel.Text = "Указан некорректный год.";
                YearEstablishedLabel.Foreground = Brushes.Red;
            }

            return success;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Изображения (*.jpg, *.png)|*.jpg;*.png";
            if (openFile.ShowDialog() == true)
            {
                AvatarFilePath.Text = openFile.FileName;
            }
        }
    }
}
