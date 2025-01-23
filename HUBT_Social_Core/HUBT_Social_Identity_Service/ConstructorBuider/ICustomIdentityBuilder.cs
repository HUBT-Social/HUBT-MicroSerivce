using HUBT_Social_Core.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Identity_Service.ConstructorBuider
{
    public interface ICustomIdentityBuilder
    {
        IServiceCollection Services { get; }
        DatabaseSetting DatabaseSetting { get; }

    }
}
