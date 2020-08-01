using Anotar.Serilog;
using MouseoverPopup.Interop;
using PluginManager.Interop.Sys;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Builders;
using SuperMemoAssistant.Services;
using SuperMemoAssistant.Sys.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.MouseoverElement
{
  [Serializable]
  public class ContentService : PerpetualMarshalByRefObject, IMouseoverContentProvider
  {

    public RemoteTask<PopupContent> FetchHtml(RemoteCancellationToken ct, string href)
    {

      try
      {

        Regex aboutRegex = new Regex(ElementEx.ElementAboutRegex);
        Regex fileRegex = new Regex(ElementEx.ElementFileRegex);

        if (href.IsNullOrEmpty())
          return null;

        Match fileMatch = fileRegex.Match(href);
        Match aboutMatch = aboutRegex.Match(href);

        if (!(fileMatch.Success || aboutMatch.Success))
          return null;

        Match matched = fileMatch.Success
          ? fileMatch
          : aboutMatch;

        if (int.TryParse(matched.Groups[1].Value, out var id))
          return GetElementContent(id);

        return null;

      }
      catch (RemotingException) 
      {
        return null;
      }
      catch (Exception ex)
      {
        LogTo.Error($"Failed to FetchHtml for {href} with exception {ex}");
        throw;
      }
    }

    private Task<PopupContent> GetElementContent(int elementId)
    {

      var element = Svc.SM.Registry.Element[elementId];
      if (element.IsNull())
        return Task.FromResult<PopupContent>(null);

      // element.ToString(); ??

      var htmlComp = element.ComponentGroup.GetFirstHtmlComponent();
      if (htmlComp.IsNull())
        return Task.FromResult<PopupContent>(null);

      string text = htmlComp.Text.Value;
      string html = CreatePopupHtml(text);

      return Task.FromResult(new PopupContent(new References(), html, false, elementId));

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
