using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.MouseoverElement
{
  public static class ElementEx
  {
    public const string ElementFileRegex = @"^file:\/\/\/.+SuperMemoElementNo=\(([0-9]+)\)";
    public const string ElementAboutRegex = @"^about:SuperMemoElementNo=\(([0-9]+)\)";
  }
}
