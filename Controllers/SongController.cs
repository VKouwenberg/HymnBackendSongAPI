using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace HymnSongAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SongController : ControllerBase
	{
		private readonly string _musicDirectory = "C:/Fontys ICT/Semester 3/IPS Individueel Project/Music/";
		
		[HttpGet("GetSongByName")]
		public async Task<IActionResult> GetSongByName([FromQuery] string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				return BadRequest("File name is required.");
			}

			string[] allowedExtensions = { ".mp3", ".flac", ".wav" };
			List<string> matchingFiles = Directory.GetFiles(_musicDirectory)
				.Where(f => allowedExtensions.Contains(Path.GetExtension(f).ToLower()) &&
							Path.GetFileNameWithoutExtension(f).Equals(fileName, StringComparison.OrdinalIgnoreCase))
				.ToList();

			if (!matchingFiles.Any())
			{
				return NotFound("No files matching the provided name were found.");
			}

			string filePath = matchingFiles.First();

			byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
			string contentType = "application/octet-stream";

			string fileExtension = Path.GetExtension(filePath).ToLower();
			switch (fileExtension)
			{
				case ".mp3":
					contentType = "audio/mpeg";
					break;
				case ".flac":
					contentType = "audio/flac";
					break;
				case ".wav":
					contentType = "audio/wav";
					break;
			}

			return File(fileBytes, contentType, Path.GetFileName(filePath));
		}

		[HttpGet("ListAllSongNames")]
		public async Task<IActionResult> ListAllSongNames()
		{
			string[] allowedExtensions = { ".mp3", ".flac", ".wav" };

			List<string> files = await Task.Run(() => Directory.GetFiles(_musicDirectory)
				.Where(f => allowedExtensions.Contains(Path.GetExtension(f).ToLower()))
				.Select(f => Path.GetFileName(f))
				.ToList());

			return Ok(files);
		}

		[HttpGet("SearchSongsByName")]
		public async Task<IActionResult> SearchSongsByName([FromQuery] string searchString)
		{
			if (string.IsNullOrEmpty(searchString))
			{
				return BadRequest("Search string is required.");
			}

			string[] allowedExtensions = { ".mp3", ".flac", ".wav" };

			List<string> files = await Task.Run(() => Directory.GetFiles(_musicDirectory)
				.Where(f => allowedExtensions.Contains(Path.GetExtension(f).ToLower()) &&
							Path.GetFileName(f).IndexOf(searchString, System.StringComparison.OrdinalIgnoreCase) >= 0)
				.Select(f => Path.GetFileName(f))
				.ToList());

			return Ok(files);
		}
	}
}
