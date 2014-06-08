using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomTomMapsAPITest.TomTom
{
	public class Status
	{
		public string status { get; set; }
		public string info { get; set; }
	}

	public class ResultStatusList
	{
		public List<Status> status { get; set; }
	}

	public class GeoResult
	{
		public double latitude { get; set; }
		public double longitude { get; set; }
		public string geohash { get; set; }
		public string mapName { get; set; }
		public string houseNumber { get; set; }
		public string type { get; set; }
		public string street { get; set; }
		public List<object> alternativeStreetName { get; set; }
		public string city { get; set; }
		public string state { get; set; }
		public string country { get; set; }
		public string countryISO3 { get; set; }
		public string postcode { get; set; }
		public string standardPostalCode { get; set; }
		public string formattedAddress { get; set; }
		public string cbsaCode { get; set; }
		public string censusBlock { get; set; }
		public string censusTract { get; set; }
		public string censusStateCode { get; set; }
		public string censusFipsCountyCode { get; set; }
		public string censusFipsMinorCivilDivision { get; set; }
		public string censusFipsPlaceCode { get; set; }
		public bool isCensusMicropolitanFlag { get; set; }
		public int widthMeters { get; set; }
		public int heightMeters { get; set; }
		public double score { get; set; }
		public double confidence { get; set; }
		public int iteration { get; set; }
		public string userTag { get; set; }
	}

	public class GeoResponse
	{
		public string svnRevision { get; set; }
		public int count { get; set; }
		public string version { get; set; }
		public string duration { get; set; }
		public string debugInformation { get; set; }
		public string consolidatedMaps { get; set; }
		public List<ResultStatusList> resultStatusList { get; set; }
		public List<GeoResult> geoResult { get; set; }
	}

	public class GeoCodingResponse
	{
		public GeoResponse geoResponse { get; set; }
	}
}