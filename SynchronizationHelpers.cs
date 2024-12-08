using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace GenXdev.Helpers
{
    public static class SynchronizationHelpers
    {
        public static void ExecuteMachineWideMutuallyExclusive(string Id, ThreadStart Callback)
        {
            // get application GUID as defined in AssemblyInfo.cs
            Id = !String.IsNullOrWhiteSpace(Id) ? Id :
                ((GuidAttribute)Assembly.GetExecutingAssembly().
                    GetCustomAttributes(typeof(GuidAttribute), false).
                        GetValue(0)).Value.ToString();

            // unique id for global mutex - Global prefix means it is global to the machine
            Id = string.Format("Global\\{{{0}}}", Id);

            // Need a place to store a return value in Mutex() constructor call

            // edited by Jeremy Wiebe to add example of setting up security for multi-user usage
            // edited by 'Marc' to work also on localized systems (don't use just "Everyone") 
            var allowEveryoneRule =
                new MutexAccessRule(
                    new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    MutexRights.FullControl, AccessControlType.Allow);

            // edited by MasonGZhwiti to prevent race condition on security settings via VanNguyen
            using (var mutex = new Mutex(false, Id, out bool createdNew))
            {
                // edited by acidzombie24
                var hasHandle = false;
                try
                {
                    try
                    {
                        // note, you may want to time out here instead of waiting forever
                        // edited by acidzombie24
                        hasHandle = mutex.WaitOne(Timeout.Infinite, false);
                        //hasHandle = mutex.WaitOne(5000, false);
                        //if (hasHandle == false)
                        //    throw new TimeoutException("Timeout waiting for exclusive access");
                    }
                    catch (AbandonedMutexException)
                    {
                        // Log the fact that the mutex was abandoned in another process,
                        // it will still get accquired
                        hasHandle = true;
                    }

                    // Perform your work here.
                    Callback();
                }
                finally
                {
                    // edited by acidzombie24, added if statement
                    if (hasHandle)
                        mutex.ReleaseMutex();
                }
            }
        }
    }
}

