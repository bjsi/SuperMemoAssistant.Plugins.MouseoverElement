using Anotar.Serilog;
using MouseoverPopup.Interop;
using PluginManager.Interop.Sys;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Builders;
using SuperMemoAssistant.Services;
using SuperMemoAssistant.Sys.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.MouseoverElement
{
  [Serializable]
  public class ContentService : PerpetualMarshalByRefObject, IContentProvider
  {

    public RemoteTask<PopupContent> FetchHtml(RemoteCancellationToken ct, string file)
    {
      try
      {

        Regex regex = new Regex(FileEx.ElementRegex);
        Match match = regex.Match(file);

        if (file.IsNullOrEmpty() || !match.Success)
          return null;

        if (int.TryParse(match.Groups[1].Value, out var id))
          return GetElementContent(id);

        return null;

      }
      catch (Exception ex)
      {
        LogTo.Error($"Failed to FetchHtml for file {file} with exception {ex}");
        throw;
      }
    }

    private Task<PopupContent> GetElementContent(int elementId)
    {

      var element = Svc.SM.Registry.Element[elementId];
      if (element.IsNull())
        return null;

      // element.ToString(); ??

      var htmlComp = element.ComponentGroup.GetFirstHtmlComponent();
      if (htmlComp.IsNull())
        return null;

      string text = htmlComp.Text.Value;
      string html = CreatePopupHtml(text);

      return Task.FromResult(new PopupContent(new References(), html));

    }

    private string CreatePopupHtml(string text)
    {

      if (text.IsNullOrEmpty())
        return null;

      string html = @"
          <html>
            <body>
              {0}
            </body>
          </html>";

      return string.Format(html, text);

    }
  }
}
