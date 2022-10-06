namespace Simple.Hydration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class HydrateWithAttribute : Attribute
    {
        public string Key { get; set; }

        public HydrateWithAttribute(string key)
        {
            Key = key;
        }
    }
}