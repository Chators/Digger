using Newtonsoft.Json;
using DgraphDotNet;
using DgraphNet.Client;
using Grpc.Core;
using DgraphNet.Client.Proto;
using Newtonsoft.Json.Linq;
using System.IO;
using static DgraphNet.Client.DgraphNetClient;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;
using DGraph.DAL;

namespace Digger.Server.DGraph
{
    public class DGraphGateway
    {
        readonly IOptions<DGraphGatewayOptions> _options;

        public DGraphGateway(IOptions<DGraphGatewayOptions> options)
        {
            _options = options;
        }

        public void DataUpload(string pathSortedJson, string dgraphSchema)
        {
            //connection à dgraph
            DgraphConnection dgraphConnection = new DgraphConnection(_options.Value.Host, _options.Value.Port, ChannelCredentials.Insecure);

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

        public FluentResults.Result<string> Request(string query)
        {
            using (var client = Clients.NewDgraphClient())
            {
                client.Connect(_options.Value.CompleteAdress);
                using (var transaction = client.NewTransaction())
                {
                    var result = transaction.Query(query);
                    transaction.Commit();
                    return (result);
                }
            }
        }

        public async Task<FluentResults.Result<string>> FindByProjectId(int projectId)
        {
            using (var client = Clients.NewDgraphClient())
            {
                client.Connect(_options.Value.CompleteAdress);
                using (var transaction = client.NewTransaction())
                {
                    var result = transaction.Query("{FindByIdProject(func: eq(projectId, \"" + projectId + "\")) {uid expand(_all_)  {uid expand(_all_) }}}");
                    transaction.Commit();
                    return (result);
                }
            }
        }

        public async Task AddNodesToNode(int projectId, string uidMasterNode, List<AddNodesToNodeData> nodes)
        {
            List<string> jsonMutation = new List<string>();
            StringBuilder req = new StringBuilder();
            

            for (int i = 0; i < nodes.Count; i++)
            {
                AddNodesToNodeData node = nodes[i];

                // Verify if node exists
                string r = GetNodeBy(projectId, node.Data, node.TypeOfData);
                dynamic nodeDGraph = JsonConvert.DeserializeObject(r);

                bool nodeExists = nodeDGraph.FindNode.Count > 0;
                bool nodeHasLink = node.Link != null;

                if (nodeExists && nodeHasLink)
                {
                    // On vérifie si les 2 noeuds ne sont pas linké
                    string sourceUid;
                    for (int y = 0; y < node.Link.Count; y++)
                    {
                        if (node.Link[y].Uid == "master") sourceUid = uidMasterNode;
                        else sourceUid = node.Link[y].Uid;

                        bool nodeAlreadyLinked = false;
                        if (nodeDGraph.FindNode[0]["link"] != null)
                        {
                            for (int z = 0; z < nodeDGraph.FindNode[0].link.Count; z++)
                            {
                                if (nodeDGraph.FindNode[0].link[z].uid == sourceUid)
                                {
                                    nodeAlreadyLinked = true;
                                    break;
                                }
                            }
                        }
                        if (nodeDGraph.FindNode[0]["~link"] != null)
                        {
                            for (int z = 0; z < nodeDGraph.FindNode[0]["~link"].Count; z++)
                            {
                                if (nodeDGraph.FindNode[0]["~link"][z].uid == sourceUid)
                                {
                                    nodeAlreadyLinked = true;
                                    break;
                                }
                            }
                        }
                        if (!nodeAlreadyLinked)
                            await AddEdge(projectId, sourceUid, (string)nodeDGraph.FindNode[0].uid);
                    }
                }
                else
                {
                    if (nodeHasLink)
                    {
                        for (int y = 0; y < node.Link.Count; y++)
                        {
                            string nodeSource;
                            if (node.Link[y].Uid == "master") nodeSource = uidMasterNode;
                            else nodeSource = node.Link[y].Uid;

                            string nodeTarget = node.Data + node.TypeOfData;
                            req = new StringBuilder();
                            req.AppendLine("{");
                            req.AppendLine("\"uid\" : \"" + nodeSource + "\",");
                            req.AppendLine("\"link\" : {");
                            req.AppendLine("\"uid\" : \"_:" + nodeTarget + "\"");
                            req.AppendLine("}");
                            req.AppendLine("}");
                            jsonMutation.Add(req.ToString());
                        }
                    }

                    req = new StringBuilder();
                    req.AppendLine("{");
                    req.AppendLine("\"uid\" : \"_:" + node.Data + node.TypeOfData + "\",");
                    req.AppendLine("\"data\" : \"" + node.Data + "\",");
                    req.AppendLine("\"author\" : \"" + node.Author + "\",");
                    req.AppendLine("\"note\" : \"" + node.Note + "\",");
                    req.AppendLine("\"projectId\" : \"" + node.ProjectId + "\",");
                    req.AppendLine("\"source\" : \"" + node.Source + "\",");
                    req.AppendLine("\"typeOfData\" : \"" + node.TypeOfData + "\",");
                    req.AppendLine("\"lastUpdate\" : \"" + node.LastUpdate + "\"");
                    req.AppendLine("}");
                    jsonMutation.Add(req.ToString());
                }
            }
            req = new StringBuilder();
            req.AppendLine("[");
            for(int n = 0; n < jsonMutation.Count; n++)
            {
                string json = jsonMutation[n];
                req.AppendLine(json);
                if (n != jsonMutation.Count - 1) req.AppendLine(",");
            }
            req.AppendLine("]");

            // Fourberie mais ça veut dire que y'a pas de changement donc ça sert à rien
            if (req.ToString().Length != 6)
            {
                //SAVE DATA
                DgraphConnection dgraphConnection = new DgraphConnection(_options.Value.Host, _options.Value.Port, ChannelCredentials.Insecure);
                DgraphConnectionPool dgraphConnectionPool = new DgraphConnectionPool().Add(dgraphConnection);
                DgraphNetClient dgraphNetClient = new DgraphNetClient(dgraphConnectionPool);
                using (var transaction = dgraphNetClient.NewTransaction())
                {
                    Console.WriteLine(req.ToString());
                    Mutation mu = new Mutation { SetJson = ByteString.CopyFromUtf8(req.ToString()) };
                    transaction.Mutate(mu);
                    transaction.Commit();

                }
            }
        }
        
        public async Task<string> AddNode(int projectId, string author, string data, string note, string source, string typeOfData)
        {
            DgraphConnection dgraphConnection = new DgraphConnection(_options.Value.Host, _options.Value.Port, ChannelCredentials.Insecure);
            DgraphConnectionPool dgraphConnectionPool = new DgraphConnectionPool().Add(dgraphConnection);
            DgraphNetClient dgraphNetClient = new DgraphNetClient(dgraphConnectionPool);

            using (var transaction = dgraphNetClient.NewTransaction())
            {
                StringBuilder req = new StringBuilder();
                req.AppendLine("{");
                req.AppendLine("\"author\" : \"" + author + "\",");
                req.AppendLine("\"data\" : \"" + data + "\",");
                req.AppendLine("\"note\" : \"" + note + "\",");
                req.AppendLine("\"projectId\" : \"" + projectId + "\",");
                req.AppendLine("\"source\" : \"" + source + "\",");
                req.AppendLine("\"typeOfData\" : \"" + typeOfData + "\",");
                req.AppendLine("\"lastUpdate\" : \"" + DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo) + "\"");
                req.AppendLine("}");
                Mutation mu = new Mutation { SetJson = ByteString.CopyFromUtf8(req.ToString()) };
                transaction.Mutate(mu);
                transaction.Commit();
            }

            string jsonResult = GetNodeBy(projectId, data, typeOfData);
            return jsonResult;
        }

        public async Task ModifyNode(int projectId, string uid, string author, string data, string note, string source, string typeOfData)
        {
            DgraphConnection dgraphConnection = new DgraphConnection(_options.Value.Host, _options.Value.Port, ChannelCredentials.Insecure);
            DgraphConnectionPool dgraphConnectionPool = new DgraphConnectionPool().Add(dgraphConnection);
            DgraphNetClient dgraphNetClient = new DgraphNetClient(dgraphConnectionPool);

            using (var transaction = dgraphNetClient.NewTransaction())
            {
                StringBuilder req = new StringBuilder();
                req.AppendLine("{");
                req.AppendLine("\"uid\" : \"" + uid + "\",");
                req.AppendLine("\"author\" : \"" + author + "\",");
                req.AppendLine("\"data\" : \"" + data + "\",");
                req.AppendLine("\"note\" : \"" + note + "\",");
                req.AppendLine("\"projectId\" : \"" + projectId + "\",");
                req.AppendLine("\"source\" : \"" + source + "\",");
                req.AppendLine("\"typeOfData\" : \"" + typeOfData + "\",");
                req.AppendLine("\"lastUpdate\" : \"" + DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo) + "\"");
                req.AppendLine("}");
                Mutation mu = new Mutation { SetJson = ByteString.CopyFromUtf8(req.ToString()) };
                transaction.Mutate(mu);
                transaction.Commit();

            }
        }

        public async Task DeleteNodesById(int projectId, List<string> nodesId)
        {
            using (var client = Clients.NewDgraphClient())
            {
                client.Connect(_options.Value.CompleteAdress);
                using (var transaction = client.NewTransaction())
                {
                    var result = transaction.Query("{FindByIdProject (func: eq(projectId,\"" + projectId + "\")) {uid}}");
                    dynamic result2 = JsonConvert.DeserializeObject(result.Value);
                    
                    for (int i = 0; i < result2.FindByIdProject.Count; i++)
                    {
                        for (int y = 0; y < nodesId.Count; y++)
                        {
                            if (result2.FindByIdProject[i].uid == nodesId[y])
                            {
                                var sourceLinkNode = transaction.Query("{FindSourceLink(func: uid(\"" + nodesId[y] + "\")) { ~link { uid } } }");
                                dynamic sourceLinkNode2 = JsonConvert.DeserializeObject(sourceLinkNode.Value);

                                if (sourceLinkNode2.FindSourceLink.Count > 0)
                                {
                                    for (int z = 0; z < sourceLinkNode2.FindSourceLink[0]["~link"].Count; z++)
                                    {
                                        string uid = sourceLinkNode2.FindSourceLink[0]["~link"][z].uid;
                                        StringBuilder req = new StringBuilder();
                                        req.AppendLine("{");
                                        req.AppendLine("\"uid\" : \"" + uid + "\",");
                                        req.AppendLine("\"link\" : {");
                                        req.AppendLine("\"uid\" : \"" + nodesId[y] + "\"");
                                        req.AppendLine("}");
                                        req.AppendLine("}");
                                        transaction.Delete(req.ToString());
                                    }
                                }
                                transaction.Delete("{\"uid\": \"" + nodesId[y] + "\"}");
                                nodesId.RemoveAt(y);
                            }
                        }
                        if (nodesId.Count <= 0) break;
                    }
                    transaction.Commit();
                }
            }
        }

        public async Task AddEdge(int projectId, string source, string target)
        {
            using (var client = Clients.NewDgraphClient())
            {
                client.Connect(_options.Value.CompleteAdress);
                using (var transaction = client.NewTransaction())
                {
                    var result = transaction.Query("{FindByIdProject (func: eq(projectId,\"" + projectId + "\")) {uid}}");
                    dynamic result2 = JsonConvert.DeserializeObject(result.Value);

                    int counter = 0;
                    for (int i = 0; i < result2.FindByIdProject.Count; i++)
                    {
                        if (result2.FindByIdProject[i].uid == source || result2.FindByIdProject[i].uid == target) counter++;
                    }

                    if (counter == 2)
                    {
                        StringBuilder req = new StringBuilder();
                        req.AppendLine("{");
                        req.AppendLine("\"uid\" : \"" + source + "\",");
                        req.AppendLine("\"link\" : {");
                        req.AppendLine("\"uid\" : \"" + target + "\"");
                        req.AppendLine("}");
                        req.AppendLine("}");
                        transaction.Mutate(req.ToString());
                        transaction.Commit();
                    }
                }
            }
        }

        public async Task DeleteEdge(int projectId, string source, string target)
        {
            using (var client = Clients.NewDgraphClient())
            {
                client.Connect(_options.Value.CompleteAdress);
                using (var transaction = client.NewTransaction())
                {
                    var result = transaction.Query("{FindByIdProject (func: eq(projectId,\"" + projectId + "\")) {uid}}");
                    dynamic result2 = JsonConvert.DeserializeObject(result.Value);

                    int counter = 0;
                    for (int i = 0; i < result2.FindByIdProject.Count; i++)
                    {
                        if (result2.FindByIdProject[i].uid == source || result2.FindByIdProject[i].uid == target) counter++;
                    }
                    
                    if (counter == 2)
                    {
                        StringBuilder req = new StringBuilder();
                        req.AppendLine("{");
                        req.AppendLine("\"uid\" : \"" + source + "\",");
                        req.AppendLine("\"link\" : {");
                        req.AppendLine("\"uid\" : \"" + target + "\"");
                        req.AppendLine("}");
                        req.AppendLine("}");
                        transaction.Delete(req.ToString());
                        transaction.Commit();
                    }
                }
            }
        }

        public async void DeleteProject(int projectId)
        {
            using (var client = Clients.NewDgraphClient())
            {
                client.Connect(_options.Value.CompleteAdress);
                using (var transaction = client.NewTransaction())
                {
                    try
                    {


                        var result = transaction.Query("{FindByIdProject (func: eq(projectId,\"" + projectId + "\")) {uid}}");
                        dynamic result2 = JsonConvert.DeserializeObject(result.Value);

                        for (int i = 0; i < result2.FindByIdProject.Count; i++)
                        {
                            transaction.Delete($"{{\"uid\": \"{result2.FindByIdProject[i].uid}\"}}");
                        }
                        transaction.Commit();
                    }
                    catch { };
                }
            }
        }

        public bool NodeExists(int projectId, string uidNode)
        {
            using (var client = Clients.NewDgraphClient())
            {
                client.Connect(_options.Value.CompleteAdress);
                using (var transaction = client.NewTransaction())
                {
                    var result = transaction.Query("{FindByIdProject (func: eq(projectId,\"" + projectId + "\")) {uid}}");
                    dynamic result2 = JsonConvert.DeserializeObject(result.Value);
                    
                    for (int i = 0; i < result2.FindByIdProject.Count; i++)
                    {
                        if (result2.FindByIdProject[i].uid == uidNode) return true;
                    }
                    return false;
                }
            }
        }

        public string GetNodeBy(int projectId, string data, string typeOfData)
        {
            using (var client = Clients.NewDgraphClient())
            {
                client.Connect(_options.Value.CompleteAdress);
                using (var transaction = client.NewTransaction())
                {
                    string query = "{FindNode (func: eq(projectId,\"" + projectId + "\")) @filter(eq(data,\"" + data + "\") AND eq(typeOfData,\"" + typeOfData + "\")){uid expand(_all_) { uid expand(_all_)}}}";
                    var result = transaction.Query(query);
                    transaction.Commit();
                    return (result.Value);
                }
            }
        }

        // prepare a list for integrate json in dgraph database
        public async Task<List<AddNodesToNodeData>> CreateAddNodesToNodeData(int projectId, string author, string nodeMaster, List<NodeSearchData> nodes)
        {
            List<AddNodesToNodeData> result = new List<AddNodesToNodeData>();

            for (int i = 0; i < nodes.Count; i++)
            {
                NodeSearchData node = nodes[i];
                result.Add(new AddNodesToNodeData());
                result[i].Author = author;
                result[i].Data = node.Data;
                result[i].LastUpdate = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);
                result[i].Note = node.Note;
                result[i].ProjectId = Convert.ToString(projectId);
                result[i].Source = node.Source;
                result[i].TypeOfData = node.TypeOfData;
                if (node.Link != null)
                {
                    result[i].Link = new List<Link>();
                    for (int y = 0; y < node.Link.Count; y++)
                    {
                        Link link = new Link();
                        link.Uid = node.Link[y].Uid;
                        result[i].Link.Add(link);
                    }
                }
            }

            return result;
        }

        // Ne pas hésiter à aller voir le projet DGraph.DB/Scripts/schemaDGraph.txt
        enum DGraphSchema
        {
            author,
            data,
            lastUpdate,
            link,
            note,
            projectId,
            source,
            typeOfData
        }
    }

    public class DGraphGatewayOptions
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string CompleteAdress { get; set; }
    }
}
