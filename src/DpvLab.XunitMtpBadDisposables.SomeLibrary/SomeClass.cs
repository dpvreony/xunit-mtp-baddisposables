namespace DpvLab.XunitMtpBadDisposables.SomeLibrary
{
    public sealed record SomeClass(int Number, SomeRandomDisposable thingThatWillFailInRunner);
}
