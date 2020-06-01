using MediaCatalog2.Model.BaseEntity;

namespace MediaCatalog2.Model.DTO
{
    public class MediaFileDTO :NamedEntity
    {
        public int TimingInFrames { get; set; }
        public string CompleteName { get; set; }
        public string Format { get; set; }
        public string FrameSize { get; set; }
        public int ParentTvProgramId { get; set; }
    }
}
