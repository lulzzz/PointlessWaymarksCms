﻿using System.IO;
using System.Threading.Tasks;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using PointlessWaymarksCmsData.Content;
using PointlessWaymarksCmsData.Database.Models;
using PointlessWaymarksCmsData.Html.GeoJsonHtml;
using PointlessWaymarksCmsData.Spatial;

namespace PointlessWaymarksCmsData.Html.LineHtml
{
    public static class LineData
    {
        public static async Task<string> GenerateLineJson(string geoJsonContent, string pageUrl)
        {
            var serializer = GeoJsonSerializer.Create(SpatialHelpers.Wgs84GeometryFactory(), 3);

            using var stringReader = new StringReader(geoJsonContent);
            using var jsonReader = new JsonTextReader(stringReader);
            var contentFeatureCollection = serializer.Deserialize<FeatureCollection>(jsonReader);

            var bounds = SpatialConverters.GeometryBoundingBox(SpatialConverters.GeoJsonToGeometries(geoJsonContent));

            var jsonDto = new LineSiteJsonData(pageUrl,
                new GeoJsonData.SpatialBounds(bounds.MaxY, bounds.MaxX, bounds.MinY, bounds.MinX),
                contentFeatureCollection);

            await using var stringWriter = new StringWriter();
            using var jsonWriter = new JsonTextWriter(stringWriter);
            serializer.Serialize(jsonWriter, jsonDto);

            return stringWriter.ToString();
        }

        public static async Task WriteLocalJsonData(LineContent geoJsonContent)
        {
            var dataFileInfo = new FileInfo(Path.Combine(
                UserSettingsSingleton.CurrentSettings().LocalSiteLineDataDirectory().FullName,
                $"Line-{geoJsonContent.ContentId}.json"));

            if (dataFileInfo.Exists)
            {
                dataFileInfo.Delete();
                dataFileInfo.Refresh();
            }

            await FileManagement.WriteAllTextToFileAndLogAsync(dataFileInfo.FullName,
                await GenerateLineJson(geoJsonContent.Line,
                    UserSettingsSingleton.CurrentSettings().LinePageUrl(geoJsonContent)));
        }

        public record LineSiteJsonData(string PageUrl, GeoJsonData.SpatialBounds Bounds, FeatureCollection GeoJson);
    }
}