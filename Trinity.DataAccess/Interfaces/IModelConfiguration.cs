

namespace Trinity.DataAccess.Interfaces
{
    public interface IModelConfiguration
    {
        void SetModelConfiguration(IModelBase model);

        void MergeModelConfiguration(IModelConfiguration configuration);
        void Validate();
    }
}