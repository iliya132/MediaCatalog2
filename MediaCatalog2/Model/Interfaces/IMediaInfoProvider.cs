using MediaCatalog2.Model.DTO;

namespace MediaCatalog2.Model.Interfaces
{
    public interface IMediaInfoProvider
    {
        MediaFileDTO GetMediaFileInfo(string fileName, TV_ProgramDTO parent);
        bool IsMediaFile(string fileName);
    }
}
