using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using CreateClient.DataContracts;

namespace CreateClientTests
{
    [DataContract]
    public class LeadData:BaseCreateContract
    {
        [DataMember(Name = "io_uuid", EmitDefaultValue = false)]
        public String Id { get; set; }
        [DataMember(Name = "io_first_name", EmitDefaultValue = false)]
        public String FirstName { get; set; }
        [DataMember(Name = "io_last_name", EmitDefaultValue = false)]
        public String LastName { get; set; }
        [DataMember(Name = "io_email", EmitDefaultValue = false)]
        public String Email { get; set; }
        [DataMember(Name = "io_owner", EmitDefaultValue = false)]
        public String Owner { get; set; }

        public override string ToString()
        {
            return String.Join("-", new string[5] { Id, FirstName, LastName, Email, Owner});
        }

    }
}
