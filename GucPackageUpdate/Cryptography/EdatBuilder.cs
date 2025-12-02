using GucPackageUpdate.Helpers;
using libps3;
using System.IO;

namespace GucPackageUpdate.Cryptography
{
    internal static class EdatBuilder
    {
        public static byte[] Decrypt(byte[] data, byte[] klicensee, byte[] rap, string filename)
        {
            using var edata = EDATA.Read(data);
            using var output = new MemoryStream();
            edata.Decrypt(filename, klicensee, rap, output);
            return output.ToArray();
        }

        public static byte[] Encrypt(byte[] data, byte[] klicensee, byte[] rap, string filename, string contentId)
        {
            using var edata = new EDATA();
            var npd = edata.NPD;
            npd.Version = 3;
            npd.License = NPD.DrmType.Local;
            npd.App = NPD.AppType.Module;
            npd.ContentId = contentId;
            StaticRandom.Random.NextBytes(npd.Digest);
            npd.DisableExpiration();
            npd.Update(filename, klicensee);

            edata.Flags = EDATA.EdataFlags.UNK_2 | EDATA.EdataFlags.EncryptedKey | EDATA.EdataFlags.UNK_4 | EDATA.EdataFlags.UNK_5;
            edata.BlockSize = 32768;
            edata.Footer = "EDATA 3.3.0.W";

            using var input = new MemoryStream(data);
            edata.Encrypt(filename, klicensee, rap, input);
            return edata.Write();
        }
    }
}
