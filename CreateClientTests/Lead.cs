using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using CreateClient.DataContracts;

namespace CreateClientTests
{
    [DataContract]
    public class Lead:BaseCreateContract
    {
        [DataMember(Name = "io_uuid", EmitDefaultValue = false)]
        public CreateString Id { get; set; }
        [DataMember(Name = "io_first_name", EmitDefaultValue = false)]
        public CreateString FirstName { get; set; }
        [DataMember(Name = "io_last_name", EmitDefaultValue = false)]
        public CreateString LastName { get; set; }
        [DataMember(Name = "io_email", EmitDefaultValue = false)]
        public CreateString Email { get; set; }
        [DataMember(Name = "io_lead_number", EmitDefaultValue = false)]
        public CreateString LeadNumber { get; set; }
        [DataMember(Name="io_owner", EmitDefaultValue = false)]
        public CreateOwner Owner { get; set; }

        public override string ToString()
        {
            return String.Join("-", new string[6] { Id.ToString(), FirstName.ToString(), LastName.ToString(), Email.ToString(), LeadNumber.ToString(), Owner.ToString() });
        }
    }

}
