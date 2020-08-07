namespace FastDFSCore.Sample
{
    public static class SizeExtensions
    {
        public static double AsMB(this long size) => size * 1.0 / 1024 / 1024;
    }
}
