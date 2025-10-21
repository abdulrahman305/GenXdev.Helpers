// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-FreeFallTime.cs
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
    /// Calculates the time required for an object to fall a given height during free fall.
    /// </para>
    ///
    /// <para type="description">
    /// This function calculates the time duration required for an object to fall a
    /// specified height using a numerical method that accounts for air resistance and
    /// terminal velocity. The calculation uses small time steps to accurately model
    /// the physics of falling objects with realistic terminal velocity constraints.
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -HeightInMeters &lt;double&gt;<br/>
    /// The height to fall in meters for which to calculate the time duration.<br/>
    /// - <b>Position</b>: 0<br/>
    /// - <b>Mandatory</b>: true<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -TerminalVelocityInMs &lt;double&gt;<br/>
    /// The terminal velocity in meters per second. Defaults to 53 m/s which is the
    /// typical terminal velocity for a human in free fall.<br/>
    /// - <b>Position</b>: 1<br/>
    /// - <b>Default</b>: 53<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -As &lt;string&gt;<br/>
    /// The unit for the output time. Defaults to 'seconds'.<br/>
    /// - <b>Position</b>: 2<br/>
    /// - <b>Default</b>: "seconds"<br/>
    /// - <b>Allowed values</b>: seconds, minutes, hours, milliseconds, days<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Calculates the time required to fall 100 meters with default human terminal velocity.</para>
    /// <para>Detailed explanation of the example.</para>
    /// <code>
    /// Get-FreeFallTime -HeightInMeters 100 -TerminalVelocityInMs 53
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Calculates the time required to fall 50 meters using positional parameter and default terminal velocity.</para>
    /// <para>Detailed explanation of the example.</para>
    /// <code>
    /// Get-FreeFallTime 50
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Calculates the time required to fall 100 meters and returns the result in minutes.</para>
    /// <para>Detailed explanation of the example.</para>
    /// <code>
    /// Get-FreeFallTime -HeightInMeters 100 -As "minutes"
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "FreeFallTime")]
    [OutputType(typeof(double))]
    public class GetFreeFallTimeCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// The height to fall in meters for which to calculate the time duration.
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "The height to fall in meters"
        )]
        public double HeightInMeters { get; set; }

        /// <summary>
        /// The terminal velocity in meters per second (default: 53 m/s for human).
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 1,
            HelpMessage = ("The terminal velocity in meters per second " +
                "(default: 53 m/s for human)")
        )]
        public double TerminalVelocityInMs { get; set; } = 53;

        /// <summary>
        /// The unit for the output time.
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 2,
            HelpMessage = "The unit for the output time"
        )]
        [ValidateSet("seconds", "minutes", "hours", "milliseconds", "days")]
        public string As { get; set; } = "seconds";

        /// <summary>
        /// Begin processing - initialization logic
        /// </summary>
        protected override void BeginProcessing()
        {
            WriteVerbose(
                "Starting free fall time calculation for " + HeightInMeters + " meters " +
                "with terminal velocity " + TerminalVelocityInMs + " m/s"
            );
        }

        /// <summary>
        /// Process record - main cmdlet logic
        /// </summary>
        protected override void ProcessRecord()
        {
            // Define the acceleration due to gravity in meters per second squared
            double gravity = 9.81;

            // Set up numerical integration parameters for accurate calculation
            double dt = 0.01;

            // Initialize time tracking variable
            double time = 0;

            // Initialize height accumulator
            double height = 0;

            // Initialize velocity tracker
            double velocity = 0;

            // Perform numerical integration using small time steps until height is reached
            while (height < HeightInMeters)
            {
                // Apply air resistance model by capping velocity at terminal velocity
                if (velocity >= TerminalVelocityInMs)
                {
                    // Maintain constant terminal velocity when reached
                    velocity = TerminalVelocityInMs;
                }
                else
                {
                    // Accelerate under gravity when below terminal velocity
                    velocity += gravity * dt;
                }

                // Calculate distance traveled in this time step
                height += velocity * dt;

                // Advance time by one step
                time += dt;

                // Prevent infinite loops with safety timeout
                if (time > 1000)
                {
                    WriteError(new ErrorRecord(
                        new Exception("Calculation timeout exceeded 1000 seconds"),
                        "CalculationTimeout",
                        ErrorCategory.OperationTimeout,
                        null
                    ));

                    WriteObject((double)0);
                    return;
                }
            }

            WriteVerbose("Calculated fall time: " + time + " seconds");

            // Convert to desired unit using PowerShell function
            var scriptBlock = ScriptBlock.Create(
                "param($Value, $FromUnit, $ToUnit) " +
                "GenXdev.Helpers\\Convert-PhysicsUnit -Value $Value -FromUnit $FromUnit -ToUnit $ToUnit"
            );

            var results = scriptBlock.Invoke(time, "seconds", As);

            // Extract the result from PSObject
            double result = 0;
            if (results.Count > 0 && results[0].BaseObject is double)
            {
                result = (double)results[0].BaseObject;
            }

            // Return the calculated time as double
            WriteObject(result);
        }

        /// <summary>
        /// End processing - cleanup logic
        /// </summary>
        protected override void EndProcessing()
        {
            // No cleanup needed
        }
    }
}