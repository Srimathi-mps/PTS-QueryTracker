using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;

namespace QueryTracker.Api.Services
{
    /// <summary>
    /// Validates users using the same tamper-proof token scheme as the original ASPX app.
    ///
    /// The Username query param (e.g. "TUFDMDE0Mzkz-aFlitOWI7wY%3d") is a tamper-proof
    /// encoded token produced by Global.TamperProofStringDecode in the old PTSQuery app:
    ///
    ///   token  = Base64(userCode) + "-" + Base64(MACTripleDES_hash)
    ///   key    = "0920"  (hardcoded in the original Global.asax.cs)
    ///
    /// MACTripleDES (removed in .NET 5+) is reimplemented here:
    ///   - Derive a 16-byte key: MD5(UTF8("0920"))
    ///   - Encrypt the data bytes with 3DES-CBC, zero IV, PKCS7 padding
    ///   - The MAC is the last 8 bytes of the ciphertext
    ///
    /// When the caller passes a plain username (not a token), decoding fails gracefully
    /// and the plain value is used directly — supports the manual login form.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly string _connStr;
        private const string TamperProofKey = "0920";

        public AuthService(IConfiguration config)
        {
            _connStr = config.GetConnectionString("jetsConstrNew")!;
        }

        // ── Public entry point ────────────────────────────────────────────────

        public string? ValidateUser(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            // Try to decode as a tamper-proof token first.
            // If decoding succeeds, use the decoded userCode.
            // If it fails (plain username from the login form), use as-is.
            string userCode = TryDecodeTamperProofToken(userName.Trim())
                           ?? userName.Trim();

            return UserExistsInDatabase(userCode)
                ? userCode.ToUpperInvariant()
                : null;
        }

        // ── Token decoder (mirrors Global.TamperProofStringDecode) ────────────

        private static string? TryDecodeTamperProofToken(string token)
        {
            try
            {
                // Step 1 & 2: URL-decode then fix space→+ artefact
                string decoded = Uri.UnescapeDataString(token).Replace(' ', '+');

                // Step 3: split on '-'
                string[] parts = decoded.Split('-');
                if (parts.Length < 2) return null;

                // Step 4: Base64-decode the first part → plain userCode
                string dataValue  = Encoding.UTF8.GetString(Convert.FromBase64String(parts[0]));
                string storedHash = Encoding.UTF8.GetString(Convert.FromBase64String(parts[1]));

                // Step 5: compute MACTripleDES hash and compare
                // MACTripleDES was removed in .NET 5+; reimplemented below.
                string calcHash = ComputeMacTripleDes(dataValue, TamperProofKey);

                // If hashes match, token is valid — return the decoded userCode
                return storedHash == calcHash ? dataValue : null;
            }
            catch
            {
                // Not a valid token — caller will treat input as a plain username
                return null;
            }
        }

        /// <summary>
        /// Reimplements System.Security.Cryptography.MACTripleDES (removed in .NET 5+).
        ///
        /// Algorithm (matches the original .NET Framework behaviour):
        ///   1. key16 = MD5( UTF8(key) )                  — 16-byte key
        ///   2. Encrypt data with TripleDES-CBC, IV=0x00*8, PKCS7 padding
        ///   3. MAC = last 8 bytes of ciphertext
        ///   4. Return UTF8 string of those 8 bytes
        /// </summary>
        private static string ComputeMacTripleDes(string data, string key)
        {
            // Derive 16-byte 3DES key from the passphrase using MD5
            byte[] keyBytes;
            using (var md5 = MD5.Create())
                keyBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(key));

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] iv        = new byte[8]; // all zeros

            using var des = TripleDES.Create();
            des.Key     = keyBytes;
            des.IV      = iv;
            des.Mode    = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;

            using var encryptor  = des.CreateEncryptor();
            using var ms         = new System.IO.MemoryStream();
            using var cs         = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            cs.Write(dataBytes, 0, dataBytes.Length);
            cs.FlushFinalBlock();

            byte[] ciphertext = ms.ToArray();

            // MAC = last 8 bytes of ciphertext
            byte[] mac = ciphertext.Skip(ciphertext.Length - 8).Take(8).ToArray();
            return Encoding.UTF8.GetString(mac);
        }

        // ── Database check via existing SP ────────────────────────────────────

        private bool UserExistsInDatabase(string userCode)
        {
            try
            {
                using var conn = new SqlConnection(_connStr);
                conn.Open();

                // Reuse the existing SP — no new SP required.
                // Returns a non-null, non-zero RoleID if the user exists.
                using var cmd = new SqlCommand("USP_PTS_GETUSERROLEBYUSERNAME", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@USERNAME", userCode);

                var result = cmd.ExecuteScalar();
                return result != null
                    && result != DBNull.Value
                    && Convert.ToInt32(result) != 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
