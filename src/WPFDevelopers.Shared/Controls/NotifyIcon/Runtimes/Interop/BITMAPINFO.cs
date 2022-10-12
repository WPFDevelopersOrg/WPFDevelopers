using System.Runtime.InteropServices;
using System.Security;

namespace WPFDevelopers.Controls.Runtimes.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct BITMAPINFO
    {
        // bmiHeader was a by-value BITMAPINFOHEADER structure
        public int bmiHeader_biSize;  // ndirect.DllLib.sizeOf( BITMAPINFOHEADER.class );

        public int bmiHeader_biWidth;

        public int bmiHeader_biHeight;

        public short bmiHeader_biPlanes;

        public short bmiHeader_biBitCount;

        public int bmiHeader_biCompression;

        public int bmiHeader_biSizeImage;

        public int bmiHeader_biXPelsPerMeter;

        public int bmiHeader_biYPelsPerMeter;

        public int bmiHeader_biClrUsed;

        public int bmiHeader_biClrImportant;


        // hamidm -- 03/08/2006
        // if the following RGBQUAD struct is added in this struct,
        // we need to update bmiHeader_biSize in the cctor to hard-coded 40
        // since it expects the size of the BITMAPINFOHEADER only
        //
        // bmiColors was an embedded array of RGBQUAD structures
        // public byte     bmiColors_rgbBlue = 0;
        // public byte     bmiColors_rgbGreen = 0;
        // public byte     bmiColors_rgbRed = 0;
        // public byte     bmiColors_rgbReserved = 0;
        public BITMAPINFO(int width, int height, short bpp)
        {
            bmiHeader_biSize = SizeOf();
            bmiHeader_biWidth = width;
            bmiHeader_biHeight = height;
            bmiHeader_biPlanes = 1;
            bmiHeader_biBitCount = bpp;
            bmiHeader_biCompression = 0;
            bmiHeader_biSizeImage = 0;
            bmiHeader_biXPelsPerMeter = 0;
            bmiHeader_biYPelsPerMeter = 0;
            bmiHeader_biClrUsed = 0;
            bmiHeader_biClrImportant = 0;
        }

        /// <SecurityNote>
        ///  Critical : Calls critical Marshal.SizeOf
        ///  Safe     : Calls method with trusted input (well known safe type)
        /// </SecurityNote>
        [SecuritySafeCritical]
        private static int SizeOf()
        {
            return Marshal.SizeOf(typeof(BITMAPINFO));
        }
    }

}
