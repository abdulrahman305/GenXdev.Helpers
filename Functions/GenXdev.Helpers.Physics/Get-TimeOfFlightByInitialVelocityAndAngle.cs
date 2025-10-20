// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-TimeOfFlightByInitialVelocityAndAngle.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.302.2025
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
    /// Calculates the time of flight for a projectile.
    /// </para>
    ///
    /// <para type="description">
    /// Uses T = (2 v sinθ) / g for ideal motion.
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -InitialVelocityInMetersPerSecond &lt;double&gt;<br/>
    /// Initial velocity in m/s.<br/>
    /// - <b>Position</b>: 0<br/>
    /// - <b>Mandatory</b>: true<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -AngleInDegrees &lt;double&gt;<br/>
    /// Launch angle in degrees.<br/>
    /// - <b>Position</b>: 1<br/>
    /// - <b>Mandatory</b>: true<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -GravityInMetersPerSecondSquared &lt;double&gt;<br/>
    /// Gravity in m/s². Default 9.81.<br/>
    /// - <b>Position</b>: 2<br/>
    /// - <b>Default</b>: 9.81<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -As &lt;string&gt;<br/>
    /// Output unit for time. Default 'seconds'.<br/>
    /// - <b>Position</b>: 3<br/>
    /// - <b>Default</b>: "seconds"<br/>
    /// - <b>Allowed Values</b>: seconds, minutes, hours, milliseconds, days<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Get-TimeOfFlightByInitialVelocityAndAngle -InitialVelocityInMetersPerSecond 20 -AngleInDegrees 45 -As "minutes"</para>
    /// <para>Calculates time of flight for a projectile launched at 20 m/s at 45 degrees, output in minutes.</para>
    /// <code>
    /// Get-TimeOfFlightByInitialVelocityAndAngle -InitialVelocityInMetersPerSecond 20 -AngleInDegrees 45 -As "minutes"
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Get-TimeOfFlightByInitialVelocityAndAngle 30 30</para>
    /// <para>Calculates time of flight for a projectile launched at 30 m/s at 30 degrees, output in seconds (default).</para>
    /// <code>
    /// Get-TimeOfFlightByInitialVelocityAndAngle 30 30
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "TimeOfFlightByInitialVelocityAndAngle")]
    [OutputType(typeof(double))]
    public class GetTimeOfFlightByInitialVelocityAndAngleCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// Initial velocity in m/s
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Initial velocity in m/s")]
        public double InitialVelocityInMetersPerSecond { get; set; }

        /// <summary>
        /// Launch angle in degrees
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Launch angle in degrees")]
        public double AngleInDegrees { get; set; }

        /// <summary>
        /// Gravity in m/s² (default: 9.81)
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 2,
            HelpMessage = "Gravity in m/s² (default: 9.81)")]
        public double GravityInMetersPerSecondSquared { get; set; } = 9.81;

        /// <summary>
        /// Output unit for time
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 3,
            HelpMessage = "Output unit for time")]
        [ValidateSet("seconds", "minutes", "hours", "milliseconds", "days")]
        public string As { get; set; } = "seconds";

        /// <summary>
        /// Begin processing - initialization logic
        /// </summary>
        protected override void BeginProcessing()
        {
        }

        /// <summary>
        /// Process record - main cmdlet logic
        /// </summary>
        protected override void ProcessRecord()
        {
            // Convert angle from degrees to radians
            double thetaRad = AngleInDegrees * Math.PI / 180.0;

            // Calculate time of flight using T = (2 v sinθ) / g
            double time = (2.0 * InitialVelocityInMetersPerSecond * Math.Sin(thetaRad)) / GravityInMetersPerSecondSquared;

            // Convert the result to the desired unit using Convert-PhysicsUnit
            var result = InvokeCommand.InvokeScript($"GenXdev.Helpers\\Convert-PhysicsUnit -Value {time} -FromUnit 'seconds' -ToUnit '{As}'");

            // Output the result
            WriteObject(result);
        }

        /// <summary>
        /// End processing - cleanup logic
        /// </summary>
        protected override void EndProcessing()
        {
        }
    }
}
// ###############################################################################