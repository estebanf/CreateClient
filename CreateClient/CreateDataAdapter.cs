using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;


namespace CreateClient
{
    /// <summary>
    /// An IDataAdapter implementation that uses an specified ApiClient to rely on a Intalio|Create instance to work as a datastore
    /// </summary>
    /// <typeparam name="T">The type of the data contract to be used when deserializing the JSON response</typeparam>
    /// <typeparam name="K">The type of the data contract to be used when serializing the request operation body</typeparam>
    public class CreateDataAdapter<T, K> : IDataAdapter
        where T:class
        where K:class
    {
        #region private members
        /// <summary>
        /// The ApiClient to use to perform the atomic operations
        /// </summary>
        private ApiClient<T, K> api;
        
        /// <summary>
        /// Type of the data contract to be used in the deserialization of json responses
        /// </summary>
        private Type dataContractInbound;

        /// <summary>
        /// Type of the data contract to be used in the serialization of body arguments
        /// </summary>
        private Type dataContractOutbound;

        /// <summary>
        /// Properties of the data contract to be used in the deserialization of json responses
        /// </summary>
        private PropertyInfo[] inboundProperties;

        /// <summary>
        /// Properties of the data contract to be used in the serialization of body arguments
        /// </summary>
        private PropertyInfo[] outboundProperties;
        #endregion

        #region Constructor
        /// <summary>
        /// Instantiate the data adapter
        /// </summary>
        /// <param name="createApi">The api client to use</param>
        public CreateDataAdapter(ApiClient<T, K> createApi)
        {
            api = createApi;
            dataContractInbound = typeof(T);
            dataContractOutbound = typeof(K);
            inboundProperties = dataContractInbound.GetProperties();
            outboundProperties = dataContractOutbound.GetProperties();
        } 
        #endregion

        #region Implemented public methods
        /// <summary>
        /// In the given dataset, it will create a table to store the value of the create records if that doesn't exist. It will also fill the data of the table.
        /// </summary>
        /// <param name="dataSet">Dataset to use</param>
        /// <returns>Number of records added</returns>
        public int Fill(DataSet dataSet)
        {
            // Declare a variable to reference the Table
            DataTable t;

            // Check if the dataset has a table with a name matching the name of the data contract type
            if (!dataSet.Tables.Contains(dataContractInbound.Name))
            {
                // Table does not exists in the dataset, Let's create one
                t = new DataTable(dataContractInbound.Name);

                // Let's iterate over each property defined in the data contract type
                foreach (var propInfo in inboundProperties)
                {
                    //Create a column with the same name as the current property. For now we will use only string columns
                    DataColumn column = t.Columns.Add(propInfo.Name, typeof(String));
 
                    // We need to know what row has the uuid value of the record. We'll rely into the DataMember attribute of the property, looking for one that's targetting the io_uuid field
                    // Let's get all custom attributes of the current property
                    Array customAttributes = propInfo.CustomAttributes.ToArray<CustomAttributeData>();

                    // Check if we have any custom attribute
                    if (customAttributes.Length > 0)
                    {
                        // There are attributes. Let's iterate over them
                        foreach (CustomAttributeData data in customAttributes)
                        {
                            // Check if we are dealing with a DataMemberAttriute
                            if (data.AttributeType.Name.Equals("DataMemberAttribute"))
                            {
                                // This is a data member attribute. Let's inspect it arguments
                                foreach (CustomAttributeNamedArgument argument in data.NamedArguments)
                                {
                                    // Is the datamember targetting the io_uuid field
                                    if (argument.MemberName.Equals("Name") && argument.TypedValue.Value.Equals("io_uuid"))
                                    {
                                        // We find it. Let's add the primary key contraint.
                                        t.Constraints.Add("PK", column, true);

                                        // We'll play it safe and allow nulls value in the key
                                        column.AllowDBNull = true;

                                        // Nothing more to do, let's break
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                // add the table to the dataset
                dataSet.Tables.Add(t);
            }
            else
            {
                // The dataset has a matching table. let's get it
                int index = dataSet.Tables.IndexOf(dataContractInbound.Name);
                t = dataSet.Tables[index];
            }

            // Now, let's read all records from Create
            List<T> items = api.readAll();

            // For each retrieved record, let's create a row in the table
            foreach (T item in items)
            {
                // Create the row
                DataRow row = t.NewRow();

                // Add the properties values to the row
                setRowValues(item, row);
                
                // Add the row to the table
                t.Rows.Add(row);
            }

            // Let's accept the changes that we did to the dataset
            dataSet.AcceptChanges();

            //Return the number of items recieved.
            return items.Count;
        }

        /// <summary>
        /// Execute the needed operations on Create to reflect the changes in the dataset
        /// </summary>
        /// <param name="dataSet">Dataset to work with</param>
        /// <returns>The number of changes executed in the datastore</returns>
        public int Update(DataSet dataSet)
        {
            // Retrieve the table we should work with. It should match the name of the data contract type
            DataTable table = dataSet.Tables[dataContractInbound.Name];

            // Our changes counter
            int cont = 0;

            // Iterate over the ros
            foreach (DataRow row in table.Rows)
            {
                // Declare an instance of the Datacontract
                T result;

                // Evaluate the RowState
                switch (row.RowState)
                {
                    case DataRowState.Added:
                        // Row was added. Let's execute the create operation.
                        result = api.create(getDataObject(row));
                        
                        // Update the values of the row
                        setRowValues(result, row);
                        
                        // Increment our change counter
                        cont++;

                        // Nothing more to do here
                        break;
                    
                    case DataRowState.Deleted:
                        //Row was deleted. Let's execute the delete operation. We'll use the value of the primary key as the uuid. It should match
                        api.delete(row[table.PrimaryKey[0].ColumnName, DataRowVersion.Original].ToString());

                        // Increment our counter
                        cont++;

                        // Nothing more to do here
                        break;
                    
                    case DataRowState.Modified:
                        // Row was edited. Let's execute the update operation. We'll use the value of the primary key as the uuid. It should match
                        result = api.update(getDataObject(row), row[table.PrimaryKey[0].ColumnName, DataRowVersion.Original].ToString());

                        // Update the values of the row
                        setRowValues(result, row);

                        // Increment our change counter
                        cont++;

                        // Nothing more to do here
                        break;
                }
            }
            
            // Changes should be now in Create. We can accept the changes in the dataset now.
            dataSet.AcceptChanges();

            //Return the counter
            return cont;
        } 
        #endregion

        #region Private methods
        /// <summary>
        /// Given a data contract instance, it updates the values on a given DataRow.
        /// </summary>
        /// <param name="item">The data contract instance</param>
        /// <param name="row">The datarow to update</param>
        private void setRowValues(T item, DataRow row)
        {
            // Let's iterate over the properties of the data contract
            foreach (var propInfo in inboundProperties)
            {
                // Based on the property name, it updates the appropiate row value
                row[propInfo.Name] = propInfo.GetValue(item, null).ToString();
            }
        }
        
        /// <summary>
        /// Given a data row, it returns a instance of the data contract to be used as argument body
        /// </summary>
        /// <param name="row">Row to use</param>
        /// <returns>The instnace of the data contract to be used as argument</returns>
        private K getDataObject(DataRow row)
        {
            // Using reflection, let's create an instance of the data contract
            K dataObject = Activator.CreateInstance(dataContractOutbound) as K;

            // Let's iterate over the properties of the data contract
            foreach (var propInfo in outboundProperties)
            {
                // We don't want to do anything if the value in the data row is DBNull
                if (row[propInfo.Name] != DBNull.Value)
                {
                    // We are good, so using reflection let's set the property value.
                    propInfo.SetValue(dataObject, row[propInfo.Name]);
                }
            }

            // Return the data object
            return dataObject;
        } 
        #endregion
        #region Not implemented methods
        public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
        {
            throw new NotImplementedException();
        }
        public IDataParameter[] GetFillParameters()
        {
            throw new NotImplementedException();
        }
        public MissingMappingAction MissingMappingAction
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public MissingSchemaAction MissingSchemaAction
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public ITableMappingCollection TableMappings
        {
            get { throw new NotImplementedException(); }
        } 
        #endregion
    }
}
