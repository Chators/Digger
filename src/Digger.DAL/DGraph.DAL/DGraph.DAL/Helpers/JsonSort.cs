using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Digger.Server.DGraph
{
    public class JsonSort
    {

        private JObject JsonRead(string pathJsonSource)
        {

            JObject o1 = new JObject();
            o1 = JObject.Parse(File.ReadAllText(pathJsonSource));
            return o1;

        }

        public void SortFromDataSploit(string pathJsonSource, string domainName, string jsonTargetLocation, int projectId)
        {

            JObject o1 = JsonRead(pathJsonSource);

            //On récupère les données du JSON source qu'on veut exploiter
            JToken mailToken = o1["domain_emailhunter"];
            JToken adressToken = o1["domain_whois"]["adress"] + ", " + o1["domain_whois"]["city"];
            JToken nameToken = o1["domain_whois"]["name"];
            int total = (int)o1["domain_shodan"]["total"];
            var domain_paste = o1["domain_pastes"][1];
            var boolpaste = (bool)o1["domain_pastes"][0];



            //On créer un tableau par objet de donnée pour le nouveau JSON
            JArray mailArray = new JArray();
            JArray nameArray = new JArray();
            JArray arrayDomain = new JArray();
            if (boolpaste)
            {
                var domain_paste_to_array = (JArray)o1["domain_pastes"][1];
                //On créer un objet pour chaque sites liés

                for (int i = 0; i < domain_paste_to_array.Count; i++)
                {
                    JObject domainPaste = new JObject(
                        new JObject(
                            new JProperty("url", o1["domain_pastes"][1][i]["pagemap"]["metatags"][0]["og:url"]),
                            new JProperty("title", o1["domain_pastes"][1][i]["pagemap"]["metatags"][0]["og:title"]),
                            new JProperty("type", o1["domain_pastes"][1][i]["pagemap"]["metatags"][0]["og:type"]),
                            new JProperty("site_name", o1["domain_pastes"][1][i]["pagemap"]["metatags"][0]["og:site_name"])
                        )
                    );
                    arrayDomain.Add(domainPaste);
                }

            }

            //On créer un objet qui contient l'objet enfant
            JObject k = new JObject();
            k["related_website_list"] = arrayDomain;
            JObject m = new JObject();
            k.Add("website_name", "related_website");
            m.Add("related_website_obj", k);


            //On créer un objet pour chaque mail, et on le stock dans le tableau concerné
            if (mailToken != null)
            {
                foreach (var mail in mailToken)
                {
                    JObject mailObj = new JObject(
                        new JObject(
                            new JProperty("email_name", mail)
                            )
                    );
                    mailArray.Add(mailObj);

                }
            }

            if (nameToken != null)
            {
                foreach (var name in nameToken)
                {
                    JObject nameObj = new JObject(
                        new JObject(
                            new JProperty("name", name)
                        )
                    );
                    nameArray.Add(nameObj);

                    //on s'arrète à la première itération pour récupérer que le propriétaire du site
                    break;
                }
            }

            //On assemble le JSON trié
            JObject objSort = new JObject();

            objSort["mail_list"] = mailArray;

            //On crée un nouveau objet qui va être l'objet parent des mails
            JObject u = new JObject();
            objSort.Add("mail_name", "mails");
            u.Add("mail_obj", objSort);
            if (nameToken != null) u["owner"] = nameArray;

            //On stock dans une liste toutes les organisations liées au noms de domaine
            List<string> listOrga = new List<string>();

            //On parcours l'objet du JSON source qui contient les organisations
            for (int i = 0; i < total; i++)
            {
                //On check si il n'y pas de doublon
                if (!listOrga.Contains((string)o1["domain_shodan"]["matches"][i]["org"]))
                {
                    listOrga.Add((string)o1["domain_shodan"]["matches"][i]["org"]);
                }
            }

            //même chose que les foreach du dessus
            JArray orgRelated = new JArray();

            foreach (var item in listOrga)
            {
                JObject orgObj = new JObject(
                    new JObject(
                        new JProperty("organisation_name", item)
                    )
                );
                orgRelated.Add(orgObj);
            }

            JObject orgRelatedObj = new JObject();
            orgRelatedObj["organisation_list"] = orgRelated;
            JObject w = new JObject();
            orgRelatedObj.Add("name_org", "organisations");
            w.Add("organisation_obj", orgRelatedObj);

            //On merge les objets des organisation a celui des mails et du proriétaire du site
            u.Merge(w, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });

            //On merge l'objet des site liés
            u.Merge(m, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });

            //On ajoute une propriété au JSON trié, pour pouvoir indéxer le schéma du graph
            u.Add("domain_name", domainName);
            u.Add("project_Id", projectId);

            //On parse le JSON pour pouvoir l'écrire
            string sortedJson = JsonConvert.SerializeObject(u);

            //On écrit le JSON sur le serveur
            File.WriteAllText(jsonTargetLocation + domainName + "_sorted" + ".json", sortedJson);
        }

        public void SortAll(string pathJsonSource, string researchName, string jsonTargetLocation, int projectId)
        {

            JObject o1 = JsonRead(pathJsonSource);
            Dictionary<string, JToken> dic_proprety = new Dictionary<string, JToken>();


            //On stocks les propriétés du JSON dans un dictionnaire
            foreach (JProperty property in o1.Properties())
            {
                if (property.Value.HasValues)
                {
                    dic_proprety.Add(property.Name, property.Value);
                }
            }

            //On supprime les tableaux vides
            foreach (var item in dic_proprety)
            {
                if (item.Value.GetType() == typeof(JArray))
                {
                    if (!item.Value.HasValues)
                    {
                        dic_proprety.Remove(item.Key);
                    }
                    var lolae = item.Value;
                    JArray qs = (JArray)lolae;
                    for (int i = 0; i < qs.Count; i++)
                    {
                        if (qs[i].GetType() == typeof(JArray))
                        {
                            qs.RemoveAt(i);
                        }
                    }
                }
            }
            JObject k = new JObject();
            int b = 0;


            foreach (var item in dic_proprety)
            {
                if (item.Value.HasValues)
                {
                    JArray array = new JArray();
                    b++;
                    JObject obj = new JObject(
                       new JObject(
                           new JProperty("name_value_list" + b, item.Value)
                           )
                   );
                    array.Add(obj);
                    JObject objSort = new JObject();
                    objSort["list" + b] = array;
                    JObject u = new JObject();
                    u.Add("obj" + b, objSort);
                    k.Merge(u, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                }

            }
            k.Add("IndexableKeyWord", researchName);
            k.Add("project_id", projectId);
            var json2 = JsonConvert.SerializeObject(k);
            string json3 = json2.Replace("true", "0");
            string jsonSorted = json3.Replace("false", "1");
            File.WriteAllText(jsonTargetLocation + researchName + "_sorted" + ".json", jsonSorted);


        }
    }
}
