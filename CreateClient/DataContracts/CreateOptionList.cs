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

        public string Id {
            get
            {
                if (Raw != null)
                {
                    return Raw[0].Split(new char[1] { '=' }, 3)[0];
                }
                else
                {
                    return "";
                }
            }
        }
        public string Identifier { 
            get
            {
                if (Raw != null)
                {
                    return Raw[0].Split(new char[1] { '=' }, 3)[1];
                }
                else
                {
                    return "";
                }
            }
        }
        public string Name { 
            get
            {
                if (Raw != null)
                {
                    return Raw[0].Split(new char[1] { '=' }, 3)[2];
                }
                else
                {
                    return "";
                }
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}