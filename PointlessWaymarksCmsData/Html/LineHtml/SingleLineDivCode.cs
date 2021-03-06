﻿using System;
using PointlessWaymarksCmsData.Database.Models;

namespace PointlessWaymarksCmsData.Html.LineHtml
{
    public partial class SingleLineDiv
    {
        public SingleLineDiv(LineContent dbEntry)
        {
            DbEntry = dbEntry;

            var settings = UserSettingsSingleton.CurrentSettings();
            SiteUrl = settings.SiteUrl;
            SiteName = settings.SiteName;
            PageUrl = settings.LinePageUrl(DbEntry);
        }

        public LineContent DbEntry { get; set; }

        public DateTime? GenerationVersion { get; set; }

        public string PageUrl { get; set; }

        public string SiteName { get; set; }

        public string SiteUrl { get; set; }
    }
}