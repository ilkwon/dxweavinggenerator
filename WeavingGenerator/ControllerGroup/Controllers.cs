using System;

namespace WeavingGenerator
{
  public class Controllers
  {
    private static readonly Lazy<Controllers> s_instance = new(() => new Controllers());
    public static Controllers Instance => s_instance.Value;

    public ProjectController CurrentProjectController { get; private set; }

    private Controllers()
    {
      CurrentProjectController = new ProjectController();
    }
  }
}
