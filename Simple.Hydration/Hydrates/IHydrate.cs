namespace Simple.Hydration
{
    public interface IHydrate
    {
        public void Hydrate(object target, string? value);

        public string GetKey();
    }
}
