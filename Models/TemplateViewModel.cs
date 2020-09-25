using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace RoadProject.Models
{
    public class TemplateViewModel
    {
        public Project Project { get; set; }

        public IEnumerable<string> Categories { get; set; }

        public IDictionary<string, IEnumerable<string>> Files { get; set; }

        public static TemplateViewModel CreateInstance(Project project)
        {
            var model = new TemplateViewModel{Project = project};
            var q = from d in Directory.GetDirectories("Templates")
                    orderby d
                    select d.Split('/').LastOrDefault();
            model.Categories = new List<string>(q.ToArray());
            model.Files = new Dictionary<string, IEnumerable<string>>();
            foreach(var dir in model.Categories)
            {
                string path = $"Templates/{dir}";
                q = from f in Directory.GetFiles(path)
                    orderby f
                    select f.Split('/').LastOrDefault();
                var files = new List<string>(q.ToArray());
                model.Files.Add(dir, files);
            }

            return model;
        }
    }
}