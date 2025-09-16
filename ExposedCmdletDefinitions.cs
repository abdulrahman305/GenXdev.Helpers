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
