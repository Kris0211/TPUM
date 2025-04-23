using ClientApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace ACommunicationTest
{
    [TestClass]
    public class ASchemaGenerationTests
    {
        [TestMethod]
        public void GenerateAndSaveSchema()
        {
            // Create a schema generator with default settings
            var generator = new JSchemaGenerator();

            // Configure generator options
            generator.DefaultRequired = Required.DisallowNull;
            generator.SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName;
            generator.SchemaLocationHandling = SchemaLocationHandling.Definitions;
            generator.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

            // Generate the root schema
            JSchema schema = new JSchema();

            // Set schema metadata
            schema.SchemaVersion = new Uri("http://json-schema.org/draft-07/schema#");
            schema.Id = new Uri("http://example.com/schemas/client-api.schema.json");
            schema.Title = "Client API Schema";
            schema.Type = JSchemaType.Object;

            // Add definitions
            schema.Properties["definitions"] = new JSchema();
            var definitions = schema.Properties["definitions"];

            // Add type definitions to the schema
            definitions.Properties["ItemDTO"] = generator.Generate(typeof(ItemDTO));
            definitions.Properties["NewPriceDTO"] = generator.Generate(typeof(NewPriceDTO));
            definitions.Properties["ServerCommand"] = generator.Generate(typeof(ServerCommand));
            definitions.Properties["GetItemsCommand"] = generator.Generate(typeof(GetItemsCommand));
            definitions.Properties["SellItemCommand"] = generator.Generate(typeof(SellItemCommand));
            definitions.Properties["ServerResponse"] = generator.Generate(typeof(ServerResponse));
            definitions.Properties["UpdateAllResponse"] = generator.Generate(typeof(UpdateAllResponse));
            definitions.Properties["ReputationChangedResponse"] = generator.Generate(typeof(ReputationChangedResponse));
            definitions.Properties["TransactionResponse"] = generator.Generate(typeof(TransactionResponse));

            // Convert schema to JSON string with formatting
            string schemaJson = JsonConvert.SerializeObject(schema, Formatting.Indented);

            // Save to file
            string testDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string filePath = Path.Combine(testDirectory, "generated-schema.json");
            File.WriteAllText(filePath, schemaJson);

            Console.WriteLine($"Schema saved to: {filePath}");

            // Verify schema was created
            Assert.IsTrue(File.Exists(filePath));
            Assert.IsTrue(File.ReadAllText(filePath).Length > 0);
        }
    }
}
