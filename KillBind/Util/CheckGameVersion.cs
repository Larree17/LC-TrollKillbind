using System.Reflection;

namespace KillBind.Util
{
    public class CheckGameVersion
    {
        private const string v49Hash = "";
        private const string v47Hash = "";
        private static string currentHash = Assembly.GetEntryAssembly().ManifestModule.ModuleVersionId.ToString();

        public static bool Isv50()
        {
            if (v49Hash == currentHash || v47Hash == currentHash) { return true; }
            return false;
        }
    }
}