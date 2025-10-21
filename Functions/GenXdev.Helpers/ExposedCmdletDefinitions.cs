// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : ExposedCmdletDefinitions.cs
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



using System.Collections;

namespace GenXdev.Helpers
{
    public class ExposedToolCallInvocationError
    {
        public string error { get; set; } = null;
        public bool exceptionThrown { get; set; } = false;
        public string exceptionClass { get; set; } = null;
    }

    public class ExposedToolCallInvocationResult
    {
        public bool CommandExposed { get; set; } = false;
        public string Reason { get; set; } = null;
        public string Output { get; set; } = null;
        public string FullName { get; set; } = null;
        public string OutputType { get; set; } = null;
        public System.Collections.Hashtable UnfilteredArguments { get; set; } = new Hashtable();
        public System.Collections.Hashtable FilteredArguments { get; set; } = new Hashtable();
        public ExposedCmdletDefinition ExposedCmdLet { get; set; } = null;
        public string Error { get; set; } = null;
    }

    public class ExposedForcedCmdLetParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
    public class ExposedCmdletDefinition
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> AllowedParams { get; set; } = new List<string>();
        public List<string> DontShowDuringConfirmationParamNames { get; set; } = new List<string>();
        public List<ExposedForcedCmdLetParameter> ForcedParams { get; set; } = new List<ExposedForcedCmdLetParameter>();
        public int JsonDepth { get; set; } = 2;
        public bool OutputText { get; set; } = true;
        public bool Confirm { get; set; } = true;
    }
}
