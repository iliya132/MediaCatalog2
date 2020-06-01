using MediaCatalog2.Model.Interfaces;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MediaCatalog2.Model.Implementations
{
    public enum StreamKind
    {
        General,
        Video,
        Audio,
        Text,
        Other,
        Image,
        Menu
    }

    public enum InfoKind
    {
        Name,
        Text,
        Measure,
        Options,
        NameText,
        MeasureText,
        Info,
        HowTo
    }

    public class MediaInfo : IMediaInfoProvider
    {
        readonly string[] videoFormats = { ".264", ".3g2", ".3gp", ".3gp2", ".3gpp", ".3gpp2", ".3mm", ".3p2", ".60d", ".787", ".890", ".aaf", ".aec", ".aep", ".aepx", ".aet", ".aetx", ".ajp", ".ale", ".am", ".amc", ".amv", ".amx", ".anim", ".anx", ".arcut", ".arf", ".asf", ".asx", ".av", ".avb", ".avc", ".avchd", ".ave", ".avi", ".avp", ".avs", ".avv", ".axm", ".bdm", ".bdmv", ".bdt2", ".bdt3", ".bik", ".bik2", ".bin", ".bix", ".bk2", ".blz", ".bmc", ".bmk", ".bnp", ".box", ".bs4", ".bsf", ".bu", ".bvr", ".byu", ".camproj", ".camrec", ".camv", ".ced", ".cel", ".cine", ".cip", ".clk", ".clpi", ".cme", ".cmmp", ".cmmtpl", ".cmproj", ".cmrec", ".cpi", ".cpvc", ".cst", ".cx3", ".d2v", ".d3v", ".dad", ".dash", ".dat", ".dav", ".dce", ".dck", ".dcr", ".dif", ".dir", ".divx", ".dlx", ".dmb", ".dmsd", ".dmsd3d", ".dmsm", ".dmsm3d", ".dmss", ".dmx", ".dpa", ".dpg", ".dream", ".dsy", ".dv", ".dv-avi", ".dv4", ".dvdmedia", ".dvr", ".dvr-ms", ".dvx", ".dxr", ".dzm", ".dzp", ".dzt", ".edl", ".evo", ".exo", ".exp", ".eye", ".eyetv", ".ezt", ".f4f", ".f4m", ".f4p", ".f4v", ".fbr", ".fbz", ".fcarch", ".fcp", ".fcproject", ".ffd", ".ffm", ".flc", ".flh", ".fli", ".flic", ".flv", ".flx", ".fpdx", ".ftc", ".fvt", ".g2m", ".g64", ".g64x", ".gcs", ".gfp", ".gifv", ".gl", ".gom", ".grasp", ".gts", ".gvi", ".gvp", ".gxf", ".h264", ".hdmov", ".hdv", ".hevc", ".hkm", ".ifo", ".imovieproj", ".inp", ".insv", ".int", ".ircp", ".irf", ".ism", ".ismc", ".ismclip", ".ismv", ".iva", ".ivf", ".ivr", ".ivs", ".izz", ".izzy", ".jdr", ".jmv", ".jnr", ".jss", ".jts", ".jtv", ".k3g", ".kdenlive", ".kmv", ".ktn", ".lrec", ".lrv", ".lsf", ".lsx", ".lvix", ".m1pg", ".m21", ".m2p", ".m2t", ".m2ts", ".m2v", ".m4v", ".mani", ".mgv", ".mj2", ".mjp", ".mk3d", ".mkv", ".mnv", ".mod", ".moi", ".mov", ".mp21", ".mp4", ".mpeg", ".mpf", ".mpg", ".mpgindex", ".mpl", ".mpls", ".mproj", ".mpv", ".mqv", ".msdvd", ".mse", ".mswmm", ".mts", ".mtv", ".mvc", ".mvd", ".mve", ".mvp", ".mvy", ".mxf", ".mxv", ".n3r", ".ncor", ".nfv", ".nsv", ".ntp", ".nut", ".nuv", ".nvc", ".ogm", ".ogv", ".ogx", ".orv", ".osp", ".otrkey", ".pac", ".par", ".pds", ".pgi", ".photoshow", ".piv", ".pjs", ".plproj", ".pmf", ".pns", ".ppj", ".prel", ".pro", ".prproj", ".prtl", ".psb", ".psh", ".psv", ".pvr", ".pxv", ".qsv", ".qt", ".qtch", ".qtindex", ".qtl", ".qtm", ".qtz", ".r3d", ".ravi", ".rcd", ".rcproject", ".rcrec", ".rcut", ".rdb", ".rec", ".rm", ".rmd", ".rmp", ".rms", ".rmv", ".rmvb", ".roq", ".rp", ".rsx", ".rts", ".rum", ".rv", ".rvid", ".rvl", ".sbk", ".sbz", ".scc", ".scm", ".scn", ".screenflow", ".sdv", ".sec", ".seq", ".sfd", ".sfvidcap", ".siv", ".smi", ".smil", ".smk", ".snagproj", ".spl", ".sqz", ".srt", ".ssf", ".stl", ".str", ".stx", ".svi", ".swf", ".swi", ".swt", ".tda3mt", ".tdt", ".theater", ".thp", ".tid", ".tivo", ".tix", ".tod", ".tp", ".tp0", ".tpd", ".tpr", ".trec", ".trp", ".ts", ".tsp", ".tsv", ".ttxt", ".tvlayer", ".tvs", ".tvshow", ".usf", ".usm", ".v264", ".vbc", ".vc1", ".vcpf", ".vcr", ".vcv", ".vdo", ".vdr", ".vdx", ".veg", ".vem", ".vep", ".vf", ".vft", ".vfw", ".vfz", ".vgz", ".vid", ".video", ".viewlet", ".viv", ".vivo", ".vix", ".vlab", ".vmlf", ".vmlt", ".vob", ".vp3", ".vp6", ".vp7", ".vpj", ".vr", ".vro", ".vs4", ".vse", ".vsh", ".vsp", ".vtt", ".w32", ".wcp", ".webm", ".wfsp", ".wgi", ".wlmp", ".wm", ".wmd", ".wmmp", ".wmv", ".wmx", ".wot", ".wp3", ".wpl", ".wsve", ".wtv", ".wvm", ".wvx", ".wxp", ".xej", ".xel", ".xesc", ".xfl", ".xlmv", ".xmv", ".xvid", ".y4m", ".yog", ".yuv", ".zeg", ".zm1", ".zm2", ".zm3", ".zmv" };

        #region DllImport
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_New();
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName);
        [DllImport("MediaInfo.dll")]
        private static extern void MediaInfo_Close(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, [MarshalAs(UnmanagedType.LPWStr)] string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
        #endregion

        private IntPtr Handle;

        public MediaInfo()
        {
            try
            {
                Handle = MediaInfo_New();
            }
            catch
            {
                throw new Exception("Не удалось подключить библиотеку mediainfo.dll");
            }
        }

        private int Open(string FileName)
        {
            return (int)MediaInfo_Open(Handle, FileName);
        }

        private string Get(StreamKind StreamKind, int StreamNumber, string Parameter) 
        {
            return Marshal.PtrToStringUni(MediaInfo_Get(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, Parameter, (IntPtr)InfoKind.Text, (IntPtr)InfoKind.Name));
        }

        private void Close() 
        {
            MediaInfo_Close(Handle); 
        }

        public DTO.MediaFileDTO GetMediaFileInfo(string fileName, DTO.TV_ProgramDTO parent)
        {
            Open(fileName);
            string filename = Get(StreamKind.General, 0, "FileName");
            string frameCount = Get(StreamKind.Video, 0, "FrameCount");
            string FullName = Get(StreamKind.General, 0, "CompleteName");
            string FormatInfo = Get(StreamKind.Video, 0, "Format/Info");
            string Width = Get(StreamKind.Video, 0, "Width");
            string Height = Get(StreamKind.Video, 0, "Height");
            string Size = string.Format("{0}x{1}", Width, Height);

            try
            {
                return new DTO.MediaFileDTO
                {
                    Name = filename,
                    ParentTvProgramId = parent.Id,
                    CompleteName = FullName,
                    Format = FormatInfo,
                    FrameSize = Size,
                    TimingInFrames = Convert.ToInt32(frameCount)
                };
            }
            catch
            {
                throw new InvalidDataException("Не удалось получить информацию о файле");
            }
            
        }

        public bool IsMediaFile(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            foreach (string allowedExtension in videoFormats)
            {
                if (allowedExtension.ToLower().Equals(fileInfo.Extension.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        ~MediaInfo(){
            Close();
        }
    }
}
