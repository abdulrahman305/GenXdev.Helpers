// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Misc.cs
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



using System.Speech.Synthesis;

namespace GenXdev.Helpers
{

    /// <summary>
    /// <para type="synopsis">
    /// Provides static instances of SpeechSynthesizer for text-to-speech functionality in GenXdev Helpers.
    /// </para>
    ///
    /// <para type="description">
    /// The Misc class contains pre-initialized SpeechSynthesizer instances that can be used throughout
    /// the GenXdev.Helpers module for speech synthesis operations. This class serves as a utility
    /// for managing speech-related functionality without requiring individual cmdlet classes to
    /// instantiate their own synthesizers.
    /// </para>
    ///
    /// <para type="description">
    /// This class is designed as a static utility class, providing shared resources for speech
    /// synthesis that can be accessed across different parts of the module.
    /// </para>
    /// </summary>
    public static class Misc
    {

        /// <summary>
        /// A customized instance of SpeechSynthesizer that may have specific settings or configurations
        /// applied for enhanced speech synthesis capabilities.
        /// </summary>
        public static SpeechSynthesizer SpeechCustomized = new SpeechSynthesizer();

        /// <summary>
        /// A standard instance of SpeechSynthesizer for basic text-to-speech functionality.
        /// </summary>
        public static SpeechSynthesizer Speech = new SpeechSynthesizer();
    }
}