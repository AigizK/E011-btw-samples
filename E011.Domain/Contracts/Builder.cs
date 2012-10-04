using System;

namespace E011
{
    public static class Current
    {
        static Func<DateTime> _getTime = GetUtc;
        static Func<Guid> _getGuid = GetGuid;

        static DateTime GetUtc()
        {
            return new DateTime(DateTime.UtcNow.Ticks, DateTimeKind.Unspecified);
        }
        static Guid GetGuid()
        {
            return Guid.NewGuid();
        }

        public static void DateIs(DateTime time)
        {
            _getTime = () => new DateTime(time.Ticks, DateTimeKind.Unspecified);
        }

        public static readonly DateTime MaxValue = new DateTime(9999, 12, 31, 0, 0, 0, DateTimeKind.Unspecified);


        public static void DateIs(int year, int month = 1, int day = 1)
        {
            DateIs(new DateTime(year, month, day));
        }

        public static void GuidIs(Guid guid)
        {
            _getGuid = () => guid;
        }
        public static void GuidIs(string guid)
        {
            var g = Guid.Parse(guid);
            _getGuid = () => g;
        }

        public static void Reset()
        {
            _getTime = GetUtc;
            _getGuid = GetGuid;

        }

        public static DateTime UtcNow { get { return _getTime(); } }
        public static Guid NewGuid { get { return _getGuid(); } }
    }
}
