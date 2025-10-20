// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-RefractionAngleByIncidentAngleAndIndices.cs
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



/*
###############################################################################
Part of PowerShell module : GenXdev.Helpers.Physics
Original cmdlet filename  : Get-RefractionAngleByIncidentAngleAndIndices.cs
Original author           : René Vaessen / GenXdev
Version                   : 1.302.2025
###############################################################################
Copyright (c)  René Vaessen / GenXdev

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
###############################################################################
*/
using System;
using System.Management.Automation;

namespace GenXdev.Helpers.Physics
{
    /// <summary>
    /// <para type="synopsis">
    /// Calculates refraction angle using Snell's law.
    /// </para>
    ///
    /// <para type="description">
    /// Uses θ2 = arcsin( (n1 / n2) sin θ1 ).
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -IncidentAngleInDegrees &lt;double&gt;<br/>
    /// Incident angle in degrees.<br/>
    /// - <b>Position</b>: 0<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -IndexOfRefraction1 &lt;double&gt;<br/>
    /// Refractive index of first medium.<br/>
    /// - <b>Position</b>: 1<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -IndexOfRefraction2 &lt;double&gt;<br/>
    /// Refractive index of second medium.<br/>
    /// - <b>Position</b>: 2<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -As &lt;string&gt;<br/>
    /// Output unit for angle. Default 'degrees'.<br/>
    /// - <b>Position</b>: 3<br/>
    /// - <b>Default</b>: "degrees"<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Get refraction angle in radians</para>
    /// <para>Calculates the refraction angle when light passes from air (n=1) to water (n=1.33) at 30 degrees incidence.</para>
    /// <code>
    /// Get-RefractionAngleByIncidentAngleAndIndices -IncidentAngleInDegrees 30 -IndexOfRefraction1 1 -IndexOfRefraction2 1.33 -As "radians"
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Get refraction angle in degrees (default)</para>
    /// <para>Calculates the refraction angle when light passes from air (n=1) to glass (n=1.5) at 45 degrees incidence.</para>
    /// <code>
    /// Get-RefractionAngleByIncidentAngleAndIndices 45 1 1.5
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "RefractionAngleByIncidentAngleAndIndices")]
    [OutputType(typeof(double))]
    public class GetRefractionAngleByIncidentAngleAndIndicesCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// Incident angle in degrees
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Incident angle in degrees")]
        public double IncidentAngleInDegrees { get; set; }

        /// <summary>
        /// Refractive index of first medium
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Refractive index of first medium")]
        public double IndexOfRefraction1 { get; set; }

        /// <summary>
        /// Refractive index of second medium
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 2,
            HelpMessage = "Refractive index of second medium")]
        public double IndexOfRefraction2 { get; set; }

        /// <summary>
        /// Output unit for angle
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 3,
            HelpMessage = "Output unit for angle")]
        [ValidateSet("degrees", "radians")]
        public string As { get; set; } = "degrees";

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
            // Convert incident angle from degrees to radians
            double theta1Rad = IncidentAngleInDegrees * Math.PI / 180.0;

            // Calculate sine of refraction angle using Snell's law
            double sinTheta2 = (IndexOfRefraction1 / IndexOfRefraction2) * Math.Sin(theta1Rad);

            // Check for total internal reflection
            if (sinTheta2 > 1.0)
            {
                // Create error record for total internal reflection
                var errorRecord = new ErrorRecord(
                    new InvalidOperationException("Total internal reflection - no refraction"),
                    "TotalInternalReflection",
                    ErrorCategory.InvalidOperation,
                    null);

                // Throw terminating error to match original PowerShell behavior
                ThrowTerminatingError(errorRecord);
            }

            // Calculate refraction angle in radians
            double theta2Rad = Math.Asin(sinTheta2);

            // Convert refraction angle to degrees
            double angle = theta2Rad * 180.0 / Math.PI;

            // Use InvokeCommand to call Convert-PhysicsUnit for unit conversion
            var script = $"GenXdev.Helpers\\Convert-PhysicsUnit -Value {angle} -FromUnit 'degrees' -ToUnit '{As}'";

            // Invoke the script and get the result
            var results = InvokeCommand.InvokeScript(script);

            // Extract the double result from the collection
            double result = (double)results[0].BaseObject;

            // Write the result to the pipeline
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