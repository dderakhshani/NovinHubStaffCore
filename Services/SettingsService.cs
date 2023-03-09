using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovinDevHubStaffCore.Services
{
    public sealed class SettingsService : ISettingsService
    {
        /// <summary>
        /// The <see cref="IPropertySet"/> with the settings targeted by the current instance.
        /// </summary>
    

        /// <inheritdoc/>
        public void SetValue<T>(string key, T value)
        {
            
        }

        /// <inheritdoc/>
        public T GetValue<T>(string key)
        {
            

            return default;
        }
    }
}
