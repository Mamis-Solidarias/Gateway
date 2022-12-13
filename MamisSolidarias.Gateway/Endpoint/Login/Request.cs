namespace MamisSolidarias.Gateway.Endpoint.Login;

public class Request
{
    /// <summary>
    ///     User's email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    ///     User's password. Must have at least one uppercase, lowercase and number character
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
