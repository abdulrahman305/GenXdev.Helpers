namespace GenXdev.Misc
{
    public static class OnlyOneInstanceChecker
    {
        static Dictionary<Type, bool> instances = new Dictionary<Type, bool>();

        public static void Check(Type type)
        {
            lock (instances)
            {
                if (instances.ContainsKey(type))
                {
                    throw new InvalidOperationException("Class " + type.FullName + " instanciated more then once!");
                }

                instances[type] = true;
            }
        }
    }
}
