using System;
using System.Threading.Tasks;
using TigerApp.Shared.Models;

namespace TigerApp.Shared.Services.Platform
{
	public interface ILocationService
	{
		bool IsLocationEnabled { get; }
		Task<Location> CurrentLocationAsync();
	}

    public class GpsNotActiveException : Exception
    { 
    }

    public class GpsTimeoutException : Exception
    {
    }
}