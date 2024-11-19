using System.Security.Claims;


namespace GoldRogerServer.Helpers
{
    public class SessionHelper
    {
        public static int GetTokenUserId(ClaimsPrincipal user)
        {
            Claim? userIdClaim = user.FindFirst("UserId");
            if (userIdClaim == null)
                return 0;
            int userId = 0;
            bool isValid = int.TryParse(userIdClaim.Value, out userId);
            return isValid ? userId : 0;
        }

        public static string GetTokenUserName(ClaimsPrincipal user)
        {
            Claim? userNameClaim = user.FindFirst("Username");
            if (userNameClaim == null)
                return "";
            return userNameClaim.Value;
        }

        public static int GetTokenUserType(ClaimsPrincipal user)
        {
            Claim? userTypeClaim = user.FindFirst("UserType");
            if (userTypeClaim == null)
                return 0;
            int userType = 0;
            bool isValid = int.TryParse(userTypeClaim.Value, out userType);
            return isValid ? userType : 0;
        }
    }
}