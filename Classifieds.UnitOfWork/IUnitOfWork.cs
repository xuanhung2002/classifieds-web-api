using Classifieds.Repository.Interface;

namespace Classifieds.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
    }
}
