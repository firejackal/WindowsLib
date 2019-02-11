using System.Security.Principal;
using System.Security.Permissions;

namespace WindowsLib
{
    public static class SecurityTools
    {
        public static bool IsUserAdministrator()
        {
            try {
                // Allows to request the principal permission.
                System.AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

                //if(!isAdmin) {
                //    //PrincipalPermission p = new PrincipalPermission(null, "BUILTIN\\Administrators");
                //    //p.Demand();
                //    AccessRights();
                //    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                //}
                return isAdmin;
            } catch {
                return false;
            }
        } //IsUserAdministrator Function


        //[PrincipalPermissionAttribute(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
        //private static void AccessRights()
        //{
        //    // do nothing
        //} //IsUserAdministrator Function
    } //SecurityTools Class
} //Windows Namespace
