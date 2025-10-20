// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-FreeFallHeight.cs
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
    /// Calculates the height fallen during free fall for a given time duration.
    /// </para>
    ///
    /// <para type="description">
    /// This function calculates the distance fallen during free fall using a
    /// numerical method that accounts for air resistance and terminal velocity. The
    /// calculation uses small time steps to accurately model the physics of falling
    /// objects with realistic terminal velocity constraints.
    /// </para>
    ///
    /// PARAMETERS
    ///
    /// <para type="description">
    /// -DurationInSeconds &lt;double&gt;<br/>
    /// The time duration of the fall in seconds for which to calculate the height.<br/>
    /// - <b>Position</b>: 0<br/>
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
    /// The unit for the output height. Defaults to 'meters'.<br/>
    /// - <b>Position</b>: 2<br/>
    /// - <b>Default</b>: "meters"<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Calculates the height fallen in 10 seconds with default human terminal velocity.</para>
    /// <code>Get-FreeFallHeight -DurationInSeconds 10 -TerminalVelocityInMs 53</code>
    /// </example>
    ///
    /// <example>
    /// <para>Calculates the height fallen in 5 seconds using positional parameter and default terminal velocity.</para>
    /// <code>Get-FreeFallHeight 5</code>
    /// </example>
    ///
    /// <example>
    /// <para>Calculates the height fallen in 10 seconds and returns the result in feet.</para>
    /// <code>Get-FreeFallHeight -DurationInSeconds 10 -As "feet"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "FreeFallHeight")]
    [OutputType(typeof(double))]
    public class GetFreeFallHeightCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// The time duration of the fall in seconds
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "The time duration of the fall in seconds"
        )]
        public double DurationInSeconds { get; set; }

        /// <summary>
        /// The terminal velocity in meters per second (default: 53 m/s for human)
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 1,
            HelpMessage = ("The terminal velocity in meters per second " +
                "(default: 53 m/s for human)")
        )]
        public double TerminalVelocityInMs { get; set; } = 53;

        /// <summary>
        /// The unit for the output height
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 2,
            HelpMessage = "The unit for the output height"
        )]
        [ValidateSet("meters", "kilometers", "centimeters", "millimeters", "feet", "inches", "miles", "yards")]
        public string As { get; set; } = "meters";

        /// <summary>
        /// Begin processing - initialization logic
        /// </summary>
        protected override void BeginProcessing()
        {
            WriteVerbose(
                "Starting free fall calculation for " + DurationInSeconds + " seconds " +
                "with terminal velocity " + TerminalVelocityInMs + " m/s"
            );
        }

        /// <summary>
        /// Process record - main cmdlet logic
        /// </summary>
        protected override void ProcessRecord()
        {
            // define the acceleration due to gravity in meters per second squared
            double gravity = 9.81;

            // set up numerical integration parameters for accurate calculation
            double dt = 0.01;

            // initialize time tracking variable
            double time = 0;

            // initialize height accumulator
            double height = 0;

            // initialize velocity tracker
            double velocity = 0;

            // perform numerical integration using small time steps
            while (time < DurationInSeconds)
            {
                // apply air resistance model by capping velocity at terminal velocity
                if (velocity >= TerminalVelocityInMs)
                {
                    // maintain constant terminal velocity when reached
                    velocity = TerminalVelocityInMs;
                }
                else
                {
                    // accelerate under gravity when below terminal velocity
                    velocity += gravity * dt;
                }

                // calculate distance traveled in this time step
                height += velocity * dt;

                // advance time by one step
                time += dt;

                // prevent infinite loops with safety timeout
                if (time > 1000)
                {
                    WriteError(new ErrorRecord(
                        new Exception("Calculation timeout exceeded 1000 seconds"),
                        "CalculationTimeout",
                        ErrorCategory.OperationTimeout,
                        null
                    ));

                    WriteObject(0.0);
                    return;
                }
            }

            WriteVerbose("Calculated fall height: " + height + " meters");

            // convert to desired unit
            var scriptBlock = ScriptBlock.Create(
                "param($Value, $FromUnit, $ToUnit) " +
                "GenXdev.Helpers\\Convert-PhysicsUnit -Value $Value -FromUnit $FromUnit -ToUnit $ToUnit"
            );

            var results = scriptBlock.Invoke(height, "meters", As);

            double result = (double)((PSObject)results[0]).BaseObject;

            // return the calculated height as double
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
// ###############################################################################