using System;
using System.Text;
using AD;

namespace TigerApp.Shared.Utils
{
    public interface ITigerToken
    {
        string TokenString { get; }
        void Save(string tokenString);
    }

    public class TigerToken : ITigerToken
    {
        public string TokenString { get; protected set; }
        private const string TokenFile = "tiger_token_file";
        private readonly IFileStore _fileStore;

        public TigerToken(IFileStore fileStore, ITDesAuthStore authStore)
        {
            _fileStore = fileStore;
            var tokenString = string.Empty;
            _fileStore.TryReadTextFile(TokenFile, out tokenString);
            if (tokenString == null) {
                tokenString = authStore.AuthToken;
                Save(tokenString);
            }
            TokenString = tokenString;
        }

        public void Save(string tokenString)
        {
            TokenString = tokenString;
            _fileStore.WriteFile(TokenFile, tokenString);
        }
    }
}