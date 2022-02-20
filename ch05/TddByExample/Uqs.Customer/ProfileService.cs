using System.Text.RegularExpressions;

namespace Uqs.Customer;

public class ProfileService
{
    private const string ALPHANUMERIC_UNDERSCORE_REGEX = @"^[a-zA-Z0-9_]+$";
    private static readonly Regex _formatRegex = new (ALPHANUMERIC_UNDERSCORE_REGEX, RegexOptions.Compiled);
    public void ChangeUsername(string username)
    {
        if (username == null)
        {
            throw new ArgumentNullException(nameof(username), "Null");
        }
        if (username.Length is < 8 or > 12) 
        {
            throw new ArgumentOutOfRangeException(nameof(username), "Length");
        }
        if (!_formatRegex.Match(username).Success)
        {
            throw new ArgumentOutOfRangeException(nameof(username), "InvalidChar");
        }
    }
}