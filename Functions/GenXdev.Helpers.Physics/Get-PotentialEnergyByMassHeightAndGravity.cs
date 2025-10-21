// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-PotentialEnergyByMassHeightAndGravity.cs
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



using System.Management.Automation;

namespace GenXdev.Helpers.Physics
{
    /// <summary>
    /// <para type="synopsis">
    /// Calculates gravitational potential energy.
    /// </para>
    ///
    /// <para type="description">
    /// Uses PE = m g h.
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
    /// -HeightInMeters &lt;double&gt;<br/>
    /// Height in meters.<br/>
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
    /// Output unit for energy. Default 'joules'.<br/>
    /// - <b>Position</b>: 3<br/>
    /// - <b>Default</b>: "joules"<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Calculate potential energy for 10kg mass at 5m height in calories</para>
    /// <para>This example calculates the gravitational potential energy for a 10kg mass at a height of 5 meters, outputting the result in calories.</para>
    /// <code>
    /// Get-PotentialEnergyByMassHeightAndGravity -MassInKilograms 10 -HeightInMeters 5 -As "calories"
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Calculate potential energy using positional parameters</para>
    /// <para>This example demonstrates using positional parameters to calculate potential energy.</para>
    /// <code>
    /// Get-PotentialEnergyByMassHeightAndGravity 5 10
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "PotentialEnergyByMassHeightAndGravity")]
    [OutputType(typeof(double))]
    public class GetPotentialEnergyByMassHeightAndGravityCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// Mass in kg
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Mass in kg"
        )]
        public double MassInKilograms { get; set; }

        /// <summary>
        /// Height in meters
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Height in meters"
        )]
        public double HeightInMeters { get; set; }

        /// <summary>
        /// Gravity in m/s² (default: 9.81)
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 2,
            HelpMessage = "Gravity in m/s² (default: 9.81)"
        )]
        public double GravityInMetersPerSecondSquared { get; set; } = 9.81;

        /// <summary>
        /// Output unit for energy
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 3,
            HelpMessage = "Output unit for energy"
        )]
        [ValidateSet("joules", "calories", "kilowatthours")]
        public string As { get; set; } = "joules";

        protected override void ProcessRecord()
        {
            // Calculate the potential energy in joules
            double energy = MassInKilograms * GravityInMetersPerSecondSquared * HeightInMeters;

            // Convert the energy to the desired unit using the PowerShell function
            var results = InvokeCommand.InvokeScript(
                $"GenXdev.Helpers\\Convert-PhysicsUnit -Value {energy} -FromUnit 'joules' -ToUnit '{As}'"
            );

            // Output the result
            WriteObject(results[0]);
        }
    }
}