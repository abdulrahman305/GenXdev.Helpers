// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-ProjectileRangeByInitialSpeedAndAngle.cs
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



using System.Management.Automation;

namespace GenXdev.Helpers.Physics
{
    /// <summary>
    /// <para type="synopsis">
    /// Calculates the range of a projectile.
    /// </para>
    ///
    /// <para type="description">
    /// Uses the formula R = (v² sin(2θ)) / g for ideal projectile motion.
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -InitialSpeedInMetersPerSecond &lt;double&gt;<br/>
    /// Initial speed in m/s.<br/>
    /// - <b>Position</b>: 0<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -AngleInDegrees &lt;double&gt;<br/>
    /// Launch angle in degrees.<br/>
    /// - <b>Position</b>: 1<br/>
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
    /// Output unit for range. Default 'meters'.<br/>
    /// - <b>Position</b>: 3<br/>
    /// - <b>Default</b>: "meters"<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Calculate projectile range with initial speed 20 m/s and angle 45 degrees</para>
    /// <para>Calculates the range using the projectile motion formula.</para>
    /// <code>
    /// Get-ProjectileRangeByInitialSpeedAndAngle -InitialSpeedInMetersPerSecond 20 -AngleInDegrees 45 -As "feet"
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Calculate projectile range using positional parameters</para>
    /// <para>Calculates the range with positional parameters.</para>
    /// <code>
    /// Get-ProjectileRangeByInitialSpeedAndAngle 30 30
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ProjectileRangeByInitialSpeedAndAngle")]
    [OutputType(typeof(double))]
    public class GetProjectileRangeByInitialSpeedAndAngleCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// Initial speed in m/s
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Initial speed in m/s"
        )]
        public double InitialSpeedInMetersPerSecond { get; set; }

        /// <summary>
        /// Launch angle in degrees
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Launch angle in degrees"
        )]
        public double AngleInDegrees { get; set; }

        /// <summary>
        /// Gravity in m/s²
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 2,
            HelpMessage = "Gravity in m/s² (default: 9.81)"
        )]
        public double GravityInMetersPerSecondSquared { get; set; } = 9.81;

        /// <summary>
        /// Output unit for range
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 3,
            HelpMessage = "Output unit for range"
        )]
        [ValidateSet("meters", "kilometers", "centimeters", "millimeters", "feet", "inches", "miles", "yards")]
        public string As { get; set; } = "meters";

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
            double thetaRad = AngleInDegrees * Math.PI / 180;

            // Calculate range using R = (v² sin(2θ)) / g
            double range = (InitialSpeedInMetersPerSecond * InitialSpeedInMetersPerSecond * Math.Sin(2 * thetaRad)) / GravityInMetersPerSecondSquared;

            // Convert the unit using PowerShell function
            string script = $"GenXdev.Helpers\\Convert-PhysicsUnit -Value {range} -FromUnit 'meters' -ToUnit '{As}'";
            var results = InvokeCommand.InvokeScript(script);

            // Extract the result from the script invocation
            object result = results[0].BaseObject;

            // Write the result
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