namespace VCSJones.FiddlerCert
{
    public static class Extensions
    {
        public static bool MemoryCompare(this byte[] b1, byte[] b2)
        {
            if (b1 == null && b2 == null) return true; //Both null is true
            if ((b1 == null && b2 != null) || (b1 != null && b2 == null)) return false; //One null but not other is false
            if (ReferenceEquals(b1, b2)) return true; //Exact same arrays can be assumed equal
            if (b1.Length != b2.Length) return false; //Different lengths immediately means no
            if (b1.Length == 0) return true; //Both lengths are zero immediately means true
            return Interop.Msvcrt.memcmp(b1, b2, new System.UIntPtr((uint)b1.Length)) == 0; //Do comparison.
        }
    }
}
