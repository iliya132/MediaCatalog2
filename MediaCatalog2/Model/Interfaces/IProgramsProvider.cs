using System.Collections.Generic;
using System.Threading.Tasks;

using MediaCatalog2.Model.DTO;

namespace MediaCatalog2.Model.Interfaces
{
    public interface IProgramsProvider
    {
        IEnumerable<TV_ProgramDTO> GetPrograms();
        void AddProgram(TV_ProgramDTO Program);
        void AddVideoFileAsync(MediaFileDTO video);
        void RemoveMediaFile(MediaFileDTO video);
        void Remove(TV_ProgramDTO Program);
        void CommitChangesAsync();
        void EditProgram(TV_ProgramDTO oldValue, TV_ProgramDTO newValue);
    }
}
