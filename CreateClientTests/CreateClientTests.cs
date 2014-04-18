using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CreateClient;
using CreateClient.DataContracts;
using System.Data;
using System.Linq;

namespace CreateClientTests
{
    [TestClass]
    public class CreateClientTests
    {
        private static string host = "YOURHOST";
        private static string username = "YOURUSERNAME";
        private static string password = "YOURPASSWORD";

        private RestClient getClient()
        {
            return new RestClient(CreateClientTests.host, CreateClientTests.username, CreateClientTests.password);
        }
        private ApiClient<Lead, LeadData> getApi()
        {
            List<String> fields = new List<string>(new string[6] {"io_uuid","io_first_name","io_last_name","io_email","io_lead_number","io_owner"});
            return new ApiClient<Lead, LeadData>("io_lead", getClient() );
        }
        [TestMethod]
        public void TestRestClient()
        {
            CreateClient.RestClient client = getClient();
            client.prepare();
            Assert.IsNotNull(client.ClientId);
            Assert.IsNotNull(client.AccessToken);
        }
        [TestMethod]
        public void TestReadAll()
        {
            ApiClient<Lead, LeadData> api = getApi();
            List<Lead> leads = api.readAll();
            Assert.IsNotNull(leads);
            Assert.IsTrue(leads.Count > 0);
        }
        [TestMethod]
        public void TestReadOne()
        {
            ApiClient<Lead, LeadData> api = getApi();
            List<Lead> leads = api.readAll();
            Lead lead = api.readOne(leads[0].Id.ToString());
            Assert.IsNotNull(lead);
            Assert.AreEqual(lead.ToString(), leads[0].ToString());
            Console.WriteLine(lead);
        }
        [TestMethod]
        public void TestCreate()
        {
            ApiClient<Lead, LeadData> api = getApi();
            LeadData leadData = new LeadData();
            leadData.FirstName = "Unit";
            leadData.LastName = "Test";
            leadData.Email = "unit@test.com";
            leadData.Owner = "764f0869-8b2a-4e43-8f23-88f593863eff";
            Lead createdLead = api.create(leadData);
            Assert.IsNotNull(createdLead);
            Console.WriteLine(createdLead);
        }
        [TestMethod]
        public void TestUpdate()
        {
            ApiClient<Lead, LeadData> api = getApi();
            LeadData leadData = new LeadData();
            leadData.FirstName = "Unit2";
            leadData.LastName = "Test2";
            leadData.Email = "unit@test.com";
            leadData.Owner = "764f0869-8b2a-4e43-8f23-88f593863eff";
            Lead createdLead = api.create(leadData);

            leadData.Id = createdLead.Id.Value;
            leadData.FirstName = "Unit2Updated";

            Lead updatedLead = api.update(leadData, leadData.Id);
            Assert.IsNotNull(createdLead);
            Console.WriteLine(updatedLead);

        }
        [TestMethod]
        public void TestDelete()
        {
            ApiClient<Lead, LeadData> api = getApi();
            LeadData leadData = new LeadData();
            leadData.FirstName = "Unit3";
            leadData.LastName = "Test3";
            leadData.Email = "unit@test.com";
            leadData.Owner = "764f0869-8b2a-4e43-8f23-88f593863eff";
            Lead createdLead = api.create(leadData);
            leadData.Id = createdLead.Id.Value;
            api.delete(leadData.Id);

        }
        [TestMethod]
        public void TestAdapterFill()
        {
            ApiClient<Lead, LeadData> api = getApi();
            CreateDataAdapter<Lead, LeadData> adapter = new CreateDataAdapter<Lead, LeadData>(api);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            Assert.IsTrue(ds.Tables.Count > 0);
            Assert.IsTrue(ds.Tables["Lead"].Rows.Count > 0);
            Assert.IsTrue(ds.Tables["Lead"].PrimaryKey[0].ColumnName == "Id");
        }
        [TestMethod]
        public void TestAdapterUpdateCreate()
        {
            ApiClient<Lead, LeadData> api = getApi();
            CreateDataAdapter<Lead, LeadData> adapter = new CreateDataAdapter<Lead, LeadData>(api);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            ds.AcceptChanges();
            DataRow row = ds.Tables["Lead"].NewRow();
            row["FirstName"] = "Dataset create"; ;
            row["LastName"] = "Test";
            ds.Tables["Lead"].Rows.Add(row);
            adapter.Update(ds);
            Assert.IsNotNull(row["Id"]);
        }
        [TestMethod]
        public void TestAdapterUpdateUpdate()
        {
            ApiClient<Lead, LeadData> api = getApi();
            CreateDataAdapter<Lead, LeadData> adapter = new CreateDataAdapter<Lead, LeadData>(api);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            DataRow row = ds.Tables["Lead"].Rows[ds.Tables["Lead"].Rows.Count -1];
            row["FirstName"] = "Dataset Update"; ;
            row["LastName"] = "Test";
            adapter.Update(ds);
            Assert.IsTrue(row["FirstName"].Equals("Dataset Update"));
        }
        [TestMethod]
        public void TestAdapterUpdateDelete()
        {
            ApiClient<Lead, LeadData> api = getApi();
            CreateDataAdapter<Lead, LeadData> adapter = new CreateDataAdapter<Lead, LeadData>(api);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            ds.AcceptChanges();
            int count = ds.Tables["Lead"].Rows.Count;
            DataRow row = ds.Tables["Lead"].Rows[count - 1];
            row.Delete();
            adapter.Update(ds);
            Assert.IsTrue(count > ds.Tables["Lead"].Rows.Count);
        }
    }
}
