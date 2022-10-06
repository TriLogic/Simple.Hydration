using System.ComponentModel;

namespace Simple.Hydration
{
    public abstract class AbstractHydrate : IHydrate
    {
        public TypeConverter Converter { get; set; }

        public AbstractHydrate(Type type)
        {
            Converter = TypeDescriptor.GetConverter(type);
        }

        public abstract void Hydrate(object target, string value);

        public abstract string GetKey();
    }
}
