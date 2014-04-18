C# client for Intalio|Create RESTFul API
============

Introduction
------------

This project objective is to provide a sample about how to use .NET technologies to interact with Intalio|Create RESTFul API. I've added a reusable library an unit tests to help you understand the tasks that you may need to do in your own project.

References
----------
  - You should know about [Intalio Create]
  - If you don't have access to a [Intalio Create] instance you can [request one]
  - You will need to understand how to use [HttpWebRequest] to perform HTTP requests with C#
  - You will need to understand how [JSON Serialization works in .NET]. Make sure you understand how [DataContracts] and checkout this [complete example]
  - The library use some features of the Reflection API. If you are aiming to improve the initial code, you may want to review [System.Activator], [System.Reflection.CustomAttributeData] and [System.Reflection.PropertyInfo]
  - To support operations with datasets this library implements a custom [IDataAdapter]. 
  - Intalio|Create works with [OAuth authentication]. 
  - You should review the Intalio|Create [Rest API documentation]  
  - In case of doubts use the [Developer community site] or [reach out directly]

Data contracts
--------------

For each Intalio|Create object that you want to interact with, you will need to create 2 different data contracts. The first one will be used to deserialize the json data sent by Intalio|Create. For example, this is how Intalio|Create replies to a read one operation of the object _io_lead_ (/api/rest/v1/io_lead/491f1493-0160-4ad8-a083-6b3f8bb24a66?fields=io_uuid,io_first_name,io_last_name,io_email,io_lead_number,io_owner)

```javascript
{"__object":"io_lead","io_email":{"raw":"unit@test.com"},"io_lead_number":{"raw":"LD-000079"},"io_first_name":{"raw":"Unit2Updated"},"io_owner":{"raw":"764f0869-8b2a-4e43-8f23-88f593863eff","name":"System Administrator","deleted":false,"__object":"io_user"},"io_uuid":{"raw":"491f1493-0160-4ad8-a083-6b3f8bb24a66"},"io_last_name":{"raw":"Test2"}}
```
As you can see there's a json object within each field. A Data Contract to deal with this object would look like:

```csharp
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

    }

}

```
This data contract properties are using the the utility classes available in __CreateClient.DataContracs__ to simplify dealing with the _"raw"_ json

Operations in Intalio|Create that expect a json body don't use the _"raw"_ representation, but rather the values directly. So for those operations when we are sending the _Lead_ data to Intalio|Create, we will use a data contract like this:

```csharp
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

}

```

Using the ApiClient
-------------------
Asuming we would be using the referenced data contracts, this is how you would perform any of the operations
  - Initialize the ApiClient

```csharp
List<String> fields = new List<string>(new string[6] {"io_uuid","io_first_name","io_last_name","io_email","io_lead_number","io_owner"});
ApiClient api<Lead,LeadData> = new ApiClient("io_lead",fields,"your_create_instance_host_name","your_username","your_password");

```
  
  - Read all

```csharp
List<Lead> leads = api.readAll();
```
  - Read One

```csharp
Lead lead = api.readOne("uuid of the record to read");

```
  - Create

```csharp
LeadData leadData = new LeadData();
leadData.FirstName = "Unit";
leadData.LastName = "Test";
leadData.Email = "unit@test.com";
leadData.Owner = "764f0869-8b2a-4e43-8f23-88f593863eff";
Lead createdLead = api.create(leadData);
```
  - Update

```csharp
LeadData leadData = new LeadData();
leadData.FirstName = "Unit2";
leadData.LastName = "Test2";
leadData.Email = "unit@test.com";
leadData.Owner = "764f0869-8b2a-4e43-8f23-88f593863eff";

Lead updatedLead = api.update(leadData, "uuid of the record to update");

```
  - Delete

```csharp
api.delete("uuid of the record to delete");
```

Using the CreateDataAdapter
---------------------------
The library includes a partial implementation of an IDataAdapter. Currently only the Fill and Update methods are implemented, but that should be enough to provide more than basic support to projects that prefer to work with datasets. The following code shows how to consume the data adapter.

```csharp
//Get the adapter instance setup
List<String> fields = new List<string>(new string[6] {"io_uuid","io_first_name","io_last_name","io_email","io_lead_number","io_owner"});
ApiClient api<Lead,LeadData> = new ApiClient("io_lead",fields,"your_create_instance_host_name","your_username","your_password");
CreateDataAdapter<Lead, LeadData> adapter = new CreateDataAdapter<Lead, LeadData>(api);

// Declare the dataset
DataSet ds = new DataSet();

//Fill the dataset with the data from the server. A DataTable named "Lead" will be created
adapter.Fill(ds);

// Get access to the table
DataTable table= ds.Tables["Lead"];

//Update a row
DataRow rowUpdated = table.Rows[table.Rows.Count -1];
rowUpdated["FirstName"] = "Dataset Update"; 

//Delete a row
DataRow rowDeleted = table.Rows[0];
rowDeleted.Delete();

// Create a row
DataRow rowCreated = table.NewRow();
rowCreated["FirstName"] = "Dataset create"; ;
rowCreated["LastName"] = "Test";
table.Rows.Add(row);

//Sync all changes with the server
adapter.Update(ds);

```
Notes
-----
Code is fully documented and explained. Make sure to also review the provided unit tests to better understand how things work. Remember to change in the unit test files, the variables for your create host, username and password

[Intalio Create]:http://www.intalio.com/products/create/product-tour/
[request one]:http://www.intalio.com/access-form/
[HttpWebRequest]:http://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.aspx
[JSON Serialization works in .NET]:http://msdn.microsoft.com/en-us/library/bb412179(v=vs.110).aspx
[DataContracts]:http://msdn.microsoft.com/en-us/library/ms733127(v=vs.110).aspx
[complete example]:http://msdn.microsoft.com/en-us/library/hh674188.aspx
[System.Activator]:http://msdn.microsoft.com/en-us/library/system.activator(v=vs.110).aspx
[System.Reflection.CustomAttributeData]:http://msdn.microsoft.com/en-us/library/system.reflection.customattributedata.aspx
[System.Reflection.PropertyInfo]:http://msdn.microsoft.com/en-us/library/system.reflection.propertyinfo.aspx
[IDataAdapter]:http://msdn.microsoft.com/en-us/library/system.data.idataadapter(v=vs.110).aspx
[OAuth authentication]:https://developers.intalio.com/intalio/topics/how_do_i_programmatically_access_create_what_is_the_api_in_place
[Rest API documentation]:https://developers.intalio.com/intalio/topics/rest_api_documentation_for_version_2_5_1
[Developer community site]:https://developers.intalio.com/
[reach out directly]:https://www.linkedin.com/in/estebanf