using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Runtime.Serialization;


namespace CreateClient.DataContracts
{
    /// <summary>
    /// Represent a base data contract to interact to Intalio|Create. This is a good place to place common expected functionality
    /// </summary>
    [DataContract]
    public abstract class BaseCreateContract
    {
        /// <summary>
        /// Return the json string representation of the instance
        /// </summary>
        /// <returns>JSON String serialization</returns>
        public string ToJson()
        {
            // Create the serializer
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(this.GetType());
            
            // We need a stream to hold our data
            MemoryStream stream = new MemoryStream();

            // Serialize the current instance to our stream
            jsonSerializer.WriteObject(stream, this);

            // Read the stream and populate a string
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            string content = reader.ReadToEnd();
            reader.Close();

            // Return the string
            return content;
        }

    }
}
