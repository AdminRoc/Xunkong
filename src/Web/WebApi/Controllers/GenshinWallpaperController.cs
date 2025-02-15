﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;
using Xunkong.Core.XunkongApi;
using Xunkong.Web.Api.Filters;
using Xunkong.Web.Api.Services;

namespace Xunkong.Web.Api.Controllers
{

    [ApiController]
    [ApiVersion("0.1")]
    [Route("v{version:ApiVersion}/Genshin/Wallpaper")]
    [ServiceFilter(typeof(BaseRecordResultFilter))]
    public class GenshinWallpaperController : Controller
    {


        private readonly ILogger<GenshinWallpaperController> _logger;

        private readonly XunkongDbContext _dbContext;

        public GenshinWallpaperController(ILogger<GenshinWallpaperController> logger, XunkongDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }



        [HttpGet("random")]
        [ResponseCache(NoStore = true)]
        public async Task<ResponseBaseWrapper> GetRandomWallpaperAsJsonResultAsync(int excludeId = 0)
        {
            WallpaperInfo? info = null;
            if (excludeId == 0)
            {
                var count = await _dbContext.WallpaperInfos.Where(x => x.Recommend).CountAsync();
                var index = Random.Shared.Next(count);
                info = await _dbContext.WallpaperInfos.Where(x => x.Recommend).Skip(index).FirstOrDefaultAsync();
            }
            if (info == null)
            {
                var count = await _dbContext.WallpaperInfos.Where(x => x.Enable).CountAsync();
                var index = Random.Shared.Next(count - 1);
                info = await _dbContext.WallpaperInfos.Where(x => x.Enable && x.Id != excludeId).Skip(index).FirstOrDefaultAsync();
            }
            if (info == null)
            {
                info = await _dbContext.WallpaperInfos.Where(x => x.Enable).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            }
            return ResponseBaseWrapper.Ok(info!);
        }



        [HttpGet("next")]
        [ResponseCache(NoStore = true)]
        public async Task<ResponseBaseWrapper> GetNextWallpaperAsJsonResultAsync(int excludeId = 0)
        {
            var info = await _dbContext.WallpaperInfos.Where(x => x.Enable && x.Id > excludeId).OrderBy(x => x.Id).FirstOrDefaultAsync();
            if (info == null)
            {
                info = await _dbContext.WallpaperInfos.Where(x => x.Enable).OrderBy(x => x.Id).FirstOrDefaultAsync();
            }
            return ResponseBaseWrapper.Ok(info!);
        }



        [HttpGet("list")]
        [ResponseCache(Duration = 3600)]
        public async Task<ResponseBaseWrapper> GetWallpapersAsync(int page = 1)
        {
            var infos = await _dbContext.WallpaperInfos.Where(x => x.Enable).Skip(20 * page - 20).Take(20).ToListAsync();
            return ResponseBaseWrapper.Ok(infos);
        }




        [HttpPost("ChangeRecommend")]
        [ResponseCache(NoStore = true)]
        public async Task<ActionResult<ResponseBaseWrapper>> ChangeRecommendWallpaperAsync(int id = 0)
        {
            if (HttpContext.Request.Headers["X-Secret"] != Environment.GetEnvironmentVariable("XSECRET"))
            {
                return BadRequest();
            }
            var count = await _dbContext.WallpaperInfos.Where(x => x.Enable).CountAsync();
            WallpaperInfo? info = null;
            if (id != 0)
            {
                info = await _dbContext.WallpaperInfos.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
            }
            if (info is null)
            {
                var index = Random.Shared.Next(count);
                info = await _dbContext.WallpaperInfos.AsNoTracking().Where(x => x.Enable).Skip(index).FirstOrDefaultAsync();
            }
            await _dbContext.Database.ExecuteSqlRawAsync("UPDATE wallpapers SET Recommend=0 WHERE Recommend=1;");
            await _dbContext.Database.ExecuteSqlRawAsync("UPDATE wallpapers SET Recommend=1 WHERE Id={0};", info?.Id ?? 0);
            return ResponseBaseWrapper.Ok(info!);
        }




        [HttpGet("redirect/recommend")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> RedirectRecommendWallpaperAsync()
        {
            var count = await _dbContext.WallpaperInfos.Where(x => x.Recommend).CountAsync();
            var index = Random.Shared.Next(count);
            var info = await _dbContext.WallpaperInfos.Where(x => x.Recommend).Skip(index).FirstOrDefaultAsync();
            if (info is null)
            {
                count = await _dbContext.WallpaperInfos.Where(x => x.Enable).CountAsync();
                index = Random.Shared.Next(count);
                info = await _dbContext.WallpaperInfos.AsNoTracking().Where(x => x.Enable).Skip(index).FirstOrDefaultAsync();
            }
            var url = $"https://file.xunkong.cc/wallpaper/{HttpUtility.UrlEncode(info!.FileName)}";
            return Redirect(url);
        }




        [HttpGet("redirect/random")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> RedirectRandomWallpaperAsync()
        {
            var count = await _dbContext.WallpaperInfos.Where(x => x.Enable).CountAsync();
            var index = Random.Shared.Next(count);
            var info = await _dbContext.WallpaperInfos.AsNoTracking().Where(x => x.Enable).Skip(index).FirstOrDefaultAsync();
            var url = $"https://file.xunkong.cc/wallpaper/{HttpUtility.UrlEncode(info!.FileName)}";
            return Redirect(url);
        }



    }
}
