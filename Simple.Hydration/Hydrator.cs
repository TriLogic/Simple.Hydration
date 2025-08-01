﻿using System.Reflection;

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
        public Hydrator(List<string>? IncludedKeys = null, List<string>? ExcludedKeys = null)
        {
            IncludeHydrates();

            IncludeThese(IncludedKeys);
            ExcludeThese(ExcludedKeys);
        }
        #endregion

        #region List Tailoring
        public void IncludeThese(List<string>? keys)
        {
            if (keys == null || keys.Count == 0)
                return;

            var selected = Hydrators
                .Where(h => keys.Contains(h.GetKey()) == true)
                .ToList();

            Hydrators = selected;
        }

        public void ExcludeThese(List<string>? keys)
        {
            if (keys == null || keys.Count == 0)
                return;

            var selected = Hydrators
                .Where(h => keys.Contains(h.GetKey()) == false)
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

        public T HydrateWith(List<string>? keys, Func<string, string?> lookup)
        {
            return HydrateWith(new T(), keys, lookup);
        }

        public T HydrateWith(T target, List<string>? keys, Func<string, string?> lookup)
        {
            var selected = keys == null || keys.Count == 0 ? Hydrators : Hydrators
                .Where(h => keys.Contains(h.GetKey()) == true)
                .ToList();

            selected.ForEach(h =>
            {
                h.Hydrate(target, lookup(h.GetKey()));
            });

            return target;
        }

        public T HydrateWithout(List<string>? keys, Func<string, string?> lookup)
        {
            return HydrateWithout(new T(), keys, lookup);
        }

        public T HydrateWithout(T target, List<string>? keys, Func<string, string?> lookup)
        {
            var selected = keys == null || keys.Count == 0 ? Hydrators : Hydrators
                .Where(h => keys.Contains(h.GetKey()) == false)
                .ToList();

            selected.ForEach(h =>
            {
                h.Hydrate(target, lookup(h.GetKey()));
            });
            return target;
        }

        public List<T> HydrateMany<S>(IEnumerable<S> enumerable, Func<S, string, string?> lookup)
        {
            List<T> result = new();

            foreach (S s in enumerable)
            {
                T t = new T();
                Hydrators.ForEach(h =>
                {
                    h.Hydrate(t, lookup(s, h.GetKey()));
                });
                result.Add(t);
            }

            return result;
        }

        public List<T> HydrateManyWith<S>(IEnumerable<S> enumerable, List<string> keys, Func<S, string, string?> lookup)
        {
            var selected = keys == null || keys.Count == 0 ? Hydrators : Hydrators
                .Where(h => keys.Contains(h.GetKey()) == true)
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

        public List<T> HydrateManyWithout<S>(IEnumerable<S> enumerable, List<string>? keys, Func<S, string, string?> lookup)
        {
            var selected = keys == null || keys.Count == 0 ? Hydrators : Hydrators
                .Where(h => keys.Contains(h.GetKey()) == false)
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
            return Hydrate(new T(), lookup);
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

        public T HydrateWith(List<string>? keys, Func<string, (string? Result, bool Skip)> lookup)
        {
            return HydrateWith(new T(), keys, lookup);
        }

        public T HydrateWith(T target, List<string>? keys, Func<string, (string? Result, bool Skip)> lookup)
        {
            var selected = keys == null || keys.Count == 0 ? Hydrators : Hydrators
                .Where(h => keys.Contains(h.GetKey()) == false)
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

        public T HydrateWithout(List<string>? keys, Func<string, (string? Result, bool Skip)> lookup)
        {
            return HydrateWithout(new T(), keys, lookup);
        }

        public T HydrateWithout(T target, List<string>? keys, Func<string, (string? Result, bool Skip)> lookup)
        {
            var selected = keys == null || keys.Count == 0 ? Hydrators : Hydrators
                .Where(h => keys.Contains(h.GetKey()) == false)
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

        public List<T> HydrateMany<S>(IEnumerable<S> enumerable, Func<S, string, (string? Result, bool Skip)> lookup)
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

        public List<T> HydrateManyWith<S>(IEnumerable<S> enumerable, List<string>? keys, Func<S, string, (string? Result, bool Skip)> lookup)
        {
            var selected = keys == null || keys.Count == 0 ? Hydrators : Hydrators
                .Where(h => keys.Contains(h.GetKey()) == true)
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

        public List<T> HydrateManyWithout<S>(IEnumerable<S> enumerable, List<string>? keys, Func<S, string, (string? Result, bool Skip)> lookup)
        {
            var selected = keys == null || keys.Count == 0 ? Hydrators : Hydrators
                .Where(h => keys.Contains(h.GetKey()) == false)
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

        #region Object Hydration with Skip Lookup and Reference to Target
        public T Hydrate(Func<T, string, (string? Result, bool Skip)> lookup)
        {
            var target = new T();
            return Hydrate(target, lookup);
        }

        public T Hydrate(T target, Func<T, string, (string? Result, bool Skip)> lookup)
        {
            Hydrators.ForEach(h =>
            {
                var result = lookup(target, h.GetKey());

                if (!result.Skip)
                {
                    h.Hydrate(target, result.Result);
                }
            });
            return target;
        }

        public T HydrateWith(List<string>? keys, Func<T, string, (string? Result, bool Skip)> lookup)
        {
            return HydrateWith(new T(), keys, lookup);
        }

        public T HydrateWith(T target, List<string>? keys, Func<T, string, (string? Result, bool Skip)> lookup)
        {
            var selected = keys == null || keys.Count == 0 ? Hydrators : Hydrators
                .Where(h => keys.Contains(h.GetKey()) == false)
                .ToList();

            selected.ForEach(h =>
            {
                var result = lookup(target, h.GetKey());

                if (!result.Skip)
                {
                    h.Hydrate(target, result.Result);
                }
            });
            return target;
        }

        public T HydrateWithout(List<string>? keys, Func<T, string, (string? Result, bool Skip)> lookup)
        {
            return HydrateWithout(new T(), keys, lookup);
        }

        public T HydrateWithout(T target, List<string>? keys, Func<T, string, (string? Result, bool Skip)> lookup)
        {
            var selected = keys == null || keys.Count == 0 ? Hydrators : Hydrators
                .Where(h => keys.Contains(h.GetKey()) == false)
                .ToList();

            selected.ForEach(h =>
            {
                var result = lookup(target, h.GetKey());

                if (!result.Skip)
                {
                    h.Hydrate(target, result.Result);
                }
            });
            return target;
        }

        public List<T> HydrateMany<S>(IEnumerable<S> enumerable, Func<S, T, string, (string? Result, bool Skip)> lookup)
        {
            List<T> result = new List<T>();

            foreach (S s in enumerable)
            {
                T t = new();
                Hydrators.ForEach(h =>
                {
                    var value = lookup(s, t, h.GetKey());
                    if (!value.Skip)
                    {
                        h.Hydrate(t, value.Result);
                    }
                });

                result.Add(t);
            }

            return result;
        }

        public List<T> HydrateManyWith<S>(IEnumerable<S> enumerable, List<string>? keys, Func<S, T, string, (string? Result, bool Skip)> lookup)
        {
            var selected = keys == null || keys.Count == 0 ? Hydrators : Hydrators
                .Where(h => keys.Contains(h.GetKey()) == true)
                .ToList();

            List<T> result = new();

            foreach (S s in enumerable)
            {
                T t = new T();
                selected.ForEach(h =>
                {
                    var value = lookup(s, t, h.GetKey());
                    if (!value.Skip)
                    {
                        h.Hydrate(t, value.Result);
                    }
                });

                result.Add(t);
            }

            return result;
        }

        public List<T> HydrateManyWithout<S>(IEnumerable<S> enumerable, List<string>? keys, Func<S, T, string, (string? Result, bool Skip)> lookup)
        {
            var selected = keys == null || keys.Count == 0 ? Hydrators : Hydrators
                .Where(h => keys.Contains(h.GetKey()) == false)
                .ToList();

            List<T> result = new();

            foreach (S s in enumerable)
            {
                T t = new T();
                selected.ForEach(h =>
                {
                    var value = lookup(s, t, h.GetKey());
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
