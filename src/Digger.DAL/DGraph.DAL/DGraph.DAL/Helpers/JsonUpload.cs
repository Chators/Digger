using System.IO;
using DgraphNet.Client;
using DgraphNet.Client.Proto;
using Google.Protobuf;
using Grpc.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static DgraphNet.Client.DgraphNetClient;

namespace Digger.Server.DGraph
{

    public class JsonUpload
    {

        public void DataUpload(string pathSortedJson, string dgraphSchema)
        {
            //connection à dgraph
            DgraphConnection dgraphConnection = new DgraphConnection("localhost", 9080, ChannelCredentials.Insecure);

            DgraphConnectionPool dgraphConnectionPool = new DgraphConnectionPool().Add(dgraphConnection);

            DgraphNetClient dgraphNetClient = new DgraphNetClient(dgraphConnectionPool);


            //index le schéma 

            string schema = dgraphSchema;
            Operation op = new Operation { Schema = schema };
            dgraphNetClient.Alter(op);


            //lecture du JSON cible
            JObject jsonObj = new JObject();
            jsonObj = JObject.Parse(File.ReadAllText(pathSortedJson));

            //parsing du JSON
            string jsonString = JsonConvert.SerializeObject(jsonObj);

            //on upload le JSON sur la bdd dgraph
            using (Transaction txn = dgraphNetClient.NewTransaction())
            {
                Mutation mu = new Mutation { SetJson = ByteString.CopyFromUtf8(jsonString) };
                txn.Mutate(mu);
                txn.Commit();
            }

        }


    }
}
