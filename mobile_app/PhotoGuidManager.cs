using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRtest
{
    public class PhotoGuidManager
    {
        private static IEnumerable<Guid> _photoGuids;
        public static IEnumerable<Guid> PhotoGuids
        {
            get => _photoGuids ?? new List<Guid>();
            set => _photoGuids = value;
        }
    }
}
