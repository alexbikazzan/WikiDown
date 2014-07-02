namespace WikiDown.Website
{
    public static class DevEnvironment
    {
        static DevEnvironment()
        {
#if DEBUG
            IsDebug = true;
#endif
        }

        public static bool IsDebug { get; private set; }
    }
}