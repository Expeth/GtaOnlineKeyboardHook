using System.Threading.Tasks;
using GtaKeyboardHook.Model.Configuration;

namespace GtaKeyboardHook.Infrastructure.Configuration
{
    public interface IProfileConfigurationProvider
    {
        ProfileConfiguration GetConfig();
        Task LoadFromSourceAsync();
        void LoadFromSource();
        Task SaveAsync();
        void Save();
    }
}