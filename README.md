# Gauge.VisualStudio
Visual Studio plugin for Gauge - wires up Visual Studio features to Gauge.

## Installation

Open Visual Studio Extension Manager - `Tools` -> `Extensions and Updates`. Search for `Gauge.VisualStudio`.

The extension resides [here](https://visualstudiogallery.msdn.microsoft.com/d34964c5-3bf8-4138-be63-01214cb1db3e) on the Visual Studio Gallery.

## Features Supported (And Usage)

### Creating a new Gauge Project

- Create a new Project of Type "Class Library".
- Add a Nuget reference to "Gauge.CSharp.Lib" using the Nuget Package Manager.

This should setup a new Gauge project, and add the required meta data for Gauge to execute this project.


### Syntax Highlighting

Gauge specs are in [Markdown](http://daringfireball.net/projects/markdown/syntax) syntax. This plugin highlights Specifications, Scenarios, Steps and Tags.

Steps with missing implementation are also highlighted.

### Autocomplete

This plugin hooks into VisualStudio Intellisense, and brings in autocompletion of Step text. The step texts brought in is a union of steps already defined, concepts defined, and step text from implementation.

Hint: hit <kbd>Ctrl</kbd> + <kbd>Space</kbd> to bring up the Intellisense menu.

### Navigation

Jump from Step text to it's implementation. 

Usage: `Right Click` -> `Go to Declaration` or hit <kbd>F12</kbd>

### Smart Tag

Implement an unimplemented step - generates a method template, with a `Step` attribute having this Step Text. 

### Test Runner

Open the Test Explorer : `Menu` -> `Test` -> `Windows` -> `Test Explorer`
All the scenarios in the project should be listed. Run one or more of these tests.

## License

Gauge.VisualStudio is released under [Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0)

## Copyright

Copyright - 2014, 2015 ThoughtWorks Inc.
