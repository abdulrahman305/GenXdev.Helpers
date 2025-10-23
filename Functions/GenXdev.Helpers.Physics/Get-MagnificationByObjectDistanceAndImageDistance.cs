// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-MagnificationByObjectDistanceAndImageDistance.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 2.1.2025
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
using System.Management.Automation;

namespace GenXdev.Helpers.Physics
{
    /// <summary>
    /// <para type="synopsis">
    /// Calculates magnification for a lens.
    /// </para>
    ///
    /// <para type="description">
    /// Uses m = - (image distance / object distance).
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -ObjectDistanceInMeters &lt;double&gt;<br/>
    /// Object distance in meters.<br/>
    /// - <b>Position</b>: 0<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -ImageDistanceInMeters &lt;double&gt;<br/>
    /// Image distance in meters.<br/>
    /// - <b>Position</b>: 1<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Get-MagnificationByObjectDistanceAndImageDistance -ObjectDistanceInMeters 0.5 -ImageDistanceInMeters 1</para>
    /// <para>Calculates magnification with object distance 0.5m and image distance 1m.</para>
    /// <code>
    /// Get-MagnificationByObjectDistanceAndImageDistance -ObjectDistanceInMeters 0.5 -ImageDistanceInMeters 1
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Get-MagnificationByObjectDistanceAndImageDistance 0.3 0.6</para>
    /// <para>Calculates magnification using positional parameters.</para>
    /// <code>
    /// Get-MagnificationByObjectDistanceAndImageDistance 0.3 0.6
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MagnificationByObjectDistanceAndImageDistance")]
    [OutputType(typeof(double))]
    public class GetMagnificationByObjectDistanceAndImageDistanceCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// Object distance in meters
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Object distance in meters")]
        public double ObjectDistanceInMeters { get; set; }

        /// <summary>
        /// Image distance in meters
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Image distance in meters")]
        public double ImageDistanceInMeters { get; set; }

        /// <summary>
        /// Process record - main cmdlet logic
        /// </summary>
        protected override void ProcessRecord()
        {
            // Calculate magnification using the formula m = - (image distance / object distance)
            double magnification = -(ImageDistanceInMeters / ObjectDistanceInMeters);

            // Round to 4 decimal places and output the result
            WriteObject(System.Math.Round(magnification, 4));
        }
    }
}
// ###############################################################################