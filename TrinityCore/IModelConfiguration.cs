namespace TrinityCore
{
    public interface IModelConfiguration
    {
        void SetModelConfiguration(IModelBase model);

        void MergeModelConfiguration(IModelConfiguration configuration);
        void Validate();
    }
}