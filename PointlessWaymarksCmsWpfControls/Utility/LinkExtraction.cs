﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PointlessWaymarksCmsData;
using PointlessWaymarksCmsData.Database;
using PointlessWaymarksCmsData.Database.Models;
using PointlessWaymarksCmsWpfControls.LinkContentEditor;

namespace PointlessWaymarksCmsWpfControls.Utility
{
    public static class LinkExtraction
    {
        public static async Task ExtractNewAndShowLinkContentEditors(string toExtractFrom,
            IProgress<string> progressTracker, List<string> excludedUrls = null)
        {
            await ThreadSwitcher.ThreadSwitcher.ResumeBackgroundAsync();

            excludedUrls ??= new List<string>();
            excludedUrls = excludedUrls.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim().ToLower())
                .ToList();

            if (string.IsNullOrWhiteSpace(toExtractFrom))
            {
                progressTracker?.Report("Nothing to Extract From");
                return;
            }

            progressTracker?.Report("Looking for URLs");

            var allMatches = StringHelpers.UrlsFromText(toExtractFrom).Where(x =>
                !x.ToLower().Contains(UserSettingsSingleton.CurrentSettings().SiteUrl) &&
                !excludedUrls.Contains(x.ToLower())).ToList();

            progressTracker?.Report($"Found {allMatches.Count} Matches");

            var linksToShow = new List<string>();

            var db = await Db.Context();

            foreach (var loopMatches in allMatches)
            {
                progressTracker?.Report($"Checking to see if {loopMatches} exists in database...");

                var alreadyExists = await db.LinkContents.AnyAsync(x => x.Url.ToLower() == loopMatches.ToLower());
                if (alreadyExists)
                {
                    progressTracker?.Report($"{loopMatches} exists in database...");
                }
                else
                {
                    if (!linksToShow.Contains(loopMatches))
                    {
                        progressTracker?.Report($"Adding {loopMatches} to list to show...");
                        linksToShow.Add(loopMatches);
                    }
                }
            }

            await ThreadSwitcher.ThreadSwitcher.ResumeForegroundAsync();

            foreach (var loopLinks in linksToShow)
            {
                progressTracker?.Report($"Launching an editor for {loopLinks}...");

                var newWindow = new LinkContentEditorWindow(
                    new LinkContent
                    {
                        ContentId = Guid.NewGuid(),
                        CreatedBy = UserSettingsSingleton.CurrentSettings().DefaultCreatedBy,
                        CreatedOn = DateTime.Now,
                        Url = loopLinks,
                        ShowInLinkRss = false
                    }, true);

                newWindow.Show();
            }
        }
    }
}