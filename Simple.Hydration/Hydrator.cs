using System.Reflection;

namespace Simple.Hydration
{
    public class Hydrator<T> : IHydrator<T> where T : class, new()    
    {
        private List<IHydrate> Hydrators { get; } = new();

        public Hydrator()
        {
            IncludeHydrates();
        }

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

        public T Hydrate(Func<string, string> lookup)
        {
            T result = new T();

            Hydrators.ForEach(h =>
            {
                h.Hydrate(result, lookup(h.GetKey()));
            });

            return result;
        }

        public List<T> Hydrate<S>(IEnumerable<S> enumerable, Func<S, string, string> lookup)
        {
            List<T> result = new();
            foreach(S s in enumerable)
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

    }
}
