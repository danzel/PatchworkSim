using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace PatchworkWebRunner;

/// <summary>
/// Program
/// </summary>
public class Program
{
	/// <summary>
	/// Startup method
	/// </summary>
	/// <param name="args"></param>
	public static void Main(string[] args)
	{
		CreateWebHostBuilder(args).Build().Run();
	}

	/// <summary>
	/// Create the WebHostBuilder
	/// </summary>
	public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
		WebHost.CreateDefaultBuilder(args)
			.UseStartup<Startup>();
}
