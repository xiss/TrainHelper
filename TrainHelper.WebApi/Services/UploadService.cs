﻿using AutoMapper;
using System.Xml;
using System.Xml.Serialization;
using TrainHelper.DAL.Entities;
using TrainHelper.DAL.Providers;
using TrainHelper.WebApi.Dto;

namespace TrainHelper.WebApi.Services;

public class UploadService : IUploadService
{
    private readonly IMapper _mapper;
    private readonly ITrainDataProvider _trainDataProvider;

    public UploadService(ITrainDataProvider trainDataProvider, IMapper mapper)
    {
        _trainDataProvider = trainDataProvider;
        _mapper = mapper;
    }

    /// <summary>
    /// Upload data to database
    /// </summary>
    /// <param name="uploadFile"></param>
    public async Task<UploadDataResultDto> UploadData(UploadFileDto uploadFile)
    {
        var result = new UploadDataResultDto();
        try
        {
            var data = GetUploadDataTrainDto(uploadFile.Stream);
            if (data != null)
            {
                result = await UploadData(data);
            }
            else
            {
                result.Errors.Add($"{uploadFile.FileName} is empty");
            }
        }
        catch (Exception e)
        {
            result.Errors.Add($"Error appear while attempting to parse {uploadFile.FileName}: {e.Message}");
        }

        return result;
    }

    private static UploadDataTrainDto? GetUploadDataTrainDto(Stream stream)
    {
        var xmlReader = XmlReader.Create(stream);
        var serializer = new XmlSerializer(typeof(UploadDataTrainDto));
        return (UploadDataTrainDto?)serializer.Deserialize(xmlReader);
    }

    private async Task<UploadDataResultDto> UploadData(UploadDataTrainDto data)
    {
        var result = new UploadDataResultDto { IsSuccessful = true };

        foreach (var item in data.RowField)
        {
            var train = _mapper.Map<Train>(item);
            var car = _mapper.Map<Car>(item);
            var wayPoint = _mapper.Map<WayPoint>(item);
            wayPoint.Car = car;
            wayPoint.Station = new Station { StationName = item.LastStationName };
            car.WayPoints.Add(wayPoint);
            car.FromStation = new Station { StationName = item.FromStationName };
            car.ToStation = new Station { StationName = item.ToStationName };
            train.Cars.Add(car);

            if (await _trainDataProvider.UpsertTrain(train))
            {
                result.TotalItemsUploaded++;
            }
            else
            {
                result.IsSuccessful = false;
                result.Errors.Add($"Error with import car number: {item.CarNumber}");
            }
        }

        return result;
    }
}