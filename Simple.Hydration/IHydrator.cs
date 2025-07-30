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
        public List<T> HydrateMany<S>(IEnumerable<S> enumerable, Func<S, string, string?> lookup);
        public List<T> HydrateManyWith<S>(IEnumerable<S> enumerable, List<string>? keys, Func<S, string, string?> lookup);
        public List<T> HydrateManyWithout<S>(IEnumerable<S> enumerable, List<string>? keys, Func<S, string, string?> lookup);


        public T Hydrate(T target, Func<string, (string? Result, bool Skip)> lookup);
        public T Hydrate(Func<string, (string? Result, bool Skip)> lookup);
        public T HydrateWith(T target, List<string>? keys, Func<string, (string? Result, bool Skip)> lookup);
        public T HydrateWith(List<string>? keys, Func<string, (string? Result, bool Skip)> lookup);
        public T HydrateWithout(T target, List<string>? keys, Func<string, (string? Result, bool Skip)> lookup);
        public T HydrateWithout(List<string>? keys, Func<string, (string? Result, bool Skip)> lookup);
        public List<T> HydrateMany<S>(IEnumerable<S> enumerable, Func<S, string, (string? Result, bool Skip)> lookup);
        public List<T> HydrateManyWith<S>(IEnumerable<S> enumerable, List<string>? keys, Func<S, string, (string? Result, bool Skip)> lookup);
        public List<T> HydrateManyWithout<S>(IEnumerable<S> enumerable, List<string>? keys, Func<S, string, (string? Result, bool Skip)> lookup);


        // Lookup function accepts a reference tyo the target
        public T Hydrate(Func<T, string, (string? Result, bool Skip)> lookup);
        public T Hydrate(T target, Func<T, string, (string? Result, bool Skip)> lookup);
        public T HydrateWith(List<string>? keys, Func<T, string, (string? Result, bool Skip)> lookup);
        public T HydrateWith(T target, List<string>? keys, Func<T, string, (string? Result, bool Skip)> lookup);
        public T HydrateWithout(T target, List<string>? keys, Func<T, string, (string? Result, bool Skip)> lookup);
        public T HydrateWithout(List<string>? keys, Func<T, string, (string? Result, bool Skip)> lookup);
        public List<T> HydrateMany<S>(IEnumerable<S> enumerable, Func<S, T, string, (string? Result, bool Skip)> lookup);
        public List<T> HydrateManyWith<S>(IEnumerable<S> enumerable, List<string>? keys, Func<S, T, string, (string? Result, bool Skip)> lookup);
        public List<T> HydrateManyWithout<S>(IEnumerable<S> enumerable, List<string>? keys, Func<S, T, string, (string? Result, bool Skip)> lookup);
    }
}
