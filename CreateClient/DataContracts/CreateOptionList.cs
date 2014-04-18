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
    /// Utility class to deserialize the optionList fields recieved from Create
    /// </summary>
    [DataContract]
    public class CreateOptionList
    {
        [DataMember(Name="raw")]
        public string[] Raw { get; set; }

        public string Id { get { return Raw[0].Split(new char[1] { '=' }, 3)[0]; } }
        public string Identifier { get { return Raw[0].Split(new char[1] { '=' }, 3)[1]; } }
        public string Name { get { return Raw[0].Split(new char[1] { '=' }, 3)[2]; } }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
