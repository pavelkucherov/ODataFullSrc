using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using courses_odata.Model;
using System.Text;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;

namespace courses_odata.Controllers
{
  [Route("examples")]
  public class SiteController : Controller
  {
    public readonly CoursesContext _context;

    public SiteController(CoursesContext context) => this._context = context;

    [HttpGet("{*slug}")]
    public IActionResult Index(string slug)
    {
      ViewData["slug"] = slug;
      return View();
    }
  }

  static public class Extensions {
    public static string GenerateSlug(this string phrase)
    {
        string str = phrase.RemoveDiacritics(); //.ToLower();
        // invalid chars
        str = Regex.Replace(str, @"[^A-Za-z0-9\s-]", "");
        // convert multiple spaces into one space
        str = Regex.Replace(str, @"\s+", " ").Trim();
        // cut and trim
        str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
        str = Regex.Replace(str, @"\s", "-"); // hyphens
        return str;
    }

    public static string RemoveDiacritics(this string text)
    {
        var s = new string(text.Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());

        return s.Normalize(NormalizationForm.FormC);
    }
  }
}
