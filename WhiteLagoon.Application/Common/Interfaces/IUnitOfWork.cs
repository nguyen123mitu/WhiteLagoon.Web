namespace WhiteLagoon.Application.Common.Interfaces;

public interface IUnitOfWork
{
        IVillaRepository Villa { get; }
        IVillaNumberRepository VillaNumber { get; }
        IAmentityRepository Amenity { get; }
        void Save();
}
