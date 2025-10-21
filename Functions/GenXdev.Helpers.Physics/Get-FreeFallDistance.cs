// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-FreeFallDistance.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.304.2025
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
    /// Calculates the distance fallen during free fall for a given time duration.
    /// </para>
    ///
    /// <para type="description">
    /// This function calculates the distance fallen during free fall using a numerical
    /// method that accounts for air resistance and terminal velocity.
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -DurationInSeconds &lt;double&gt;<br/>
    /// The time duration of the fall in seconds.<br/>
    /// - <b>Position</b>: 0<br/>
    /// - <b>Mandatory</b>: true<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -TerminalVelocityInMetersPerSecond &lt;double&gt;<br/>
    /// The terminal velocity in meters per second. Defaults to 53 m/s.<br/>
    /// - <b>Position</b>: 1<br/>
    /// - <b>Default</b>: 53<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -GravityInMetersPerSecondSquared &lt;double&gt;<br/>
    /// The acceleration due to gravity in m/s². Defaults to 9.81.<br/>
    /// - <b>Position</b>: 2<br/>
    /// - <b>Default</b>: 9.81<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -As &lt;string&gt;<br/>
    /// The unit for the output distance. Defaults to 'meters'.<br/>
    /// - <b>Position</b>: 3<br/>
    /// - <b>Default</b>: meters<br/>
    /// - <b>Valid values</b>: meters, kilometers, centimeters, millimeters, feet, inches, miles, yards<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Calculates the distance fallen in 10 seconds in feet.</para>
    /// <para>Detailed explanation of the example.</para>
    /// <code>
    /// Get-FreeFallDistance -DurationInSeconds 10 -TerminalVelocityInMetersPerSecond 53 -As "feet"
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Calculates the distance in 5 seconds in meters.</para>
    /// <para>Detailed explanation of the example.</para>
    /// <code>
    /// Get-FreeFallDistance 5
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "FreeFallDistance")]
    [OutputType(typeof(double))]
    public class GetFreeFallDistanceCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// The time duration of the fall in seconds
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "The time duration of the fall in seconds")]
        public double DurationInSeconds { get; set; }

        /// <summary>
        /// The terminal velocity in meters per second (default: 53)
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 1,
            HelpMessage = "The terminal velocity in meters per second (default: 53)")]
        public double TerminalVelocityInMetersPerSecond { get; set; } = 53;

        /// <summary>
        /// The acceleration due to gravity in m/s² (default: 9.81)
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 2,
            HelpMessage = "The acceleration due to gravity in m/s² (default: 9.81)")]
        public double GravityInMetersPerSecondSquared { get; set; } = 9.81;

        /// <summary>
        /// The unit for the output distance
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 3,
            HelpMessage = "The unit for the output distance")]
        [ValidateSet("meters", "kilometers", "centimeters", "millimeters", "feet", "inches", "miles", "yards")]
        public string As { get; set; } = "meters";

        private double time;
        private double distance;
        private double velocity;

        /// <summary>
        /// Begin processing - initialization logic
        /// </summary>
        protected override void BeginProcessing()
        {
            // set up numerical integration parameters
            // initialize variables
            time = 0;
            distance = 0;
            velocity = 0;

            WriteVerbose(
                "Starting free fall distance calculation for " + DurationInSeconds + " seconds " +
                "with terminal velocity " + TerminalVelocityInMetersPerSecond + " m/s");
        }

        /// <summary>
        /// Process record - main cmdlet logic
        /// </summary>
        protected override void ProcessRecord()
        {
            double dt = 0.01;

            while (time < DurationInSeconds)
            {
                // cap velocity at terminal
                if (velocity >= TerminalVelocityInMetersPerSecond)
                {
                    velocity = TerminalVelocityInMetersPerSecond;
                }
                else
                {
                    velocity += GravityInMetersPerSecondSquared * dt;
                }

                // accumulate distance
                distance += velocity * dt;

                time += dt;

                if (time > 1000)
                {
                    throw new Exception("Calculation timeout exceeded");
                }
            }

            WriteVerbose("Calculated distance: " + distance + " meters");
        }

        /// <summary>
        /// End processing - cleanup logic
        /// </summary>
        protected override void EndProcessing()
        {
            // convert to desired unit
            var scriptBlock = ScriptBlock.Create(@"
param($Value, $FromUnit, $ToUnit)
GenXdev.Helpers\Convert-PhysicsUnit -Value $Value -FromUnit $FromUnit -ToUnit $ToUnit
");
            var result = scriptBlock.Invoke(distance, "meters", As);

            WriteObject(result[0].BaseObject);
        }
    }
}
