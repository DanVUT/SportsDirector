using MFORMATSLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MFormat.Model
{
    //Class for creating thumbnails (pictures at some time)
    public class Thumbnailer
    {
        public static readonly Thumbnailer instance = new Thumbnailer();
        private MFReaderClass mediaReader;
        private string currentMedia;

        private Thumbnailer()
        {
            this.mediaReader = new MFReaderClass();
        }

        public BitmapSource GetThumbnail(string filename,double time = 0)
        {
            if(this.currentMedia != filename)
            {
                try
                {
                    this.mediaReader.ReaderClose();
                    this.mediaReader.ReaderOpen(filename, "");
                    this.currentMedia = filename;
                } catch
                {
                    return null;
                }
            }
            MFFrame sourceFrame;
            MFFrame frame;

            this.mediaReader.SourceFrameGetByTime(time, -1, out sourceFrame, "");
            sourceFrame.MFClone(out frame, eMFrameClone.eMFC_Full, eMFCC.eMFCC_ARGB32);

            int nSize, audioSamples;
            long pImage32;
            M_AV_PROPS avProps;
            frame.MFVideoGetBytes(out nSize, out pImage32);
            frame.MFAVPropsGet(out avProps, out audioSamples);
            int nRowBytes = avProps.vidProps.nRowBytes;
            var ptr = new IntPtr(pImage32);
            var bmp = BitmapSource.Create(avProps.vidProps.nWidth,
                    Math.Abs(avProps.vidProps.nHeight), 72, 72,
                    PixelFormats.Bgra32, null, ptr, nSize, nRowBytes);
            bmp.Freeze();
            return bmp;
        }



        public static Thumbnailer Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
