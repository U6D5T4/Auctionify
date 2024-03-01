using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Auctionify.Core.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum AnalyticReportPeriod
	{
		Day = 1,
		Week,
		Month,
		Year,
		Total
	}
}
