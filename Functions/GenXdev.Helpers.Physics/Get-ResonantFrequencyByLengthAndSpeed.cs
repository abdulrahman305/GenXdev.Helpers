// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-ResonantFrequencyByLengthAndSpeed.cs
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
    /// Calculates resonant frequency for a closed pipe.
    /// </para>
    ///
    /// <para type="description">
    /// Uses f = v / (4 L) for fundamental frequency.
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -LengthInMeters &lt;double&gt;<br/>
    /// Length in meters.<br/>
    /// - <b>Position</b>: 0<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -SpeedInMetersPerSecond &lt;double&gt;<br/>
    /// Wave speed in m/s.<br/>
    /// - <b>Position</b>: 1<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -Medium &lt;string&gt;<br/>
    /// The medium (mutually exclusive with SpeedInMetersPerSecond).<br/>
    /// - <b>Position</b>: 1<br/>
    /// - <b>Accepted Values</b>: air, water, seawater, steel, glass, lead, gold, copper, rubber, vacuum, helium, co2, methane<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -As &lt;string&gt;<br/>
    /// Output unit for frequency. Default 'hertz'.<br/>
    /// - <b>Position</b>: 2<br/>
    /// - <b>Default</b>: "hertz"<br/>
    /// - <b>Accepted Values</b>: hertz, kilohertz, megahertz<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Get-ResonantFrequencyByLengthAndSpeed -LengthInMeters 0.5 -Medium "air" -As "kilohertz"</para>
    /// <para>Calculates the resonant frequency for a 0.5 meter pipe in air, output in kilohertz.</para>
    /// <code>
    /// Get-ResonantFrequencyByLengthAndSpeed -LengthInMeters 0.5 -Medium "air" -As "kilohertz"
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Get-ResonantFrequencyByLengthAndSpeed 1 -SpeedInMetersPerSecond 343</para>
    /// <para>Calculates the resonant frequency for a 1 meter pipe with wave speed 343 m/s.</para>
    /// <code>
    /// Get-ResonantFrequencyByLengthAndSpeed 1 -SpeedInMetersPerSecond 343
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ResonantFrequencyByLengthAndSpeed")]
    [OutputType(typeof(double))]
    public class GetResonantFrequencyByLengthAndSpeedCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// Length in meters
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Length in meters")]
        public double LengthInMeters { get; set; }

        /// <summary>
        /// Wave speed in m/s
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            ParameterSetName = "BySpeed",
            HelpMessage = "Wave speed in m/s")]
        public double SpeedInMetersPerSecond { get; set; }

        /// <summary>
        /// The medium
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            ParameterSetName = "ByMedium",
            HelpMessage = "The medium")]
        [ValidateSet("air", "water", "seawater", "steel", "glass", "lead", "gold", "copper", "rubber", "vacuum", "helium", "co2", "methane")]
        public string Medium { get; set; }

        /// <summary>
        /// Output unit for frequency
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 2,
            HelpMessage = "Output unit for frequency")]
        [ValidateSet("hertz", "kilohertz", "megahertz")]
        public string As { get; set; } = "hertz";

        /// <summary>
        /// Begin processing - initialization logic
        /// </summary>
        protected override void BeginProcessing()
        {
            // Check if using ByMedium parameter set
            if (ParameterSetName == "ByMedium")
            {
                // Set speed based on medium
                switch (Medium.ToLower())
                {
                    case "air":
                        SpeedInMetersPerSecond = 343;
                        break;
                    case "water":
                        SpeedInMetersPerSecond = 1480;
                        break;
                    case "seawater":
                        SpeedInMetersPerSecond = 1530;
                        break;
                    case "steel":
                        SpeedInMetersPerSecond = 5960;
                        break;
                    case "glass":
                        SpeedInMetersPerSecond = 4540;
                        break;
                    case "lead":
                        SpeedInMetersPerSecond = 1210;
                        break;
                    case "gold":
                        SpeedInMetersPerSecond = 3240;
                        break;
                    case "copper":
                        SpeedInMetersPerSecond = 4600;
                        break;
                    case "rubber":
                        SpeedInMetersPerSecond = 60;
                        break;
                    case "vacuum":
                        SpeedInMetersPerSecond = 0;
                        break;
                    case "helium":
                        SpeedInMetersPerSecond = 965;
                        break;
                    case "co2":
                        SpeedInMetersPerSecond = 259;
                        break;
                    case "methane":
                        SpeedInMetersPerSecond = 430;
                        break;
                }

                // Write verbose message
                WriteVerbose($"Using wave speed in {Medium}: {SpeedInMetersPerSecond} m/s");

                // Check for vacuum
                if (SpeedInMetersPerSecond == 0)
                {
                    // Throw terminating error for vacuum
                    ThrowTerminatingError(new ErrorRecord(
                        new Exception("No wave propagation in vacuum"),
                        "VacuumError",
                        ErrorCategory.InvalidArgument,
                        null));
                }
            }
        }

        /// <summary>
        /// Process record - main cmdlet logic
        /// </summary>
        protected override void ProcessRecord()
        {
            // Calculate frequency using f = v / (4 * L)
            double frequency = SpeedInMetersPerSecond / (4.0 * LengthInMeters);

            // Store frequency for end processing
            this.frequency = frequency;
        }

        /// <summary>
        /// End processing - cleanup and output logic
        /// </summary>
        protected override void EndProcessing()
        {
            // Convert frequency using GenXdev.Helpers\Convert-PhysicsUnit
            var scriptBlock = ScriptBlock.Create(
                "param($value, $fromUnit, $toUnit) " +
                "GenXdev.Helpers\\Convert-PhysicsUnit -Value $value -FromUnit $fromUnit -ToUnit $toUnit");

            // Invoke the script block
            var results = scriptBlock.Invoke(frequency, "hertz", As);

            // Get the result as double
            double result = (double)results[0].BaseObject;

            // Write the result
            WriteObject(result);
        }

        // Private field to store frequency between ProcessRecord and EndProcessing
        private double frequency;
    }
}
// ###############################################################################