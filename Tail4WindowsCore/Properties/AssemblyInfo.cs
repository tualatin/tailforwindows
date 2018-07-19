using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("T4WCore")]
[assembly: AssemblyDescription("Professional log reader for Windows® Core")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Virtual Studios")]
[assembly: AssemblyProduct("T4WCore")]
[assembly: AssemblyCopyright("Copyright ©  2018 M. Zoennchen")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("7e255b50-cfb7-4ad8-9694-e569427fe866")]

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
[assembly: AssemblyFileVersion("1.0.33")]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Config/log4net.xml", Watch = true)]
