namespace Org.Vs.TailForWin.Business.Data.SystemInformation
{
  /// <summary>
  /// CPU Info
  /// </summary>
  public class CpuInfo
  {
    /// <summary>
    /// Full name of CPU
    /// </summary>
    public string Name
    {
      get;
      set;
    }

    /// <summary>
    /// Clockspeed of CPU
    /// </summary>
    public string ClockSpeed
    {
      get;
      set;
    }

    /// <summary>
    /// CPU manufacturer
    /// </summary>
    public string Manufacturer
    {
      get;
      set;
    }

    /// <summary>
    /// Native number of processors
    /// </summary>
    public string NumberOfProcessors
    {
      get;
      set;
    }

    /// <summary>
    /// Logical number of processors
    /// </summary>
    public string LogicalNumberOfProcessors
    {
      get;
      set;
    }
  }
}
