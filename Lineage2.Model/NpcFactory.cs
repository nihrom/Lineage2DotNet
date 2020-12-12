using Lineage2.Model.Templates;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Lineage2.Model
{
    public class NpcFactory
    {
        private readonly ILogger logger = Log.Logger.ForContext<NpcFactory>();

        public void Initialize()
        {
            XmlDocument doc = new XmlDocument();
            string path = Directory.GetCurrentDirectory() + @"\" + @"data\xml\npcs\";
            string[] xmlFilesArray = Directory.GetFiles(path);

            try
            {
                //StatsSet set = new StatsSet();

                foreach (string i in xmlFilesArray)
                {
                    doc.Load(i);

                    XmlNodeList nodes = doc.DocumentElement?.SelectNodes("/list/npc");

                    if (nodes == null)
                        continue;

                    List<NpcTemplate> npcTemplates = new List<NpcTemplate>();

                    foreach (XmlNode node in nodes)
                    {
                        XmlElement ownerElement = node.Attributes?[0].OwnerElement;
                        if ((ownerElement != null) && (node.Attributes != null) && "npc".Equals(ownerElement.Name))
                        {
                            NpcTemplate npc = new NpcTemplate();

                            XmlNamedNodeMap attrs = node.Attributes;

                            int npcId = int.Parse(attrs.GetNamedItem("id").Value);
                            int templateId = attrs.GetNamedItem("idTemplate") == null ? npcId : int.Parse(attrs.GetNamedItem("idTemplate").Value);
                            //attrs.GetNamedItem("name").Value;
                            //attrs.GetNamedItem("title").Value;

                            npc.NpcId = npcId;
                            npc.TemplateId = templateId;

                            foreach (XmlNode innerData in node.ChildNodes)
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

                            npcTemplates.Add(npc);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //Log.ErrorException("Error parsing NPC templates: ", e);
            }
        }

        public double GetDouble(XmlNode node)
        {
            return double.Parse(node.Attributes["val"].Value);
        }

        public int GetInt(XmlNode node)
        {
            return int.Parse(node.Attributes["val"].Value);
        }

        public string GetString(XmlNode node)
        {
            return node.Attributes["val"].Value;
        }
    }
}
