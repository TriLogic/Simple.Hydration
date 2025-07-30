using System.ComponentModel;
using System.Reflection;

namespace Simple.Hydration
{
    public class Hydrator<T> : IHydrator<T> where T : class, new()    
    {
        private List<IHydrate> Hydrators { get; set; } = new();

        #region Constructors and Destructors
        public Hydrator()
        {
            IncludeHydrates();
        }
        public Hydrator(IEnumerable<string>? IncludedKeys = null, IEnumerable<string>? ExcludedKeys = null)
        {
            IncludeHydrates();

            IncludeThese(IncludedKeys);
            ExcludeThese(ExcludedKeys);
        }
        #endregion

        #region List Tailoring
        public void IncludeThese(IEnumerable<string>? SpecificKeys)
        {
            if (SpecificKeys == null)
                return;

            var selected = Hydrators
                .Where(h => SpecificKeys.FirstOrDefault(h.GetKey()) != null)
                .ToList();

            Hydrators = selected;
        }

        public void ExcludeThese(IEnumerable<string>? SpecificKeys)
        {
            if (SpecificKeys == null)
                return;

            var selected = Hydrators
                .Where(h => SpecificKeys.FirstOrDefault(h.GetKey()) == null)
                .ToList();

            Hydrators = selected;
        }
        #endregion

        #region Hydration List Creation
        private void IncludeHydrates()
        {
            Type type = typeof(T);

            // process fields and properties
            List<MemberInfo> members = type
                .GetMembers(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => m.MemberType == MemberTypes.Field | m.MemberType == MemberTypes.Property)
                .ToList();

            // fields first
            members
                .Where(m => m.MemberType == MemberTypes.Field)
                .ToList()
                .ForEach(m =>
                {
                    Hydrators.Add(new FieldHydrate((FieldInfo)m));
                });

            // properties second
            members
                .Where(m => m.MemberType == MemberTypes.Property)
                .ToList()
                .ForEach(m =>
                {
                    Hydrators.Add(new PropertyHydrate((PropertyInfo)m));
                });
        }
        #endregion

        #region Object Hydration with Simple Lookup
        public T Hydrate(Func<string, string?> lookup)
        {
            return Hydrate(new(), lookup);
        }

        public T Hydrate(T target, Func<string, string?> lookup)
        {
            Hydrators.ForEach(h =>
            {
                h.Hydrate(target, lookup(h.GetKey()));
            });
            return target;
        }

        public T HydrateWith(IEnumerable<string>? keys, Func<string, string?> lookup)
        {
            return HydrateWith(new(), keys, lookup);
        }

        public T HydrateWith(T target, IEnumerable<string>? keys, Func<string, string?> lookup)
        {
            var selected = keys == null ? Hydrators : Hydrators
                .Where(h => keys.FirstOrDefault(h.GetKey()) != null)
                .ToList();

            Hydrators.ForEach(h =>
            {
                h.Hydrate(target, lookup(h.GetKey()));
            });

            return target;
        }

        public T HydrateWithout(IEnumerable<string>? keys, Func<string, string?> lookup)
        {
            return HydrateWithout(new(), keys, lookup);
        }

        public T HydrateWithout(T target, IEnumerable<string>? keys, Func<string, string?> lookup)
        {
            var selected = keys == null ? Hydrators : Hydrators
                .Where(h => keys.FirstOrDefault(h.GetKey()) == null)
                .ToList();

            Hydrators.ForEach(h =>
            {
                h.Hydrate(target, lookup(h.GetKey()));
            });
            return target;
        }

        public List<T> Hydrate<S>(IEnumerable<S> enumerable, Func<S, string, string?> lookup)
        {
            List<T> result = new();

            foreach (S s in enumerable)
            {
                T t = new();
                Hydrators.ForEach(h =>
                {
                    h.Hydrate(t, lookup(s, h.GetKey()));
                });
                result.Add(t);
            }

            return result;
        }

        public List<T> HydrateWith<S>(IEnumerable<S> enumerable, IEnumerable<string> keys, Func<S, string, string?> lookup)
        {
            var selected = keys == null ? Hydrators : Hydrators
                .Where(h => keys.FirstOrDefault(h.GetKey()) != null)
                .ToList();

            List<T> result = new();

            foreach (S s in enumerable)
            {
                T t = new();
                selected.ForEach(h =>
                {
                    h.Hydrate(t, lookup(s, h.GetKey()));
                });
                result.Add(t);
            }

            return result;
        }

        public List<T> HydrateWithout<S>(IEnumerable<S> enumerable, IEnumerable<string>? keys, Func<S, string, string?> lookup)
        {
            var selected = keys == null ? Hydrators : Hydrators
                .Where(h => keys.FirstOrDefault(h.GetKey()) == null)
                .ToList();

            List<T> result = new();

            foreach (S s in enumerable)
            {
                T t = new();
                selected.ForEach(h =>
                {
                    h.Hydrate(t, lookup(s, h.GetKey()));
                });
                result.Add(t);
            }

            return result;
        }
        #endregion

        #region Object Hydration with Skip Lookup
        public T Hydrate(Func<string, (string? Result, bool Skip)> lookup)
        {
            return Hydrate(new(), lookup);
        }

        public T Hydrate(T target, Func<string, (string? Result, bool Skip)> lookup)
        {
            Hydrators.ForEach(h =>
            {
                var result = lookup(h.GetKey());

                if (!result.Skip)
                {
                    h.Hydrate(target, result.Result);
                }
            });
            return target;
        }

        public T HydrateWith(IEnumerable<string>? keys, Func<string, (string? Result, bool Skip)> lookup)
        {
            return HydrateWith(new(), keys, lookup);
        }

        public T HydrateWith(T target, IEnumerable<string>? keys, Func<string, (string? Result, bool Skip)> lookup)
        {
            var selected = keys == null ? Hydrators : Hydrators
                .Where(h => keys.FirstOrDefault(h.GetKey()) != null)
                .ToList();

            selected.ForEach(h =>
            {
                var result = lookup(h.GetKey());

                if (!result.Skip)
                {
                    h.Hydrate(target, result.Result);
                }
            });
            return target;
        }

        public T HydrateWithout(IEnumerable<string>? keys, Func<string, (string? Result, bool Skip)> lookup)
        {
            return HydrateWithout(new(), keys, lookup);
        }

        public T HydrateWithout(T target, IEnumerable<string>? keys, Func<string, (string? Result, bool Skip)> lookup)
        {
            var selected = keys == null ? Hydrators : Hydrators
                .Where(h => keys.FirstOrDefault(h.GetKey()) == null)
                .ToList();

            selected.ForEach(h =>
            {
                var result = lookup(h.GetKey());

                if (!result.Skip)
                {
                    h.Hydrate(target, result.Result);
                }
            });
            return target;
        }

        public List<T> Hydrate<S>(IEnumerable<S> enumerable, Func<S, string, (string? Result, bool Skip)> lookup)
        {
            List<T> result = new();

            foreach (S s in enumerable)
            {
                T t = new();
                Hydrators.ForEach(h =>
                {
                    var value = lookup(s, h.GetKey());
                    if (!value.Skip)
                    {
                        h.Hydrate(t, value.Result);
                    }
                });

                result.Add(t);
            }

            return result;
        }

        public List<T> HydrateWith<S>(IEnumerable<S> enumerable, IEnumerable<string>? keys, Func<S, string, (string? Result, bool Skip)> lookup)
        {
            var selected = keys == null ? Hydrators : Hydrators
                .Where(h => keys.FirstOrDefault(h.GetKey()) == null)
                .ToList();

            List<T> result = new();

            foreach (S s in enumerable)
            {
                T t = new();
                selected.ForEach(h =>
                {
                    var value = lookup(s, h.GetKey());
                    if (!value.Skip)
                    {
                        h.Hydrate(t, value.Result);
                    }
                });

                result.Add(t);
            }

            return result;
        }

        public List<T> HydrateWithout<S>(IEnumerable<S> enumerable, IEnumerable<string>? keys, Func<S, string, (string? Result, bool Skip)> lookup)
        {
            var selected = keys == null ? Hydrators : Hydrators
                .Where(h => keys.FirstOrDefault(h.GetKey()) == null)
                .ToList();

            List<T> result = new();

            foreach (S s in enumerable)
            {
                T t = new();
                selected.ForEach(h =>
                {
                    var value = lookup(s, h.GetKey());
                    if (!value.Skip)
                    {
                        h.Hydrate(t, value.Result);
                    }
                });

                result.Add(t);
            }

            return result;
        }
        #endregion
    }
}
