using System.ComponentModel;

namespace WPFDevelopers.Controls
{
    public enum DialogResult
    {
        Canceled,
        Confirmed
    }

    public enum DialogCloseReason
    {
        Confirm,
        Cancel,
        CloseButton,
        Escape,
        WindowClosing
    }

    public sealed class DialogClosingEventArgs : CancelEventArgs
    {
        public DialogClosingEventArgs(DialogCloseReason reason)
        {
            Reason = reason;
        }

        public DialogCloseReason Reason { get; private set; }
    }
}
