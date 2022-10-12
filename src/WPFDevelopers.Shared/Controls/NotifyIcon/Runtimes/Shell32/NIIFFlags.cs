namespace WPFDevelopers.Controls.Runtimes.Shell32
{
    public enum NIIFFlags
    {
        /// <summary>
        ///  No icon.
        /// </summary>
        NIIF_NONE = 0x00,

        /// <summary>
        /// An information icon.
        /// </summary>
        NIIF_INFO = 0x01,

        /// <summary>
        ///  A warning icon.
        /// </summary>
        NIIF_WARNING = 0x02,

        /// <summary>
        /// An error icon. 
        /// </summary>
        NIIF_ERROR = 0x03,

        /// <summary>
        /// Windows XP: Use the icon identified in hIcon as the notification balloon's title icon.
        /// Windows Vista and later: Use the icon identified in hBalloonIcon as the notification balloon's title icon.
        /// </summary>
        NIIF_USER = 0x04,

        /// <summary>
        /// Windows XP and later. Do not play the associated sound. Applies only to notifications.
        /// </summary>
        NIIF_NOSOUND = 0x10,

        /// <summary>
        /// Windows Vista and later. The large version of the icon should be used as the notification icon. 
        /// This corresponds to the icon with dimensions SM_CXICON x SM_CYICON. 
        /// If this flag is not set, the icon with dimensions XM_CXSMICON x SM_CYSMICON is used.
        /// Applications that use older customized icons (NIIF_USER with hIcon) must provide a new SM_CXICON x SM_CYICON version in the tray icon (hIcon). 
        /// These icons are scaled down when they are displayed in the System Tray or System Control Area (SCA).
        /// New customized icons (NIIF_USER with hBalloonIcon) must supply an SM_CXICON x SM_CYICON version in the supplied icon (hBalloonIcon).
        /// </summary>
        NIIF_LARGE_ICON = 0x20,

        /// <summary>
        /// Do not display the balloon notification if the current user is in "quiet time", 
        /// which is the first hour after a new user logs into his or her account for the first time. 
        /// During this time, most notifications should not be sent or shown. 
        /// This lets a user become accustomed to a new computer system without those distractions. 
        /// Quiet time also occurs for each user after an operating system upgrade or clean installation. 
        /// A notification sent with this flag during quiet time is not queued;
        /// it is simply dismissed unshown. The application can resend the notification later if it is still valid at that time.
        /// During quiet time, certain notifications should still be sent because they are expected by the user as feedback in response to a user action, 
        /// for instance when he or she plugs in a USB device or prints a document.
        /// </summary>
        NIIF_RESPECT_QUIET_TIME = 0x80,

        /// <summary>
        /// Windows XP and later. Reserved.
        /// </summary>
        NIIF_ICON_MASK = 0x0F,


    }
}
