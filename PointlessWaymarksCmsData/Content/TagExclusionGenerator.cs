﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PointlessWaymarksCmsData.Database;
using PointlessWaymarksCmsData.Database.Models;

namespace PointlessWaymarksCmsData.Content
{
    public static class TagExclusionGenerator
    {
        public static async Task<(GenerationReturn generationReturn, TagExclusion returnContent)> Save(
            TagExclusion toSave)
        {
            var validationResult = await Validate(toSave);
            if (validationResult.HasError) return (validationResult, null);

            var db = await Db.Context();

            if (toSave.Id < 1)
            {
                toSave.Tag = Db.TagListItemCleanup(toSave.Tag);
                toSave.ContentVersion = DateTime.Now.ToUniversalTime().TrimDateTimeToSeconds();

                await db.AddAsync(toSave);
                await db.SaveChangesAsync(true);
                return (await GenerationReturn.Success("Tag Exclusion Saved"), toSave);
            }

            var toModify = await db.TagExclusions.SingleAsync(x => x.Id == toSave.Id);

            toModify.Tag = Db.TagListItemCleanup(toSave.Tag);
            toModify.ContentVersion = DateTime.Now.ToUniversalTime().TrimDateTimeToSeconds();

            await db.SaveChangesAsync(true);

            return (await GenerationReturn.Success("Tag Exclusion Saved"), toModify);
        }

        public static async Task<GenerationReturn> Validate(TagExclusion toValidate)
        {
            if (toValidate == null) return await GenerationReturn.Error("Excluded Tag can not be empty");

            if (string.IsNullOrWhiteSpace(toValidate.Tag))
                return await GenerationReturn.Error("Excluded Tag can not be blank");

            var validationResult = CommonContentValidation.ValidateTags(toValidate.Tag.TrimNullToEmpty());
            if (!validationResult.isValid) return await GenerationReturn.Error(validationResult.explanation);

            var cleanedTag = Db.TagListItemCleanup(toValidate.Tag);

            var db = await Db.Context();
            if (db.TagExclusions.Any(x => x.Id != toValidate.Id && x.Tag == cleanedTag))
                return await GenerationReturn.Error(
                    $"It appears that the tag '{cleanedTag}' is already in the Exclusion List?");

            return await GenerationReturn.Success("Tag Exclusion is Valid");
        }
    }
}