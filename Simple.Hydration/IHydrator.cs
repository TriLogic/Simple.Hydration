using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Hydration
{
    public interface IHydrator<T>
    {
        public T Hydrate(T target, Func<string, string?> lookup);
        public T Hydrate(Func<string, string?> lookup);
        public T HydrateWith(T target, List<string>? keys, Func<string, string?> lookup);
        public T HydrateWith(List<string>? keys, Func<string, string?> lookup);
        public T HydrateWithout(T target, List<string>? keys, Func<string, string?> lookup);
        public T HydrateWithout(List<string>? keys, Func<string, string?> lookup);
        public List<T> Hydrate<S>(IEnumerable<S> enumerable, Func<S, string, string?> lookup);
        public List<T> HydrateWith<S>(IEnumerable<S> enumerable, List<string>? keys, Func<S, string, string?> lookup);
        public List<T> HydrateWithout<S>(IEnumerable<S> enumerable, List<string>? keys, Func<S, string, string?> lookup);


        public T Hydrate(T target, Func<string, (string? Result, bool Skip)> lookup);
        public T Hydrate(Func<string, (string? Result, bool Skip)> lookup);
        public T HydrateWith(T target, List<string>? keys, Func<string, (string? Result, bool Skip)> lookup);
        public T HydrateWith(List<string>? keys, Func<string, (string? Result, bool Skip)> lookup);
        public T HydrateWithout(T target, List<string>? keys, Func<string, (string? Result, bool Skip)> lookup);
        public T HydrateWithout(List<string>? keys, Func<string, (string? Result, bool Skip)> lookup);
        public List<T> Hydrate<S>(IEnumerable<S> enumerable, Func<S, string, (string? Result, bool Skip)> lookup);
        public List<T> HydrateWith<S>(IEnumerable<S> enumerable, List<string>? keys, Func<S, string, (string? Result, bool Skip)> lookup);
        public List<T> HydrateWithout<S>(IEnumerable<S> enumerable, List<string>? keys, Func<S, string, (string? Result, bool Skip)> lookup);

    }
}
