using AutoMapper;

namespace DND.Common.Infrastructure.Interfaces.Automapper
{
    public interface IHaveCustomMappings
    {
        void CreateMappings(IMapperConfigurationExpression configuration);
    }
}
