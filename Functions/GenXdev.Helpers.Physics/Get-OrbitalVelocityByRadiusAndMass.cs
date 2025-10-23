// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-OrbitalVelocityByRadiusAndMass.cs
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
    /// Calculates orbital velocity.
    /// </para>
    ///
    /// <para type="description">
    /// Uses v = sqrt(G M / r).
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -RadiusInMeters &lt;double&gt;<br/>
    /// Orbital radius in meters.<br/>
    /// - <b>Position</b>: 0<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -CentralMassInKilograms &lt;double&gt;<br/>
    /// Central mass in kg.<br/>
    /// - <b>Position</b>: 1<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -As &lt;string&gt;<br/>
    /// Output unit for velocity. Default 'm/s'.<br/>
    /// - <b>Position</b>: 2<br/>
    /// - <b>Default</b>: "m/s"<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Calculate orbital velocity at Earth's surface approximately</para>
    /// <para>Uses Earth's mass and radius for demonstration.</para>
    /// <code>
    /// Get-OrbitalVelocityByRadiusAndMass -RadiusInMeters 6371000 -CentralMassInKilograms 5.972e24 -As "km/h"
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Calculate orbital velocity with default units</para>
    /// <para>Simple calculation with default m/s output.</para>
    /// <code>
    /// Get-OrbitalVelocityByRadiusAndMass 10000000 1e26
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "OrbitalVelocityByRadiusAndMass")]
    [OutputType(typeof(double))]
    public class GetOrbitalVelocityByRadiusAndMassCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// Orbital radius in meters
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Orbital radius in meters")]
        public double RadiusInMeters { get; set; }

        /// <summary>
        /// Central mass in kg
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Central mass in kg")]
        public double CentralMassInKilograms { get; set; }

        /// <summary>
        /// Output unit for velocity
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 2,
            HelpMessage = "Output unit for velocity")]
        [ValidateSet("m/s", "km/h", "mph", "ft/s")]
        public string As { get; set; } = "m/s";

        private const double GravitationalConstant = 6.67430e-11; // m³ kg⁻¹ s⁻²

        /// <summary>
        /// Begin processing - initialize gravitational constant
        /// </summary>
        protected override void BeginProcessing()
        {
            // Gravitational constant is defined as a constant
        }

        /// <summary>
        /// Process record - calculate orbital velocity and convert units
        /// </summary>
        protected override void ProcessRecord()
        {
            // Calculate orbital velocity using v = sqrt(G * M / r)
            double velocity = Math.Sqrt(GravitationalConstant * CentralMassInKilograms / RadiusInMeters);

            // Convert velocity to desired unit using PowerShell function for compatibility
            var script = $"GenXdev.Helpers\\Convert-PhysicsUnit -Value {velocity} -FromUnit 'm/s' -ToUnit '{As}'";
            var result = InvokeCommand.InvokeScript(script);

            // Output the converted velocity
            WriteObject(result[0]);
        }

        /// <summary>
        /// End processing - no cleanup needed
        /// </summary>
        protected override void EndProcessing()
        {
            // No cleanup required
        }
    }
}
// ###############################################################################