using System.Diagnostics;
using PowerArgs;

namespace ProjectDependencyVisualiser
{
    
    [ArgExample(@"ProjectReferenceDGMLGenerator.exe -O c:\temp\temp.dgml -R x:\ReleaseRoot -E ""*.Test*.csproj, *.Tests*.csproj""", "Maps all the references in projects under the ReleaseRoot folder that are not tests and outputs to test.dgml")]
    public class TheArgs
    {
        [ArgDescription("Shows the help documentation")]
        [HelpHook]
        public bool Help { get; set; }

        [ArgDescription("The path to the output file. Will be overwritten if it exists")]
        [ArgRequired(PromptIfMissing = true)]
        public string OutputFile { get; set; }

        [ArgDescription("A list of comma separated path filter patterns to ignore")]
        [DefaultValue("")]
        public string ExcludeFilters { get; set; }

        [ArgDescription("A list of comma separated path filter patterns to include")]
        [DefaultValue("")]
        public string IncludeFilters { get; set; }

        [ArgDescription("The path to search for projects. Uses current path if not supplied")]
        public string RootFolder { get; set; }


    }
}