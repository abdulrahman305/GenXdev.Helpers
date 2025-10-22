// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-MomentumByMassAndVelocity.cs
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
    /// Calculates linear momentum.
    /// </para>
    ///
    /// <para type="description">
    /// Uses p = m v.
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
    /// -VelocityInMetersPerSecond &lt;double&gt;<br/>
    /// Velocity in m/s.<br/>
    /// - <b>Position</b>: 1<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -As &lt;string&gt;<br/>
    /// Output unit for momentum. Default 'kgm/s'.<br/>
    /// - <b>Position</b>: 2<br/>
    /// - <b>Default</b>: "kgm/s"<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Calculate momentum with mass 10kg and velocity 5m/s</para>
    /// <para>Calculates linear momentum using p = m * v.</para>
    /// <code>
    /// Get-MomentumByMassAndVelocity -MassInKilograms 10 -VelocityInMetersPerSecond 5 -As "kgm/s"
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Calculate momentum using positional parameters</para>
    /// <para>Calculates linear momentum with positional parameters.</para>
    /// <code>
    /// Get-MomentumByMassAndVelocity 5 10
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MomentumByMassAndVelocity")]
    [OutputType(typeof(double))]
    public class GetMomentumByMassAndVelocityCommand : PSGenXdevCmdlet
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
        /// Velocity in m/s
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Velocity in m/s"
        )]
        public double VelocityInMetersPerSecond { get; set; }

        /// <summary>
        /// Output unit for momentum
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 2,
            HelpMessage = "Output unit for momentum"
        )]
        [ValidateSet("kgm/s")]
        public string As { get; set; } = "kgm/s";

        /// <summary>
        /// Process record - main cmdlet logic
        /// </summary>
        protected override void ProcessRecord()
        {
            // Calculate momentum using p = m * v
            double momentum = MassInKilograms * VelocityInMetersPerSecond;

            // Convert the unit using PowerShell function
            string script = $"GenXdev.Helpers\\Convert-PhysicsUnit -Value {momentum} -FromUnit 'kgm/s' -ToUnit '{As}'";
            var results = InvokeCommand.InvokeScript(script);

            // Extract the result from the script invocation
            object result = results[0].BaseObject;

            // Write the result
            WriteObject(result);
        }
    }
}