// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : RefactorSetDefinition.cs
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



namespace GenXdev.Helpers
{

    public class RefactorDefinition
    {
        public string Name { get; set; }
        public int Priority { get; set; }
        public RefactorSettings RefactorSettings { get; set; } = new RefactorSettings();
        public SelectionSettings SelectionSettings { get; set; } = new SelectionSettings();
        public List<RefactorLogItem> Log { get; set; } = new List<RefactorLogItem>();
        public RefactorState State { get; set; } = new RefactorState();
    }

    public class RefactorSettings
    {
        public string PromptKey { get; set; }
        public string Prompt { get; set; }
        public List<string> KeysToSend { get; set; } = new List<string>();
        public int Code { get; set; } = -1;
        public int VisualStudio { get; set; } = -1;
    }

    public class SelectionSettings
    {
        public string Script { get; set; }
        public bool AutoAddModifiedFiles { get; set; }

        public RefactorSelectionLLMSettings LLM { get; set; } = new RefactorSelectionLLMSettings();
    }

    public class RefactorSelectionLLMSettings
    {
        public string Prompt { get; set; }
        public string Model { get; set; }
        public string HuggingFaceIdentifier { get; set; }
        public double Temperature { get; set; }
        public int MaxToken { get; set; } = -1;
        public int TTLSeconds { get; set; } = 0;
        public double Gpu { get; set; }
        public int Cpu { get; set; } // Added: Number of CPU cores to dedicate
        public int TimeoutSeconds { get; set; } // Added: Timeout for AI operations
        public bool SelectByFreeRam { get; set; } // Added: Select config by system RAM
        public bool SelectByFreeGpuRam { get; set; } // Added: Select config by GPU RAM
        public string LLMQueryType { get; set; } = "SimpleIntelligence"; // Added: Type of LLM query
        public bool NoSupportForJsonSchema { get; set; } // Added: LLM doesn't support JSON schema
        public bool NoSupportForImageUpload { get; set; } // Added: LLM doesn't support image uploads
        public bool NoSupportForToolCalls { get; set; } // Added: LLM doesn't support tool calls
        public bool Force { get; set; }
        public string ApiEndpoint { get; set; }
        public string ApiKey { get; set; }
        public List<ExposedCmdletDefinition> ExposedCmdlets { get; set; } = new List<ExposedCmdletDefinition>();
    }

    public class RefactorLogItem
    {
        public System.DateTime Timestamp { get; set; } = System.DateTime.UtcNow;
        public string Message { get; set; } = "";
    }

    public class RefactorState
    {
        public string Status { get; set; } = "Definition Created";
        public System.DateTime LastUpdated { get; set; } = System.DateTime.UtcNow;
        public System.DateTime? LastRefactoring { get; set; }
        public int PercentageComplete { get; set; }
        public int RefactoredIndex { get; set; } = -1;
        public int SelectedIndex { get; set; } = -1;
        public int UnselectedIndex { get; set; } = -1;
        public List<string> Selected { get; set; } = new List<string>();
        public List<string> Refactored { get; set; } = new List<string>();
        public List<string> Unselected { get; set; } = new List<string>();
    }
}
