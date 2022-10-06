using System.Reflection;

namespace Simple.Hydration
{
    public class FieldHydrate : AbstractHydrate
    {
        public FieldInfo Info { get; set; }

        private string Key { get; set; }

        public FieldHydrate(FieldInfo info)
            : base(info.FieldType)
        {
            Info = info;

            var attr = info.GetCustomAttribute<HydrateWithAttribute>();
            Key = attr == null ? info.Name : attr.Key;
        }

        public override string GetKey() => Key;

        public override void Hydrate(object target, string value)
        {
            Info.SetValue(target, Converter.ConvertFromString(value));
        }
    }
}
