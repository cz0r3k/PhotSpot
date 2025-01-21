using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRtest
{
    public class EventInfoManager
    {
        private static Guid? _eventId;

        private static string _eventName;

        public static Guid? EventId
        {
            get => _eventId;
            set => _eventId = value;
        }

        public static string EventName
        {
            get => _eventName;
            set => _eventName = value;
        }
    }
}
