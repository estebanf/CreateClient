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
    /// Utility class to deserialize the owner field recieved from Create
    /// </summary>
    [DataContract]
    public class CreateOwner
    {
        [DataMember(Name = "raw")]
        public string Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}
