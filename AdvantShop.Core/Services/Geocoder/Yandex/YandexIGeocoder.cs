using System;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Modules.Interfaces;

namespace AdvantShop.Geocoder.Yandex
{
    public class YandexIGeocoder: IGeocoder
    {
        protected YandexGeocoderClient YandexGeocoderClient;
        
        public YandexIGeocoder() { }

        public YandexIGeocoder(string apiKey)
        {
            YandexGeocoderClient = new YandexGeocoderClient(apiKey, new Uri(SettingsMain.SiteUrl));
        }

        public YandexIGeocoder(string apiKey, Uri urlReferer)
        {
            YandexGeocoderClient = new YandexGeocoderClient(apiKey, urlReferer);
        }
        
        public Core.Modules.Interfaces.GeocoderMetaData Geocode(string address)
        {
            if (YandexGeocoderClient is null)
                return default;
            
            var geocodeResult = YandexGeocoderClient.Geocode(new GeocodeParams
            {
                Geocode = address,
                Results = 1,
            }, out _);
            
            
            if (!(geocodeResult?.Response.GeoObjectCollection.MetaDataProperty.GeocoderResponseMetaData.Found > 0))
                return default;

            var featureMember = geocodeResult.Response.GeoObjectCollection.FeatureMember.First();

            var result = new Core.Modules.Interfaces.GeocoderMetaData
            {
                Point = new Core.Modules.Interfaces.Point(
                    featureMember.GeoObject.Point.Pos.Longitude,
                    featureMember.GeoObject.Point.Pos.Latitude)
            };
            
            if (featureMember.GeoObject.BoundedBy?.Envelope != null)
            {
                result.BoundedBy = new Core.Modules.Interfaces.BoundedBy()
                {
                    LowerCorner = new Core.Modules.Interfaces.Point(
                        featureMember.GeoObject.BoundedBy.Envelope.LowerCorner.Longitude,
                        featureMember.GeoObject.BoundedBy.Envelope.LowerCorner.Latitude),
                    UpperCorner = new Core.Modules.Interfaces.Point(
                        featureMember.GeoObject.BoundedBy.Envelope.UpperCorner.Longitude,
                        featureMember.GeoObject.BoundedBy.Envelope.UpperCorner.Latitude),
                };
            }

            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Precision != null)
                result.Precision = GetPrecision(featureMember);

            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Kind != null)
            {
                result.Kind = GetKind(featureMember);

                if (result.Kind == Core.Modules.Interfaces.Kind.Other) 
                    result.Kind = GetKindByAddressComponents(featureMember) ?? result.Kind;
            }
            
            return result;
        }

        private Core.Modules.Interfaces.Precision GetPrecision(FeatureMember featureMember)
        {
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Precision == Precision.Exact)
                return Core.Modules.Interfaces.Precision.Exact;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Precision == Precision.Number)
                return Core.Modules.Interfaces.Precision.Number;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Precision == Precision.Near)
                return Core.Modules.Interfaces.Precision.Near;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Precision == Precision.Range)
                return Core.Modules.Interfaces.Precision.Range;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Precision == Precision.Street)
                return Core.Modules.Interfaces.Precision.Street;
                
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Precision == Precision.Other)
                return Core.Modules.Interfaces.Precision.Locality;

            return Core.Modules.Interfaces.Precision.None;
        }

        private Core.Modules.Interfaces.Kind GetKind(FeatureMember featureMember)
        {
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Kind == Kind.Entrance)
                return Core.Modules.Interfaces.Kind.Entrance;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Kind == Kind.House)
                return Core.Modules.Interfaces.Kind.House;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Kind == Kind.Street)
                return Core.Modules.Interfaces.Kind.Street;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Kind == Kind.Metro)
                return Core.Modules.Interfaces.Kind.Metro;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Kind == Kind.District)
                return Core.Modules.Interfaces.Kind.District;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Kind == Kind.Locality)
                return Core.Modules.Interfaces.Kind.Locality;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Kind == Kind.Area)
                return Core.Modules.Interfaces.Kind.Area;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Kind == Kind.Province)
                return Core.Modules.Interfaces.Kind.Province;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Kind == Kind.Country)
                return Core.Modules.Interfaces.Kind.Country;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Kind == Kind.Hydro)
                return Core.Modules.Interfaces.Kind.Hydro;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Kind == Kind.RailwayStation)
                return Core.Modules.Interfaces.Kind.RailwayStation;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Kind == Kind.Route)
                return Core.Modules.Interfaces.Kind.Route;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Kind == Kind.Vegetation)
                return Core.Modules.Interfaces.Kind.Vegetation;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Kind == Kind.Airport)
                return Core.Modules.Interfaces.Kind.Airport;
            
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Kind == Kind.Other)
                return Core.Modules.Interfaces.Kind.Other;

            return Core.Modules.Interfaces.Kind.None;
        }

        private Core.Modules.Interfaces.Kind? GetKindByAddressComponents(FeatureMember featureMember)
        {
            Core.Modules.Interfaces.GeocoderMetaData result;
            if (featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Address?.Components?.Any() is true)
            {
                var addressComponents = featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Address.Components;

                bool existsEntrance = false, existsHouse = false;
                foreach (var addressComponent in addressComponents)
                {
                    if (addressComponent.Kind == AKind.Entrance)
                        existsEntrance = true;
                    else if (addressComponent.Kind == AKind.House)
                        existsHouse = true;
                }
                    
                if (existsEntrance)
                    return Core.Modules.Interfaces.Kind.Entrance;
                if (existsHouse)
                    return Core.Modules.Interfaces.Kind.House;
            }

            return null;
        }

        public ReverseGeocoderData ReverseGeocode(Core.Modules.Interfaces.Point point)
        {
            if (YandexGeocoderClient is null)
                return default;
            
            var geocodeResult = YandexGeocoderClient.Geocode(new GeocodeParams
            {
                Geocode = $"{point.Longitude.ToInvatiant()},{point.Latitude.ToInvatiant()}",
                Sco = Sco.Longlat,
                Kind = GeocodeParamsKind.House,
                Results = 1,
            }, out _);
            
            
            if (!(geocodeResult?.Response.GeoObjectCollection.MetaDataProperty.GeocoderResponseMetaData.Found > 0))
                return default;

            var featureMember = geocodeResult.Response.GeoObjectCollection.FeatureMember
                                             .First(x =>
                                                  x?.GeoObject?.MetaDataProperty?.GeocoderMetaData?.Address != null);

            var address = new ReverseGeocoderAddress
            {
                AddressString = featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Text,
                Zip = featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Address.PostalCode,
            };
            
            foreach (var addressComponent in featureMember.GeoObject.MetaDataProperty.GeocoderMetaData.Address.Components)
            {
                if (addressComponent.Kind == AKind.Country)
                    address.Country = addressComponent.Name;
                else if (addressComponent.Kind == AKind.Province) 
                    address.Region = addressComponent.Name;
                // else if (addressComponent.Kind == AKind.Area) 
                //     address.District = addressComponent.Name;
                else if (addressComponent.Kind == AKind.Locality) 
                    address.City = addressComponent.Name
                                                   .Replace("посёлок городского типа", string.Empty)
                                                   .Replace("посёлок", string.Empty)
                                                   .Replace("село", string.Empty)
                                                   .Replace("деревня", string.Empty)
                                                   .Trim();
                else if (addressComponent.Kind == AKind.Street) 
                    address.Street = addressComponent.Name;
                else if (addressComponent.Kind == AKind.House) 
                    address.House = addressComponent.Name;
            }

            return new ReverseGeocoderData
            {
                Address = address
            };
        }
    }
}