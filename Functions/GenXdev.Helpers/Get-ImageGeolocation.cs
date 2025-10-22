// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Get-ImageGeolocation.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.308.2025
// ################################################################################
// Copyright (c)  René Vaessen / GenXdev
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ################################################################################



using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace GenXdev.Helpers
{
    /// <summary>
    /// <para type="synopsis">
    /// Extracts geolocation data from an image file.
    /// </para>
    ///
    /// <para type="description">
    /// This function reads EXIF metadata from an image file to extract its latitude and
    /// longitude coordinates. It supports images that contain GPS metadata in their EXIF
    /// data. The function uses the System.Drawing.Image class to load the image and
    /// parse the GPS coordinates from property items.
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -ImagePath &lt;String&gt;<br/>
    /// The full path to the image file to analyze. The file must be a valid image format
    /// that supports EXIF metadata (JPEG, TIFF, etc.).<br/>
    /// - <b>Position</b>: 0<br/>
    /// - <b>Mandatory</b>: true<br/>
    /// - <b>ValueFromPipeline</b>: true<br/>
    /// - <b>ValueFromPipelineByPropertyName</b>: true<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Get geolocation data from an image file</para>
    /// <para>This example extracts GPS coordinates from a vacation photo.</para>
    /// <code>
    /// Get-ImageGeolocation -ImagePath "C:\Pictures\vacation.jpg"
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Use pipeline input to get geolocation data</para>
    /// <para>This example demonstrates using the pipeline to pass the image path.</para>
    /// <code>
    /// "C:\Pictures\vacation.jpg" | Get-ImageGeolocation
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ImageGeolocation")]
    [OutputType(typeof(Hashtable))]
    public class GetImageGeolocationCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// The full path to the image file to analyze
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Path to the image file to analyze")]
        [ValidateNotNullOrEmpty]
        public string ImagePath { get; set; }

        /// <summary>
        /// Begin processing - validate image path exists
        /// </summary>
        protected override void BeginProcessing()
        {
            // Use PowerShell Test-Path for exact path resolution behavior
            var scriptBlock = ScriptBlock.Create("param($path) Test-Path -LiteralPath $path");
            var testResult = scriptBlock.Invoke(ImagePath);

            if (!(bool)testResult[0].BaseObject)
            {
                WriteError(new ErrorRecord(
                    new FileNotFoundException($"The specified image path '{ImagePath}' does not exist."),
                    "PathNotFound",
                    ErrorCategory.ObjectNotFound,
                    ImagePath));
                return;
            }

            WriteVerbose($"Processing image: {ImagePath}");
        }

        /// <summary>
        /// Process record - extract GPS metadata from image
        /// </summary>
        protected override void ProcessRecord()
        {
            Image image = null;

            try
            {
                // Load the image file
                WriteVerbose("Loading image file");
                image = Image.FromFile(ImagePath);

                // Get all property items
                var propertyItems = image.PropertyItems;

                // Extract GPS metadata properties
                WriteVerbose("Extracting GPS metadata");
                var latitudeRef = propertyItems.FirstOrDefault(pi => pi.Id == 0x0001);
                var latitude = propertyItems.FirstOrDefault(pi => pi.Id == 0x0002);
                var longitudeRef = propertyItems.FirstOrDefault(pi => pi.Id == 0x0003);
                var longitude = propertyItems.FirstOrDefault(pi => pi.Id == 0x0004);

                // Check if GPS data exists
                if (latitude != null && longitude != null && latitudeRef != null && longitudeRef != null)
                {
                    // Calculate actual latitude and longitude values
                    uint latNum = BitConverter.ToUInt32(latitude.Value, 0);
                    uint latDen = BitConverter.ToUInt32(latitude.Value, 4);
                    double lat = (double)latNum / latDen;

                    uint lonNum = BitConverter.ToUInt32(longitude.Value, 0);
                    uint lonDen = BitConverter.ToUInt32(longitude.Value, 4);
                    double lon = (double)lonNum / lonDen;

                    // Adjust for south and west hemispheres
                    if (latitudeRef.Value[0] == (byte)'S')
                    {
                        lat = -lat;
                    }

                    if (longitudeRef.Value[0] == (byte)'W')
                    {
                        lon = -lon;
                    }

                    WriteVerbose($"GPS coordinates found: {lat}, {lon}");

                    var result = new Hashtable
                    {
                        ["Latitude"] = lat,
                        ["Longitude"] = lon
                    };

                    WriteObject(result);
                }
                else
                {
                    WriteVerbose("No GPS metadata found in image");
                    WriteObject(null);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(
                    ex,
                    "ImageProcessingError",
                    ErrorCategory.InvalidOperation,
                    ImagePath));
            }
            finally
            {
                if (image != null)
                {
                    image.Dispose();
                }
            }
        }

        /// <summary>
        /// End processing - no cleanup needed
        /// </summary>
        protected override void EndProcessing()
        {
        }
    }
}