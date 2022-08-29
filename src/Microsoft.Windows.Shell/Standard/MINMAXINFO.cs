namespace Standard
{
    public struct MINMAXINFO
    {
        //public MINMAXINFO(POINT ptReserved, POINT ptMaxSize, POINT ptMaxPosition, POINT ptMinTrackSize, POINT ptMaxTrackSize)
        //{
        //    this.ptReserved = ptReserved;
        //    this.ptMaxSize = ptMaxSize;
        //    this.ptMaxPosition = ptMaxPosition;
        //    this.ptMinTrackSize = ptMinTrackSize;
        //    this.ptMaxTrackSize = ptMaxTrackSize;
        //}

        public POINT ptReserved;

        public POINT ptMaxSize;

        public POINT ptMaxPosition;

        public POINT ptMinTrackSize;

        public POINT ptMaxTrackSize;
    }
}
