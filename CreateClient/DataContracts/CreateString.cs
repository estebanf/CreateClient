using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace CreateClient.DataContracts
{
    /// <summary>
    /// Utility class to manage the deserialization of Create string values
    /// </summary>
    [DataContract]
    public class CreateString
    {
        /// <summary>
        /// The raw value of the string
        /// </summary>
        [DataMember(Name = "raw")]
        public string Value { get; set; }

        /// <summary>
        /// An overrided version of ToString to return our value
        /// </summary>
        /// <returns>The field value</returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Operator to be able to assign an string value to instance of CreateString
        /// </summary>
        /// <param name="s">Source string</param>
        /// <returns>An instance of CreateString with the value set to the recieved string</returns>
        public static implicit operator CreateString(string s)
        {
            CreateString obj = new CreateString();
            obj.Value = s;
            return obj;
        }
    }
}
