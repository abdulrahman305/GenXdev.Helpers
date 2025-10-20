// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : ConvertTo-HashTable.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace GenXdev.Helpers
{
    /// <summary>
    /// <para type="synopsis">
    /// Converts a PSCustomObject to a HashTable recursively.
    /// </para>
    ///
    /// <para type="description">
    /// This function converts a PSCustomObject and all its nested PSCustomObject
    /// properties into HashTables. It handles arrays and other collection types by
    /// processing each element recursively.
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -InputObject &lt;System.Object[]&gt;<br/>
    /// The PSCustomObject to convert into a HashTable. Accepts pipeline input.<br/>
    /// - <b>Position</b>: 0<br/>
    /// - <b>Mandatory</b>: true<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Convert a PSCustomObject to a HashTable</para>
    /// <para>This example demonstrates how to convert a PSCustomObject with nested properties into a HashTable.</para>
    /// <code>
    /// $object = [PSCustomObject]@{
    ///     Name = "John"
    ///     Age = 30
    ///     Details = [PSCustomObject]@{
    ///         City = "New York"
    ///     }
    /// }
    /// $hashTable = ConvertTo-HashTable -InputObject $object
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsData.ConvertTo, "HashTable")]
    [OutputType(typeof(Hashtable), typeof(System.Collections.IEnumerable), typeof(System.ValueType), typeof(string))]
    public class ConvertToHashTableCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// The PSCustomObject to convert into a HashTable
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            HelpMessage = "The PSCustomObject to convert into a HashTable"
        )]
        [ValidateNotNull]
        public object[] InputObject { get; set; }

        /// <summary>
        /// Begin processing - initialization logic
        /// </summary>
        protected override void BeginProcessing()
        {
            WriteVerbose("Starting PSCustomObject to HashTable conversion");
        }

        /// <summary>
        /// Process record - main cmdlet logic
        /// </summary>
        protected override void ProcessRecord()
        {
            foreach (var item in InputObject)
            {
                var result = ConvertToHashTable(item);
                WriteObject(result);
            }
        }

        /// <summary>
        /// End processing - cleanup logic
        /// </summary>
        protected override void EndProcessing()
        {
            WriteVerbose("Completed HashTable conversion");
        }

        private object ConvertToHashTable(object input)
        {
            // return empty hashtable if input is null
            if (input == null)
            {
                return new Hashtable(StringComparer.OrdinalIgnoreCase);
            }

            // handle simple value types directly
            if (input is System.ValueType || input is string)
            {
                return input;
            }

            // handle PSCustomObject
            var psObj = input as PSObject;
            if (psObj != null)
            {
                var resultTable = new Dictionary<string, object>();

                // process each property of the current object
                foreach (var property in psObj.Properties)
                {
                    var value = property.Value;

                    // handle nested PSCustomObject properties
                    if (value is PSObject)
                    {
                        value = ConvertToHashTable(value);
                    }
                    // handle collection properties
                    else if (value is System.Collections.IEnumerable && !(value is string))
                    {
                        var list = new List<object>();
                        foreach (var element in (System.Collections.IEnumerable)value)
                        {
                            if (element is PSObject)
                            {
                                list.Add(ConvertToHashTable(element));
                            }
                            else
                            {
                                list.Add(element);
                            }
                        }
                        value = list.ToArray();
                    }
                    // handle simple value properties
                    else
                    {
                        // value remains as is
                    }

                    resultTable[property.Name] = value;
                }

                // create final hashtable with sorted keys
                var finalResultTable = new Hashtable(StringComparer.OrdinalIgnoreCase);
                foreach (var key in resultTable.Keys.OrderBy(k => k, StringComparer.Ordinal))
                {
                    finalResultTable[key] = resultTable[key];
                }

                return finalResultTable;
            }

            // handle collection types
            else if (input is System.Collections.IEnumerable && !(input is string))
            {
                var list = new List<object>();
                foreach (var element in (System.Collections.IEnumerable)input)
                {
                    list.Add(ConvertToHashTable(element));
                }

                return list.ToArray();
            }

            // handle other types
            else
            {
                return input;
            }
        }
    }
}