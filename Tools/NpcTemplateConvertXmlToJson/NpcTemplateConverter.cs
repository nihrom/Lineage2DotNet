using Lineage2.Model;
using Lineage2.Model.Templates;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Tools.NpcTemplateConvertXmlToJson
{
    public class NpcTemplateConverter
    {
        private readonly ILogger logger = Log.Logger.ForContext<NpcFactory>();

        public void Convert()
        {
            string path = Directory.GetCurrentDirectory() + @"\" + @"data\xml\npcs\";
            string[] xmlFilesArray = Directory.GetFiles(path);
            var npcTemplates = new ConcurrentBag<NpcTemplate>();

            try
            {
                Parallel.ForEach(xmlFilesArray, path =>
                {
                    ConvertFile(path);
                });

                logger.Information($"Load {npcTemplates.Count} npcTemplates");
            }
            catch (Exception e)
            {
                logger.Error(e, $"Не удалось распарсить NPC templates  - {e.Message}");
            }
        }

        public double GetDouble(XmlNode node)
        {
            return double.Parse(node.Attributes["val"].Value, NumberStyles.Any, new CultureInfo("en-us"));
        }

        public int GetInt(XmlNode node)
        {
            return int.Parse(node.Attributes["val"].Value);
        }

        public string GetString(XmlNode node)
        {
            return node.Attributes["val"].Value;
        }

        public void WriteTemplateFromNode(XmlNode innerData, NpcTemplate npc)
        {
            if (innerData.Attributes["name"] != null && innerData.Attributes["val"] != null)
            {
                switch (innerData.Attributes["name"].Value)
                {
                    case "radius": npc.CollisionRadius = GetDouble(innerData); break;
                    case "height": npc.CollisionHeight = GetDouble(innerData); break;
                    case "rHand": npc.RHand = GetInt(innerData); break;
                    case "lHand": npc.LHand = GetInt(innerData); break;
                    case "type": npc.Type = GetString(innerData); break;
                    case "exp": npc.Exp = GetInt(innerData); break;
                    case "sp": npc.Sp = GetInt(innerData); break;
                    case "hp": npc.Hp = GetDouble(innerData); break;
                    case "mp": npc.Mp = GetDouble(innerData); break;
                    case "hpRegen": npc.BaseHpReg = GetDouble(innerData); break;
                    case "mpRegen": npc.BaseMpReg = GetDouble(innerData); break;
                    case "pAtk": npc.BasePAtk = GetDouble(innerData); break;
                    case "pDef": npc.BasePDef = GetDouble(innerData); break;
                    case "mAtk": npc.BaseMAtk = GetDouble(innerData); break;
                    case "mDef": npc.BaseMDef = GetDouble(innerData); break;
                    case "crit": npc.BaseCritRate = GetInt(innerData); break;
                    case "atkSpd": npc.BasePAtkSpd = GetInt(innerData); break;
                    case "str": npc.BaseStr = GetInt(innerData); break;
                    case "int": npc.BaseInt = GetInt(innerData); break;
                    case "dex": npc.BaseDex = GetInt(innerData); break;
                    case "wit": npc.BaseWit = GetInt(innerData); break;
                    case "con": npc.BaseCon = GetInt(innerData); break;
                    case "men": npc.BaseMen = GetInt(innerData); break;
                    case "corpseTime": npc.CorpseTime = GetInt(innerData); break;
                    case "walkSpd": npc.BaseWalkSpd = GetInt(innerData); break;
                    case "runSpd": npc.BaseRunSpd = GetInt(innerData); break;
                    case "dropHerbGroup": npc.DropHerbGroup = GetInt(innerData); break;
                    case "": break;
                }
                string DataValue = innerData.Attributes["val"].Value;
            }

            //if (innerData.Name == "drops")
            //{
            //    string type = set.GetString("type");
            //    bool isRaid = type.EqualsIgnoreCase("L2RaidBoss") || type.EqualsIgnoreCase("L2GrandBoss");
            //    List<DropCategory> drops = new List<DropCategory>();
            //    foreach (XmlNode dropCat in innerData.ChildNodes)
            //    {
            //        if ("category".EqualsIgnoreCase(dropCat.Name))
            //        {
            //            attrs = dropCat.Attributes;

            //            DropCategory category = new DropCategory(Int32.Parse(attrs.GetNamedItem("id").Value));

            //            foreach (XmlNode item in dropCat.ChildNodes)
            //            {
            //                if ("drop".EqualsIgnoreCase(item.Name))
            //                {
            //                    attrs = item.Attributes;

            //                    DropData data = new DropData();
            //                    data.SetItemId(Int32.Parse(attrs.GetNamedItem("itemid").Value));
            //                    data.SetMinDrop(Int32.Parse(attrs.GetNamedItem("min").Value));
            //                    data.SetMaxDrop(Int32.Parse(attrs.GetNamedItem("max").Value));
            //                    data.SetChance(Int32.Parse(attrs.GetNamedItem("chance").Value));


            //                    //TODO: warning undefined itemId
            //                    //if (ItemTable.getInstance().getTemplate(data.GetItemId()) == null)
            //                    //{
            //                    //    Log.Warning("Droplist data for undefined itemId: " + data.getItemId());
            //                    //    continue;
            //                    //}
            //                    category.AddDropData(data, isRaid);
            //                }
            //            }
            //            drops.Add(category);
            //        }
            //    }

            //    set.Set("drops", drops);
            //}
        }

        public void ConvertFile(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNodeList nodes = doc.DocumentElement?.SelectNodes("/list/npc");

            if (nodes == null)
                return;

            var localNpcTemplates = new List<NpcTemplate>();

            //ConcurrentBag<string> resultCollection = new ConcurrentBag<string>();
            //ParallelLoopResult result = Parallel.ForEach(nodes, node =>
            //{
            //    resultCollection.Add(AddB(word));
            //});

            foreach (XmlNode node in nodes)
            {
                XmlElement ownerElement = node.Attributes?[0].OwnerElement;
                if ((ownerElement != null) && (node.Attributes != null) && "npc".Equals(ownerElement.Name))
                {
                    NpcTemplate npc = new NpcTemplate();

                    XmlNamedNodeMap attrs = node.Attributes;

                    npc.NpcId = int.Parse(attrs.GetNamedItem("id").Value);
                    npc.TemplateId = attrs.GetNamedItem("idTemplate") == null ? npc.NpcId : int.Parse(attrs.GetNamedItem("idTemplate").Value);
                    npc.Name = attrs.GetNamedItem("name").Value;
                    npc.Title = attrs.GetNamedItem("title").Value;

                    foreach (XmlNode innerData in node.ChildNodes)
                    {
                        WriteTemplateFromNode(innerData, npc);
                    }

                    localNpcTemplates.Add(npc);
                }
            }

            var resultJson = JsonConvert.SerializeObject(localNpcTemplates);

            string outputPath = Directory.GetCurrentDirectory() + @"\" + @"data\json\npcs\";
            var fileName = Path.GetFileName(path).Replace(".xml", string.Empty);

            var fileStream = File.Open(Path.Combine(outputPath, fileName + ".json"), FileMode.OpenOrCreate);

            using (var outputFile = new StreamWriter(fileStream))
            {
                outputFile.Write(resultJson);
            }
        }
    }
}
