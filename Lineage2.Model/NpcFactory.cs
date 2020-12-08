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


                    foreach (XmlNode node in nodes)
                    {
                        XmlElement ownerElement = node.Attributes?[0].OwnerElement;
                        if ((ownerElement != null) && (node.Attributes != null) && "npc".Equals(ownerElement.Name))
                        {
                            XmlNamedNodeMap attrs = node.Attributes;

                            int npcId = int.Parse(attrs.GetNamedItem("id").Value);
                            int templateId = attrs.GetNamedItem("idTemplate") == null ? npcId : int.Parse(attrs.GetNamedItem("idTemplate").Value);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                //Log.ErrorException("Error parsing NPC templates: ", e);
            }
        }
    }
}
