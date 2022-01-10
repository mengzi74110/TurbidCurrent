namespace Common
{
    public class BundleNormal<T> : BundleBase<T> where T:UnityEngine.Object
    {
        string addressName;
        public override string AddressName => addressName;

        public BundleNormal(string path)
        {
            addressName = path.ToLower();
        }
    }
}
