using System;

namespace WeavingGenerator
{
  public class Controllers
  {
    private static readonly Lazy<Controllers> _instance = new(() => new Controllers());
    public static Controllers Instance => _instance.Value;

    public ProjectController ProjectController { get; private set; }

    private Controllers()
    {
      ProjectController = new ProjectController();
    }
  }
}
