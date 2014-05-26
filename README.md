Project Dependency Visualiser
===========================

Creates a map of dependencies between csproj project files inside a given directory.

The generated map is a dgml file, natively supported by all versions of Visual Studio so you can zoom and arrange till your hearts content.

![Sample Dependency Graph](/docs/mmbot.dependencies.png\?raw=true "Sample Dependency Graph")

## Getting Started
From this github repo go to the [releases section](https://github.com/PeteGoo/ProjectDependencyVisualiser/releases) and download the latest release. Unzip to a folder and run as below.

## Usage:

Generate a dgml file by looking in the folder c:\mycode for csproj files

```
.\ProjectDependencyVisualiser.exe -O c:\temp\temp.dgml -R c:\mycode
```

Same as above but ignoring Test projects
```
.\ProjectDependencyVisualiser.exe -O c:\temp\temp.dgml -R c:\mycode -E "*.Test.*.csproj,*.Tests.*.csproj"
```

```
Usage: ProjectDependencyVisualiser options

   OPTION                 TYPE      DESCRIPTION
   -Help (-H)             switch    Shows the help documentation
   -OutputFile (-O)       string*   The path to the output file. Will be overwritten if it exists
   -ExcludeFilters (-E)   string    A list of comma separated path filter patterns to ignore [default=]
   -IncludeFilters (-I)   string    A list of comma separated path filter patterns to include [default=]
   -RootFolder (-R)       string    The path to search for projects. Uses current path if not supplied

   EXAMPLE: ProjectDependencyVisualiser.exe -O c:\temp\temp.dgml -R x:\ReleaseRoot -E "*.Test*.csproj, *.Tests*.csproj"
   Maps all the references in projects under the ReleaseRoot folder that are not tests and outputs to test.dgml

```