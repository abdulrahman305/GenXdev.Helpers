// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-DragForceByVelocityDensityAreaAndCoefficient.cs
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



using System.Management.Automation;

namespace GenXdev.Helpers.Physics
{
    /// <summary>
    /// <para type="synopsis">
    /// Calculates drag force.
    /// </para>
    ///
    /// <para type="description">
    /// Uses F = 1/2 C ρ A v².
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -VelocityInMetersPerSecond &lt;double&gt;<br/>
    /// Velocity in m/s.<br/>
    /// - <b>Position</b>: 0<br/>
    /// - <b>Mandatory</b>: true<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -DensityInKilogramsPerCubicMeter &lt;double&gt;<br/>
    /// Fluid density in kg/m³.<br/>
    /// - <b>Position</b>: 1<br/>
    /// - <b>Mandatory</b>: true<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -AreaInSquareMeters &lt;double&gt;<br/>
    /// Cross-sectional area in m².<br/>
    /// - <b>Position</b>: 2<br/>
    /// - <b>Mandatory</b>: true<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -Coefficient &lt;double&gt;<br/>
    /// Drag coefficient.<br/>
    /// - <b>Position</b>: 3<br/>
    /// - <b>Mandatory</b>: true<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -As &lt;string&gt;<br/>
    /// Output unit for force. Default 'newtons'.<br/>
    /// - <b>Position</b>: 4<br/>
    /// - <b>Default</b>: "newtons"<br/>
    /// - <b>Valid values</b>: newtons, poundforce<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Example 1: Calculate drag force with specific parameters</para>
    /// <para>Calculates drag force using velocity 10 m/s, air density 1.225 kg/m³, area 1 m², and coefficient 0.5, outputting in poundforce.</para>
    /// <code>
    /// Get-DragForceByVelocityDensityAreaAndCoefficient -VelocityInMetersPerSecond 10 -DensityInKilogramsPerCubicMeter 1.225 -AreaInSquareMeters 1 -Coefficient 0.5 -As "poundforce"
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Example 2: Calculate drag force using positional parameters</para>
    /// <para>Calculates drag force using positional parameters: velocity 20 m/s, density 1.225 kg/m³, area 2 m², coefficient 0.3.</para>
    /// <code>
    /// Get-DragForceByVelocityDensityAreaAndCoefficient 20 1.225 2 0.3
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "DragForceByVelocityDensityAreaAndCoefficient")]
    [OutputType(typeof(double))]
    public class GetDragForceByVelocityDensityAreaAndCoefficientCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// Velocity in m/s
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Velocity in m/s")]
        public double VelocityInMetersPerSecond { get; set; }

        /// <summary>
        /// Fluid density in kg/m³
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Fluid density in kg/m³")]
        public double DensityInKilogramsPerCubicMeter { get; set; }

        /// <summary>
        /// Cross-sectional area in m²
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 2,
            HelpMessage = "Cross-sectional area in m²")]
        public double AreaInSquareMeters { get; set; }

        /// <summary>
        /// Drag coefficient
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 3,
            HelpMessage = "Drag coefficient")]
        public double Coefficient { get; set; }

        /// <summary>
        /// Output unit for force
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 4,
            HelpMessage = "Output unit for force")]
        [ValidateSet("newtons", "poundforce")]
        public string As { get; set; } = "newtons";

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
            // Calculate the drag force using the formula F = 1/2 C ρ A v²
            double force = 0.5 * Coefficient * DensityInKilogramsPerCubicMeter * AreaInSquareMeters * VelocityInMetersPerSecond * VelocityInMetersPerSecond;

            // Convert the force to the desired unit using the Convert-PhysicsUnit cmdlet
            var script = $"GenXdev.Helpers\\Convert-PhysicsUnit -Value {force} -FromUnit 'newtons' -ToUnit '{As}'";
            var results = InvokeCommand.InvokeScript(script);

            // Output the converted force value
            WriteObject(results[0].BaseObject);
        }

        /// <summary>
        /// End processing - cleanup logic
        /// </summary>
        protected override void EndProcessing()
        {
        }
    }
}