# FivePD API

### Reporting errors

Create a new issue here (on GitHub) in the Issues tab with the appropriate label regarding your issue. This way, we can easily manage and solve issues quickly.

### Requesting new features

To request a new feature, either create a **pull request** or create a new **issue** with the **feature request** label.

### Get started

Before you start making your callouts, it's recommended to take a look at [how to write your first script in FiveM](https://docs.fivem.net/docs/scripting-manual/runtimes/csharp/) as this API is basically nothing more, but a wrapper to make it easier to write callouts with the reduced chance of doing something error prone (With extended features).

##### Creating a new callout using the example
1. Download the CalloutExample project.
2. Open the project and start making your new callout.

##### From scratch in Visual Studio
1. Create a new C# Class Library project and make sure the target framework version is 4.5.2.
2. Go to Project > <ProjectName> Properties > Change the assembly name to <ProjectName>.net
2. Add CalloutAPI.dll as a reference.
3. Add CitizenFX.Core.dll as a reference.
4. Derive Callout class
(For detailed instructions on how to create your first FiveM script, [click here](https://docs.fivem.net/docs/scripting-manual/runtimes/csharp/ "refer here"))

For documentation, [visit the "wiki" tab](https://github.com/KDani-99/FivePD-API/wiki).

[Fields and properties](https://github.com/KDani-99/FivePD-API/wiki/Fields-and-Properties)

[Methods and events](https://github.com/KDani-99/FivePD-API/wiki/Methods-and-Events)

[CalloutPropertiesAttribute](https://github.com/KDani-99/FivePD-API/wiki/CalloutPropertiesAttribute)
