// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-KineticEnergyByMassAndVelocity.cs
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
    /// Calculates kinetic energy.
    /// </para>
    ///
    /// <para type="description">
    /// Uses KE = 1/2 m v².
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
    /// - <b>Mandatory</b>: true<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -VelocityInMetersPerSecond &lt;double&gt;<br/>
    /// Velocity in m/s.<br/>
    /// - <b>Position</b>: 1<br/>
    /// - <b>Mandatory</b>: true<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -As &lt;string&gt;<br/>
    /// Output unit for energy. Default 'joules'.<br/>
    /// - <b>Position</b>: 2<br/>
    /// - <b>Default</b>: "joules"<br/>
    /// - <b>Allowed values</b>: joules, calories, kilowatthours<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Get-KineticEnergyByMassAndVelocity -MassInKilograms 10 -VelocityInMetersPerSecond 5 -As "calories"</para>
    /// <para>Calculates kinetic energy for a 10kg mass moving at 5 m/s and outputs in calories.</para>
    /// <code>
    /// Get-KineticEnergyByMassAndVelocity -MassInKilograms 10 -VelocityInMetersPerSecond 5 -As "calories"
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Get-KineticEnergyByMassAndVelocity 5 10</para>
    /// <para>Calculates kinetic energy for a 5kg mass moving at 10 m/s using positional parameters.</para>
    /// <code>
    /// Get-KineticEnergyByMassAndVelocity 5 10
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "KineticEnergyByMassAndVelocity")]
    [OutputType(typeof(double))]
    public class GetKineticEnergyByMassAndVelocityCommand : PSGenXdevCmdlet
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
        /// Velocity in m/s
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Velocity in m/s")]
        public double VelocityInMetersPerSecond { get; set; }

        /// <summary>
        /// Output unit for energy
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 2,
            HelpMessage = "Output unit for energy")]
        [ValidateSet("joules", "calories", "kilowatthours")]
        public string As { get; set; } = "joules";

        /// <summary>
        /// Process record - main cmdlet logic
        /// </summary>
        protected override void ProcessRecord()
        {
            // Calculate kinetic energy using KE = 1/2 m v²
            double energy = 0.5 * MassInKilograms * VelocityInMetersPerSecond * VelocityInMetersPerSecond;

            // Convert energy to desired unit using GenXdev.Helpers\Convert-PhysicsUnit
            var script = $"GenXdev.Helpers\\Convert-PhysicsUnit -Value {energy} -FromUnit 'joules' -ToUnit '{As}'";
            var results = InvokeCommand.InvokeScript(script);

            // Output the converted energy value
            if (results != null && results.Count > 0)
            {
                WriteObject(results[0]);
            }
        }
    }
}