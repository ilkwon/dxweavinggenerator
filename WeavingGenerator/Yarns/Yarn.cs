using Jm.DBConn;
using System.Collections.Generic;
using System;

namespace WeavingGenerator
{
  public partial class Yarn
  {
    private int idx = 0;
    private string name;
    private string weight = "";
    private string unit = "";
    private string type = "";
    private string textured = "";
    private string image = "";
    private string metal = "";
    private string reg_dt = "";

    public int Idx
    {
      get { return idx; }
      set { idx = value; }
    }

    public string Name
    {
      get { return name; }
      set { name = value; }
    }
    public string Weight
    {
      get { return weight; }
      set { weight = value; }
    }
    public string Unit
    {
      get { return unit; }
      set { unit = value; }
    }
    public string Type
    {
      get { return type; }
      set { type = value; }
    }
    public string Textured
    {
      get { return textured; }
      set { textured = value; }
    }
    public string Metal
    {
      get { return metal; }
      set { metal = value; }
    }
    public string Image
    {
      get { return image; }
      set { image = value; }
    }
    public string Reg_dt
    {
      get { return reg_dt; }
      set { reg_dt = value; }
    }

  }
}
