﻿using System;

namespace PointlessWaymarksCmsData.Database.Models
{
    public class PhotoMetadata
    {
        public string Aperture { get; set; }
        public string CameraMake { get; set; }
        public string CameraModel { get; set; }
        public string FocalLength { get; set; }
        public int? Iso { get; set; }
        public string Lens { get; set; }
        public string License { get; set; }
        public string PhotoCreatedBy { get; set; }
        public DateTime PhotoCreatedOn { get; set; }
        public string ShutterSpeed { get; set; }
        public string Summary { get; set; }
        public string Tags { get; set; }
        public string Title { get; set; }
    }
}