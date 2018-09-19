namespace DND.Common.Infrastrucutre.Interfaces.Domain.Dtos
{
    public interface IDto : IObjectValidatable
    {

    }

    public interface IDtoWithId : IDto
    {
        object Id { get; set; }
    }

    public interface IDto<T> : IDtoWithId
    {
        new T Id { get; set; }
    }
}
