using AutoMapper;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Interfaces.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetFlex.BLL.Interfaces;
using NetFlex.BLL.ModelsDTO;
using NetFlex.WEB.ViewModels;

namespace NetFlex.WEB.Controllers
{
	[GraphRoute("content")]
	public class ContentController : GraphController
	{
		private readonly IVideoService _videoService;
		private readonly IReviewService _reviewService;

		public ContentController(IVideoService videoService, IReviewService reviewService)
		{
			_videoService = videoService;
			_reviewService = reviewService;
		}

		[Query("getMovieById")]
		public async Task<FullVideoInfoViewModel> getMovieById(Guid id)
		{
			FilmDTO filmDTO = await _videoService.GetFilm(id);

			var getFilm = await _videoService.GetFilm(id);
			var config = new MapperConfiguration(cfg => cfg.CreateMap<FilmDTO, FilmViewModel>());
			var mapper = new Mapper(config);
			var movie = mapper.Map<FilmDTO, FilmViewModel>(getFilm);

			var getGenres = await _videoService.GetGenres(id);
			var config1 = new MapperConfiguration(cfg => cfg.CreateMap<GenreVideoDTO, GenreVideosViewModel>());
			var mapper1 = new Mapper(config1);
			var genres = mapper1.Map<IEnumerable<GenreVideoDTO>, List<GenreVideosViewModel>>(getGenres);

			var getReviews = await _reviewService.GetReviews(id);
			var config2 = new MapperConfiguration(cfg => cfg.CreateMap<ReviewDTO, ReviewViewModel>());
			var mapper2 = new Mapper(config2);
			var reviews = mapper2.Map<IEnumerable<ReviewDTO>, List<ReviewViewModel>>(getReviews);

			var fullVideo = new FullVideoInfoViewModel()
			{
				Film = movie,
				Genres = genres,
				Reviews = reviews
			};

			return fullVideo;
		}

		[Query("getSerialById")]
		public async Task<FullVideoInfoViewModel> getSerialById(Guid id)
		{
			var getSerial = await _videoService.GetSerial(id);
			var config = new MapperConfiguration(cfg => cfg.CreateMap<SerialDTO, SerialViewModel>());
			var mapper = new Mapper(config);
			var serial = mapper.Map<SerialDTO, SerialViewModel>(getSerial);

			var getGenres = await _videoService.GetGenres(id);
			var config1 = new MapperConfiguration(cfg => cfg.CreateMap<GenreVideoDTO, GenreVideosViewModel>());
			var mapper1 = new Mapper(config1);
			var genres = mapper1.Map<IEnumerable<GenreVideoDTO>, List<GenreVideosViewModel>>(getGenres);

			var getReviews = await _reviewService.GetReviews(id);
			var config2 = new MapperConfiguration(cfg => cfg.CreateMap<ReviewDTO, ReviewViewModel>());
			var mapper2 = new Mapper(config2);
			var reviews = mapper2.Map<IEnumerable<ReviewDTO>, List<ReviewViewModel>>(getReviews);

			var getEpisodes = await _videoService.GetEpisodes(id);
			var config3 = new MapperConfiguration(cfg => cfg.CreateMap<EpisodeDTO, EpisodeViewModel>());
			var mapper3 = new Mapper(config3);
			var episodes = mapper3.Map<IEnumerable<EpisodeDTO>, List<EpisodeViewModel>>(getEpisodes);

			serial.Episodes = episodes;

			var fullVideo = new FullVideoInfoViewModel()
			{
				Serial = serial,
				Genres = genres,
				Reviews = reviews,
			};

			return fullVideo;
		}

		[Query("getMovies")]
		public async Task<IEnumerable<FilmViewModel>> getMovies() {

			var getFilms = await _videoService.GetFilms();
			var config = new MapperConfiguration(cfg => cfg.CreateMap<FilmDTO, FilmViewModel>());
			var mapper = new Mapper(config);
			var movies = mapper.Map<IEnumerable<FilmDTO>, IEnumerable<FilmViewModel>>(getFilms);

			return movies;
		}

		[Query("getSerials")]
		public async Task<IEnumerable<SerialViewModel>> getSerials() 
		{
			var getSerials = await _videoService.GetSerials();
			var config = new MapperConfiguration(cfg => cfg.CreateMap<SerialDTO, SerialViewModel>());
			var mapper = new Mapper(config);
			var serials = mapper.Map<IEnumerable<SerialDTO>, IEnumerable<SerialViewModel>>(getSerials);

			return serials;

		}

	}
}
