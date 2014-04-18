using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
namespace CreateClient.DataContracts
{
    /// <summary>
    /// Data contract to deserialize the response from the OAuth
    /// </summary>
    [DataContract]
    public class AuthResponse:BaseCreateContract
    {
        /// <summary>
        /// The access token
        /// </summary>
        [DataMember(Name="access_token")]
        public string AccessToken { get; set; }
    }
}
