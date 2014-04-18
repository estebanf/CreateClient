using System;
using System.Net;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using CreateClient.DataContracts;

namespace CreateClient
{
    /// <summary>
    /// Class to encapsulate all interactions with Intalio|create Rest API
    /// </summary>
    public class RestClient
    {
        #region private variables
        /// <summary>
        /// Holds the value of the OAuth Acess Token
        /// </summary>
        private string accessToken;

        /// <summary>
        /// Holds the user login to be used when consuming the API
        /// </summary>
        private string userLogin;

        /// <summary>
        /// Holds the hostname of the Intalio|Create instance to target
        /// </summary>
        private string createHost;

        /// <summary>
        /// Holds the user password to authenticate against the Intalio|create instance
        /// </summary>
        private string userPassword;

        /// <summary>
        /// Holds the value of the assigned Client-Id
        /// </summary>
        private string clientId;

        /// <summary>
        /// Holds the value of the authenticated host
        /// </summary>
        private string authHost;

        /// <summary>
        /// Holds a flag indicating if the OAuth had been done
        /// </summary>
        private bool ready;
        #endregion
        
        #region public properties
        /// <summary>
        /// The provided access token. Read only
        /// </summary>
        public string AccessToken
        {
            get { return accessToken; }
        }
        
        /// <summary>
        /// The target Intalio|Create instance. Read only
        /// </summary>
        public string CreateHost
        {
            get { return createHost; }
        }
        
        /// <summary>
        /// The used user login. Read only
        /// </summary>
        public string UserLogin
        {
            get { return userLogin; }
        }
        
        /// <summary>
        /// The provided ClientId. Read only
        /// </summary>
        public string ClientId
        {
            get { return clientId; }
        }
        #endregion
        
        #region Constructor
        /// <summary>
        /// Instantiate the Rest Client. The instance will not be authenticated, neither ready to use.
        /// </summary>
        /// <param name="host">Intalio|create instance to target</param>
        /// <param name="login">User login to use</param>
        /// <param name="password">User password to use</param>
        public RestClient(String host, String login, String password)
        {
            createHost = host;
            userLogin = login;
            userPassword = password;
            ready = false;
        }
        #endregion
        
        #region Methods
        /// <summary>
        /// Execute the OAuth with Intalio|Create. Needed before executing any operation
        /// </summary>
        public void prepare()
        {
            // Calls the method to get the Client-ID value
            retrieveClientId();

            // Calls the method to get the Access token
            retrieveAccessToken();
            
            // If everything went fine, then we mark the instance ready to work
            ready = true;
        }
        
        /// <summary>
        /// Perform the read all records operation of a Create Object.
        /// </summary>
        /// <typeparam name="T">The type of the data contract to be used when deserializing the JSON response</typeparam>
        /// <param name="identifier">The create object full identifier</param>
        /// <param name="fields">List of fields to retrieve</param>
        /// <returns>A list the deserialized records</returns>
        public List<T> readAll<T>(string identifier, List<String> fields)
        {
            // If the instance has not been prepared, let's do it
            if (!ready) { prepare(); }

            // Create the request targeting the read all operation endpoint and specifying the fields to retrieve
            string queryString = "";
            if (fields!= null && fields.Count > 0)
            {
                queryString = "?fields=" + String.Join(",", fields);
            }
            HttpWebRequest request = createAuthRequest("http://" + createHost + "/api/rest/v1/" + identifier + queryString,"GET");

            // return the deserialized list.
            return readResponse<List<T>>(request);
        }

        /// <summary>
        /// Perform the read one operation of a Create record
        /// </summary>
        /// <typeparam name="T">The type of the data contract to be used when deserializing the JSON response</typeparam>
        /// <param name="identifier">The create object full identifier</param>
        /// <param name="fields">List of fields to retrieve</param>
        /// <param name="value">UUID value of the target record</param>
        /// <returns>A deserialized representation of the record</returns>
        public T readOne<T>(string identifier, List<String> fields, string value)
            where T : class
        {
            // If the instance has not been prepared, let's do it
            if (!ready) { prepare(); }

            // Create the request targeting the read one operation endpoint and specifying the fields to retrieve
            string queryString = "";
            if (fields !=null && fields.Count > 0)
            {
                queryString = "?fields=" + String.Join(",", fields);
            }
            HttpWebRequest request = createAuthRequest("http://" + createHost + "/api/rest/v1/" + identifier + "/" + value + queryString, "GET");
        
            // Return the deserialized record
            return readResponse<T>(request);
        }

        /// <summary>
        /// Perform the create operation of a Create record
        /// </summary>
        /// <typeparam name="T">The type of the data contract to be used when deserializing the JSON response</typeparam>
        /// <typeparam name="K">The type of the data contract to be used when serializing the request operation body</typeparam>
        /// <param name="identifier">The create object full identifier</param>
        /// <param name="dataObject">An instance of the data contract to be used as the request operation body</param>
        /// <returns>A deserialized representation of the created record</returns>
        public T create<T, K>(string identifier, K dataObject)
            where T : class
        {
            // If the instance has not been prepared, let's do it
            if (!ready) { prepare(); }

            // Create the request targeting the create operation endpoint
            HttpWebRequest request = createAuthRequest("http://" + createHost + "/api/rest/v1/" + identifier + "/new","POST");
            
            // Serialize the body of the request
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(K));
            jsonSerializer.WriteObject(request.GetRequestStream(), dataObject);

            // Return the deserialized response record
            return readResponse<T>(request);
        }
        
        /// <summary>
        /// Perform the update operation of a Create record
        /// </summary>
        /// <typeparam name="T">The type of the data contract to be used when deserializing the JSON response</typeparam>
        /// <typeparam name="K">The type of the data contract to be used when serializing the request operation body</typeparam>
        /// <param name="identifier">The create object full identifier</param>
        /// <param name="dataObject">An instance of the data contract to be used as the request operation body</param>
        /// <param name="value">UUID value of the target record</param>
        /// <returns>A deserialized representation of the updated record</returns>
        public T update<T, K>(string identifier, K dataObject, string value)
            where T : class
        {
            // If the instance has not been prepared, let's do it
            if (!ready) { prepare(); }

            // Create the request targeting the create operation endpoint
            HttpWebRequest request = createAuthRequest("http://" + createHost + "/api/rest/v1/" + identifier + "/" + value, "PUT");
            
            // Serialize the body of the request
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(K));
            jsonSerializer.WriteObject(request.GetRequestStream(), dataObject);

            // Return the deserialized response record
            return readResponse<T>(request);
        }
        
        /// <summary>
        /// Perform the delete operation of a Create record
        /// </summary>
        /// <param name="identifier">The create object full identifier</param>
        /// <param name="value">UUID value of the target record</param>
        public void delete(string identifier, string value)
        {
            // Create the request targeting the delete operation endpoint
            HttpWebRequest request = createAuthRequest("http://" + createHost + "/api/rest/v1/" + identifier + "/" + value, "DELETE");
            
            // Execute the request
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse) { }
        }
        #endregion
        
        #region private methods
        /// <summary>
        /// Retrieves the Client-ID value needed to complete the OAuth
        /// </summary>
        private void retrieveClientId()
        {
            // Create an http request targeting the /oauth/client endpoint in the Intalio|create instance
            HttpWebRequest request = WebRequest.Create("http://" + createHost + "/oauth/client") as HttpWebRequest;
        
            //Read the response
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the value of the Client-Id header
                clientId = response.Headers.Get("Client-id");
            
                // Get the value of the Auth-Hose header
                authHost = "http:" + response.Headers.Get("Auth-Host");
            }
        }

        /// <summary>
        /// Execute the OAuth against the authentication host
        /// </summary>
        private void retrieveAccessToken()
        {
            // First we create an string array with the values that we need to post to the authentication host. Values need to be url encoded
            string[] data = new string[4];
            data[0] = "client_id=" + HttpUtility.UrlEncode(clientId);
            data[1] = "username=" + HttpUtility.UrlEncode(userLogin);
            data[2] = "password=" + HttpUtility.UrlEncode(userPassword);
            data[3] = "grant_type=password";
            
            // We convert the array to a single string in the correct format.
            String postData = String.Join("&", data);

            //Create a HTTP request to Post the data
            HttpWebRequest request = WebRequest.Create(authHost + "/auth/token") as HttpWebRequest;
            request.Method = "POST";
            request.ContentLength = postData.Length;
            request.ContentType = "application/x-www-form-urlencoded";

            // Write the value of our postDatastring to the Request Stream
            StreamWriter myWriter = new StreamWriter(request.GetRequestStream());
            myWriter.Write(postData);
            myWriter.Close();

            // Make the request and read the response
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Deserialize the response intho a AuthResponse instance
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(AuthResponse));
                AuthResponse auth = jsonSerializer.ReadObject(response.GetResponseStream()) as AuthResponse;
                
                // Store the value of the access token
                accessToken = auth.AccessToken;
            }
        }
        
        /// <summary>
        /// Given a HTTP Request, it will execute the request, read the response and return the deserialized body.
        /// </summary>
        /// <typeparam name="T">The type of the data contract to use for deserializing the response</typeparam>
        /// <param name="request">The configured HttpWebRequest</param>
        /// <returns>The deserialized body</returns>
        private T readResponse<T>(HttpWebRequest request)
            where T : class
        {
            // Declare the variable to hold the deserialized response
            T data;

            // Execute the request and read the response
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Deserialize the response using the specified type as Data contract
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
                data = jsonSerializer.ReadObject(response.GetResponseStream()) as T;
            }

            // Return the data
            return data;
        }

        /// <summary>
        /// Create a HttpWebRequest with the required headers to perform an authenticated operation against the Intalio|Create instance
        /// </summary>
        /// <param name="url">Url to target</param>
        /// <param name="method">HTTP Verb to use</param>
        /// <returns>The configured HttpWebRequest</returns>
        private HttpWebRequest createAuthRequest(string url, string method){
            // Instantiate the HttpWebRequest
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            
            // Add the needed headers
            request.Headers.Add("Authorization", "OAuth");
            request.Headers.Add("user", userLogin);
            request.Headers.Add("access-token", accessToken);
            request.ContentType = "application/json";

            // Set the specified VERB
            request.Method = method;

            // Return the request
            return request;
        }
        #endregion
    }
}


