// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-TerminalVelocityByMassGravityDensityAndArea.cs
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
    /// Calculates terminal velocity.
    /// </para>
    ///
    /// <para type="description">
    /// Uses v = sqrt(2 m g / (ρ A C)).
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -MassInKilograms &lt;double&gt;<br/>
    /// Mass in kg.<br/>
    /// - <b>Position</b>: 0<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -GravityInMetersPerSecondSquared &lt;double&gt;<br/>
    /// Gravity in m/s². Default 9.81.<br/>
    /// - <b>Position</b>: 1<br/>
    /// - <b>Default</b>: 9.81<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -DensityInKilogramsPerCubicMeter &lt;double&gt;<br/>
    /// Fluid density in kg/m³.<br/>
    /// - <b>Position</b>: 2<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -AreaInSquareMeters &lt;double&gt;<br/>
    /// Cross-sectional area in m².<br/>
    /// - <b>Position</b>: 3<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -DragCoefficient &lt;double&gt;<br/>
    /// Drag coefficient. Default 0.5.<br/>
    /// - <b>Position</b>: 4<br/>
    /// - <b>Default</b>: 0.5<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -As &lt;string&gt;<br/>
    /// Output unit for velocity. Default 'm/s'.<br/>
    /// - <b>Position</b>: 5<br/>
    /// - <b>Default</b>: "m/s"<br/>
    /// - <b>Allowed Values</b>: m/s, km/h, mph, ft/s<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Example 1: Calculate terminal velocity with specific parameters</para>
    /// <para>This example calculates the terminal velocity for a mass of 80 kg in air with a drag coefficient of 1.0, outputting the result in mph.</para>
    /// <code>
    /// Get-TerminalVelocityByMassGravityDensityAndArea -MassInKilograms 80 -DensityInKilogramsPerCubicMeter 1.225 -AreaInSquareMeters 0.7 -DragCoefficient 1.0 -As "mph"
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Example 2: Calculate terminal velocity using positional parameters</para>
    /// <para>This example uses positional parameters to calculate terminal velocity.</para>
    /// <code>
    /// Get-TerminalVelocityByMassGravityDensityAndArea 70 1.225 0.8 0.8
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "TerminalVelocityByMassGravityDensityAndArea")]
    [OutputType(typeof(double))]
    public class GetTerminalVelocityByMassGravityDensityAndAreaCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// Mass in kg
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Mass in kg")]
        public double MassInKilograms { get; set; }

        /// <summary>
        /// Gravity in m/s² (default: 9.81)
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 1,
            HelpMessage = "Gravity in m/s² (default: 9.81)")]
        public double GravityInMetersPerSecondSquared { get; set; } = 9.81;

        /// <summary>
        /// Fluid density in kg/m³
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 2,
            HelpMessage = "Fluid density in kg/m³")]
        public double DensityInKilogramsPerCubicMeter { get; set; }

        /// <summary>
        /// Cross-sectional area in m²
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 3,
            HelpMessage = "Cross-sectional area in m²")]
        public double AreaInSquareMeters { get; set; }

        /// <summary>
        /// Drag coefficient (default: 0.5)
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 4,
            HelpMessage = "Drag coefficient (default: 0.5)")]
        public double DragCoefficient { get; set; } = 0.5;

        /// <summary>
        /// Output unit for velocity
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 5,
            HelpMessage = "Output unit for velocity")]
        [ValidateSet("m/s", "km/h", "mph", "ft/s")]
        public string As { get; set; } = "m/s";

        protected override void BeginProcessing()
        {
        }

        /// <summary>
        /// Process record - main cmdlet logic
        /// </summary>
        protected override void ProcessRecord()
        {
            // Calculate terminal velocity using the formula v = sqrt(2 m g / (ρ A C))
            double velocity = Math.Sqrt((2 * MassInKilograms * GravityInMetersPerSecondSquared) /
                (DensityInKilogramsPerCubicMeter * AreaInSquareMeters * DragCoefficient));

            // Convert the velocity to the desired unit using GenXdev.Helpers\Convert-PhysicsUnit
            var scriptBlock = ScriptBlock.Create(@"
                param($value, $fromUnit, $toUnit)
                GenXdev.Helpers\Convert-PhysicsUnit -Value $value -FromUnit $fromUnit -ToUnit $toUnit
            ");
            var result = scriptBlock.Invoke(velocity, "m/s", As);

            // Output the result
            WriteObject(result[0].BaseObject);
        }

        protected override void EndProcessing()
        {
        }
    }
}