using System;
using System.Reactive.Linq;

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
