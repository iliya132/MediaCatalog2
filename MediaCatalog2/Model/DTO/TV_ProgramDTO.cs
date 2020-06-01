using MediaCatalog2.Model.BaseEntity;
using System.Collections.ObjectModel;

namespace MediaCatalog2.Model.DTO
{
    public class TV_ProgramDTO :NamedEntity
    {
        public string Description { get; set; }
        public string Actors { get; set; }
        public int YearEstablished { get; set; }
        public string AvatarSourcePath { get; set; }
        private ObservableCollection<MediaFileDTO> _mediaFiles = new ObservableCollection<MediaFileDTO>();
        public ObservableCollection<MediaFileDTO> MediaFiles
        {
            get
            {
                return _mediaFiles;
            }
            set
            {
                _mediaFiles = value;
            }
        }
    }
}
