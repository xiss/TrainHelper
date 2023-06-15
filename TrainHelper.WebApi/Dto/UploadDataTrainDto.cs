using System.ComponentModel;
using System.Xml.Serialization;

namespace TrainHelper.WebApi.Dto;

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", IsNullable = false, ElementName = "Root")]
public class UploadDataTrainDto
{
    [XmlElement("row")]
    public Item[] RowField { get; set; } = Array.Empty<Item>();

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class Item
    {
        public int CarNumber { get; set; }
        public string FreightEtsngName { get; set; } = null!;
        public uint FreightTotalWeightKg { get; set; }
        public string FromStationName { get; set; } = null!;
        public string InvoiceNum { get; set; } = null!;
        public string LastOperationName { get; set; } = null!;
        public string LastStationName { get; set; } = null!;
        public byte PositionInTrain { get; set; }
        public string ToStationName { get; set; } = null!;
        public string TrainIndexCombined { get; set; } = null!;
        public int TrainNumber { get; set; }

        [XmlIgnore]
        public DateTimeOffset WhenLastOperation { get; set; }

        [XmlElement("WhenLastOperation")]
        public string WhenLastOperationForXml // format: 2011-11-11T15:05:46.4733406+01:00
        {
            get => WhenLastOperation.ToString("o");  // o = yyyy-MM-ddTHH:mm:ss.fffffffzzz
            set => WhenLastOperation = DateTimeOffset.Parse(value);
        }
    }
}