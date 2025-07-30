using System.Reflection;

namespace Simple.Hydration
{
    public class PropertyHydrate : AbstractHydrate
    {
        public PropertyInfo Info { get; set; }

        private string Key { get; set; }

        public PropertyHydrate(PropertyInfo info)
            : base(info.PropertyType)
        {
            Info = info;
            var attr = info.GetCustomAttribute<HydrateWithAttribute>();
            Key = attr == null ? Info.Name : attr.Key;
        }

        public override string GetKey() => Key;

        public override void Hydrate(object target, string? value)
        {
            if (value == null)
            {
                Info.SetValue(target, null);
            }
            else
            {
                Info.SetValue(target, Converter.ConvertFromString(value));
            }
        }
    }
}
