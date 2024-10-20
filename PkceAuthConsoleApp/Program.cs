using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace
 GoogleAuthConsoleApp
{
    class Program
    {
        private const string ClientId = "765584575246-s6pbqfqgq8mto0ishpov21jr1q6s3cll.apps.googleusercontent.com";
        private const string ClientSecret = "";
        private const
 string RedirectUri = "http://localhost:5000"; // Replace with your desired redirect URI

        static async Task Main()
        {
            try
            {
                // Generate a random code verifier and encoder
                var codeVerifier = GenerateCodeVerifier();
                var codeChallenge = GenerateCodeChallenge(codeVerifier);

                // Construct the authorization URL
                var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={ClientId}&redirect_uri={RedirectUri}&response_type=code&scope=https://www.googleapis.com/auth/userinfo.email&code_challenge={codeChallenge}&code_challenge_method=S256";


                // Open the authorization URL in a web browser
                System.Diagnostics.Process.Start(authUrl);

                // Wait for user input to continue
                Console.WriteLine("Enter the authorization code from the browser:");
                var authorizationCode = Console.ReadLine();

                // Exchange the authorization code for an access token
                var accessToken = await ExchangeCodeForAccessToken(authorizationCode, codeVerifier);

                // Use the access token to make API calls
                var userInfo = await GetUserInfo(accessToken);
                Console.WriteLine($"User information: {JsonConvert.SerializeObject(userInfo)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private static string GenerateCodeVerifier()
        {
            // Generate a random string of 128 characters
            //var randomBytes = new byte[128];
            //using (var rng = new System.Security.Cryptography.RandomNumberGenerator())
            //{
            //    rng.GetBytes(randomBytes);
            //}
            //return Base64UrlEncode(randomBytes);
            return "DUMMY";
        }

        private static string GenerateCodeChallenge(string codeVerifier)

        {
            // Hash the code verifier using SHA-256 and encode it
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                return Base64UrlEncode(hashedBytes);
            }
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        private static async Task<string> ExchangeCodeForAccessToken(string authorizationCode, string codeVerifier)
        {
            using (var httpClient = new HttpClient())
            {
                var requestBody = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", ClientId),
                    new KeyValuePair<string,
 string>("client_secret", ClientSecret),
                    new KeyValuePair<string, string>("code", authorizationCode),
                    new KeyValuePair<string, string>("redirect_uri",
 RedirectUri),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code_verifier",
 codeVerifier)
                });

                var response
 = await httpClient.PostAsync("https://oauth2.googleapis.com/token", requestBody);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content);

                return tokenResponse.AccessToken;
            }
        }

        private static async Task<UserInfo> GetUserInfo(string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
 accessToken);
                var response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserInfo>(content);

            }
        }

        private class TokenResponse
        {
            public string AccessToken { get; set; }
        }

        private class UserInfo
        {
            public string Email { get; set; }
        }
    }
}