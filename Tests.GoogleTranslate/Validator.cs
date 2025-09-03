using Apps.GoogleTranslate.Connections;
using Blackbird.Applications.Sdk.Common.Authentication;
using Tests.GoogleTranslate.Base;

namespace Tests.GoogleTranslate;

[TestClass]
public class Validator : TestBase
{
    private ConnectionValidator _validator => new();
    [TestMethod]
    public async Task ValidatesCorrectConnection()
    {
        var result = await _validator.ValidateConnection(Creds, CancellationToken.None);

        Console.WriteLine(result.Message);
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public async Task DoesNotValidateIncorrectConnection()
    {
        var newCreds = Creds.Select(x => new AuthenticationCredentialsProvider(x.KeyName + "_incorrect", x.Value + "_incorrect"));

        var result = await _validator.ValidateConnection(newCreds, CancellationToken.None);

        Console.WriteLine(result.Message);
        Assert.IsFalse(result.IsValid);
    }
}