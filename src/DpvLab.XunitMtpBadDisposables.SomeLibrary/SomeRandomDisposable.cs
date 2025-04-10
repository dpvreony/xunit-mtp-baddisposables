namespace DpvLab.XunitMtpBadDisposables.SomeLibrary
{
    public sealed class SomeRandomDisposable : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
