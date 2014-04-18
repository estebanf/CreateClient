using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateClient
{
    /// <summary>
    /// This client wraps the operation of the RestClient in the context of a single Create Object for easier consumption
    /// </summary>
    /// <typeparam name="T">The type of the data contract to be used when deserializing the JSON response</typeparam>
    /// <typeparam name="K">The type of the data contract to be used when serializing the request operation body</typeparam>
    public class ApiClient<T,K>
        where T: class
        where K: class
    {
        #region private fields
        /// <summary>
        /// Holds the Create object full identifier
        /// </summary>

        private string identifier;
        /// <summary>
        /// List of fields to be used to specified what fields to retrieve
        /// </summary>
        
        private List<String> fields;
        /// <summary>
        /// RestClient  to use
        /// </summary>
        private RestClient client;
        #endregion

        #region public properties
        /// <summary>
        /// Create object full identifier. Read only
        /// </summary>
        public string Identifier
        {
            get { return identifier; }
        }

        /// <summary>
        /// List of fields to retrieve
        /// </summary>
        public List<String> Fields
        {
            get { return fields; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create an ApiClient instance
        /// </summary>
        /// <param name="object_identifier">Create object full identifier</param>
        /// <param name="restClient">Configured RestClient</param>
        public ApiClient(string object_identifier, RestClient restClient)
        {
            fields = new List<String>();
            identifier = object_identifier;
            client = restClient;
        }

        /// <summary>
        /// Create an ApiClient instance
        /// </summary>
        /// <param name="object_identifier">Create object full identifier</param>
        /// <param name="object_fields">List of fields to retrieve</param>
        /// <param name="restClient">Configured RestClient</param>
        public ApiClient(string object_identifier, List<String> object_fields, RestClient restClient)
        {
            fields = object_fields;
            identifier = object_identifier;
            client = restClient;
        }

        /// <summary>
        /// Create an ApiClient instance
        /// </summary>
        /// <param name="object_identifier">Create object full identifier</param>
        /// <param name="object_fields">List of fields to retrieve</param>
        /// <param name="host">Hostname of Intalio|Create instance</param>
        /// <param name="login">User login to authenticate</param>
        /// <param name="password">User password to authenticate</param>
        public ApiClient(string object_identifier, List<String> object_fields, String host, String login, String password)
        {
            fields = object_fields;
            identifier = object_identifier;
            client = new RestClient(host, login, password);
        }
        #endregion
        
        #region Methods
        /// <summary>
        /// Read all the records of the configured Create object
        /// </summary>
        /// <returns>A list of the deserialized create records</returns>
        public List<T> readAll()
        {
            return client.readAll<T>(identifier,fields);
        }

        /// <summary>
        /// Read on record of the configured Create object
        /// </summary>
        /// <param name="value">UUID of the record to read</param>
        /// <returns>A instance of the deserialized record</returns>
        public T readOne(string value)
        {
            return client.readOne<T>(identifier, fields, value);
        }

        /// <summary>
        /// Create a record of the configured Create object
        /// </summary>
        /// <param name="dataObject">An instance of the arguments data contract</param>
        /// <returns>A instance of the deserialized record</returns>
        public T create(K dataObject)
        {
            return client.create<T, K>(identifier, dataObject);
        }

        /// <summary>
        /// Update a record of the configured Create object
        /// </summary>
        /// <param name="dataObject">An instance of the arguments data contract</param>
        /// <param name="value">UUID of the record to update</param>
        /// <returns>A instance of the deserialized record</returns>
        public T update(K dataObject, string value)
        {
            return client.update<T, K>(identifier, dataObject, value);
        }

        /// <summary>
        /// Delete a record of the configured Create object
        /// </summary>
        /// <param name="value">UUID of the record to update</param>
        public void delete(string value)
        {
            client.delete(identifier, value);
        }
        #endregion
    }
}
