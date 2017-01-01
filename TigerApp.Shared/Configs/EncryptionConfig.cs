using System;
using AD;

namespace TigerApp.Shared.Config
{
    public class EncryptionConfig : IEncryptionConfig
    {
        public byte[] InitVector => new byte[] { 65, 110, 68, 26, 69, 178, 200, 219 };
        public byte[] Key => new byte[] { 12, 23, 34, 45, 56, 67, 78, 89, 90, 101, 112, 123, 134, 145, 156, 167, 178, 189, 190, 201, 212, 223, 234, 245 };
    }
}