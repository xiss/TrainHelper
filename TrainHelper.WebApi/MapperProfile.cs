using AutoMapper;
using TrainHelper.DAL.Entities;
using TrainHelper.WebApi.Dto;

namespace TrainHelper.WebApi;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        // Car
        CreateMap<UploadDataTrainDto.Item, Car>()
            .ForMember(c => c.CarNumber, m => m.MapFrom(s => s.CarNumber))
            .ForMember(d => d.Id, m => m.Ignore())
            .ForMember(d => d.Train, m => m.Ignore())
            .ForMember(d => d.Invoice, m => m.MapFrom(s => s))
            .ForMember(d => d.FromStation, m => m.Ignore())
            .ForMember(d => d.Freight, m => m.MapFrom(s => s))
            .ForMember(d => d.WayPoints, m => m.Ignore())
            .ForMember(d => d.ToStation, m => m.Ignore());
        CreateMap<Car, CarDto>()
            .ForMember(d => d.InvoiceNumber, m => m.MapFrom(s => s.Invoice.InvoiceNum))
            .ForMember(d => d.FreightEtsngName, m => m.MapFrom(s => s.Freight.FreightEtsngName))
            .ForMember(d => d.WhenLastOperation, m => m.MapFrom(s => s.WayPoints.Max(w => w.OperationDate)))
            .ForMember(d => d.FreightTotalWeightTn, m => m.MapFrom(s => (double)s.FreightTotalWeightKg / 1000))
            .ForMember(d => d.LastOperationName, m => m.MapFrom(s => s.WayPoints.OrderByDescending(w => w.OperationDate).First().Operation.OperationName)); //TODO

        // Invoice
        CreateMap<UploadDataTrainDto.Item, Invoice>()
            .ForMember(d => d.Id, m => m.Ignore());
        CreateMap<Invoice, InvoiceDto>();

        // Train
        CreateMap<UploadDataTrainDto.Item, Train>()
            .ForMember(d => d.Number, m => m.MapFrom(s => s.TrainNumber))
            .ForMember(d => d.Cars, m => m.MapFrom(s => new List<Car>()))
            .ForMember(d => d.Id, m => m.Ignore());
        CreateMap<Train, TrainDto>()
            .ForMember(d => d.FromStation, m => m.Ignore())
            .ForMember(d => d.ToStation, m => m.Ignore())
            .ForMember(d => d.LastStation, m => m.Ignore())
            .ForMember(d => d.WhenLastOperation, m => m.Ignore())
            .ForMember(d => d.LastOperationName, m => m.Ignore());

        // WayPoint
        CreateMap<UploadDataTrainDto.Item, WayPoint>()
            .ForMember(d => d.OperationDate, m => m.MapFrom(s => s.WhenLastOperation.UtcDateTime))
            .ForMember(d => d.Operation, m => m.MapFrom(s => s))
            .ForMember(d => d.Id, m => m.Ignore())
            .ForMember(d => d.Station, m => m.Ignore())
            .ForMember(d => d.Car, m => m.Ignore());

        // Station
        CreateMap<Station, StationDto>();

        // User
        CreateMap<User, UserDto>();

        // Freight
        CreateMap<UploadDataTrainDto.Item, Freight>()
            .ForMember(d => d.Id, m => m.Ignore());

        // CarOperation
        CreateMap<UploadDataTrainDto.Item, CarOperation>()
            .ForMember(d => d.OperationName, m => m.MapFrom(d => d.LastOperationName))
            .ForMember(d => d.Id, m => m.Ignore());

        // User
        CreateMap<CreateUserDto, User>()
            .ForMember(d => d.Id, m => m.Ignore())
            .ForMember(d => d.PasswordHash, m => m.Ignore());
    }
}