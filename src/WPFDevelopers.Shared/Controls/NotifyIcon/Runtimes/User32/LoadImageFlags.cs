using System;

namespace WPFDevelopers.Controls.Runtimes.User32
{
    [Flags]
    public enum LoadImageFlags : uint
    {
        //
        // Summary:
        //     When the uType parameter specifies PInvoke.User32.ImageType.IMAGE_BITMAP, causes
        //     the function to return a DIB section bitmap rather than a compatible bitmap.
        //     This flag is useful for loading a bitmap without mapping it to the colors of
        //     the display device.
        LR_CREATEDIBSECTION = 0x2000u,
        //
        // Summary:
        //     The default flag; it does nothing. All it means is "not PInvoke.User32.LoadImageFlags.LR_MONOCHROME".
        LR_DEFAULTCOLOR = 0x0u,
        //
        // Summary:
        //     Uses the width or height specified by the system metric values for cursors or
        //     icons, if the cxDesired or cyDesired values are set to zero. If this flag is
        //     not specified and cxDesired and cyDesired are set to zero, the function uses
        //     the actual resource size. If the resource contains multiple images, the function
        //     uses the size of the first image.
        LR_DEFAULTSIZE = 0x40u,
        //
        // Summary:
        //     Loads the stand-alone image from the file specified by lpszName (icon, cursor,
        //     or bitmap file).
        LR_LOADFROMFILE = 0x10u,
        //
        // Summary:
        //     Searches the color table for the image and replaces the following shades of gray
        //     with the corresponding 3-D color.
        //     • Dk Gray, RGB(128,128,128) with COLOR_3DSHADOW
        //     • Gray, RGB(192,192,192) with COLOR_3DFACE
        //     • Lt Gray, RGB(223,223,223) with COLOR_3DLIGHT
        //     Do not use this option if you are loading a bitmap with a color depth greater
        //     than 8bpp.
        LR_LOADMAP3DCOLORS = 0x1000u,
        //
        // Summary:
        //     Retrieves the color value of the first pixel in the image and replaces the corresponding
        //     entry in the color table with the default window color (COLOR_WINDOW). All pixels
        //     in the image that use that entry become the default window color. This value
        //     applies only to images that have corresponding color tables. Do not use this
        //     option if you are loading a bitmap with a color depth greater than 8bpp. If fuLoad
        //     includes both the PInvoke.User32.LoadImageFlags.LR_LOADTRANSPARENT and PInvoke.User32.LoadImageFlags.LR_LOADMAP3DCOLORS
        //     values, PInvoke.User32.LoadImageFlags.LR_LOADTRANSPARENT takes precedence. However,
        //     the color table entry is replaced with COLOR_3DFACE rather than COLOR_WINDOW.
        LR_LOADTRANSPARENT = 0x20u,
        //
        // Summary:
        //     Loads the image in black and white.
        LR_MONOCHROME = 0x1u,
        //
        // Summary:
        //     Shares the image handle if the image is loaded multiple times. If PInvoke.User32.LoadImageFlags.LR_SHARED
        //     is not set, a second call to LoadImage for the same resource will load the image
        //     again and return a different handle. When you use this flag, the system will
        //     destroy the resource when it is no longer needed. Do not use PInvoke.User32.LoadImageFlags.LR_SHARED
        //     for images that have non-standard sizes, that may change after loading, or that
        //     are loaded from a file. When loading a system icon or cursor, you must use PInvoke.User32.LoadImageFlags.LR_SHARED
        //     or the function will fail to load the resource. This function finds the first
        //     image in the cache with the requested resource name, regardless of the size requested.
        LR_SHARED = 0x8000u,
        //
        // Summary:
        //     Uses true VGA colors.
        LR_VGACOLOR = 0x80u
    }

}
