using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("T4WUI.Plugins")]
[assembly: AssemblyDescription("Professional log reader for Windows® UI Plugins")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Virtual Studios")]
[assembly: AssemblyProduct("T4WUI.Plugins")]
[assembly: AssemblyCopyright("Copyright ©  2019 - 2020 M. Zoennchen")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("1cbf978a-e8bd-4f25-88ea-b069d1fdf62e")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyFileVersion("1.0.11")]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Config/log4net.xml", Watch = true)]
[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]
