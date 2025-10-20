// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Misc.cs
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



using System.Speech.Synthesis;

namespace GenXdev.Helpers
{
    public static class Misc
    {
        public static SpeechSynthesizer SpeechCustomized = new SpeechSynthesizer();
        public static SpeechSynthesizer Speech = new SpeechSynthesizer();
    }
}