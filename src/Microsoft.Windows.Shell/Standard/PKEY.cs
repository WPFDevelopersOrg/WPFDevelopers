using System;
using System.Runtime.InteropServices;

namespace Standard
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PKEY
    {
        public PKEY(Guid fmtid, uint pid)
        {
            this._fmtid = fmtid;
            this._pid = pid;
        }

        private readonly Guid _fmtid;

        private readonly uint _pid;

        public static readonly PKEY Title = new PKEY(new Guid("F29F85E0-4FF9-1068-AB91-08002B27B3D9"), 2U);

        public static readonly PKEY AppUserModel_ID = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5U);

        public static readonly PKEY AppUserModel_IsDestListSeparator = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 6U);

        public static readonly PKEY AppUserModel_RelaunchCommand = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 2U);

        public static readonly PKEY AppUserModel_RelaunchDisplayNameResource = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 4U);

        public static readonly PKEY AppUserModel_RelaunchIconResource = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 3U);
    }
}
