using Lineage2.Model.Templates;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Lineage2.Model
{
    public class NpcFactory
    {
        private readonly ILogger logger = Log.Logger.ForContext<NpcFactory>();

        public Dictionary<int, NpcTemplate> Initialize()
        {
            string path = Directory.GetCurrentDirectory() + @"\data\json\npcs\";
            string[] xmlFilesArray = Directory.GetFiles(path);
            var npcTemplates = new ConcurrentBag<NpcTemplate>();

            try
            {
                Parallel.ForEach(xmlFilesArray, path =>
                {
                    var result = LoadTemplateFromFile(path);
                    result.ForEach(r => npcTemplates.Add(r));
                });
            }
            catch (Exception e)
            {
                logger.Error(e, $"Не удалось распарсить NPCTemplates  - {e.Message}");
            }

            logger.Information("Из файловой системы загружено {0} NpcTemplates", npcTemplates.Count);
            return npcTemplates.ToDictionary(template => template.NpcId, template => template);
        }

        public List<NpcTemplate> LoadTemplateFromFile(string path)
        {
            string jsonNpcTemplates = File.ReadAllText(path);

            var localNpcTemplates = JsonConvert.DeserializeObject<List<NpcTemplate>>(jsonNpcTemplates);

            return localNpcTemplates;
        }
    }
}
