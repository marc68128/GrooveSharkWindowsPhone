using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrooveSharkWindowsPhone.Helpers
{
    public static class ObservableExt
    {
        public static IObservable<T> WhereNotNull<T>(this IObservable<T> obs)
        {
            return obs.Where(x => !ReferenceEquals(null, x));
        }
    }
}
