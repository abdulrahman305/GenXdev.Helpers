<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : Get-WebLanguageDictionary.ps1
Original author           : René Vaessen / GenXdev
Version                   : 1.286.2025
################################################################################
MIT License

Copyright 2021-2025 GenXdev

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
################################################################################>
###############################################################################

<#
.SYNOPSIS
Returns a reversed dictionary for all languages supported by Google Search

.DESCRIPTION
Returns a reversed dictionary for all languages supported by Google Search
#>
function Get-WebLanguageDictionary {

    $result = Microsoft.PowerShell.Utility\New-Object System.Collections.Generic.Dictionary"[String, string]"
    $result['Afrikaans'] = 'af';
    $result['Akan'] = 'ak';
    $result['Albanian'] = 'sq';
    $result['Amharic'] = 'am';
    $result['Arabic'] = 'ar';
    $result['Armenian'] = 'hy';
    $result['Azerbaijani'] = 'az';
    $result['Basque'] = 'eu';
    $result['Belarusian'] = 'be';
    $result['Bemba'] = 'bem';
    $result['Bengali'] = 'bn';
    $result['Bihari'] = 'bh';
    $result['Bork, bork, bork!'] = 'xx-bork';
    $result['Bosnian'] = 'bs';
    $result['Breton'] = 'br';
    $result['Bulgarian'] = 'bg';
    $result['Cambodian'] = 'km';
    $result['Catalan'] = 'ca';
    $result['Cherokee'] = 'chr';
    $result['Chichewa'] = 'ny';
    $result['Chinese (Simplified)'] = 'zh-CN';
    $result['Chinese (Traditional)'] = 'zh-TW';
    $result['Corsican'] = 'co';
    $result['Croatian'] = 'hr';
    $result['Czech'] = 'cs';
    $result['Danish'] = 'da';
    $result['Dutch'] = 'nl';
    $result['Elmer Fudd'] = 'xx-elmer';
    $result['English'] = 'en';
    $result['Esperanto'] = 'eo';
    $result['Estonian'] = 'et';
    $result['Ewe'] = 'ee';
    $result['Faroese'] = 'fo';
    $result['Filipino'] = 'tl';
    $result['Finnish'] = 'fi';
    $result['French'] = 'fr';
    $result['Frisian'] = 'fy';
    $result['Ga'] = 'gaa';
    $result['Galician'] = 'gl';
    $result['Georgian'] = 'ka';
    $result['German'] = 'de';
    $result['Greek'] = 'el';
    $result['Guarani'] = 'gn';
    $result['Gujarati'] = 'gu';
    $result['Hacker'] = 'xx-hacker';
    $result['Haitian Creole'] = 'ht';
    $result['Hausa'] = 'ha';
    $result['Hawaiian'] = 'haw';
    $result['Hebrew'] = 'iw';
    $result['Hindi'] = 'hi';
    $result['Hungarian'] = 'hu';
    $result['Icelandic'] = 'is';
    $result['Igbo'] = 'ig';
    $result['Indonesian'] = 'id';
    $result['Interlingua'] = 'ia';
    $result['Irish'] = 'ga';
    $result['Italian'] = 'it';
    $result['Japanese'] = 'ja';
    $result['Javanese'] = 'jw';
    $result['Kannada'] = 'kn';
    $result['Kazakh'] = 'kk';
    $result['Kinyarwanda'] = 'rw';
    $result['Kirundi'] = 'rn';
    $result['Klingon'] = 'xx-klingon';
    $result['Kongo'] = 'kg';
    $result['Korean'] = 'ko';
    $result['Krio (Sierra Leone)'] = 'kri';
    $result['Kurdish'] = 'ku';
    $result['Kurdish (Soranî)'] = 'ckb';
    $result['Kyrgyz'] = 'ky';
    $result['Laothian'] = 'lo';
    $result['Latin'] = 'la';
    $result['Latvian'] = 'lv';
    $result['Lingala'] = 'ln';
    $result['Lithuanian'] = 'lt';
    $result['Lozi'] = 'loz';
    $result['Luganda'] = 'lg';
    $result['Luo'] = 'ach';
    $result['Macedonian'] = 'mk';
    $result['Malagasy'] = 'mg';
    $result['Malay'] = 'ms';
    $result['Malayalam'] = 'ml';
    $result['Maltese'] = 'mt';
    $result['Maori'] = 'mi';
    $result['Marathi'] = 'mr';
    $result['Mauritian Creole'] = 'mfe';
    $result['Moldavian'] = 'mo';
    $result['Mongolian'] = 'mn';
    $result['Montenegrin'] = 'sr-ME';
    $result['Nepali'] = 'ne';
    $result['Nigerian Pidgin'] = 'pcm';
    $result['Northern Sotho'] = 'nso';
    $result['Norwegian'] = 'no';
    $result['Norwegian (Nynorsk)'] = 'nn';
    $result['Occitan'] = 'oc';
    $result['Oriya'] = 'or';
    $result['Oromo'] = 'om';
    $result['Pashto'] = 'ps';
    $result['Persian'] = 'fa';
    $result['Pirate'] = 'xx-pirate';
    $result['Polish'] = 'pl';
    $result['Portuguese (Brazil)'] = 'pt-BR';
    $result['Portuguese (Portugal)'] = 'pt-PT';
    $result['Punjabi'] = 'pa';
    $result['Quechua'] = 'qu';
    $result['Romanian'] = 'ro';
    $result['Romansh'] = 'rm';
    $result['Runyakitara'] = 'nyn';
    $result['Russian'] = 'ru';
    $result['Scots Gaelic'] = 'gd';
    $result['Serbian'] = 'sr';
    $result['Serbo-Croatian'] = 'sh';
    $result['Sesotho'] = 'st';
    $result['Setswana'] = 'tn';
    $result['Seychellois Creole'] = 'crs';
    $result['Shona'] = 'sn';
    $result['Sindhi'] = 'sd';
    $result['Sinhalese'] = 'si';
    $result['Slovak'] = 'sk';
    $result['Slovenian'] = 'sl';
    $result['Somali'] = 'so';
    $result['Spanish'] = 'es';
    $result['Spanish (Latin American)'] = 'es-419';
    $result['Sundanese'] = 'su';
    $result['Swahili'] = 'sw';
    $result['Swedish'] = 'sv';
    $result['Tajik'] = 'tg';
    $result['Tamil'] = 'ta';
    $result['Tatar'] = 'tt';
    $result['Telugu'] = 'te';
    $result['Thai'] = 'th';
    $result['Tigrinya'] = 'ti';
    $result['Tonga'] = 'to';
    $result['Tshiluba'] = 'lua';
    $result['Tumbuka'] = 'tum';
    $result['Turkish'] = 'tr';
    $result['Turkmen'] = 'tk';
    $result['Twi'] = 'tw';
    $result['Uighur'] = 'ug';
    $result['Ukrainian'] = 'uk';
    $result['Urdu'] = 'ur';
    $result['Uzbek'] = 'uz';
    $result['Vietnamese'] = 'vi';
    $result['Welsh'] = 'cy';
    $result['Wolof'] = 'wo';
    $result['Xhosa'] = 'xh';
    $result['Yiddish'] = 'yi';
    $result['Yoruba'] = 'yo';
    $result['Zulu'] = 'zu';

    return $result;
}