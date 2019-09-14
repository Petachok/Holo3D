using HoloToolkitExtensions.RemoteAssets;
using System;
using System.Collections;
using UnityEngine;

public class MapTile : DynamicTextureDownloader
{
    public IMapUrlBuilder MapBuilder { get; set; }

    private TileInfo _tileData;

    public MapTile()
    {
        MapBuilder = MapBuilder != null ? MapBuilder : new OpenStreetMapTileBuilder();
    }

    public void SetTileData(TileInfo tiledata, bool forceReload = false)
    {
        if (_tileData == null || !_tileData.Equals(tiledata) || forceReload)
        {
            TileData = tiledata;
            StartLoadElevationDataFromWeb();
        }
    }

    public TileInfo TileData
    {
        get { return _tileData; }
        private set
        {
            _tileData = value;
            ImageUrl = MapBuilder.GetTileUrl(_tileData);
        }
    }

    private string _mapToken = "dgZxShxJlQAa5XwSLZ4a~R6IoStUNcPAG-QvS6irWvg~AnoGutfm5-qUdUb4J37n-Jk-DLeR5chbDno-yXYMUADN0h8lw7Tocwq5uEevHvdm";

    public bool IsDownloading { get; private set; }

    private WWW _downloader;

    private void StartLoadElevationDataFromWeb()
    {
        if (_tileData == null)
            return;

        var northEast = _tileData.GetNorthEast();
        var southWest = _tileData.GetSouthWest();

        var urlData = string.Format(
            "http://dev.virtualearth.net/REST/v1/Elevation/Bounds?bounds={0},{1},{2},{3}&rows=11&cols=11&key={4}",
            southWest.Lat, southWest.Lon, northEast.Lat, northEast.Lon, _mapToken);
        _downloader = new WWW(urlData);
        IsDownloading = true;
    }

    protected override void OnUpdate()
    {
        ProcessElevationDataFromWeb();
    }

    private void ProcessElevationDataFromWeb()
    {
        if (TileData == null || _downloader == null)
            return;

        if (IsDownloading && _downloader.isDone)
        {
            IsDownloading = false;

            var elevationData = JsonUtility.FromJson<ElevationResult>(_downloader.text);
            if (elevationData == null)
                return;
        }
    }
}