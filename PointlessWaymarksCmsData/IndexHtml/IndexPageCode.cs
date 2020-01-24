﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using AngleSharp.Html;
using AngleSharp.Html.Parser;
using HtmlTags;
using PointlessWaymarksCmsData.CommonHtml;
using PointlessWaymarksCmsData.Models;
using PointlessWaymarksCmsData.PostHtml;

namespace PointlessWaymarksCmsData.IndexHtml
{
    public partial class IndexPage
    {
        public IndexPage()
        {
            var settings = UserSettingsSingleton.CurrentSettings();
            SiteUrl = settings.SiteUrl;
            SiteName = settings.SiteName;
            SiteKeywords = settings.SiteKeywords;
            SiteSummary = settings.SiteSummary;
            SiteAuthors = settings.SiteAuthors;
            PageUrl = settings.IndexPageUrl();

            var db = Db.Context().Result;

            Posts = db.PostContents.OrderByDescending(x => x.CreatedOn).Take(8).ToList();

            var mainImageGuid = Posts.FirstOrDefault(x => x.MainPicture != null)?.MainPicture;

            if (mainImageGuid != null) MainImage = new PictureSiteInformation(mainImageGuid.Value);
        }

        public PictureSiteInformation MainImage { get; set; }

        public string PageUrl { get; set; }

        public List<PostContent> Posts { get; set; }
        public string SiteAuthors { get; set; }
        public string SiteKeywords { get; set; }

        public string SiteName { get; set; }
        public string SiteSummary { get; set; }

        public string SiteUrl { get; set; }

        public HtmlTag IndexPosts()
        {
            if (!Posts.Any()) return HtmlTag.Empty();

            var indexBodyContainer = new DivTag().AddClass("index-posts-container");

            foreach (var loopPosts in Posts)
            {
                var post = new SinglePostDiv(loopPosts);
                var indexPostContentDiv = new DivTag().AddClass("index-posts-content");
                indexPostContentDiv.Encoded(false).Text(post.TransformText());
                indexBodyContainer.Children.Add(indexPostContentDiv);
                indexBodyContainer.Children.Add(HorizontalRule.StandardRule());
            }

            return indexBodyContainer;
        }

        public HtmlTag Title()
        {
            var titleContainer = new DivTag().AddClass("index-title-container");
            var titleHeader = new HtmlTag("H1").AddClass("index-title-content").Text(SiteName);
            var titleSiteSummary = new HtmlTag("H5").AddClass("index-title-summary-content").Text(SiteSummary);

            titleContainer.Children.Add(titleHeader);
            titleContainer.Children.Add(titleSiteSummary);

            return titleContainer;
        }

        public void WriteLocalHtml()
        {
            var settings = UserSettingsSingleton.CurrentSettings();

            var parser = new HtmlParser();
            var htmlDoc = parser.ParseDocument(TransformText());

            var stringWriter = new StringWriter();
            htmlDoc.ToHtml(stringWriter, new PrettyMarkupFormatter());

            var htmlString = stringWriter.ToString();

            var htmlFileInfo = new FileInfo($@"{settings.LocalSiteRootDirectory}\index.html");

            if (htmlFileInfo.Exists)
            {
                htmlFileInfo.Delete();
                htmlFileInfo.Refresh();
            }

            File.WriteAllText(htmlFileInfo.FullName, htmlString);
        }
    }
}