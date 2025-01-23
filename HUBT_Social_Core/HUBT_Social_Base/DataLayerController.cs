using AutoMapper;
using HUBT_Social_Core;
using HUBT_Social_Core.Settings;
using Microsoft.Extensions.Options;


namespace HUBT_Social_Base
{
    public class DataLayerController(IMapper mapper, IOptions<JwtSetting> jwtSettings) : CoreController
    {
        protected readonly IMapper _mapper = mapper;
        protected readonly JwtSetting _jwtSetting = jwtSettings.Value;
    }
}
