using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Ionic.Zlib;

namespace SafeInCloudImp
{
    internal class Decryptor
    {
        private const int Magic = 1285;
        private static byte[] ReadFully(Stream stream)
        {
            var buffer = new byte[4096];
            using (var memoryStream = new MemoryStream())
            {
                while (true)
                {
                    var count = stream.Read(buffer, 0, buffer.Length);
                    if (count > 0)
                        memoryStream.Write(buffer, 0, count);
                    else
                        break;
                }
                return memoryStream.ToArray();
            }
        }

        private static short ReadShort(Stream stream)
        {
            return (short)(stream.ReadByte() | (stream.ReadByte() << 8));
        }

        private static byte[] ReadByteArray(Stream stream)
        {
            var buffer = new byte[stream.ReadByte()];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        private static byte[] GetSecretKey(byte[] password, byte[] salt, int rounds)
        {
            return new Rfc2898DeriveBytes(password, salt, rounds).GetBytes(0x20);
        }

        private static byte[] GetPasswordBytes(string password, int passwordEncodeType)
        {
            if (passwordEncodeType != 1)
                return Encoding.UTF8.GetBytes(password);

            var buffer = new byte[password.Length];
            for (var i = 0; i < password.Length; i++)
            {
                buffer[i] = (byte)password[i];
            }
            return buffer;
        }

        private static ICryptoTransform GetDecryptor(byte[] iv, byte[] secretKey)
        {
            return new AesCryptoServiceProvider
            {
                BlockSize = 128,
                KeySize = 256,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                IV = iv,
                Key = secretKey
            }.CreateDecryptor();
        }

        private static CryptoStream DecryptStream(Stream inputStream, string password)
        {
            var magic = ReadShort(inputStream);
            if (magic != Magic)
                throw new IOException(string.Format("Wrong magic: {0}", magic));
            var passwordEncodeType = inputStream.ReadByte();
            var salt1 = ReadByteArray(inputStream);
            var secretKey1 = GetSecretKey(GetPasswordBytes(password, passwordEncodeType), salt1, 10000);
            var iv1 = ReadByteArray(inputStream);

            var salt2 = ReadByteArray(inputStream);
            byte[] buffer;
            using (var ms = new MemoryStream(ReadByteArray(inputStream)))
            {
                using (var stream = new CryptoStream(ms, GetDecryptor(iv1, secretKey1),
                    CryptoStreamMode.Read))
                    buffer = ReadFully(stream);
            }

            using (var ms = new MemoryStream(buffer))
            {
                var iv2 = ReadByteArray(ms);
                var key2 = ReadByteArray(ms);
                var secretKey3 = ReadByteArray(ms);
                var secretKey2 = GetSecretKey(key2, salt2, 1000);
                if (!secretKey3.SequenceEqual(secretKey2))
                    throw new IOException("Wrong password");
                return new CryptoStream(inputStream, GetDecryptor(iv2, key2), CryptoStreamMode.Read);
            }
        }

        public static Stream LoadDatabase(Stream inputStream, string password)
        {
            return new ZlibStream(DecryptStream(inputStream, password), CompressionMode.Decompress);
        }

        public static Stream DecodeStream(Stream inputStream)
        {
            var ms = new MemoryStream(ReadFully(inputStream));
            var crypted = ms.Length > 0 && ReadShort(ms) == Magic;
            ms.Position = 0;
            if (!crypted)
                return ms;

            var password = "";
            while (true)
            {
                try
                {
                    if (InputBox.Query("Enter SafeInCloud password", "Password", ref password))
                        return LoadDatabase(ms, password);
                    return null;
                }
                catch
                {
                    ms.Position = 0;
                    MessageBox.Show("Problem while importing encrypted file.\nMaybe wrong password?\nTry again!",
                        "Import error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}