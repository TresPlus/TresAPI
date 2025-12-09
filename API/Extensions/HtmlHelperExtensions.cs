using Microsoft.AspNetCore.Mvc.Rendering;

namespace API.Extensions
{
  public static class HtmlHelperExtensions
  {
    public static string IsActivePage(this IHtmlHelper html, string activeClass = "active", params string[] pages)
    {
      var currentPage = html.ViewContext.RouteData.Values["page"]?.ToString() ?? "";
      return pages.Any(p => string.Equals(currentPage, p, StringComparison.OrdinalIgnoreCase)) ? activeClass : "";
    }

  }
}
