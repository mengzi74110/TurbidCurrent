namespace TurbidCurrent
{
    public class Single<T> where T : class, new()
    {
        protected static T instance;
        public static T Instacne
        {
            get
            {
                if (instance == null)
                    instance = new T();
                return instance;
            }
        }

        protected Single() { }
    }
}
