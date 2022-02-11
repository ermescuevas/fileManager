using Ermes.FileManager.Core.Domain;
using System.Collections.Generic;

namespace Ermes.FileManager.Core
{
    public class BreadcrumbService
    {
        public string GetCssString()
        {
            string breadCumbCss = @"
                    <style>
                    ul.breadcrumb {
                      padding: 10px 16px;
                      list-style: none;
                      background-color: #eee;
                    }
                    ul.breadcrumb li {
                      display: inline;
                      font-size: 18px;
                    }
                    ul.breadcrumb li+li:before {
                      padding: 8px;
                      color: black;
                      content: '/\00a0';
                    }
                    ul.breadcrumb li a {
                      color: #0275d8;
                      text-decoration: none;
                    }
                    ul.breadcrumb li a:hover {
                      color: #01447e;
                      text-decoration: underline;
                    }
                    </style>";

            return breadCumbCss;
        }
        public string GetBreadcrumb(IEnumerable<Breadcrumb> breadcrumbs)
        {
            string breadCrumb = "";
            if (breadcrumbs != null)
            {
                breadCrumb += "<ul class='breadcrumb'> \n";
                foreach (var item in breadcrumbs)
                {
                    breadCrumb += $"<li><a href='{item.Url}'>{item.Name}</a></li> \n";
                }
                breadCrumb += "</ul> \n";
            }

            return breadCrumb;
        }
        public string CreateHtmlStructure(IEnumerable<Breadcrumb> breadcrumbs, bool addCssStructure = true)
        {
            string html = @"<!DOCTYPE html>
                            <html>
                            <head>
                            <meta name='viewport' content='width=device-width, initial-scale=1'>
            ";
            if (addCssStructure)
            {
                html += GetCssString();
            }
            html += @"</head>
                      <body>";
            html += GetBreadcrumb(breadcrumbs);
            html += @"</body>
                      </html>";

            return html;
        }
    }
}