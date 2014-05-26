using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using PowerArgs;

namespace ProjectDependencyVisualiser
{
    class Program
    {
        private const string MsBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

        static void Main(string[] args)
        {
            if (args != null && args.Length == 1 && args[0].Trim() == "/?")
            {
                ArgUsage.GetStyledUsage<TheArgs>().Write();
                return;
            }

            var arguments = Args.Parse<TheArgs>(args);

            if (arguments == null)
            {
                return;
            }

            var startingFolder = arguments.RootFolder ?? Environment.CurrentDirectory;

            if (!Path.IsPathRooted(startingFolder))
            {
                startingFolder = Path.Combine(Environment.CurrentDirectory, startingFolder);
            }

            if (!Directory.Exists(startingFolder))
            {
                throw new DirectoryNotFoundException(startingFolder);
            }

            var projectFiles = Directory.GetFiles(startingFolder, "*.csproj", SearchOption.AllDirectories);

            var excludeFilters = (arguments.ExcludeFilters ?? string.Empty).Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            var includeFilters = (arguments.IncludeFilters ?? string.Empty).Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();

            if (includeFilters.Any())
            {
                projectFiles = GlobMatch(projectFiles, includeFilters).ToArray();
            }

            if (excludeFilters.Any())
            {
                projectFiles = projectFiles.Except(GlobMatch(projectFiles, excludeFilters)).ToArray();
            }

            var references = (from projectFilePath in projectFiles
                let xdoc = XDocument.Load(projectFilePath)
                let idElement = xdoc.Descendants(XName.Get("ProjectGuid", MsBuildNamespace)).FirstOrDefault()
                let nameElement = xdoc.Descendants(XName.Get("AssemblyName", MsBuildNamespace)).FirstOrDefault()
                where idElement != null && nameElement != null
                select new
                {
                    ProjectId = idElement.Value,
                    Name = nameElement.Value,
                    References = GetProjectReferences(xdoc).ToArray()
                }).ToArray();

            var allProjects = references.Select(r => r.Name)
                .Concat(references.SelectMany(r => r.References)).Distinct().OrderBy(r => r).ToArray();

            var dgmlNs = "http://schemas.microsoft.com/vs/2009/dgml";

            var outputXml = XDocument.Parse(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<DirectedGraph xmlns=""http://schemas.microsoft.com/vs/2009/dgml"">
  <Nodes>
  </Nodes>
  <Links>
  </Links>
</DirectedGraph>
".Trim());
            outputXml.Root.Element(XName.Get("Nodes", dgmlNs)).Add(
                allProjects
                .Select((projectName, i) => 
                    new XElement(XName.Get("Node", dgmlNs), 
                        new XAttribute("Id", i.ToString()), 
                        new XAttribute("Label", projectName))));

            outputXml.Root.Element(XName.Get("Links", dgmlNs)).Add(from project in references
                                                from reference in project.References
                                                select new XElement(XName.Get("Link", dgmlNs),
                                                    new XAttribute("Source", Array.IndexOf(allProjects, project.Name)),
                                                    new XAttribute("Target", Array.IndexOf(allProjects, reference))));

            outputXml.Save(arguments.OutputFile);

        }

        private static IEnumerable<string> GetProjectReferences(XDocument projectFileDocument)
        {
            return
                from projectReference in
                    projectFileDocument.Descendants(XName.Get("ProjectReference", MsBuildNamespace))
                    let nameElement = projectReference.Element(XName.Get("Name", MsBuildNamespace))
                    select nameElement.Value;

        }

        private static IEnumerable<string> GlobMatch(IEnumerable<string> paths, IEnumerable<string> globFilters)
        {
            // Construct corresponding regular expression. Note Regex.Escape!
            Regex[] patterns = globFilters
                .Select(f => new Regex(Regex.Escape(f).Replace(@"\*", ".*").Replace(@"\?", ".")))
              .ToArray();

            return paths.Where(path => patterns.Length == 0 || patterns.Any(p => p.IsMatch(path))).ToArray();
        }
    }
}
